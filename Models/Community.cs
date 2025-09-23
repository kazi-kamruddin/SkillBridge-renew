using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SkillBridge.Models
{
    public class Community
    {
        public int Id { get; set; }
        public int SkillId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }


        public virtual Skill Skill { get; set; }
        public virtual ICollection<CommunityPost> Posts { get; set; }
    }



    public class CommunityPost
    {
        public int Id { get; set; }
        public int CommunityId { get; set; }
        public string CreatedByUserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public virtual Community Community { get; set; }
        public virtual ApplicationUser CreatedByUser { get; set; }
        public virtual ICollection<CommunityComment> Comments { get; set; }
    }



    public class CommunityComment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string CreatedByUserId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual CommunityPost Post { get; set; }
        public virtual ApplicationUser CreatedByUser { get; set; }
    }



    public class CommunityIndexViewModel
    {
        public List<CommunityViewModel> SkillsYouKnow { get; set; } = new List<CommunityViewModel>();
        public List<CommunityViewModel> SkillsYouWantToLearn { get; set; } = new List<CommunityViewModel>();
        public List<CommunityViewModel> OtherCommunities { get; set; } = new List<CommunityViewModel>();
    }

    public class CommunityViewModel
    {
        public int CommunityId { get; set; }
        public string SkillName { get; set; }
        public string CategoryName { get; set; }
        public bool IsMember { get; set; } 
    }
}