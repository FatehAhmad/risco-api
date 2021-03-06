namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotificationTablesColumnModified : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserDevices", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.UserDevices", "UpdatedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserDevices", "UpdatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.UserDevices", "CreatedDate", c => c.DateTime(nullable: false));
        }
    }
}
