namespace Kool.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RegistKoolitus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Registreerimines", "CreatedAt", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Opetajas", "ApplicationUserId", c => c.String(maxLength: 128));
            AlterColumn("dbo.Registreerimines", "ApplicationUserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Registreerimines", "Staatus", c => c.Int(nullable: false));
            CreateIndex("dbo.Koolitus", "KeelekursusId");
            CreateIndex("dbo.Koolitus", "OpetajaId");
            CreateIndex("dbo.Opetajas", "ApplicationUserId");
            CreateIndex("dbo.Registreerimines", "KoolitusId");
            CreateIndex("dbo.Registreerimines", "ApplicationUserId");
            AddForeignKey("dbo.Koolitus", "KeelekursusId", "dbo.Keelekursus", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Opetajas", "ApplicationUserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Koolitus", "OpetajaId", "dbo.Opetajas", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Registreerimines", "ApplicationUserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Registreerimines", "KoolitusId", "dbo.Koolitus", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Registreerimines", "KoolitusId", "dbo.Koolitus");
            DropForeignKey("dbo.Registreerimines", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Koolitus", "OpetajaId", "dbo.Opetajas");
            DropForeignKey("dbo.Opetajas", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Koolitus", "KeelekursusId", "dbo.Keelekursus");
            DropIndex("dbo.Registreerimines", new[] { "ApplicationUserId" });
            DropIndex("dbo.Registreerimines", new[] { "KoolitusId" });
            DropIndex("dbo.Opetajas", new[] { "ApplicationUserId" });
            DropIndex("dbo.Koolitus", new[] { "OpetajaId" });
            DropIndex("dbo.Koolitus", new[] { "KeelekursusId" });
            AlterColumn("dbo.Registreerimines", "Staatus", c => c.String());
            AlterColumn("dbo.Registreerimines", "ApplicationUserId", c => c.String());
            AlterColumn("dbo.Opetajas", "ApplicationUserId", c => c.String());
            DropColumn("dbo.Registreerimines", "CreatedAt");
        }
    }
}
