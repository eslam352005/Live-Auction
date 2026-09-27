using System;
using System.Collections.Generic;
using System.Text;

namespace Live_Auction.Domain.Enums
{
    public enum PaymentStatus { 
        Authorized = 0,
        Captured = 1,
        Released = 2,
        Failed = 3
    }

}
