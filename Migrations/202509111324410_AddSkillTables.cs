namespace SkillBridge.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSkillTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SkillCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Skills",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(),
                        SkillCategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SkillCategories", t => t.SkillCategoryId, cascadeDelete: true)
                .Index(t => t.SkillCategoryId);
            
            CreateTable(
                "dbo.SkillStages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StageNumber = c.Int(nullable: false),
                        Description = c.String(nullable: false, maxLength: 200),
                        SkillId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Skills", t => t.SkillId, cascadeDelete: true)
                .Index(t => t.SkillId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SkillStages", "SkillId", "dbo.Skills");
            DropForeignKey("dbo.Skills", "SkillCategoryId", "dbo.SkillCategories");
            DropIndex("dbo.SkillStages", new[] { "SkillId" });
            DropIndex("dbo.Skills", new[] { "SkillCategoryId" });
            DropTable("dbo.SkillStages");
            DropTable("dbo.Skills");
            DropTable("dbo.SkillCategories");
        }
    }
}
