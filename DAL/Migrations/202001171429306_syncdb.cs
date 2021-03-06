namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class syncdb : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserNotificationSettings", "UserId", "dbo.Users");
            AddForeignKey("dbo.UserNotificationSettings", "UserId", "dbo.Users", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserNotificationSettings", "UserId", "dbo.Users");
            AddForeignKey("dbo.UserNotificationSettings", "UserId", "dbo.Users", "Id", cascadeDelete: true);
        }
    }
}
