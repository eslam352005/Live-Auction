using System;
using System.Collections.Generic;
using System.Text;

namespace Live_Auction.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Auction> Auctions { get; set; }
    }
}
