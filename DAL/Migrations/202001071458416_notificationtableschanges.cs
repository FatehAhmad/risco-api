namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notificationtableschanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserNotificationSettings", "IsPush", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserNotificationSettings", "IsEmail", c => c.Boolean(nullable: false));
            AddColumn("dbo.UserNotificationSettings", "IsSMS", c => c.Boolean(nullable: false));
            DropTable("dbo.CommunicationTypes");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.CommunicationTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Description = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropColumn("dbo.UserNotificationSettings", "IsSMS");
            DropColumn("dbo.UserNotificationSettings", "IsEmail");
            DropColumn("dbo.UserNotificationSettings", "IsPush");
        }
    }
}
