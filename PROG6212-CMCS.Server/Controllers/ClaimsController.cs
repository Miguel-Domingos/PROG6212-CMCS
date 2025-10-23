using Microsoft.AspNetCore.Mvc;
using PROG6212_CMCS.Server.Data;
using PROG6212_CMCS.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace PROG6212_CMCS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ClaimsController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll()
        {
            var claims = _context.Claims
                .Include(c => c.Lecturer)
                .ThenInclude(l => l.User)
                .Include(c => c.Documents)
                .Include(c => c.Approvals)
                .ToList();

            return Ok(claims);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var claim = _context.Claims
                .Include(c => c.Lecturer)
                .Include(c => c.Documents)
                .Include(c => c.Approvals)
                .FirstOrDefault(c => c.ClaimId == id);

            if (claim == null) return NotFound();
            return Ok(claim);
        }

        [HttpPost]
        public IActionResult Create(Claim claim)
        {
            var lecturer = _context.Lecturers.Find(claim.LecturerId);
            if (lecturer == null) return BadRequest("Lecturer not found");

            claim.TotalAmount = claim.HoursWorked * lecturer.HourlyRate;
            claim.Status = ClaimStatus.Pending;
            claim.ClaimDate = DateTime.UtcNow;

            _context.Claims.Add(claim);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = claim.ClaimId }, claim);
        }

        [HttpPut("{id}/status")]
        public IActionResult UpdateStatus(int id, [FromBody] string status)
        {
            var claim = _context.Claims.Find(id);
            if (claim == null) return NotFound();

            if (Enum.TryParse(status, true, out ClaimStatus newStatus))
            {
                claim.Status = newStatus;
                _context.SaveChanges();
                return Ok(claim);
            }

            return BadRequest("Invalid status value");
        }
    }
}
