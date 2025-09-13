using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SkillBridge.Models
{
    public class InteractionDetailsViewModel
    {
        public int Id { get; set; }
        public string Status { get; set; }

        public InteractionSkillViewModel SkillRequested { get; set; }
        public InteractionSkillViewModel SkillOffered { get; set; }

        public List<InteractionSession> Sessions { get; set; }
    }

    public class InteractionSkillViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<SkillStage> Stages { get; set; }
    }

    public class InteractionRatingViewModel
    {
        public int InteractionId { get; set; }
        public string OtherUserName { get; set; }

        [Required]
        [Range(1, 10)]
        [Display(Name = "Rating (1-10)")]
        public int RatingValue { get; set; }

        [Display(Name = "Comment (optional)")]
        public string Comment { get; set; }
    }
}
