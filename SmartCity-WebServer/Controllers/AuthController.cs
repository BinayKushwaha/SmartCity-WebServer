using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartCity.Application;
using SmartCity.Application.DTOs;
using SmartCity.Domain;

namespace SmartCity_WebServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly TokenService _tokenService;

        public AuthController(SignInManager<ApplicationUser> signInManager,
            TokenService tokenService,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _tokenService = tokenService;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null) return Unauthorized("Invalid credentials");

            var result = await _signInManager
                .CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);

            if (!result.Succeeded) return Unauthorized("Invalid credentials");

            var token = await _tokenService.CreateTokenAsync(user);
            return Ok(new AuthResponseDto(token, user.Email!, user.Id));
        }
    }
}
