using Microsoft.AspNetCore.Identity;
using Souq_BaniMazarAPI.Data;
using Souq_BaniMazarAPI.Models;

namespace Souq_BaniMazarAPI.Seeds
{
    public static class SeedInitial
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Seed Admin Role
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("Seller"))
            {
                await roleManager.CreateAsync(new IdentityRole("Seller"));
            }
            if (!await roleManager.RoleExistsAsync("Buyer"))
            {
                await roleManager.CreateAsync(new IdentityRole("Buyer"));
            }

            var adminEmail = "admin@gmail.com";
            var adminPhone = "01000000000";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, Name = "Admin" ,PhoneNumber=adminPhone};
                await userManager.CreateAsync(admin, "AdminP@ssw0rd!");
                await userManager.AddToRoleAsync(admin, "Admin");
            }
 
        }
    }

}
