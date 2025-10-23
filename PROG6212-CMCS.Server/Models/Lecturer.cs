// Models/Lecturer.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace PROG6212_CMCS.Server.Models
{
    public class Lecturer
    {
        [Key]
        public int LecturerId { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User? User { get; set; }

        [MaxLength(200)]
        public string BankDetails { get; set; } = string.Empty;

        // taxa por hora
        [Column(TypeName = "decimal(18,2)")]
        public decimal HourlyRate { get; set; }

        public ICollection<Claim>? Claims { get; set; }
    }
}
