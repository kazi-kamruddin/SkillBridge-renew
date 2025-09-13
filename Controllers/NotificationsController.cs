using System.Linq;
using System.Web.Mvc;
using SkillBridge.Models;
using Microsoft.AspNet.Identity;

namespace SkillBridge.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: /Notifications
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var notifications = _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            return View(notifications);
        }

        // POST: /Notifications/MarkAsRead/5
        [HttpPost]
        public ActionResult MarkAsRead(int id)
        {
            var userId = User.Identity.GetUserId();
            var notification = _context.Notifications
                .FirstOrDefault(n => n.Id == id && n.UserId == userId);

            if (notification == null)
                return HttpNotFound();

            notification.IsRead = true;
            _context.SaveChanges();

            return Json(new { success = true });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _context.Dispose();
            base.Dispose(disposing);
        }
    }
}
