namespace Kool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModel_Registr : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Registreerimines", new[] { "KoolitusId" });
            DropIndex("dbo.Registreerimines", new[] { "ApplicationUserId" });
            CreateIndex("dbo.Registreerimines", new[] { "KoolitusId", "ApplicationUserId" }, unique: true, name: "IX_UserKoolitus");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Registreerimines", "IX_UserKoolitus");
            CreateIndex("dbo.Registreerimines", "ApplicationUserId");
            CreateIndex("dbo.Registreerimines", "KoolitusId");
        }
    }
}
