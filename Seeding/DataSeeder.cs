

using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;
using MSN.Data;
using MSN.Models;

namespace MSN.Seeding
{

    //public static class DataSeeder
    //{
    //    public static async Task SeedDataAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    //    {
    //        // Seed roles first
    //        if (!await roleManager.RoleExistsAsync("Admin"))
    //        {
    //            await roleManager.CreateAsync(new IdentityRole("Admin"));
    //        }

    //        if (!await roleManager.RoleExistsAsync("Member"))
    //        {
    //            await roleManager.CreateAsync(new IdentityRole("Member"));
    //        }

    //        // Seed users
    //        if (await userManager.FindByNameAsync("Youssef") == null)
    //        {
    //            var user = new ApplicationUser
    //            {
    //                UserName = "Youssef",
    //                Email = "youssef@gmail.com",
    //                Role = "Admin", // Ensure this is used correctly in your app logic
    //                PhotoUrl = "..."
    //            };
    //            await userManager.CreateAsync(user, "Password123!");
    //            await userManager.AddToRoleAsync(user, "Admin"); // Link the user to the "Admin" role
    //        }

    //        // ... Repeat for other users ...
    //    }
    //}

    public static class DataSeeder
    {
        public static async Task SeedDataAsync(DataContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!context.Users.Any())
            {
                ApplicationUser user1 = new ApplicationUser
                {
                    UserName = "Youssef",
                    Email = "youssef@gmail.com",
                    Role = "Admin",
                    PhotoUrl = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTVQP5_OOoV0CIvHU6CqQ20o8FFhpNoDnnXNA&usqp=CAU",
                    Token = ""

                };

                ApplicationUser user2 = new ApplicationUser
                {
                    UserName = "Henry",
                    Email = "henry@gmail.com",
                    Role = "Member",
                    PhotoUrl = "https://www.davidchang.ca/wp-content/uploads/2020/09/David-Chang-Photography-Headshots-Toronto-61-1536x1536.jpg",
                    Token = ""
                };

                ApplicationUser user3 = new ApplicationUser
                {
                    UserName = "Estelle",
                    Email = "estelle@gmail.com",
                    Role = "Admin",
                    PhotoUrl = "https://media.istockphoto.com/id/1318482009/photo/young-woman-ready-for-job-business-concept.jpg?s=612x612&w=0&k=20&c=Jc1NcoUMoM78AxPTh9EApaPU2kXh2evb499JgW99b0g=",
                    Token = ""
                };

                userManager.CreateAsync(user1, "1234").GetAwaiter().GetResult();
                userManager.CreateAsync(user2, "1234").GetAwaiter().GetResult();
                userManager.CreateAsync(user3, "1234").GetAwaiter().GetResult();



                if (!context.Roles.Any())
                {
                    var role1 = new IdentityRole("ADMIN");
                    var role2 = new IdentityRole("MEMBER");

                    await roleManager.CreateAsync(role1);
                    await roleManager.CreateAsync(role2);
                }


            }
        }

    }
}

