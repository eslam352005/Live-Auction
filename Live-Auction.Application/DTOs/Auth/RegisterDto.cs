using System;
using System.Collections.Generic;
using System.Text;

namespace Live_Auction.Application.DTOs.Auth
{
    public class RegisterDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }

        public string password { get; set; }

    }
}
