using Live_Auction.Application.DTOs.Auth;
using Live_Auction.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Live_Auction.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService _service) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterDto registerDto)
        {
            var result = await _service.RegisterAsync(registerDto);
            return Ok(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDto loginDto)
        {
            var result = await _service.LoginAsync(loginDto);
            return Ok(result);
        }
        [HttpPost("logout")]
        public async Task<IActionResult> LogOutAsync(string userId)
        {
            var result = await _service.LogOutAsync(userId);
            return Ok(result);
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var result = await _service.ForgotPasswordAsync(forgotPasswordDto);
            return Ok(result);
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var result = await _service.ResetPasswordAsync(resetPasswordDto);
            return Ok(result);
        }
        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            var result = await _service.RefreshTokenAsync(refreshTokenDto);
            return Ok(result);
        }
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtpAsync(VerifyOtpDto verifyOtpDto)
        {
            var result = await _service.VerifyOtpAsync(verifyOtpDto);
            return Ok(result);
        }
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtpAsync(ResendOtpDto resendOtpDto)
        {
            var result = await _service.ResendOtpAsync(resendOtpDto);
            return Ok(result);
        }
    }
}
