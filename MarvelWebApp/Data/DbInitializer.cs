using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MarvelWebApp.Models;

namespace MarvelWebApp.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Make sure the database is created
            await dbContext.Database.MigrateAsync();

            // Seed roles
            await RoleSeeder.SeedRolesAsync(roleManager);

            // Seed default user
            await SeedDefaultUserAsync(userManager);
        }

        private static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager)
        {
            // Admin User
            var adminEmail = "admin@admin.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "Admin",
                    UserID = 1,
                    // Password = "Admin@123"
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                // var result = await userManager.CreateAsync(adminUser, adminUser.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

             // Manager User
            var managerEmail = "manager@manager.com";
            var managerUser = await userManager.FindByEmailAsync(managerEmail);

            if (managerUser == null)
            {
                managerUser = new ApplicationUser
                {
                    UserName = managerEmail,
                    Email = managerEmail,
                    FirstName = "Manager",
                    LastName = "Manager",
                    UserID = 2,
                    // Password = "Manager@123"
                };

                var result = await userManager.CreateAsync(managerUser, "Manager@123");

                // var result = await userManager.CreateAsync(managerUser, managerUser.Password);
                // var result = await userManager.CreateAsync(managerUser, "Manager@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(managerUser, "Manager");
                }
            }

            // Regular User
            var userEmail = "user@user.com";
            var user = await userManager.FindByEmailAsync(userEmail);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    FirstName = "User",
                    LastName = "User",
                    UserID = 3,
                    // Password = "User@123"
                };

                var result = await userManager.CreateAsync(user, "User@123");
                // var result = await userManager.CreateAsync(user, user.Password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
            }
        }
    }
}