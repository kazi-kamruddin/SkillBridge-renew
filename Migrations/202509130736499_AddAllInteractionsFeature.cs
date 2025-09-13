namespace SkillBridge.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAllInteractionsFeature : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Interactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RequesterId = c.String(nullable: false, maxLength: 128),
                        ReceiverId = c.String(nullable: false, maxLength: 128),
                        SkillOfferedId = c.Int(nullable: false),
                        SkillRequestedId = c.Int(nullable: false),
                        Status = c.String(nullable: false, maxLength: 20),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ReceiverId)
                .ForeignKey("dbo.AspNetUsers", t => t.RequesterId)
                .ForeignKey("dbo.Skills", t => t.SkillOfferedId)
                .ForeignKey("dbo.Skills", t => t.SkillRequestedId)
                .Index(t => t.RequesterId)
                .Index(t => t.ReceiverId)
                .Index(t => t.SkillOfferedId)
                .Index(t => t.SkillRequestedId);
            
            CreateTable(
                "dbo.Ratings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InteractionId = c.Int(nullable: false),
                        FromUserId = c.String(nullable: false, maxLength: 128),
                        ToUserId = c.String(nullable: false, maxLength: 128),
                        RatingValue = c.Int(nullable: false),
                        Comment = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.FromUserId)
                .ForeignKey("dbo.Interactions", t => t.InteractionId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ToUserId)
                .Index(t => t.InteractionId)
                .Index(t => t.FromUserId)
                .Index(t => t.ToUserId);
            
            CreateTable(
                "dbo.InteractionSessions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InteractionId = c.Int(nullable: false),
                        SkillId = c.Int(nullable: false),
                        StageNumber = c.Int(nullable: false),
                        LearnerConfirmed = c.Boolean(nullable: false),
                        MentorConfirmed = c.Boolean(nullable: false),
                        Status = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Interactions", t => t.InteractionId, cascadeDelete: true)
                .ForeignKey("dbo.Skills", t => t.SkillId, cascadeDelete: true)
                .Index(t => t.InteractionId)
                .Index(t => t.SkillId);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Type = c.String(nullable: false, maxLength: 50),
                        ReferenceId = c.Int(),
                        Message = c.String(nullable: false),
                        IsRead = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Interactions", "SkillRequestedId", "dbo.Skills");
            DropForeignKey("dbo.Interactions", "SkillOfferedId", "dbo.Skills");
            DropForeignKey("dbo.InteractionSessions", "SkillId", "dbo.Skills");
            DropForeignKey("dbo.InteractionSessions", "InteractionId", "dbo.Interactions");
            DropForeignKey("dbo.Interactions", "RequesterId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Interactions", "ReceiverId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Ratings", "ToUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Ratings", "InteractionId", "dbo.Interactions");
            DropForeignKey("dbo.Ratings", "FromUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Notifications", new[] { "UserId" });
            DropIndex("dbo.InteractionSessions", new[] { "SkillId" });
            DropIndex("dbo.InteractionSessions", new[] { "InteractionId" });
            DropIndex("dbo.Ratings", new[] { "ToUserId" });
            DropIndex("dbo.Ratings", new[] { "FromUserId" });
            DropIndex("dbo.Ratings", new[] { "InteractionId" });
            DropIndex("dbo.Interactions", new[] { "SkillRequestedId" });
            DropIndex("dbo.Interactions", new[] { "SkillOfferedId" });
            DropIndex("dbo.Interactions", new[] { "ReceiverId" });
            DropIndex("dbo.Interactions", new[] { "RequesterId" });
            DropTable("dbo.Notifications");
            DropTable("dbo.InteractionSessions");
            DropTable("dbo.Ratings");
            DropTable("dbo.Interactions");
        }
    }
}
