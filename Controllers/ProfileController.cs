using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SkillBridge.Models;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SkillBridge.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;

        private ApplicationDbContext db = new ApplicationDbContext();

        public ProfileController() { }

        public ProfileController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public ApplicationSignInManager SignInManager
        {
            get => _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            private set => _signInManager = value;
        }

        // GET: /Profile/Index
        public async Task<ActionResult> Index()
        {
            var userId = User.Identity.GetUserId();

            var hasPassword = await UserManager.HasPasswordAsync(userId);

            var user = await UserManager.FindByIdAsync(userId);
            var userInfo = db.UserInformations.FirstOrDefault(u => u.UserId == userId);

            var userSkills = db.UserSkills
                .Where(us => us.UserId == userId)
                .Select(us => new UserSkillViewModel
                {
                    SkillName = us.Skill.Name,
                    CategoryName = us.Skill.SkillCategory.Name,
                    KnownUpToStage = us.KnownUpToStage ?? 0,
                    TotalStages = us.Skill.SkillStages.Count(),
                    Status = us.Status
                }).ToList();

            var teachingSkills = userSkills.Where(s => s.Status == "Teaching").ToList();
            var learningSkills = userSkills.Where(s => s.Status == "Learning").ToList();

            var model = new IndexViewModel
            {
                HasPassword = hasPassword,
                FullName = userInfo?.FullName ?? "",
                Email = user.Email,
                Bio = userInfo?.Bio ?? "",
                Profession = userInfo?.Profession ?? "",
                Location = userInfo?.Location ?? "",
                Age = userInfo?.Age ?? 0,
                TeachingSkills = teachingSkills,
                LearningSkills = learningSkills
            };

            return View(model);
        }


        // GET: /Profile/UpdateProfile
        public async Task<ActionResult> UpdateProfile()
        {
            var userId = User.Identity.GetUserId();
            var user = await UserManager.FindByIdAsync(userId);
            var userInfo = db.UserInformations.FirstOrDefault(u => u.UserId == userId);
            var userSkills = db.UserSkills.Where(us => us.UserId == userId).ToList();

            var skillCategories = db.SkillCategories
                .Include("Skills.SkillStages")
                .ToList();

            var model = new UpdateProfileViewModel
            {
                FullName = userInfo.FullName,
                Email = user.Email,
                Bio = userInfo.Bio,
                Profession = userInfo.Profession,
                Location = userInfo.Location,
                Age = userInfo.Age,
                SkillsToLearn = userSkills.Where(s => s.Status == "Learning").Select(s => s.SkillId).ToList(),
                SkillsIKnow = userSkills.Where(s => s.Status == "Teaching").Select(s => new UpdateProfileViewModel.UserKnownSkill
                {
                    SkillId = s.SkillId,
                    KnownUpToStage = s.KnownUpToStage ?? 1
                }).ToList(),
                AllSkillCategories = skillCategories
            };

            return View(model);
        }


        // POST: /Profile/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AllSkillCategories = db.SkillCategories.Include("Skills.SkillStages").ToList();
                return View(model);
            }

            var userId = User.Identity.GetUserId();
            var userInfo = db.UserInformations.FirstOrDefault(u => u.UserId == userId);

            userInfo.FullName = model.FullName;
            userInfo.Bio = model.Bio;
            userInfo.Profession = model.Profession;
            userInfo.Location = model.Location;
            userInfo.Age = model.Age;

            var existingSkills = db.UserSkills.Where(us => us.UserId == userId).ToList();
            db.UserSkills.RemoveRange(existingSkills);

            if (model.SkillsToLearn != null)
            {
                foreach (var skillId in model.SkillsToLearn.Distinct())
                {
                    db.UserSkills.Add(new UserSkill
                    {
                        UserId = userId,
                        SkillId = skillId,
                        Status = "Learning"
                    });
                }
            }

            if (model.SkillsIKnow != null)
            {
                foreach (var sk in model.SkillsIKnow)
                {
                    db.UserSkills.Add(new UserSkill
                    {
                        UserId = userId,
                        SkillId = sk.SkillId,
                        Status = "Teaching",
                        KnownUpToStage = sk.KnownUpToStage
                    });
                }
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }




        // GET: /Profile/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: /Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                ViewBag.StatusMessage = "Your password has been changed successfully.";
                return View();
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error);

            return View(model);
        }

        //////////////////////////////////////////////////////////////////

        public ActionResult PublicProfile(string id)
        {
            if (id == null) return HttpNotFound();

            // Fetch user basic info
            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return HttpNotFound();

            var userInfo = db.UserInformations.FirstOrDefault(ui => ui.UserId == id);

            // Fetch user skills
            var userSkills = db.UserSkills
                .Include(us => us.Skill.SkillCategory)
                .Where(us => us.UserId == id)
                .ToList();

            var model = new PublicProfileViewModel
            {
                UserId = user.Id,
                FullName = userInfo?.FullName ?? "",
                Profession = userInfo?.Profession ?? "",
                Location = userInfo?.Location ?? "",
                Bio = userInfo?.Bio ?? "",
                SkillsToTeach = userSkills
                    .Where(us => us.Status == "Teaching")
                    .Select(us => new SkillViewModel
                    {
                        SkillId = us.SkillId,
                        SkillName = us.Skill.Name,
                        Stage = us.KnownUpToStage ?? 1
                    }).ToList(),
                SkillsToLearn = userSkills
                    .Where(us => us.Status == "Learning")
                    .Select(us => new SkillViewModel
                    {
                        SkillId = us.SkillId,
                        SkillName = us.Skill.Name,
                        Stage = us.KnownUpToStage ?? 1
                    }).ToList()
            };

            return View(model);
        }





        //////////////////////////////////////////////////////////////////

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _userManager?.Dispose();
                _signInManager?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
