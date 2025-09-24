﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;

namespace SkillBridge.Models
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }

        // Personal info
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public string Profession { get; set; }
        public string Location { get; set; }
        public int Age { get; set; }

        // Skills
        public List<UserSkillViewModel> TeachingSkills { get; set; }
        public List<UserSkillViewModel> LearningSkills { get; set; }


        public double AverageRating { get; set; }
        public int RatingsReceived { get; set; }
        public int InteractionsCompleted { get; set; }


        public string ProfileImageUrl { get; set; }
    }


    public class UserSkillViewModel
    {
        public string SkillName { get; set; }
        public string CategoryName { get; set; }
        public int KnownUpToStage { get; set; }
        public int TotalStages { get; set; }
        public string Status { get; set; }
    }

    public class UpdateProfileViewModel
    {
        // Personal info
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public string Profession { get; set; }
        public string Location { get; set; }
        public int Age { get; set; }

        // Skills
        public List<int> SkillsToLearn { get; set; } = new List<int>();

        public List<UserKnownSkill> SkillsIKnow { get; set; } = new List<UserKnownSkill>();

        public List<SkillCategory> AllSkillCategories { get; set; } = new List<SkillCategory>();

        public class UserKnownSkill
        {
            public int SkillId { get; set; }
            public int KnownUpToStage { get; set; }
        }
    }




    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
