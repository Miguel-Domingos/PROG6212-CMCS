using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PROG6212_CMCS.Server.Data;
using PROG6212_CMCS.Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PROG6212_CMCS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // ✅ LOGIN
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                    return BadRequest("Email and password are required.");

                var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
                if (user == null) return Unauthorized("Invalid Credentials");

                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                    return Unauthorized("Invalid Credentials.");

                var role = _context.Roles.FirstOrDefault(r => r.RoleId == user.RoleId);
                var token = GenerateJwtToken(user, role?.RoleName ?? "Unknown");

                return Ok(new
                {
                    message = "Login successful",
                    token,
                    user = new
                    {
                        user.UserId,
                        user.Name,
                        user.Email,
                        Role = role?.RoleName
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login.", error = ex.Message });
            }
        }

        // ✅ REGISTER
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email) ||
                    string.IsNullOrWhiteSpace(request.Password) ||
                    string.IsNullOrWhiteSpace(request.Name))
                {
                    return BadRequest("All fields are required.");
                }

                if (_context.Users.Any(u => u.Email == request.Email))
                    return Conflict("Email already exists.");

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var user = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    PasswordHash = hashedPassword,
                    RoleId = request.RoleId
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                return Ok(new { message = "User registered successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration.", error = ex.Message });
            }
        }

        // 🔒 GERA O TOKEN JWT
        private string GenerateJwtToken(User user, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new System.Security.Claims.Claim("id", user.UserId.ToString()),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role),
                new System.Security.Claims.Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // DTOs
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RoleId { get; set; }
    }
}
