using Microsoft.AspNetCore.Mvc;
using PROG6212_CMCS.Server.Data;
using PROG6212_CMCS.Server.Models;

namespace PROG6212_CMCS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public RolesController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.Roles.ToList());
    }
}
