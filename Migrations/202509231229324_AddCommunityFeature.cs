namespace SkillBridge.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCommunityFeature : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Communities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SkillId = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Skills", t => t.SkillId, cascadeDelete: true)
                .Index(t => t.SkillId);
            
            CreateTable(
                "dbo.CommunityPosts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CommunityId = c.Int(nullable: false),
                        CreatedByUserId = c.String(maxLength: 128),
                        Title = c.String(),
                        Content = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Communities", t => t.CommunityId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedByUserId)
                .Index(t => t.CommunityId)
                .Index(t => t.CreatedByUserId);
            
            CreateTable(
                "dbo.CommunityComments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PostId = c.Int(nullable: false),
                        CreatedByUserId = c.String(maxLength: 128),
                        Content = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedByUserId)
                .ForeignKey("dbo.CommunityPosts", t => t.PostId, cascadeDelete: true)
                .Index(t => t.PostId)
                .Index(t => t.CreatedByUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Communities", "SkillId", "dbo.Skills");
            DropForeignKey("dbo.CommunityPosts", "CreatedByUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CommunityPosts", "CommunityId", "dbo.Communities");
            DropForeignKey("dbo.CommunityComments", "PostId", "dbo.CommunityPosts");
            DropForeignKey("dbo.CommunityComments", "CreatedByUserId", "dbo.AspNetUsers");
            DropIndex("dbo.CommunityComments", new[] { "CreatedByUserId" });
            DropIndex("dbo.CommunityComments", new[] { "PostId" });
            DropIndex("dbo.CommunityPosts", new[] { "CreatedByUserId" });
            DropIndex("dbo.CommunityPosts", new[] { "CommunityId" });
            DropIndex("dbo.Communities", new[] { "SkillId" });
            DropTable("dbo.CommunityComments");
            DropTable("dbo.CommunityPosts");
            DropTable("dbo.Communities");
        }
    }
}
