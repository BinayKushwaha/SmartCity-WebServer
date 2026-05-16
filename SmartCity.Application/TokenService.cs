namespace SmartCity.Application
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using SmartCity.Domain;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    public class TokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenService(IConfiguration configuration,
            UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;

        }

        public async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            // 1. Get roles from database
            var roles = await _userManager.GetRolesAsync(user);

            // 2. Build claims
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email,          user.Email!),
                new(ClaimTypes.Name,           user.UserName!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            // 3. Add each role as a separate claim  ✅
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 4. Build token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
