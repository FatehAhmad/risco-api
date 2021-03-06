namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotificationTablesColumnadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserDevices", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.UserDevices", "UpdatedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserDevices", "UpdatedDate");
            DropColumn("dbo.UserDevices", "CreatedDate");
        }
    }
}
