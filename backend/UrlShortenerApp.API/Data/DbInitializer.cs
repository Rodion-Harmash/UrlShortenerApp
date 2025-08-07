using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UrlShortenerApp.API.Models;

namespace UrlShortenerApp.API.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            var roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@short.io";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    DisplayName = "Administrator",
                    RegisteredAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(admin, "Admin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                    adminUser = admin;
                }
                else
                {
                    throw new Exception("Failed to create seeded admin user.");
                }
            }

            // === Add AboutInfo seed ===
            if (!await context.AboutInfos.AnyAsync())
            {
                var about = new AboutInfo
                {
                    Content = "This application shortens URLs using a GUID-based short code algorithm. " +
                              "Each authorized user can create unique short URLs that redirect to original destinations. " +
                              "Admins can manage all URLs, while regular users can manage only their own. " +
                              "Anonymous users can browse shortened URLs. The system tracks click counts and supports JWT-based authorization.",
                    LastEditedAt = DateTime.UtcNow,
                    EditedById = adminUser!.Id
                };

                context.AboutInfos.Add(about);
                await context.SaveChangesAsync();
            }
        }
    }
}
