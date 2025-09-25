﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

    // =====================================
    // VIEWMODELS
    // =====================================

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

    public class CommunityLandingViewModel
    {
        public int CommunityId { get; set; }
        public string CommunityName { get; set; }
        public string SkillName { get; set; }
        public bool IsMember { get; set; }
        public List<CommunityPostListItemViewModel> Posts { get; set; } = new List<CommunityPostListItemViewModel>();
    }

    public class CommunityPostListItemViewModel
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string CreatedByFullName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PostDetailsViewModel
    {
        public int PostId { get; set; }
        public int CommunityId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CreatedByUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsMember { get; set; }

        public List<CommunityCommentViewModel> Comments { get; set; } = new List<CommunityCommentViewModel>();

        public CommunityCommentCreateModel NewComment { get; set; } = new CommunityCommentCreateModel();
    }


    public class CommunityCommentViewModel
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public string CreatedByFullName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CommunityCommentCreateModel
    {
        [Required]
        public int PostId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; }
    }

    public class CommunityPostCreateModel
    {
        [Required]
        public int CommunityId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(2000)]
        public string Content { get; set; }
    }
}
