using SkillBridge.Models;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace SkillBridge.Controllers
{
    [Authorize]
    public class CommunitiesController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            var userSkills = db.UserSkills
                .Where(us => us.UserId == userId)
                .ToList();

            var teachingSkillIds = userSkills
                .Where(us => us.Status == "Teaching")
                .Select(us => us.SkillId)
                .ToList();

            var learningSkillIds = userSkills
                .Where(us => us.Status == "Learning")
                .Select(us => us.SkillId)
                .ToList();

            var allCommunities = db.Communities
                .Select(c => new
                {
                    c.Id,
                    SkillId = c.Skill.Id,
                    SkillName = c.Skill.Name,
                    CategoryName = c.Skill.SkillCategory.Name
                }).ToList();

            var model = new CommunityIndexViewModel
            {
                SkillsYouKnow = allCommunities
                    .Where(c => teachingSkillIds.Contains(c.SkillId))
                    .Select(c => new CommunityViewModel
                    {
                        CommunityId = c.Id,
                        SkillName = c.SkillName,
                        CategoryName = c.CategoryName,
                        IsMember = true
                    }).ToList() ?? new System.Collections.Generic.List<CommunityViewModel>(),

                SkillsYouWantToLearn = allCommunities
                    .Where(c => learningSkillIds.Contains(c.SkillId))
                    .Select(c => new CommunityViewModel
                    {
                        CommunityId = c.Id,
                        SkillName = c.SkillName,
                        CategoryName = c.CategoryName,
                        IsMember = true
                    }).ToList() ?? new System.Collections.Generic.List<CommunityViewModel>(),

                OtherCommunities = allCommunities
                    .Where(c => !teachingSkillIds.Contains(c.SkillId) && !learningSkillIds.Contains(c.SkillId))
                    .Select(c => new CommunityViewModel
                    {
                        CommunityId = c.Id,
                        SkillName = c.SkillName,
                        CategoryName = c.CategoryName,
                        IsMember = false
                    }).ToList() ?? new System.Collections.Generic.List<CommunityViewModel>()
            };

            return View(model);
        }
    }
}
