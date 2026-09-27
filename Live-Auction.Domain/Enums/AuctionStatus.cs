using System;
using System.Collections.Generic;
using System.Text;

namespace Live_Auction.Domain.Enums
{
    public enum AuctionStatus
    {
        Scheduled = 0,
        Active = 1,
        Ended = 2,
        Cancelled = 3
    }
}
