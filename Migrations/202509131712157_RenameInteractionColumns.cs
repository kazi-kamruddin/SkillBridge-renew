namespace SkillBridge.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameInteractionColumns : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Interactions", name: "ReceiverId", newName: "User1Id");
            RenameColumn(table: "dbo.Interactions", name: "RequesterId", newName: "User2Id");
            RenameColumn(table: "dbo.Interactions", name: "SkillOfferedId", newName: "SkillFromRequesterId");
            RenameColumn(table: "dbo.Interactions", name: "SkillRequestedId", newName: "SkillFromTeacherId");
            RenameIndex(table: "dbo.Interactions", name: "IX_ReceiverId", newName: "IX_User1Id");
            RenameIndex(table: "dbo.Interactions", name: "IX_RequesterId", newName: "IX_User2Id");
            RenameIndex(table: "dbo.Interactions", name: "IX_SkillRequestedId", newName: "IX_SkillFromTeacherId");
            RenameIndex(table: "dbo.Interactions", name: "IX_SkillOfferedId", newName: "IX_SkillFromRequesterId");
            AddColumn("dbo.InteractionSessions", "User1Confirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.InteractionSessions", "User2Confirmed", c => c.Boolean(nullable: false));
            DropColumn("dbo.InteractionSessions", "LearnerConfirmed");
            DropColumn("dbo.InteractionSessions", "MentorConfirmed");
        }
        
        public override void Down()
        {
            AddColumn("dbo.InteractionSessions", "MentorConfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.InteractionSessions", "LearnerConfirmed", c => c.Boolean(nullable: false));
            DropColumn("dbo.InteractionSessions", "User2Confirmed");
            DropColumn("dbo.InteractionSessions", "User1Confirmed");
            RenameIndex(table: "dbo.Interactions", name: "IX_SkillFromRequesterId", newName: "IX_SkillOfferedId");
            RenameIndex(table: "dbo.Interactions", name: "IX_SkillFromTeacherId", newName: "IX_SkillRequestedId");
            RenameIndex(table: "dbo.Interactions", name: "IX_User2Id", newName: "IX_RequesterId");
            RenameIndex(table: "dbo.Interactions", name: "IX_User1Id", newName: "IX_ReceiverId");
            RenameColumn(table: "dbo.Interactions", name: "SkillFromTeacherId", newName: "SkillRequestedId");
            RenameColumn(table: "dbo.Interactions", name: "SkillFromRequesterId", newName: "SkillOfferedId");
            RenameColumn(table: "dbo.Interactions", name: "User2Id", newName: "RequesterId");
            RenameColumn(table: "dbo.Interactions", name: "User1Id", newName: "ReceiverId");
        }
    }
}
