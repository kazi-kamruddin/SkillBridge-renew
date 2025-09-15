namespace SkillBridge.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserRatings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserRatings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        InteractionsCompleted = c.Int(nullable: false),
                        RatingsReceived = c.Int(nullable: false),
                        AccumulatedRating = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserRatings", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.UserRatings", new[] { "UserId" });
            DropTable("dbo.UserRatings");
        }
    }
}
