using System;
using System.Collections.Generic;
using System.Text;

namespace Live_Auction.Domain.Enums
{
    public enum BidStatus { 
        Active = 0,
        Outbid = 1,
        Won = 2,
        Released = 3 
    }
}
