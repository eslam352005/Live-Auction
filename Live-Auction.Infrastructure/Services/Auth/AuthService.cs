
using Azure;
using Hangfire;
using Live_Auction.Application.DTOs.Auth;
using Live_Auction.Application.DTOs.Response;
using Live_Auction.Application.Interfaces;
using Live_Auction.Domain.Entities;
using Live_Auction.Infrastructure.Services.Auth.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SqlServer.Server;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace Live_Auction.Infrastructure.Services.Auth
{
    public class AuthService(UserManager<ApplicationUser> _userManager,
            SignInManager<ApplicationUser> _signInManager,
            IEmailService _emailService,
            IConfiguration _configuration,
            IBackgroundJobClient _backgroundJobClient) : IAuthService
    {

        public async Task<ResponseDto<AuthResponseDto>> RegisterAsync(RegisterDto registerDto)
        {
            var user = new ApplicationUser()
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                UserName = registerDto.Email
            };
            var result = await _userManager.CreateAsync(user, registerDto.password);
            if (!result.Succeeded)
            {
                return ResponseHandler.BadRequest<AuthResponseDto>("Can't Register Right Now");
            }
            await _userManager.AddToRoleAsync(user, "User");


             _backgroundJobClient.Enqueue(() => GenerateAndSendOtpAsync(user.Id));
            var response = new AuthResponseDto()
            {
                FullName = user.FullName,
                UserId = user.Id,
                Email = user.Email,
                IsVerified = false,
                AccessToken = null!,
                RefreshToken = null!,
                RefreshTokenExpiryDate = default
            };
            return ResponseHandler.Success(response, "User Registered Successfully, Check your email for the Otp");
        }

        public async Task<ResponseDto<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if(user == null)
            {
              return ResponseHandler.NotFound<AuthResponseDto>("This User Is Not Registered Yet");
            }
            if (!user.IsActive)
            {
                return ResponseHandler.BadRequest<AuthResponseDto>("You are Not Verefied Yet, Check your email");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(
                user,
                loginDto.Password,
                lockoutOnFailure: true
            );

            if (result.IsLockedOut)
            {
                var lockoutEnd = await _userManager.GetLockoutEndDateAsync(user);
                return ResponseHandler.BadRequest<AuthResponseDto>($"Account locked until {lockoutEnd?.DateTime:HH:mm}");
            }

            if (result.IsNotAllowed)
            {
                return ResponseHandler.BadRequest<AuthResponseDto>("Please confirm your email first");
            }

            if (result.RequiresTwoFactor)
            {
                return ResponseHandler.BadRequest<AuthResponseDto>("Two-factor authentication required");
            }

            if (!result.Succeeded)
            {
                return ResponseHandler.BadRequest<AuthResponseDto>("Invalid Email Or Password");
            }
            var accessToken = await CreateTokenAsync(user);
            var refreshToken =  GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = refreshTokenExpiry;
             await _userManager.UpdateAsync(user);
            var response = new AuthResponseDto()
            {
                FullName = user.FullName,
                UserId = user.Id,
                Email = user.Email!,
                IsVerified = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiryDate = refreshTokenExpiry
            };
            return ResponseHandler.Success(response, "User Loggined in Successfully");
        }

        public async Task<ResponseDto<bool>> LogOutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user == null)
            {
                return ResponseHandler.NotFound<bool>("This User Is Not Found");
            }
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);
            return ResponseHandler.Success(true,"Logged Out Successfully");
        }

        public async Task<ResponseDto<bool>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);

            if (user == null)
            {
                return ResponseHandler.NotFound<bool>($"Email {forgotPasswordDto.Email} is not Registered");
            }

            
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            var clientUrl = _configuration["AppSettings:ClientUrl"];
            var resetLink = $"{clientUrl}/reset-password?email={forgotPasswordDto.Email}&token={encodedToken}";

            var emailBody = $@"
               <h2>Reset Password</h2>
               <p>Hi {user.FullName},</p>
               <p>Click here to reset your password:</p>
               <p><a href='{resetLink}'>Reset Password</a></p>
               <p>Link expires in 1 hour.</p>";

            await _emailService.SendEmailAsync(user.Email!, "Reset Password", emailBody);

            return ResponseHandler.Success(true, "If the email exists, a password reset link has been sent");
        }

        public async Task<ResponseDto<bool>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return ResponseHandler.BadRequest<bool>("Invalid request");
            }

            
            var decodedToken = Encoding.UTF8.GetString(
                WebEncoders.Base64UrlDecode(resetPasswordDto.Token)
            );

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ResponseHandler.BadRequest<bool>( "Failed to reset password",errors);
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);

            return ResponseHandler.Success(true, "Password rested successfully");
        }

        public async Task<ResponseDto<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            var principal = GetPrincipalFromExpiredToken(refreshTokenDto.AccessToken);
            if (principal == null)
            {
                return ResponseHandler.UnAuthorized<AuthResponseDto>("Invalid token");
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return ResponseHandler.UnAuthorized<AuthResponseDto>("Invalid token");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ResponseHandler.UnAuthorized<AuthResponseDto>("User not found");
            }

            if (user.RefreshToken != refreshTokenDto.RefreshToken)
            {
                return ResponseHandler.UnAuthorized<AuthResponseDto>("Invalid refresh token");
            }

            if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return ResponseHandler.UnAuthorized<AuthResponseDto>("Refresh token has expired");
            }

            var newAccessToken = await CreateTokenAsync(user);
            var newRefreshToken = GenerateRefreshToken();
            var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = newRefreshTokenExpiry;
            await _userManager.UpdateAsync(user);

            var userReturn = new AuthResponseDto()
            {
                FullName = user.FullName!,
                Email = user.Email!,
                UserId = user.Id,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiryDate = newRefreshTokenExpiry
            };

            return ResponseHandler.Success(userReturn);
        }

        public async Task<ResponseDto<bool>> VerifyOtpAsync(VerifyOtpDto verifyOtpDto)
        {
            var user = await _userManager.FindByEmailAsync(verifyOtpDto.Email);
            if (user == null)
            {
                return ResponseHandler.NotFound<bool>("User with this Email has not registered");
            }
            if(user.OtpCode != verifyOtpDto.OtpCode)
            {
                return ResponseHandler.BadRequest<bool>("Invalid Otp Code");
            }
            if(DateTime.UtcNow > user.OtpExpiryTime)
            {
                return ResponseHandler.BadRequest<bool>("Request Otp time Out");
            }
            user.IsActive = true;
            user.OtpCode = null;
            user.OtpExpiryTime = null;
            await _userManager.UpdateAsync(user);
            return ResponseHandler.Success(true, "Otp verified successfully");
        }

        public async Task<ResponseDto<bool>> ResendOtpAsync(ResendOtpDto resendOtpDto)
        {
            var user = await _userManager.FindByEmailAsync(resendOtpDto.Email);
            if (user == null)
            {
                return ResponseHandler.NotFound<bool>("User with this Email has not registered");
            }
            if (user.IsActive)
            {
                return ResponseHandler.BadRequest<bool>("User is Already Verified");
            }
            if (user.OtpExpiryTime.HasValue && user.OtpExpiryTime.Value.AddMinutes(-8) > DateTime.UtcNow)
            {
            return ResponseHandler.BadRequest<bool>("Please wait before requesting a new OTP");
            }
            _backgroundJobClient.Enqueue(() => GenerateAndSendOtpAsync(user.Id));
            return ResponseHandler.Success(true, "Resent Otp successfully");
        }




        // Helper Methods
        private string GenerateOtpCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // 6 أرقام
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var secretKey = _configuration["JwtOptions:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT Secret Key is not configured");
            }

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["JwtOptions:Issuer"],
                ValidAudience = _configuration["JwtOptions:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }
        public async Task GenerateAndSendOtpAsync(string userId)
        {
            // توليد OTP عشوائي (6 أرقام)
            var otp = GenerateOtpCode();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ;
            }
            // حفظ OTP في Database
            user.OtpCode = otp;
            user.OtpExpiryTime = DateTime.UtcNow.AddMinutes(10); // صلاحية 10 دقائق
            await _userManager.UpdateAsync(user);

            // إرسال OTP عبر Email
            var emailBody = $@"
        <html>
        <body style='font-family: Arial, sans-serif;'>
            <h2>Email Verification</h2>
            <p>Hi {user.FullName},</p>
            <p>Thank you for registering! Your OTP verification code is:</p>
            <div style='background-color: #f0f0f0; padding: 20px; text-align: center; font-size: 32px; font-weight: bold; letter-spacing: 5px; margin: 20px 0;'>
                {otp}
            </div>
            <p>This code will expire in 10 minutes.</p>
            <p>If you didn't request this code, please ignore this email.</p>
            <br/>
            <p>Best regards,<br/>Base Project Team</p>
        </body>
        </html>
    ";

            await _emailService.SendEmailAsync(
                user.Email!,
                "Email Verification - OTP Code",
                emailBody
            );
        }
        private async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var secretKey = _configuration["JwtOptions:SecretKey"];

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT Secret Key is not configured");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtOptions:Issuer"],
                audience: _configuration["JwtOptions:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

       
    }
}