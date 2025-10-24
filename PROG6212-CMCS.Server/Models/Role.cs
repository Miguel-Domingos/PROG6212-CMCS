// Models/Role.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PROG6212_CMCS.Server.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required, MaxLength(50)]
        public string RoleName { get; set; } = string.Empty;

        // Ex: 0 = lowest, 100 = highest
        public int AccessLevel { get; set; }

        public string? Description { get; set; }

        [JsonIgnore]
        public ICollection<User>? Users { get; set; }
    }
}
