using Live_Auction.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Live_Auction.Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string PaymentIntentId { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int AuctionId { get; set; }
        public Auction Auction { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
