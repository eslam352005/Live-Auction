using Live_Auction.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace Live_Auction.Domain.Entities
{
    public class Auction
    {
        private Auction() { } // EF

        public Auction(string title, string description, decimal startingPrice,
            DateTime startTime, DateTime endTime, int categoryId, string sellerId)
        {
            Title = title;
            Description = description;
            StartingPrice = startingPrice;
            CurrentPrice = startingPrice;
            StartTime = startTime;
            EndTime = endTime;
            CategoryId = categoryId;
            SellerId = sellerId;
            Status = AuctionStatus.Scheduled;
            Bids = new List<Bid>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public decimal StartingPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public AuctionStatus Status { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public string SellerId { get; set; }
        public ApplicationUser Seller { get; set; }

        public string? WinnerId { get; set; }
        public ApplicationUser? Winner { get; set; }

        public ICollection<Bid> Bids { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } // Optimistic Concurrency
    }
}
