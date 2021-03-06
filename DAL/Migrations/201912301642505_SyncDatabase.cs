namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SyncDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MuteUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstUser_Id = c.Int(nullable: false),
                        SecondUser_Id = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.FirstUser_Id)
                .ForeignKey("dbo.Users", t => t.SecondUser_Id)
                .Index(t => t.FirstUser_Id)
                .Index(t => t.SecondUser_Id);
            
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
                        CreatedDate = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.SendingUser_Id)
                .Index(t => t.SendingUser_Id);
            
            CreateTable(
                "dbo.UserNotificationSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        NotificationTypeId = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NotificationTypes", t => t.NotificationTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.NotificationTypeId);
            
        }
        
        public override void Down()
        {

        }
    }
}
