using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Souq_BaniMazarAPI.DTOs;
using Souq_BaniMazarAPI.Models;
using Souq_BaniMazarAPI.Services;

namespace Souq_BaniMazarAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            IConfiguration config,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _config = config;
            _env = env;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto register)
        {
            var user = new ApplicationUser
            {
                UserName = register.Email,
                Email = register.Email,
                Name = register.Name,
                PhoneNumber = register.PhoneNumber,
                NationalIdUrl = string.Empty,
                IsApproved=false
            };

            var result = await _userManager.CreateAsync(user, register.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);
            // Handle uploaded image if provided
            if (register.NationalId is not null && register.NationalId.Length > 0)
            {
                // Example: save to wwwroot/uploads
                var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads","SellersIds",$"{user.UserName}");
                if (!Directory.Exists(uploadsRoot))
                {
                    Directory.CreateDirectory(uploadsRoot);
                }

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(register.NationalId.FileName)}";
                var filePath = Path.Combine(uploadsRoot, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await register.NationalId.CopyToAsync(stream);
                }

                 user.NationalIdUrl = $"/uploads/SellersIds/{user.UserName}/{fileName}";
                 await _userManager.UpdateAsync(user);
            }
            await _userManager.AddToRoleAsync(user, "Seller");

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto login)
        {
            var user = await _userManager.FindByEmailAsync(login.Email);
            if (user == null || !user.IsApproved) return Unauthorized();

            var result = await _signInManager.CheckPasswordSignInAsync(user, login.Password, lockoutOnFailure: true);
            if (!result.Succeeded) return Unauthorized();

            var (accessToken, refreshToken) = await _tokenService.CreateTokensAsync(user);

            var refreshDays = 30d;
            if (double.TryParse(_config["Jwt:RefreshTokenExpirationDays"], out var parsed)) refreshDays = parsed;

            var sameSite = SameSiteMode.None;
            if (_config["AllowAngular:CrossOrigin"] == "true") sameSite = SameSiteMode.None;
            var secure = false; // true in production; false in development
            //var secure = !_env.IsDevelopment(); // true in production; false in development

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = secure,
                SameSite = sameSite,
                Expires = DateTimeOffset.UtcNow.AddDays(refreshDays),
            });

            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault() ?? string.Empty;
            var userDto = new
            {
                id = user.Id,
                email = user.Email,
                userName = user.UserName,
                name = user.Name
            };

            return Ok(new
            {
                accessToken,
                user = userDto,
                role = primaryRole
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
                return Unauthorized();

            var tokens = await _tokenService.RefreshAsync(refreshToken);
            if (tokens == null) return Unauthorized();

            var refreshDays = 30d;
            if (double.TryParse(_config["Jwt:RefreshTokenExpirationDays"], out var parsed))
                refreshDays = parsed;

            var sameSite = SameSiteMode.None;
            if (_config["AllowAngular:CrossOrigin"] == "true")
                sameSite = SameSiteMode.None;

            var secure = false;
            //var secure = !_env.IsDevelopment();

            var (accessToken, newRefreshToken) = tokens.Value;

            Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = secure,
                SameSite = sameSite,
                Expires = DateTimeOffset.UtcNow.AddDays(refreshDays)
            });

            // Get user info from the refresh token to return role
            var userId = await _tokenService.GetUserIdFromRefreshTokenAsync(refreshToken);
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var primaryRole = roles.FirstOrDefault() ?? string.Empty;

                    return Ok(new { accessToken, role = primaryRole });
                }
            }

            return Ok(new { accessToken });
        }

        [Authorize]
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken)) return BadRequest();

            await _tokenService.RevokeRefreshTokenAsync(refreshToken);

            // delete cookie on client
            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = !_env.IsDevelopment(),
                SameSite = SameSiteMode.None // match how you set it initially
            });

            return Ok();
        }

        // GET /api/auth/me - return minimal profile for front-end
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            // get current user id from claims
            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault() ?? string.Empty;

            var userDto = new
            {
                id = user.Id,
                email = user.Email,
                userName = user.UserName,
                name = user.Name,
                role = primaryRole
            };

            return Ok(userDto);
        }
    }
}
