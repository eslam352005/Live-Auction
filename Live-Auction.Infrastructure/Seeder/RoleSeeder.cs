using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Live_Auction.Infrastructure.Seeder
{
    public static class RoleSeeder
    {

        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            if(await roleManager.Roles.AnyAsync())
            {
                return;
            }
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            await roleManager.CreateAsync(new IdentityRole("User"));
            
        }
    }
}
