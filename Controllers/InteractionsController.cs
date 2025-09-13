using System.Linq;
using System.Web.Mvc;
using SkillBridge.Models;
using Microsoft.AspNet.Identity;

namespace SkillBridge.Controllers
{
    [Authorize]
    public class InteractionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InteractionsController()
        {
            _context = new ApplicationDbContext();
        }

        ///////////////////////////////////////////////////////////////////////////

        // GET: Interactions
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            var interactions = _context.Interactions
                .Where(i => i.RequesterId == userId || i.ReceiverId == userId)
                .OrderByDescending(i => i.CreatedAt)
                .ToList();

            return View(interactions);
        }

        ///////////////////////////////////////////////////////////////////////////

        public ActionResult Details(int id)
        {
            var userId = User.Identity.GetUserId();

            var interaction = _context.Interactions
                .Where(i => i.Id == id && (i.RequesterId == userId || i.ReceiverId == userId))
                .Select(i => new InteractionDetailsViewModel
                {
                    Id = i.Id,
                    Status = i.Status,
                    SkillRequested = new InteractionSkillViewModel
                    {
                        Id = i.SkillRequested.Id,
                        Name = i.SkillRequested.Name,
                        Stages = i.SkillRequested.SkillStages.OrderBy(s => s.StageNumber).ToList()
                    },
                    SkillOffered = new InteractionSkillViewModel
                    {
                        Id = i.SkillOffered.Id,
                        Name = i.SkillOffered.Name,
                        Stages = i.SkillOffered.SkillStages.OrderBy(s => s.StageNumber).ToList()
                    },
                    Sessions = i.Sessions.ToList()
                })
                .FirstOrDefault();

            if (interaction == null)
                return HttpNotFound();

            return View(interaction);
        }

        ///////////////////////////////////////////////////////////////////////////

        [HttpPost]
        [Authorize]
        public ActionResult CompleteStage(int interactionId, int skillId, int stageNumber, string role)
        {
            var userId = User.Identity.GetUserId();

            var interaction = _context.Interactions.FirstOrDefault(i => i.Id == interactionId);
            if (interaction == null) return HttpNotFound();

            var session = _context.InteractionSessions
                .FirstOrDefault(s => s.InteractionId == interactionId && s.SkillId == skillId && s.StageNumber == stageNumber);

            if (session == null)
            {
                session = new InteractionSession
                {
                    InteractionId = interactionId,
                    SkillId = skillId,
                    StageNumber = stageNumber,
                    LearnerConfirmed = false,
                    MentorConfirmed = false,
                    Status = "Pending"
                };
                _context.InteractionSessions.Add(session);
            }

            if (role == "Learner") session.LearnerConfirmed = true;
            if (role == "Mentor") session.MentorConfirmed = true;

            _context.SaveChanges();

            // Trigger notification to the other user
            var otherUserId = role == "Learner" ? interaction.ReceiverId : interaction.RequesterId;
            var skill = _context.Skills.FirstOrDefault(s => s.Id == skillId);

            if (skill != null)
            {
                var notif = new Notification
                {
                    UserId = otherUserId,
                    Type = "StageCompleted",
                    ReferenceId = interactionId,
                    Message = $"{User.Identity.GetUserName()} marked stage {stageNumber} of {skill.Name} as completed."
                };
                _context.Notifications.Add(notif);
                _context.SaveChanges();
            }

            return RedirectToAction("Details", new { id = interactionId });
        }

        ///////////////////////////////////////////////////////////////////////////

        [HttpPost]
        [Authorize]
        public ActionResult CompleteInteraction(int id)
        {
            var userId = User.Identity.GetUserId();

            var interaction = _context.Interactions
                .Where(i => i.Id == id && (i.RequesterId == userId || i.ReceiverId == userId))
                .FirstOrDefault();

            if (interaction == null)
                return HttpNotFound();

            // Check if all stages are confirmed
            var allLearnerConfirmed = _context.InteractionSessions
                .Where(s => s.InteractionId == id && s.SkillId == interaction.SkillRequestedId)
                .All(s => s.LearnerConfirmed);

            var allMentorConfirmed = _context.InteractionSessions
                .Where(s => s.InteractionId == id && s.SkillId == interaction.SkillOfferedId)
                .All(s => s.MentorConfirmed);

            if (!allLearnerConfirmed || !allMentorConfirmed)
            {
                TempData["Error"] = "You cannot complete this interaction until all stages are confirmed by both users.";
                return RedirectToAction("Details", new { id = id });
            }

            interaction.Status = "Completed";
            _context.SaveChanges();

            // Notify the other user
            var otherUserId = interaction.RequesterId == userId ? interaction.ReceiverId : interaction.RequesterId;
            var notif = new Notification
            {
                UserId = otherUserId,
                Type = "InteractionCompleted",
                ReferenceId = interaction.Id,
                Message = $"{User.Identity.GetUserName()} completed the interaction. Please rate them."
            };
            _context.Notifications.Add(notif);
            _context.SaveChanges();

            return RedirectToAction("RateInteraction", new { id = id });
        }

        ///////////////////////////////////////////////////////////////////////////

        [Authorize]
        public ActionResult RateInteraction(int id)
        {
            var userId = User.Identity.GetUserId();

            var interaction = _context.Interactions
                .Where(i => i.Id == id && (i.RequesterId == userId || i.ReceiverId == userId))
                .FirstOrDefault();

            if (interaction == null)
                return HttpNotFound();

            var otherUserName = interaction.RequesterId == userId
                                ? interaction.Receiver.UserName
                                : interaction.Requester.UserName;

            var vm = new InteractionRatingViewModel
            {
                InteractionId = interaction.Id,
                OtherUserName = otherUserName
            };

            return View(vm);
        }

        ///////////////////////////////////////////////////////////////////////////

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult RateInteraction(InteractionRatingViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var userId = User.Identity.GetUserId();

            var interaction = _context.Interactions
                .Where(i => i.Id == vm.InteractionId && (i.RequesterId == userId || i.ReceiverId == userId))
                .FirstOrDefault();

            if (interaction == null)
                return HttpNotFound();

            var toUserId = interaction.RequesterId == userId ? interaction.ReceiverId : interaction.RequesterId;

            var rating = new Rating
            {
                InteractionId = vm.InteractionId,
                FromUserId = userId,
                ToUserId = toUserId,
                RatingValue = vm.RatingValue,
                Comment = vm.Comment
            };

            _context.Ratings.Add(rating);
            _context.SaveChanges();

            TempData["Success"] = "Rating submitted successfully!";
            return RedirectToAction("Index");
        }

        ///////////////////////////////////////////////////////////////////////////

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _context.Dispose();
            base.Dispose(disposing);
        }
    }
}
