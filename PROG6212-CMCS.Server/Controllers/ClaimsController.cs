using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212_CMCS.Server.Data;
using PROG6212_CMCS.Server.Models;

namespace PROG6212_CMCS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClaimsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ClaimsController(ApplicationDbContext context) => _context = context;

        // 🔹 1. Retorna todas as claims (admin ou coord)
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

        // 🔹 2. Retorna apenas as claims do lecturer logado
        [HttpGet("mine")]
        public IActionResult GetMine()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var claims = _context.Claims
                .Include(c => c.Lecturer)
                    .ThenInclude(l => l.User)
                .Include(c => c.Documents)
                .Include(c => c.Approvals)
                .Where(c => c.Lecturer.UserId == userId)
                .ToList();

            return Ok(claims);
        }

        // 🔹 3. Retorna apenas claims pendentes (para coordenador)
        [HttpGet("pending")]
        public IActionResult GetPendingClaims()
        {
            var claims = _context.Claims
                .Include(c => c.Lecturer)
                    .ThenInclude(l => l.User)
                .Include(c => c.Documents)
                .Include(c => c.Approvals)
                .Where(c => c.Status == ClaimStatus.Pending)
                .OrderByDescending(c => c.ClaimDate)
                .ToList();

            return Ok(claims);
        }

        // 🔹 4. Cria claim (para lecturer)
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

        // 🔹 5. Retorna claim específica
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

        // 🔹 6. Aprovar claim (usando JWT)
        [HttpPost("{id}/approve")]
        public IActionResult ApproveClaim(int id, [FromBody] string? comments = null)
        {
            var userId = GetUserIdFromToken();
            if (userId == null) return Unauthorized();

            var claim = _context.Claims.Find(id);
            if (claim == null) return NotFound();

            if (claim.Status != ClaimStatus.Pending)
                return BadRequest("Only pending claims can be approved.");

            claim.Status = ClaimStatus.Approved;

            _context.ClaimApprovals.Add(new ClaimApproval
            {
                ClaimId = id,
                ApprovalDate = DateTime.UtcNow,
                Decision = ApprovalDecision.Approved,
                Comments = comments ?? "Approved by coordinator",
                ApproverId = userId.Value
            });

            _context.SaveChanges();
            return Ok(new { message = "Claim approved successfully", claim });
        }

        // 🔹 7. Rejeitar claim (usando JWT)
        [HttpPost("{id}/reject")]
        public IActionResult RejectClaim(int id, [FromBody] string? comments = null)
        {
            var userId = GetUserIdFromToken();
            if (userId == null) return Unauthorized();

            var claim = _context.Claims.Find(id);
            if (claim == null) return NotFound();

            if (claim.Status != ClaimStatus.Pending)
                return BadRequest("Only pending claims can be rejected.");

            claim.Status = ClaimStatus.Rejected;

            _context.ClaimApprovals.Add(new ClaimApproval
            {
                ClaimId = id,
                ApprovalDate = DateTime.UtcNow,
                Decision = ApprovalDecision.Rejected,
                Comments = comments ?? "Rejected by coordinator",
                ApproverId = userId.Value
            });

            _context.SaveChanges();
            return Ok(new { message = "Claim rejected successfully", claim });
        }

        // 🔹 8. Helper: pega ID do usuário logado
        private int? GetUserIdFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userIdClaim == null) return null;
            return int.Parse(userIdClaim);
        }
    }
}
