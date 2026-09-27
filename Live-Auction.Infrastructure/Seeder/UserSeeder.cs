using Live_Auction.Domain.Entities;
using Live_Auction.Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;


namespace Live_Auction.Infrastructure.Seeder
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            if (await userManager.Users.AnyAsync())
            {
                return;
            }
            var admin = new ApplicationUser
            {
                UserName = "admin@auction.com",
                Email = "admin@auction.com",
                FullName = "System Admin",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Admin123");
            await userManager.AddToRoleAsync(admin, "Admin");

            await context.SaveChangesAsync();

        }
    }
}
