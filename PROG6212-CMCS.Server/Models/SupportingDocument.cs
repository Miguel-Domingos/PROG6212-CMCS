// Models/SupportingDocument.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6212_CMCS.Server.Models
{
    public class SupportingDocument
    {
        [Key]
        public int DocumentId { get; set; }

        [ForeignKey(nameof(Claim))]
        public int ClaimId { get; set; }
        public Claim? Claim { get; set; }

        [Required, MaxLength(260)]
        public string FileName { get; set; } = string.Empty;

        // caminho relativo/URL (ex: /uploads/claims/1/file.pdf)
        [Required, MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        // opcional: MIME type
        public string? ContentType { get; set; }
    }
}
