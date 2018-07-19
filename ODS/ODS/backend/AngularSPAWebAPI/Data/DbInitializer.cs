using AngularSPAWebAPI.Data;
using AngularSPAWebAPI.Models;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Services
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            this._userManager = userManager;
            this._roleManager = roleManager;
        }

        public async Task Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return; // Db has been seeded.
            }

            // Creates Roles.
            await _roleManager.CreateAsync(new IdentityRole("administrator"));
            await _roleManager.CreateAsync(new IdentityRole("user"));

            // Seeds an admin user.
            var user = new ApplicationUser
            {
                AccessFailedCount = 0,
                Email = "info@jansenbyods.com",
                EmailConfirmed = true,
                LockoutEnabled = false,
                NormalizedEmail = "info@jansenbyods.com",
                NormalizedUserName = "info@jansenbyods.com",
                TwoFactorEnabled = false,
                UserName = "info@jansenbyods.com"
            };

            var result = await _userManager.CreateAsync(user, "Catharina2018*");

            if (result.Succeeded)
            {
                var adminUser = await _userManager.FindByNameAsync(user.UserName);
                // Assigns the administrator role.
                await _userManager.AddToRoleAsync(adminUser, "administrator");
                // Assigns claims.
                var claims = new List<Claim> {
                    new Claim(type: JwtClaimTypes.PreferredUserName, value: user.UserName)
                };
                await _userManager.AddClaimsAsync(adminUser, claims);
            }
        }
    }
}
