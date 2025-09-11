using Microsoft.AspNet.Identity;
using SkillBridge.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SkillBridge.Controllers
{
    [Authorize]
    public class CompleteProfileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /CompleteProfile/
        public ActionResult Index()
        {
            // Fetch all skill categories, skills, and stages (EF6 compatible)
            var skillData = db.SkillCategories
                .Include("Skills.SkillStages") // EF6 string-based Include
                .ToList();

            var model = new CompleteProfileViewModel
            {
                AllSkillCategories = skillData
            };

            return View(model);
        }

        // POST: /CompleteProfile/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(CompleteProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate skill data if validation fails
                model.AllSkillCategories = db.SkillCategories
                    .Include("Skills.SkillStages")
                    .ToList();
                return View(model);
            }

            var userId = User.Identity.GetUserId();

            // Save basic info
            var userInfo = new UserInformation
            {
                UserId = userId,
                FullName = model.FullName,
                Age = model.Age,
                Profession = model.Profession,
                Location = model.Location,
                Bio = model.Bio
            };
            db.UserInformations.Add(userInfo);

            // Save skills the user wants to learn
            if (model.SkillsToLearn != null && model.SkillsToLearn.Any())
            {
                var validSkillIds = db.Skills.Select(s => s.Id).ToHashSet();
                foreach (var skillId in model.SkillsToLearn.Distinct())
                {
                    if (validSkillIds.Contains(skillId))
                    {
                        db.UserSkills.Add(new UserSkill
                        {
                            UserId = userId,
                            SkillId = skillId,
                            Status = "Learning"
                        });
                    }
                }
            }

            // Save skills the user knows
            if (model.SkillsIKnow != null && model.SkillsIKnow.Any())
            {
                var validSkillIds = db.Skills.Select(s => s.Id).ToHashSet();
                foreach (var skillKnown in model.SkillsIKnow)
                {
                    if (skillKnown.SkillId > 0 && validSkillIds.Contains(skillKnown.SkillId))
                    {
                        db.UserSkills.Add(new UserSkill
                        {
                            UserId = userId,
                            SkillId = skillKnown.SkillId,
                            Status = "Teaching",
                            KnownUpToStage = skillKnown.KnownUpToStage
                        });
                    }
                }
            }

            db.SaveChanges();

            // Redirect to home after profile completion
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
