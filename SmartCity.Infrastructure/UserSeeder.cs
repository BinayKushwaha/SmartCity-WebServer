using Microsoft.AspNetCore.Identity;
using SmartCity.Domain;

namespace SmartCity.Infrastructure
{
    public class UserSeeder
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserSeeder(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            await SeedRolesAsync();
            await SeedUsersAsync();
        }

        public async Task SeedRolesAsync()
        {
            string[] roles = { "HouseSeeker", "Broker" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create role '{role}': " +
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
        }

        private async Task SeedUsersAsync()
        {
            // Seed a Broker user
            if (await _userManager.FindByEmailAsync("broker@smartcity.com") == null)
            {
                var broker = new ApplicationUser
                {
                    UserName = "brooker@smartcity.com",
                    FullName="Jane William",
                    EmailConfirmed=true,
                    Email= "brooker@smartcity.com"
                };

                var result = await _userManager.CreateAsync(broker, "Broker@123");

                if (result.Succeeded)
                    await _userManager.AddToRoleAsync(broker, "Broker");
            }

            // Seed a HouseSeeker user
            if (await _userManager.FindByEmailAsync("seeker@smartcity.com") == null)
            {
                var seeker = new ApplicationUser
                {
                    UserName = "seeker@smartcity.com",
                    Email = "seeker@smartcity.com",
                    FullName = "John Smith",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(seeker, "Seeker@123");

                if (result.Succeeded)
                    await _userManager.AddToRoleAsync(seeker, "HouseSeeker");
            }
        }
    }
}
