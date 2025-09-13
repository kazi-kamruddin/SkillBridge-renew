using System;
using System.Linq;
using System.Web.Mvc;
using SkillBridge.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

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

        // AJAX: Get notifications as JSON for badge
        [HttpGet]
        public JsonResult GetNotifications()
        {
            var userId = User.Identity.GetUserId();

            var notificationsFromDb = _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(10)
                .ToList();

            var notifications = notificationsFromDb.Select(n => new
            {
                n.Id,
                n.Message,
                n.IsRead,
                CreatedAt = n.CreatedAt.ToString("g"),
                Url = Url.Action("Index", "Notifications")
            }).ToList();

            var unreadCount = notifications.Count(n => !n.IsRead);
            return Json(new { notifications, unreadCount }, JsonRequestBehavior.AllowGet);
        }

        // POST: Mark notification as read
        [HttpPost]
        public ActionResult MarkAsRead(int id)
        {
            var userId = User.Identity.GetUserId();
            var notification = _context.Notifications
                .FirstOrDefault(n => n.Id == id && n.UserId == userId);

            if (notification == null) return HttpNotFound();

            notification.IsRead = true;
            _context.SaveChanges();
            return Json(new { success = true });
        }

        // POST: Decline skill request
        [HttpPost]
        public ActionResult DeclineSkillRequest(int notificationId)
        {
            var userId = User.Identity.GetUserId();
            var notif = _context.Notifications.FirstOrDefault(n => n.Id == notificationId && n.UserId == userId);
            if (notif == null) return Json(new { success = false });

            notif.IsRead = true;
            _context.SaveChanges();

            // Notify requester that request was declined
            var skillRequest = _context.SkillRequests.FirstOrDefault(r => r.Id == notif.ReferenceId);
            if (skillRequest != null)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = skillRequest.RequesterId,
                    Type = "Info",
                    Message = $"Your skill request for <b>{skillRequest.Skill.Name}</b> was declined by {User.Identity.Name}.",
                    CreatedAt = DateTime.Now,
                    IsRead = false
                });
                _context.SaveChanges();
            }

            return Json(new { success = true });
        }

        // GET: Accept request → return skills requester can teach
        [HttpGet]
        public JsonResult AcceptSkillRequest(int notificationId)
        {
            var userId = User.Identity.GetUserId();
            var notif = _context.Notifications.FirstOrDefault(n => n.Id == notificationId && n.UserId == userId);
            if (notif == null) return Json(new { skills = new object[0] }, JsonRequestBehavior.AllowGet);

            var skillRequest = _context.SkillRequests.FirstOrDefault(r => r.Id == notif.ReferenceId);
            if (skillRequest == null) return Json(new { skills = new object[0] }, JsonRequestBehavior.AllowGet);

            // Find skills requester knows that current user wants to learn
            var userSkills = _context.UserSkills
                .Where(us => us.UserId == skillRequest.RequesterId && us.Status == "Known")
                .Select(us => new { us.SkillId, us.Skill.Name })
                .ToList();

            return Json(new { skills = userSkills }, JsonRequestBehavior.AllowGet);
        }

        // POST: Initialize interaction after selecting skill
        [HttpPost]
        public ActionResult InitializeInteraction(int notificationId, int skillId)
        {
            var userId = User.Identity.GetUserId();
            var notif = _context.Notifications.FirstOrDefault(n => n.Id == notificationId && n.UserId == userId);
            if (notif == null) return Json(new { success = false });

            var skillRequest = _context.SkillRequests.FirstOrDefault(r => r.Id == notif.ReferenceId);
            if (skillRequest == null) return Json(new { success = false });

            // Create interaction using new model columns
            var interaction = new Interaction
            {
                User1Id = userId, // current user (teacher)
                User2Id = skillRequest.RequesterId, // requester
                SkillFromTeacherId = skillRequest.SkillId, // what current user teaches
                SkillFromRequesterId = skillId, // what requester teaches
                Status = "Ongoing",
                CreatedAt = DateTime.Now
            };
            _context.Interactions.Add(interaction);

            // Mark notification as read
            notif.IsRead = true;

            // Notify requester
            _context.Notifications.Add(new Notification
            {
                UserId = skillRequest.RequesterId,
                Type = "Info",
                Message = $"Your skill request for <b>{skillRequest.Skill.Name}</b> has been accepted by {User.Identity.Name}.",
                CreatedAt = DateTime.Now,
                IsRead = false
            });

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
