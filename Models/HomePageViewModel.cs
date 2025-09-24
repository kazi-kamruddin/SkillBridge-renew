using System;
using System.Collections.Generic;

namespace SkillBridge.Models
{
    public class HomePageViewModel
    {
        public List<SkillCategory> SkillCategories { get; set; }

        public List<UserSkill> MySkills { get; set; }
        public CommunityPost MyLatestPost { get; set; }
        public CommunityPost OtherLatestPost { get; set; }

        public int? LatestInteractionId { get; set; }
        public string LatestInteractionOtherUser { get; set; }
        public string LatestInteractionSkillYouTeach { get; set; }
        public string LatestInteractionSkillYouLearn { get; set; }
        public string LatestInteractionStatus { get; set; }

        public bool IsLoggedIn { get; set; }
    }

}
