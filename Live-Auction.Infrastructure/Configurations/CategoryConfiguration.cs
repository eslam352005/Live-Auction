using Live_Auction.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Live_Auction.Infrastructure.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
           
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasMany(c => c.Auctions)
                .WithOne(a => a.Category)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                    new Category { Id = 1, Name = "إلكترونيات" },
                    new Category { Id = 2, Name = "تحف وأنتيكات" },
                    new Category { Id = 3, Name = "ساعات" },
                    new Category { Id = 4, Name = "سيارات كلاسيكية" }
);
        }
    }
}
