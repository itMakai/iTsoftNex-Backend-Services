using iTsoftNex.IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace iTsoftNex.IdentityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // --- 1. User Registration ---
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new ApplicationUser
            {
                // FullName is initialized here, satisfying the non-nullable check
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Optional: Assign a default role here, e.g., "Cashier"
                return Ok(new { Status = "Success", Message = "User created successfully!" });
            }

            return BadRequest(result.Errors);
        }

        // --- 2. User Login ---
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                user.LastLogin = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                // 1. Define Claims
                var authClaims = new List<Claim>
                {
                    // FIX: user.UserName and user.Id are guaranteed to be non-null after FindByNameAsync, 
                    // so we use '!' to remove the warning.
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserId", user.Id!), 
                    // StoreId is an int (value type) and safely converts to string
                    new Claim("StoreId", user.StoreId.ToString())
                };

                // 2. Create the Token
                // FIX: Added '!' to safely access configuration values
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JwtSettings:Issuer"],
                    audience: _configuration["JwtSettings:Audience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                // 3. Return the Token
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }
    }

    // Simple models for request body
    public record RegisterModel(string FullName, string Email, string Password);

    // FIX: Removed the extra 'string' from the Password parameter
    public record LoginModel(string Email, string Password);
}