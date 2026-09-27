using Live_Auction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Live_Auction.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Auction> Auctions { get; }
        DbSet<Bid> Bids { get; }
        DbSet<Category> Categories { get; }
        DbSet<Payment> Payments { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
