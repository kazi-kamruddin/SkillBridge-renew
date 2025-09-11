namespace SkillBridge.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserInformationAndUserSkills : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserInformations",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        FullName = c.String(nullable: false, maxLength: 150),
                        Age = c.Int(nullable: false),
                        Profession = c.String(nullable: false, maxLength: 100),
                        Location = c.String(nullable: false, maxLength: 100),
                        Bio = c.String(nullable: false, maxLength: 500),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserSkills",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        SkillId = c.Int(nullable: false),
                        Status = c.String(nullable: false, maxLength: 10),
                        KnownUpToStage = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Skills", t => t.SkillId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.SkillId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserSkills", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserSkills", "SkillId", "dbo.Skills");
            DropForeignKey("dbo.UserInformations", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.UserSkills", new[] { "SkillId" });
            DropIndex("dbo.UserSkills", new[] { "UserId" });
            DropIndex("dbo.UserInformations", new[] { "UserId" });
            DropTable("dbo.UserSkills");
            DropTable("dbo.UserInformations");
        }
    }
}
