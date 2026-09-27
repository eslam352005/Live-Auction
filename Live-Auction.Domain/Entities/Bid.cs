using Live_Auction.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Live_Auction.Domain.Entities
{
    public class Bid
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public BidStatus Status { get; set; }

        public int AuctionId { get; set; }
        public Auction Auction { get; set; }

        public string BidderId { get; set; }
        public ApplicationUser Bidder { get; set; }
    }
}
