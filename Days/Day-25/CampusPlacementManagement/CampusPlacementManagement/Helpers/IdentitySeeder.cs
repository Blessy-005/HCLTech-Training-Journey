using CampusPlacementManagement.Models.Entities;
using CampusPlacementManagement.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagement.Helpers;

public static class IdentitySeeder
{
    public static async Task SeedRolesAsync(
        RoleManager<IdentityRole> roleManager)
    {
        string[] roles =
        {
            "SuperAdmin",
            "PlacementOfficer",
            "CompanyHR",
            "Student"
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(
                    new IdentityRole(role));

                if (!result.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        result.Errors.Select(e => e.Description));

                    throw new InvalidOperationException(
                        $"Failed to create role {role}: {errors}");
                }
            }
        }
    }

    public static async Task SeedSuperAdminAsync(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        var email = configuration["BootstrapAdmin:Email"];
        var password = configuration["BootstrapAdmin:Password"];
        var fullName = configuration["BootstrapAdmin:FullName"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(fullName))
        {
            return;
        }

        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = fullName,
                AccountStatus = AccountStatus.Approved
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(
                    $"Failed to create SuperAdmin: {errors}");
            }
        }

        if (user.AccountStatus == AccountStatus.Approved &&
            !await userManager.IsInRoleAsync(user, "SuperAdmin"))
        {
            var roleResult = await userManager.AddToRoleAsync(
                user, "SuperAdmin");

            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Failed to assign the SuperAdmin role.");
            }
        }
    }
}
