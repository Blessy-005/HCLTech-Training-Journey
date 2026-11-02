using Microsoft.AspNetCore.Identity;
using MovieTicketBooking.Models;

namespace MovieTicketBooking.Data
{
    public class DbSeeder
    {
        public static async Task SeedRolesAsync(
            RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(
                        new IdentityRole(role));

                    if (!result.Succeeded)
                        throw new InvalidOperationException(
                            $"Failed to create role: {role}");
                }
            }
        }

        public static async Task SeedAdminAsync(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            var email = configuration["AdminSeed:Email"];
            var password = configuration["AdminSeed:Password"];

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException(
                    "Admin seed credentials are not configured.");

            var admin = await userManager.FindByEmailAsync(email);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    FullName = "System Administrator",
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(
                    admin, password);

                if (!createResult.Succeeded)
                    throw new InvalidOperationException(
                        string.Join(", ", createResult.Errors.Select(
                            e => e.Description)));
            }
            else
{
    var token = await userManager.GeneratePasswordResetTokenAsync(admin);

    var resetResult = await userManager.ResetPasswordAsync(
        admin, token, password);

    if (!resetResult.Succeeded)
        throw new InvalidOperationException(
            string.Join(", ", resetResult.Errors.Select(
                e => e.Description)));
}

            if (!await userManager.IsInRoleAsync(admin, "Admin"))
            {
                var roleResult = await userManager.AddToRoleAsync(
                    admin, "Admin");

                if (!roleResult.Succeeded)
                    throw new InvalidOperationException(
                        "Failed to assign Admin role.");
            }
        }
    }
}
