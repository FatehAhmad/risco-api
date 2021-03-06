namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class altertablenotification : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.NotificationActions", "SendingUser_Id", "dbo.Users");
            DropIndex("dbo.NotificationActions", new[] { "SendingUser_Id" });
            AddColumn("dbo.Notifications", "IsEmailSent", c => c.Boolean(nullable: false));
            AddColumn("dbo.Notifications", "IsSMSSent", c => c.Boolean(nullable: false));
            DropTable("dbo.NotificationActions");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.NotificationActions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Description = c.String(),
                        SendingUser_Id = c.Int(nullable: false),
                        EntityType = c.Int(nullable: false),
                        EntityId = c.Int(nullable: false),
                        IsNotificationGenerated = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.Notifications", "IsSMSSent");
            DropColumn("dbo.Notifications", "IsEmailSent");
            CreateIndex("dbo.NotificationActions", "SendingUser_Id");
            AddForeignKey("dbo.NotificationActions", "SendingUser_Id", "dbo.Users", "Id");
        }
    }
}
