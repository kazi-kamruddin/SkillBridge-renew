using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace SkillBridge.Models
{    public class ApplicationUser : IdentityUser
    {
        // Generates the ClaimsIdentity for login cookies
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
    }



    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<SkillCategory> SkillCategories { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<SkillStage> SkillStages { get; set; }
        public DbSet<UserInformation> UserInformations { get; set; }
        public DbSet<UserSkill> UserSkills { get; set; }
        public DbSet<Interaction> Interactions { get; set; }
        public DbSet<InteractionSession> InteractionSessions { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Notification> Notifications { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Interactions → AspNetUsers
            modelBuilder.Entity<Interaction>()
                .HasRequired(i => i.Requester)
                .WithMany()
                .HasForeignKey(i => i.RequesterId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Interaction>()
                .HasRequired(i => i.Receiver)
                .WithMany()
                .HasForeignKey(i => i.ReceiverId)
                .WillCascadeOnDelete(false);

            // Ratings → AspNetUsers
            modelBuilder.Entity<Rating>()
                .HasRequired(r => r.FromUser)
                .WithMany()
                .HasForeignKey(r => r.FromUserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Rating>()
                .HasRequired(r => r.ToUser)
                .WithMany()
                .HasForeignKey(r => r.ToUserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Interaction>()
                .HasRequired(i => i.SkillRequested)
                .WithMany()
                .HasForeignKey(i => i.SkillRequestedId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Interaction>()
                .HasRequired(i => i.SkillOffered)
                .WithMany()
                .HasForeignKey(i => i.SkillOfferedId)
                .WillCascadeOnDelete(false);
        }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
