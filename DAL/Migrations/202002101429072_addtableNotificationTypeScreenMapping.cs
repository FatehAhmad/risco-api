namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addtableNotificationTypeScreenMapping : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NotificationTypeScreenMappings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NotificationTypeId = c.Int(),
                        ScreenId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NotificationTypes", t => t.NotificationTypeId)
                .Index(t => t.NotificationTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NotificationTypeScreenMappings", "NotificationTypeId", "dbo.NotificationTypes");
            DropIndex("dbo.NotificationTypeScreenMappings", new[] { "NotificationTypeId" });
            DropTable("dbo.NotificationTypeScreenMappings");
        }
    }
}
