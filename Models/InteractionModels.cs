using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillBridge.Models
{
    public class Interaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string RequesterId { get; set; }
        [ForeignKey("RequesterId")]
        public virtual ApplicationUser Requester { get; set; }

        [Required]
        public string ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public virtual ApplicationUser Receiver { get; set; }

        [Required]
        public int SkillOfferedId { get; set; }
        [ForeignKey("SkillOfferedId")]
        public virtual Skill SkillOffered { get; set; }

        [Required]
        public int SkillRequestedId { get; set; }
        [ForeignKey("SkillRequestedId")]
        public virtual Skill SkillRequested { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<InteractionSession> Sessions { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
    }

    public class InteractionSession
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int InteractionId { get; set; }
        [ForeignKey("InteractionId")]
        public virtual Interaction Interaction { get; set; }

        [Required]
        public int SkillId { get; set; }
        [ForeignKey("SkillId")]
        public virtual Skill Skill { get; set; }

        [Required]
        [Range(1, 7)]
        public int StageNumber { get; set; }

        public bool LearnerConfirmed { get; set; } = false;
        public bool MentorConfirmed { get; set; } = false;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";
    }

    public class Rating
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int InteractionId { get; set; }
        [ForeignKey("InteractionId")]
        public virtual Interaction Interaction { get; set; }

        [Required]
        public string FromUserId { get; set; }
        [ForeignKey("FromUserId")]
        public virtual ApplicationUser FromUser { get; set; }

        [Required]
        public string ToUserId { get; set; }
        [ForeignKey("ToUserId")]
        public virtual ApplicationUser ToUser { get; set; }

        [Required]
        [Range(1, 10)]
        public int RatingValue { get; set; }

        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }   

        public int? ReferenceId { get; set; }  
        [Required]
        public string Message { get; set; }

        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
