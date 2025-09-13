namespace SkillBridge.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSkillRequestsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SkillRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RequesterId = c.String(nullable: false, maxLength: 128),
                        ReceiverId = c.String(nullable: false, maxLength: 128),
                        SkillId = c.Int(nullable: false),
                        Status = c.String(nullable: false, maxLength: 20),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ReceiverId)
                .ForeignKey("dbo.AspNetUsers", t => t.RequesterId)
                .ForeignKey("dbo.Skills", t => t.SkillId)
                .Index(t => t.RequesterId)
                .Index(t => t.ReceiverId)
                .Index(t => t.SkillId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SkillRequests", "SkillId", "dbo.Skills");
            DropForeignKey("dbo.SkillRequests", "RequesterId", "dbo.AspNetUsers");
            DropForeignKey("dbo.SkillRequests", "ReceiverId", "dbo.AspNetUsers");
            DropIndex("dbo.SkillRequests", new[] { "SkillId" });
            DropIndex("dbo.SkillRequests", new[] { "ReceiverId" });
            DropIndex("dbo.SkillRequests", new[] { "RequesterId" });
            DropTable("dbo.SkillRequests");
        }
    }
}
