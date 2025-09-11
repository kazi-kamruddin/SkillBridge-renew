using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SkillBridge.Models
{
    public class CompleteProfileViewModel
    {
        // Basic Info
        [Required(ErrorMessage = "Full name is required.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(1, 150, ErrorMessage = "Age must be between 1 and 150.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Profession is required.")]
        public string Profession { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Bio is required.")]
        [StringLength(500)]
        public string Bio { get; set; }

        // Skills
        [Required(ErrorMessage = "Select at least one skill to learn.")]
        public List<int> SkillsToLearn { get; set; } = new List<int>(); // Skill IDs

        [Required(ErrorMessage = "Select at least one skill you know.")]
        public List<UserKnownSkill> SkillsIKnow { get; set; } = new List<UserKnownSkill>();

        // Skill selection helper
        public List<SkillCategory> AllSkillCategories { get; set; } = new List<SkillCategory>();

        // Nested class for known skills
        public class UserKnownSkill
        {
            public int SkillId { get; set; }
            [Required]
            public int KnownUpToStage { get; set; }
        }
    }
}
