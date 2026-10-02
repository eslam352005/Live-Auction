using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Live_Auction.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            
        }
        public string FullName { get; set; }
        public bool IsActive { get; set; } = false;
        public string? StripeCustomerId { get; set; }
        public string? OtpCode { get; set; }
        public DateTime? OtpExpiryTime { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
