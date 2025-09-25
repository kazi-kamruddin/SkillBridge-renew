﻿using Microsoft.AspNet.Identity;
using SkillBridge.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SkillBridge.Controllers
{
    [Authorize]
    public class CommunitiesController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var currentUserId = User.Identity.GetUserId();

            var userSkills = db.UserSkills
                .Where(us => us.UserId == currentUserId)
                .Include(us => us.Skill)
                .ToList();

            var teachingSkills = userSkills
                .Where(us => us.Status == "Teaching")
                .Select(us => us.Skill)
                .ToList();

            var learningSkills = userSkills
                .Where(us => us.Status == "Learning")
                .Select(us => us.Skill)
                .ToList();

            var allCommunities = db.Communities.Include(c => c.Skill).Include(c => c.Skill.SkillCategory).ToList();

            var model = new CommunityIndexViewModel
            {
                SkillsYouKnow = allCommunities
                    .Where(c => teachingSkills.Contains(c.Skill))
                    .Select(c => new CommunityViewModel
                    {
                        CommunityId = c.Id,
                        SkillName = c.Skill.Name,
                        CategoryName = c.Skill.SkillCategory.Name,
                        IsMember = true
                    }).ToList(),

                SkillsYouWantToLearn = allCommunities
                    .Where(c => learningSkills.Contains(c.Skill) && !teachingSkills.Contains(c.Skill))
                    .Select(c => new CommunityViewModel
                    {
                        CommunityId = c.Id,
                        SkillName = c.Skill.Name,
                        CategoryName = c.Skill.SkillCategory.Name,
                        IsMember = true
                    }).ToList(),

                OtherCommunities = allCommunities
                    .Where(c => !teachingSkills.Contains(c.Skill) && !learningSkills.Contains(c.Skill))
                    .Select(c => new CommunityViewModel
                    {
                        CommunityId = c.Id,
                        SkillName = c.Skill.Name,
                        CategoryName = c.Skill.SkillCategory.Name,
                        IsMember = false
                    }).ToList()
            };

            return View(model);
        }


        public ActionResult Landing(int id)
        {
            var community = db.Communities.Include(c => c.Skill).FirstOrDefault(c => c.Id == id);
            if (community == null) return HttpNotFound();

            var currentUserId = User.Identity.GetUserId();
            bool isMember = db.UserSkills.Any(us => us.UserId == currentUserId && us.SkillId == community.SkillId);

            var posts = db.CommunityPosts
                .Where(p => p.CommunityId == id)
                .OrderByDescending(p => p.CreatedAt)
                .ToList()
                .Select(p => new CommunityPostListItemViewModel
                {
                    PostId = p.Id,
                    Title = p.Title,
                    CreatedByFullName = db.UserInformations
                        .Where(ui => ui.UserId == p.CreatedByUserId)
                        .Select(ui => ui.FullName)
                        .FirstOrDefault() ?? "Unknown",
                    CreatedAt = p.CreatedAt
                })
                .ToList();



            var model = new CommunityLandingViewModel
            {
                CommunityId = community.Id,
                CommunityName = community.Name,
                SkillName = community.Skill.Name,
                IsMember = isMember,
                Posts = posts
            };

            return View(model);
        }


        public ActionResult CreatePost(int communityId)
        {
            var currentUserId = User.Identity.GetUserId();
            var community = db.Communities.Find(communityId);
            if (community == null) return HttpNotFound();

            bool isMember = db.UserSkills.Any(us => us.UserId == currentUserId && us.SkillId == community.SkillId);
            if (!isMember) return new HttpUnauthorizedResult();

            var model = new CommunityPostCreateModel
            {
                CommunityId = communityId
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePost(CommunityPostCreateModel model)
        {
            var currentUserId = User.Identity.GetUserId();
            var community = db.Communities.Find(model.CommunityId);
            if (community == null) return HttpNotFound();

            bool isMember = db.UserSkills.Any(us => us.UserId == currentUserId && us.SkillId == community.SkillId);
            if (!isMember) return new HttpUnauthorizedResult();

            var post = new CommunityPost
            {
                CommunityId = model.CommunityId,
                CreatedByUserId = currentUserId,
                Title = model.Title,
                Content = model.Content,
                CreatedAt = DateTime.UtcNow
            };

            db.CommunityPosts.Add(post);
            db.SaveChanges();

            return RedirectToAction("Landing", new { id = model.CommunityId });
        }



        public ActionResult PostDetails(int id)
        {
            var post = db.CommunityPosts
                .Include(p => p.Community)
                .Include(p => p.Comments)
                .FirstOrDefault(p => p.Id == id);

            if (post == null) return HttpNotFound();

            var currentUserId = User.Identity.GetUserId();
            bool isMember = db.UserSkills.Any(us => us.UserId == currentUserId && us.SkillId == post.Community.SkillId);

            var model = new PostDetailsViewModel
            {
                PostId = post.Id,
                CommunityId = post.CommunityId,
                Title = post.Title,
                Content = post.Content,
                CreatedByUserName = db.UserInformations
                .Where(ui => ui.UserId == post.CreatedByUserId)
                .Select(ui => ui.FullName)
                .FirstOrDefault() ?? "Unknown",
                        CreatedAt = post.CreatedAt,
                        IsMember = isMember,
                        Comments = post.Comments.OrderBy(c => c.CreatedAt)
                .Select(c => new CommunityCommentViewModel
                {
                    CommentId = c.Id,
                    Content = c.Content,
                    CreatedByFullName = db.UserInformations
                        .Where(ui => ui.UserId == c.CreatedByUserId)
                        .Select(ui => ui.FullName)
                        .FirstOrDefault() ?? "Unknown",
                    CreatedAt = c.CreatedAt
                }).ToList(),
                NewComment = new CommunityCommentCreateModel
                {
                    PostId = post.Id
                }
            };


            return View(model);
        }



        //[HttpGet]
        //public ActionResult CreateComment()
        //{
        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateComment(CommunityCommentCreateModel model)
        {
            var currentUserId = User.Identity.GetUserId();

            var post = db.CommunityPosts
                .Include(p => p.Community)
                .FirstOrDefault(p => p.Id == model.PostId);

            if (post == null) return HttpNotFound();

            bool isMember = db.UserSkills.Any(us => us.UserId == currentUserId && us.SkillId == post.Community.SkillId);
            if (!isMember) return new HttpUnauthorizedResult();

            var comment = new CommunityComment
            {
                PostId = model.PostId,
                CreatedByUserId = currentUserId,
                Content = model.Content,
                CreatedAt = DateTime.UtcNow
            };

            db.CommunityComments.Add(comment);
            db.SaveChanges();

            return RedirectToAction("PostDetails", new { id = model.PostId });
        }

    }
}
