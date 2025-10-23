// Models/ClaimApproval.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6212_CMCS.Server.Models
{
    public class ClaimApproval
    {
        [Key]
        public int ApprovalId { get; set; }

        [ForeignKey(nameof(Claim))]
        public int ClaimId { get; set; }
        public Claim? Claim { get; set; }

        // quem aprovou (user)
        [ForeignKey(nameof(Approver))]
        public int ApproverId { get; set; }
        public User? Approver { get; set; }

        public DateTime ApprovalDate { get; set; } = DateTime.UtcNow;

        public ApprovalDecision Decision { get; set; } = ApprovalDecision.Pending;

        public string? Comments { get; set; }
    }
}
