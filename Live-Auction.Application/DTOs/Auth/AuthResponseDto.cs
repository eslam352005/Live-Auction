using System;
using System.Collections.Generic;
using System.Text;

namespace Live_Auction.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsVerified { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryDate { get; set; }

    }
}
