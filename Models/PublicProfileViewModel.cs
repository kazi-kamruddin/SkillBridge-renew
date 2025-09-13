using System.Collections.Generic;

namespace SkillBridge.Models
{
    public class PublicProfileViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Profession { get; set; }
        public string Location { get; set; }
        public string Bio { get; set; }

        public List<SkillViewModel> SkillsToTeach { get; set; }
        public List<SkillViewModel> SkillsToLearn { get; set; }
    }

    public class SkillViewModel
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public int Stage { get; set; }

        // New property for request status
        public string RequestStatus { get; set; } = "None"; // None | Pending | Declined
    }
}
