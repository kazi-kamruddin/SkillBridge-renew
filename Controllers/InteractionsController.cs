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

        
        ////////////////////////////////////////////////////////////////////////////
        
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


        ////////////////////////////////////////////////////////////////////////////
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



        ////////////////////////////////////////////////////////////////////////////


        [HttpPost]
        public ActionResult MarkStageDone(int stageNumber, int interactionId)
        {
            var userId = User.Identity.GetUserId();
            var session = db.InteractionSessions
                .Include(s => s.Interaction)
                .FirstOrDefault(s => s.InteractionId == interactionId && s.StageNumber == stageNumber);

            if (session == null) return HttpNotFound();

            bool stageWasAlreadyConfirmed = session.User1Confirmed && session.User2Confirmed;

            if (session.Interaction.User1Id == userId)
                session.User1Confirmed = true;
            else if (session.Interaction.User2Id == userId)
                session.User2Confirmed = true;

            // If both confirmed, mark session as confirmed
            if (session.User1Confirmed && session.User2Confirmed)
            {
                session.Status = "Confirmed";

                // Create notifications for both users
                var notif1 = new Notification
                {
                    UserId = session.Interaction.User1Id,
                    Type = "Info",
                    Message = $"Stage {session.StageNumber} of {session.Skill.Name} completed by {session.Interaction.User2.UserName}"
                };
                var notif2 = new Notification
                {
                    UserId = session.Interaction.User2Id,
                    Type = "Info",
                    Message = $"Stage {session.StageNumber} of {session.Skill.Name} completed by {session.Interaction.User1.UserName}"
                };
                db.Notifications.Add(notif1);
                db.Notifications.Add(notif2);
            }

            db.SaveChanges();

            // Return updated status to client
            return Json(new
            {
                success = true,
                status = session.Status
            });
        }


        ////////////////////////////////////////////////////////////////////////////
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


        ////////////////////////////////////////////////////////////////////////////


        // Show rating page
        public ActionResult RateInteraction(int interactionId)
        {
            var userId = User.Identity.GetUserId();
            var interaction = db.Interactions
                .Include(i => i.SkillFromRequester)
                .Include(i => i.SkillFromTeacher)
                .Include(i => i.User1)
                .Include(i => i.User2)
                .FirstOrDefault(i => i.Id == interactionId && i.Status == "Completed");

            if (interaction == null) return HttpNotFound();

            // Determine which skill the current user learned
            string skillName = interaction.User1Id == userId ? interaction.SkillFromTeacher.Name : interaction.SkillFromRequester.Name;
            string fromUserName = interaction.User1Id == userId ? interaction.User2.UserName : interaction.User1.UserName;
            string toUserId = userId;

            var model = new InteractionRatingViewModel
            {
                InteractionId = interaction.Id,
                SkillName = skillName,
                FromUserName = fromUserName,
                ToUserId = toUserId
            };

            return View(model);
        }

        // Submit rating via AJAX
        [HttpPost]
        public ActionResult SubmitRating(InteractionRatingViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors) });

            var rating = new Rating
            {
                InteractionId = model.InteractionId,
                FromUserId = model.ToUserId,
                ToUserId = db.Interactions.Find(model.InteractionId).User1Id == model.ToUserId
                           ? db.Interactions.Find(model.InteractionId).User2Id
                           : db.Interactions.Find(model.InteractionId).User1Id,
                RatingValue = model.RatingValue,
                Comment = model.Comment
            };

            db.Ratings.Add(rating);

            // Remove the feedback notification
            var notif = db.Notifications.FirstOrDefault(n => n.UserId == model.ToUserId && n.Type == "Feedback" && n.ReferenceId == model.InteractionId);
            if (notif != null) db.Notifications.Remove(notif);

            db.SaveChanges();

            return Json(new { success = true });
        }



        ////////////////////////////////////////////////////////////////////////////
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


        ////////////////////////////////////////////////////////////////////////////
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

        ////////////////////////////////////////////////////////////////////////////

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





        ////////////////////////////////////////////////////////////////////////////

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
