using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212_CMCS.Server.Data;
using PROG6212_CMCS.Server.Models;

namespace PROG6212_CMCS.Server.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class CreateUserRequest
        {
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public int RoleId { get; set; }
            public string BankDetails { get; set; } = string.Empty;
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
        {
            var role = _context.Roles.Where(r => r.RoleId == request.RoleId).First();
            
            
            if (role == null) {
                return BadRequest("Role does not exist");
            }
            if (_context.Users.Any(u => u.Email == request.Email))
                return Conflict("Email already registered.");

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

            // Se for Lecturer, cria perfil automaticamente
            if (role.RoleName == "Lecturer")
            {
                var lecturer = new Lecturer
                {
                    UserId = user.UserId,
                    BankDetails = request.BankDetails
                };

                _context.Lecturers.Add(lecturer);
                _context.SaveChanges();
            }

            return Ok(new
            {
                message = "User created successfully",
                user = new
                {
                    user.UserId,
                    user.Name,
                    user.Email,
                    Role = role.RoleName
                }
            });
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = _context.Users
                .Include(u => u.Role)
                .Include(u => u.LecturerProfile)
                .ToList();

            return Ok(users);
        }
    }
}
