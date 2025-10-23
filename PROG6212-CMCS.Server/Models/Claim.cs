// Models/Claim.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6212_CMCS.Server.Models
{
    public class Claim
    {
        [Key]
        public int ClaimId { get; set; }

        [ForeignKey(nameof(Lecturer))]
        public int LecturerId { get; set; }
        public Lecturer? Lecturer { get; set; }

        public DateTime ClaimDate { get; set; } = DateTime.UtcNow;

        // horas trabalhadas (suporta frações)
        [Column(TypeName = "decimal(10,2)")]
        public decimal HoursWorked { get; set; }

        // calculado: HoursWorked * Lecturer.HourlyRate (podes popular no controller)
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public ClaimStatus Status { get; set; } = ClaimStatus.Pending;

        public string? Notes { get; set; }

        public ICollection<SupportingDocument>? Documents { get; set; }

        public ICollection<ClaimApproval>? Approvals { get; set; }
    }
}
