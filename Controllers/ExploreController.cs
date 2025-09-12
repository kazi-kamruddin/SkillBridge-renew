using Microsoft.AspNet.Identity;
using SkillBridge.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

public class ExploreController : Controller
{
    private ApplicationDbContext db = new ApplicationDbContext();

    public ActionResult Index()
    {
        var currentUserId = User.Identity.GetUserId();

        // TODO: Later: implement best match / partial match logic
        var model = new ExploreViewModel
        {
            BestMatches = new List<PublicProfileViewModel>(), // empty for now
            PartialMatches = new List<PublicProfileViewModel>()
        };

        return View(model);
    }
}
