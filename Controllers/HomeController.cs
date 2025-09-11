using SkillBridge.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace SkillBridge.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var categories = db.SkillCategories
                .Include("Skills.SkillStages") 
                .ToList()
                .Select(c => new SkillCategory
                {
                    Id = c.Id,
                    Name = c.Name,
                    Skills = c.Skills
                        .Where(s => s.Name.Length >= 4)
                        .Select(s => new Skill
                        {
                            Id = s.Id,
                            Name = s.Name,
                            SkillCategoryId = s.SkillCategoryId,
                            SkillStages = s.SkillStages.ToList()
                        }).ToList()
                })
                .Where(c => c.Skills.Any())
                .ToList();

            return View(categories);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}
