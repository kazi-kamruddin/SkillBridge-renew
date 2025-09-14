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
            if (notif == null || notif.Type != "SkillRequest") return Json(new { success = false });

            var skillRequest = _context.SkillRequests.FirstOrDefault(r => r.Id == notif.ReferenceId);
            if (skillRequest != null && skillRequest.Status == "Pending")
            {
                skillRequest.Status = "Declined";

                notif.Type = "Info";
                notif.Message = $"You declined the skill request for <b>{skillRequest.Skill.Name}</b> from {skillRequest.Requester.UserName}.";
                notif.IsRead = true;

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

        // GET: Accept request → return skills requester can teach that current user wants to learn
        [HttpGet]
        public JsonResult AcceptSkillRequest(int notificationId)
        {
            var userId = User.Identity.GetUserId();
            var notif = _context.Notifications.FirstOrDefault(n => n.Id == notificationId && n.UserId == userId);
            if (notif == null || notif.Type != "SkillRequest") return Json(new { skills = new object[0] }, JsonRequestBehavior.AllowGet);

            var skillRequest = _context.SkillRequests
                .Include(r => r.Skill)
                .FirstOrDefault(r => r.Id == notif.ReferenceId && r.Status == "Pending");
            if (skillRequest == null) return Json(new { skills = new object[0] }, JsonRequestBehavior.AllowGet);

            var requesterId = skillRequest.RequesterId;
            var receiverId = userId;

            var requesterSkills = _context.UserSkills
                .Include(us => us.Skill)
                .Where(us => us.UserId == requesterId && us.Status == "Teaching");

            var receiverSkills = _context.UserSkills
                .Where(us => us.UserId == receiverId && us.Status == "Learning");

            var matchingSkills = requesterSkills
                .Where(rs => receiverSkills.Any(ls => ls.SkillId == rs.SkillId))
                .Select(rs => new { rs.SkillId, rs.Skill.Name })
                .ToList();

            return Json(new { skills = matchingSkills }, JsonRequestBehavior.AllowGet);
        }




        // POST: Initialize interaction after selecting skill
        [HttpPost]
        public ActionResult InitializeInteraction(int notificationId, int skillId)
        {
            var userId = User.Identity.GetUserId();
            var notif = _context.Notifications.FirstOrDefault(n => n.Id == notificationId && n.UserId == userId);
            if (notif == null || notif.Type != "SkillRequest") return Json(new { success = false });

            var skillRequest = _context.SkillRequests
                .Include(r => r.Skill) // include skill for name
                .Include(r => r.Requester) // include requester for name
                .FirstOrDefault(r => r.Id == notif.ReferenceId && r.Status == "Pending");
            if (skillRequest == null) return Json(new { success = false });

            // Create the interaction
            var interaction = new Interaction
            {
                User1Id = userId, // the current user (receiver)
                User2Id = skillRequest.RequesterId, // the requester
                SkillFromTeacherId = skillRequest.SkillId, // the skill requester can teach
                SkillFromRequesterId = skillId, // the skill current user wants to learn
                Status = "Ongoing",
                CreatedAt = DateTime.Now
            };
            _context.Interactions.Add(interaction);
            _context.SaveChanges(); // Save first to generate Interaction.Id

            // -------------------------------
            // Initialize InteractionSessions for both skills
            // -------------------------------

            // Skill the requester teaches (User2 teaches)
            var teacherSkillStages = _context.SkillStages
                .Where(ss => ss.SkillId == skillRequest.SkillId)
                .OrderBy(ss => ss.StageNumber)
                .ToList();

            foreach (var stage in teacherSkillStages)
            {
                _context.InteractionSessions.Add(new InteractionSession
                {
                    InteractionId = interaction.Id,
                    SkillId = skillRequest.SkillId,
                    StageNumber = stage.StageNumber,
                    Status = "Pending",
                    User1Confirmed = false,
                    User2Confirmed = false
                });
            }

            // Skill the current user teaches (User1 teaches)
            var requesterSkillStages = _context.SkillStages
                .Where(ss => ss.SkillId == skillId)
                .OrderBy(ss => ss.StageNumber)
                .ToList();

            foreach (var stage in requesterSkillStages)
            {
                _context.InteractionSessions.Add(new InteractionSession
                {
                    InteractionId = interaction.Id,
                    SkillId = skillId,
                    StageNumber = stage.StageNumber,
                    Status = "Pending",
                    User1Confirmed = false,
                    User2Confirmed = false
                });
            }

            // Update the skill request status
            skillRequest.Status = "Accepted";

            // Update the notification for the receiver
            notif.Type = "Info";
            notif.Message = $"You accepted the skill request for <b>{skillRequest.Skill.Name}</b> from {skillRequest.Requester.UserName}.";
            notif.IsRead = true;

            // Add notification for the requester
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


        [HttpGet]
        public JsonResult GetRealtimeNotifications()
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
                n.Type,
                CreatedAt = n.CreatedAt.ToString("g"),
                Url = Url.Action("Index", "Notifications")
            }).ToList();

            var unreadCount = notifications.Count(n => !n.IsRead);
            return Json(new { notifications, unreadCount }, JsonRequestBehavior.AllowGet);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing) _context.Dispose();
            base.Dispose(disposing);
        }
    }
}
