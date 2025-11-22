using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PROG6212_CMCS.Server.Data;
using PROG6212_CMCS.Server.Hubs;
using PROG6212_CMCS.Server.Models;

namespace PROG6212_CMCS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClaimsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotificationsHub> _hub;

        public ClaimsController(ApplicationDbContext context, IHubContext<NotificationsHub> hub)
        {
            _context = context;
            _hub = hub;
        }

        // =====================
        // 1. Todas claims
        // =====================
        [HttpGet]
        public IActionResult GetAll()
        {
            var claims = _context.Claims
                .Include(c => c.Lecturer).ThenInclude(l => l.User)
                .Include(c => c.Documents)
                .Include(c => c.Approvals)
                .ToList();

            return Ok(claims);
        }

        // =====================
        // 2. Claims do lecturer logado
        // =====================
        [HttpGet("mine")]
        public IActionResult GetMine()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userIdClaim == null) return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var claims = _context.Claims
                .Include(c => c.Lecturer).ThenInclude(l => l.User)
                .Include(c => c.Documents)
                .Include(c => c.Approvals)
                .Where(c => c.Lecturer.UserId == userId)
                .ToList();

            return Ok(claims);
        }

        // =====================
        // 3. Claims pendentes
        // =====================
        [HttpGet("pending")]
        public IActionResult GetPendingClaims()
        {
            var claims = _context.Claims
                .Include(c => c.Lecturer).ThenInclude(l => l.User)
                .Include(c => c.Documents)
                .Include(c => c.Approvals)
                .Where(c => c.Status == ClaimStatus.Pending)
                .OrderByDescending(c => c.ClaimDate)
                .ToList();

            return Ok(claims);
        }

        // =====================
        // 4. Criar claim (LECTURER)
        // =====================
        [HttpPost]
        public IActionResult Create(Claim claim)
        {
            var lecturer = _context.Lecturers.Find(claim.LecturerId);
            if (lecturer == null)
                return BadRequest("Lecturer not found");

            if (claim.HourlyRate <= 0)
                return BadRequest("HourlyRate must be greater than zero");

            if (claim.HoursWorked <= 0)
                return BadRequest("HoursWorked must be greater than zero");

            claim.TotalAmount = claim.HoursWorked * claim.HourlyRate;
            claim.Status = ClaimStatus.Pending;
            claim.ClaimDate = DateTime.UtcNow;

            _context.Claims.Add(claim);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = claim.ClaimId }, claim);
        }

        // =====================
        // 5. Claim específica
        // =====================
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var claim = _context.Claims
                .Include(c => c.Lecturer).ThenInclude(l => l.User)
                .Include(c => c.Documents)
                .Include(c => c.Approvals)
                .FirstOrDefault(c => c.ClaimId == id);

            if (claim == null) return NotFound();
            return Ok(claim);
        }

        // =====================
        // 6. APROVAR claim
        // =====================
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveClaim(int id, [FromBody] string? comments = null)
        {
            var userId = GetUserIdFromToken();
            if (userId == null) return Unauthorized();

            var claim = _context.Claims
                .Include(c => c.Lecturer).FirstOrDefault(c => c.ClaimId == id);

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

            // 🔔 NOTIFICA APENAS O DONO DO CLAIM
            await _hub.Clients.All
                .SendAsync("claimUpdated", new
                {
                    claimId = claim.ClaimId,
                    status = "Approved",
                    message = "Your claim has been approved."
                });

            return Ok(new { message = "Claim approved successfully", claim });
        }

        // =====================
        // 7. REJEITAR claim
        // =====================
        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectClaim(int id, [FromBody] string? comments = null)
        {
            var userId = GetUserIdFromToken();
            if (userId == null) return Unauthorized();

            var claim = _context.Claims
                .Include(c => c.Lecturer).FirstOrDefault(c => c.ClaimId == id);

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

            // 🔔 NOTIFICA APENAS O DONO DO CLAIM
            await _hub.Clients.All
                .SendAsync("claimUpdated", new
                {
                    claimId = claim.ClaimId,
                    status = "Rejected",
                    message = "Your claim has been rejected."
                });

            return Ok(new { message = "Claim rejected successfully", claim });
        }

        private int? GetUserIdFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (userIdClaim == null) return null;
            return int.Parse(userIdClaim);
        }
    }
}
