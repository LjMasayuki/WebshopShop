using Microsoft.AspNetCore.Identity;

namespace WebshopShop.Services
{
    public static class DbSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

            const string adminRole = "Admin";
            const string adminEmail = "admin@gmail.com";
            const string adminUsername = "admin";

            if (!await roleManager.RoleExistsAsync(adminRole))
                await roleManager.CreateAsync(new IdentityRole(adminRole));

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser != null)
            {
                // fix username if it was set to email previously
                if (adminUser.UserName == adminEmail)
                {
                    adminUser.UserName = adminUsername;
                    await userManager.UpdateAsync(adminUser);
                }

                if (!await userManager.IsInRoleAsync(adminUser, adminRole))
                    await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
    }
}