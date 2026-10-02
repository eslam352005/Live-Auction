using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Live_Auction.Application.DTOs.Auth
{
    public class ResendOtpDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
