

using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;
using MSN.Data;
using MSN.Models;

namespace MSN.Seeding
{
    public static class DataSeeder
    {
        public static async Task SeedData(DataContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!context.Users.Any())
            {
                //ApplicationUser user1 = new ApplicationUser
                //{
                //    UserName = "you",
                //    Email = "youssef.kharoufi96@gmail.com",
                //};

                //ApplicationUser user2 = new ApplicationUser
                //{
                //    UserName = "gataga",
                //    Email = "y.kharoufi96@gmail.com",
                //};

                //userManager.CreateAsync(user1, "1234").GetAwaiter().GetResult();
                //userManager.CreateAsync(user2, "1234").GetAwaiter().GetResult();
            }

            if(!context.Roles.Any())
            {
                var role1 = new IdentityRole("ADMIN");
                var role2 = new IdentityRole("MEMBER");

                await roleManager.CreateAsync(role1);
                await roleManager.CreateAsync(role2);
            }

            
        }
    }
}

