using Live_Auction.Application.DTOs.Auth;
using Live_Auction.Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Live_Auction.Application.Interfaces
{
    public interface IAuthService
    {
        public Task<ResponseDto<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
        public Task<ResponseDto<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        public Task<ResponseDto<bool>> LogOutAsync(string userId);
        Task<ResponseDto<bool>> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<ResponseDto<bool>> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<ResponseDto<AuthResponseDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
        Task<ResponseDto<bool>> VerifyOtpAsync(VerifyOtpDto verifyOtpDto);
        Task<ResponseDto<bool>> ResendOtpAsync(ResendOtpDto resendOtpDto);
    }
}
