using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillBridge.Models
{
    // User Information table
    public class UserInformation
    {
        [Key]
        [ForeignKey("User")]
        public string UserId { get; set; } // FK to AspNetUsers

        [Required]
        [StringLength(150)]
        public string FullName { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        [StringLength(100)]
        public string Profession { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; }

        [Required]
        [StringLength(500)]
        public string Bio { get; set; }

        public virtual ApplicationUser User { get; set; }
    }

    // User Skills table
    public class UserSkill
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; } // FK to AspNetUsers
        public virtual ApplicationUser User { get; set; }

        [Required]
        [ForeignKey("Skill")]
        public int SkillId { get; set; } // FK to Skills
        public virtual Skill Skill { get; set; }

        [Required]
        [StringLength(10)]
        public string Status { get; set; } // "Teaching" or "Learning"

        public int? KnownUpToStage { get; set; } // Nullable if Status = Learning
    }
}
