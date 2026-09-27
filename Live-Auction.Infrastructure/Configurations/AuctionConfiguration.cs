using Live_Auction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Live_Auction.Infrastructure.Configurations
{
    public class AuctionConfiguration : IEntityTypeConfiguration<Auction>
    {
        public void Configure(EntityTypeBuilder<Auction> builder)
        {
            
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(a => a.Description)
                .HasMaxLength(1000);

            builder.Property(a => a.ImageUrl)
                .HasMaxLength(300);

            builder.Property(a => a.StartingPrice)
                .HasColumnType("decimal(12,2)");

            builder.Property(a => a.CurrentPrice)
                .HasColumnType("decimal(12,2)");

            builder.Property(a => a.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Optimistic Concurrency - العمود ده هيتحدث تلقائي كل مرة الـ Row يتعدل
            builder.Property(a => a.RowVersion)
                .IsRowVersion();

            builder.HasOne(a => a.Category)
                .WithMany(c => c.Auctions)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Seller)
                .WithMany()
                .HasForeignKey(a => a.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Winner)
                .WithMany()
                .HasForeignKey(a => a.WinnerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasMany(a => a.Bids)
                .WithOne(b => b.Auction)
                .HasForeignKey(b => b.AuctionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
