// Models/User.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROG6212_CMCS.Server.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, MaxLength(120)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        // Para POE basta um hash simples; não implementar auth completa se não for necessário
        public string? PasswordHash { get; set; }

        [ForeignKey(nameof(Role))]
        public int RoleId { get; set; }
        public Role? Role { get; set; }

        // Se o User for Lecturer, a relação opcional aparece em Lecturer
        public Lecturer? LecturerProfile { get; set; }
    }
}
