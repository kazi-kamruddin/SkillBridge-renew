using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SkillBridge.Models
{
    public class InteractionIndexViewModel
    {
        public int InteractionId { get; set; }
        public string OtherUserName { get; set; }
        public string SkillYouTeach { get; set; }
        public string SkillYouLearn { get; set; }
        public string Status { get; set; }
    }

    public class SkillStageBlock
    {
        public int StageNumber { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } // Red, Yellow, Green
        public bool UserConfirmed { get; set; }
    }

    public class InteractionSessionsViewModel
    {
        public int InteractionId { get; set; }
        public string UserId { get; set; }
        public List<SkillStageBlock> SkillBlocks { get; set; }
    }

}
