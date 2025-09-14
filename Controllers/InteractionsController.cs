using System.Linq;
using System.Web.Mvc;
using SkillBridge.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Collections.Generic;

namespace SkillBridge.Controllers
{
    [Authorize]
    public class InteractionsController : Controller
    {
        private readonly ApplicationDbContext db;

        public InteractionsController()
        {
            db = new ApplicationDbContext();
        }

        // Interaction Index Page
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var interactions = db.Interactions
                .Where(i => i.User1Id == userId || i.User2Id == userId)
                .Where(i => i.Status != "Completed")
                .Include(i => i.User1)
                .Include(i => i.User2)
                .Include(i => i.SkillFromRequester)
                .Include(i => i.SkillFromTeacher)
                .ToList();

            // Map to ViewModel
            var model = interactions.Select(i => new InteractionIndexViewModel
            {
                InteractionId = i.Id,
                Status = i.Status,
                OtherUserName = i.User1Id == userId ? i.User2.UserName : i.User1.UserName,
                SkillYouLearn = i.User1Id == userId ? i.SkillFromTeacher.Name : i.SkillFromRequester.Name,
                SkillYouTeach = i.User1Id == userId ? i.SkillFromRequester.Name : i.SkillFromTeacher.Name
            }).ToList();

            return View(model);
        }

        // Interaction Sessions Page
        public ActionResult Sessions(int id)
        {
            var interaction = db.Interactions
                .Include(i => i.Sessions.Select(s => s.Skill))
                .Include(i => i.SkillFromRequester.SkillStages)
                .Include(i => i.SkillFromTeacher.SkillStages)
                .FirstOrDefault(i => i.Id == id);

            if (interaction == null) return HttpNotFound();

            var model = new InteractionSessionsViewModel
            {
                InteractionId = interaction.Id,
                UserId = User.Identity.GetUserId(),
                SkillBlocks = BuildSkillBlocks(interaction)
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult MarkStageDone(int stageNumber, int interactionId)
        {
            var userId = User.Identity.GetUserId();
            var session = db.InteractionSessions
                .FirstOrDefault(s => s.InteractionId == interactionId && s.StageNumber == stageNumber);

            if (session == null) return HttpNotFound();

            if (session.Interaction.User1Id == userId)
                session.User1Confirmed = true;
            else if (session.Interaction.User2Id == userId)
                session.User2Confirmed = true;

            db.SaveChanges();
            return Json(new { success = true });
        }

        // End Interaction
        public ActionResult EndInteraction(int id)
        {
            var interaction = db.Interactions
                .Include(i => i.Sessions)
                .FirstOrDefault(i => i.Id == id);

            if (interaction == null) return HttpNotFound();

            string userId = User.Identity.GetUserId();
            string learningUserId = interaction.User1Id == userId ? interaction.User2Id : interaction.User1Id;

            int skillId = interaction.User1Id == learningUserId ? interaction.SkillFromRequesterId : interaction.SkillFromTeacherId;

            // Update learning user's skill
            var learningUserSkill = db.UserSkills.FirstOrDefault(us => us.UserId == learningUserId && us.SkillId == skillId);
            if (learningUserSkill != null)
            {
                learningUserSkill.Status = "Teaching";
                learningUserSkill.KnownUpToStage = MaxStageCompleted(interaction, skillId);
            }

            interaction.Status = "Completed";
            db.SaveChanges();

            CreateFeedbackNotification(interaction);

            return RedirectToAction("Index");
        }

        // Helper Methods
        private List<SkillStageBlock> BuildSkillBlocks(Interaction interaction)
        {
            var userId = User.Identity.GetUserId();
            var blocks = new List<SkillStageBlock>();

            foreach (var session in interaction.Sessions)
            {
                bool confirmed = (session.Interaction.User1Id == userId && session.User1Confirmed) ||
                                 (session.Interaction.User2Id == userId && session.User2Confirmed);

                string status = "Red";
                if (session.User1Confirmed || session.User2Confirmed) status = "Yellow";
                if (session.User1Confirmed && session.User2Confirmed) status = "Green";

                blocks.Add(new SkillStageBlock
                {
                    StageNumber = session.StageNumber,
                    Description = session.Skill.Description,
                    Status = status,
                    UserConfirmed = confirmed
                });
            }

            return blocks.OrderBy(b => b.StageNumber).ToList();
        }

        private int MaxStageCompleted(Interaction interaction, int skillId)
        {
            var sessions = interaction.Sessions.Where(s => s.SkillId == skillId).ToList();
            if (!sessions.Any()) return 0;

            return sessions
                .Where(s => s.User1Confirmed && s.User2Confirmed)
                .Select(s => s.StageNumber)
                .DefaultIfEmpty(0)
                .Max();
        }

        private void CreateFeedbackNotification(Interaction interaction)
        {
            var notif1 = new Notification
            {
                UserId = interaction.User1Id,
                Type = "Feedback",
                Message = $"Rate your experience learning {interaction.SkillFromTeacher.Name} from {interaction.User2.UserName}"
            };

            var notif2 = new Notification
            {
                UserId = interaction.User2Id,
                Type = "Feedback",
                Message = $"Rate your experience learning {interaction.SkillFromRequester.Name} from {interaction.User1.UserName}"
            };

            db.Notifications.Add(notif1);
            db.Notifications.Add(notif2);
            db.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
