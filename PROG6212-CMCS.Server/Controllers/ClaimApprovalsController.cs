using Microsoft.AspNetCore.Mvc;
using PROG6212_CMCS.Server.Data;
using PROG6212_CMCS.Server.Models;

namespace PROG6212_CMCS.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClaimApprovalsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ClaimApprovalsController(ApplicationDbContext context) => _context = context;

        [HttpPost]
        public IActionResult Approve(ClaimApproval approval)
        {
            var claim = _context.Claims.Find(approval.ClaimId);
            if (claim == null) return NotFound("Claim not found");

            approval.ApprovalDate = DateTime.UtcNow;
            _context.ClaimApprovals.Add(approval);

            // Atualiza status principal do claim
            if (approval.Decision == ApprovalDecision.Approved)
                claim.Status = ClaimStatus.Approved;
            else if (approval.Decision == ApprovalDecision.Rejected)
                claim.Status = ClaimStatus.Rejected;

            _context.SaveChanges();
            return Ok(approval);
        }
    }
}