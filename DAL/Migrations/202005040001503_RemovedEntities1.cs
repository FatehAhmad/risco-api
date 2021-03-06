namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedEntities1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DeliveryMan_AvailibilitySchedule", "DeliveryMan_Id", "dbo.DeliveryMen");
            DropForeignKey("dbo.DelivererAddresses", "DeliveryMan_Id", "dbo.DeliveryMen");
            DropForeignKey("dbo.DeliveryManRatings", "Deliverer_Id", "dbo.DeliveryMen");
            DropForeignKey("dbo.Notifications", "DeliveryMan_ID", "dbo.DeliveryMen");
            DropForeignKey("dbo.UserRatings", "Deliverer_Id", "dbo.DeliveryMen");
            DropForeignKey("dbo.DeliveryManRatings", "User_ID", "dbo.Users");
            DropForeignKey("dbo.Favourites", "User_ID", "dbo.Users");
            DropForeignKey("dbo.Categories", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.DeliveryMen", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.StoreDeliveryHours", "Id", "dbo.Stores");
            DropForeignKey("dbo.StoreOrders", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.StoreRatings", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.PaymentCards", "User_ID", "dbo.Users");
            DropForeignKey("dbo.StoreRatings", "User_Id", "dbo.Users");
            DropForeignKey("dbo.BoxVideos", "Box_Id", "dbo.Boxes");
            DropForeignKey("dbo.UserSubscriptions", "Box_Id", "dbo.Boxes");
            DropIndex("dbo.DeliveryManRatings", new[] { "User_ID" });
            DropIndex("dbo.DeliveryManRatings", new[] { "Deliverer_Id" });
            DropIndex("dbo.DeliveryMen", new[] { "Store_Id" });
            DropIndex("dbo.DeliveryMan_AvailibilitySchedule", new[] { "DeliveryMan_Id" });
            DropIndex("dbo.DelivererAddresses", new[] { "DeliveryMan_Id" });
            DropIndex("dbo.Notifications", new[] { "DeliveryMan_ID" });
            DropIndex("dbo.UserRatings", new[] { "Deliverer_Id" });
            DropIndex("dbo.Favourites", new[] { "User_ID" });
            DropIndex("dbo.Categories", new[] { "Store_Id" });
            DropIndex("dbo.StoreDeliveryHours", new[] { "Id" });
            DropIndex("dbo.StoreOrders", new[] { "Store_Id" });
            DropIndex("dbo.StoreRatings", new[] { "User_Id" });
            DropIndex("dbo.StoreRatings", new[] { "Store_Id" });
            DropIndex("dbo.PaymentCards", new[] { "User_ID" });
            DropIndex("dbo.UserSubscriptions", new[] { "Box_Id" });
            DropIndex("dbo.BoxVideos", new[] { "Box_Id" });
            DropColumn("dbo.Notifications", "DeliveryMan_ID");
            DropColumn("dbo.UserRatings", "Deliverer_Id");
            DropTable("dbo.DeliveryManRatings");
            DropTable("dbo.DeliveryMen");
            DropTable("dbo.DeliveryMan_AvailibilitySchedule");
            DropTable("dbo.DelivererAddresses");
            DropTable("dbo.Favourites");
            DropTable("dbo.Categories");
            DropTable("dbo.StoreDeliveryHours");
            DropTable("dbo.StoreOrders");
            DropTable("dbo.StoreRatings");
            DropTable("dbo.PaymentCards");
            DropTable("dbo.Boxes");
            DropTable("dbo.BoxVideos");
            DropTable("dbo.Applications");
            DropTable("dbo.Banner_Images");
            DropTable("dbo.Franchisors");
            DropTable("dbo.StorePayments");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.StorePayments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Franchisors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CommissionFormula = c.String(),
                        AdminName = c.String(),
                        Password = c.String(),
                        AccountNo = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Banner_Images",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        Product_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Applications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OwnerName = c.String(nullable: false),
                        OwnerPassword = c.String(nullable: false),
                        AccountNo = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.BoxVideos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VideoUrl = c.String(),
                        Title = c.String(),
                        Description = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        Box_Id = c.Int(nullable: false),
                        ThumbnailUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Boxes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IntroUrl = c.String(),
                        IntroUrlThumbnail = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        Description = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        ReleaseDate = c.DateTime(nullable: false),
                        BoxCategory_Id = c.Int(nullable: false),
                        Price = c.Double(nullable: false),
                        Status = c.Short(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PaymentCards",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CardNumber = c.String(nullable: false),
                        ExpiryDate = c.DateTime(nullable: false),
                        CCV = c.String(nullable: false),
                        NameOnCard = c.String(nullable: false),
                        CardType = c.Int(nullable: false),
                        User_ID = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StoreRatings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        User_Id = c.Int(nullable: false),
                        Store_Id = c.Int(nullable: false),
                        Rating = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StoreOrders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderNo = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                        Store_Id = c.Int(nullable: false),
                        Subtotal = c.Double(nullable: false),
                        Total = c.Double(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        RemoveFromDelivererHistory = c.Boolean(nullable: false),
                        RemoveFromUserHistory = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StoreDeliveryHours",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Monday_From = c.Time(nullable: false, precision: 7),
                        Monday_To = c.Time(nullable: false, precision: 7),
                        Tuesday_From = c.Time(nullable: false, precision: 7),
                        Tuesday_To = c.Time(nullable: false, precision: 7),
                        Wednesday_From = c.Time(nullable: false, precision: 7),
                        Wednesday_To = c.Time(nullable: false, precision: 7),
                        Thursday_From = c.Time(nullable: false, precision: 7),
                        Thursday_To = c.Time(nullable: false, precision: 7),
                        Friday_From = c.Time(nullable: false, precision: 7),
                        Friday_To = c.Time(nullable: false, precision: 7),
                        Saturday_From = c.Time(nullable: false, precision: 7),
                        Saturday_To = c.Time(nullable: false, precision: 7),
                        Sunday_From = c.Time(nullable: false, precision: 7),
                        Sunday_To = c.Time(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Status = c.Short(nullable: false),
                        Store_Id = c.Int(nullable: false),
                        ImageUrl = c.String(),
                        ParentCategoryId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Favourites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        User_ID = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DelivererAddresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DeliveryMan_Id = c.Int(nullable: false),
                        Country = c.String(nullable: false),
                        City = c.String(nullable: false),
                        StreetName = c.String(nullable: false),
                        Floor = c.String(),
                        Apartment = c.String(),
                        NearestLandmark = c.String(nullable: false),
                        BuildingName = c.String(nullable: false),
                        Type = c.Short(nullable: false),
                        IsPrimary = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Latitude = c.Double(),
                        Longitude = c.Double(),
                        Location = c.Geography(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DeliveryMan_AvailibilitySchedule",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        IsAvailable = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        DeliveryMan_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DeliveryMen",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false),
                        Address1 = c.String(),
                        Address2 = c.String(),
                        Email = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        ZipCode = c.String(),
                        DateOfBirth = c.String(),
                        Phone = c.String(nullable: false),
                        ProfilePictureUrl = c.String(),
                        IsOnline = c.Boolean(nullable: false),
                        Status = c.Short(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        UserName = c.String(),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PhoneConfirmed = c.Boolean(nullable: false),
                        Latitude = c.Double(),
                        Longitude = c.Double(),
                        Location = c.Geography(),
                        Type = c.Short(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        IsNotificationsOn = c.Boolean(nullable: false),
                        Store_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DeliveryManRatings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Rating = c.Short(nullable: false),
                        User_ID = c.Int(nullable: false),
                        Deliverer_Id = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.UserRatings", "Deliverer_Id", c => c.Int(nullable: false));
            AddColumn("dbo.Notifications", "DeliveryMan_ID", c => c.Int());
            CreateIndex("dbo.BoxVideos", "Box_Id");
            CreateIndex("dbo.UserSubscriptions", "Box_Id");
            CreateIndex("dbo.PaymentCards", "User_ID");
            CreateIndex("dbo.StoreRatings", "Store_Id");
            CreateIndex("dbo.StoreRatings", "User_Id");
            CreateIndex("dbo.StoreOrders", "Store_Id");
            CreateIndex("dbo.StoreDeliveryHours", "Id");
            CreateIndex("dbo.Categories", "Store_Id");
            CreateIndex("dbo.Favourites", "User_ID");
            CreateIndex("dbo.UserRatings", "Deliverer_Id");
            CreateIndex("dbo.Notifications", "DeliveryMan_ID");
            CreateIndex("dbo.DelivererAddresses", "DeliveryMan_Id");
            CreateIndex("dbo.DeliveryMan_AvailibilitySchedule", "DeliveryMan_Id");
            CreateIndex("dbo.DeliveryMen", "Store_Id");
            CreateIndex("dbo.DeliveryManRatings", "Deliverer_Id");
            CreateIndex("dbo.DeliveryManRatings", "User_ID");
            AddForeignKey("dbo.UserSubscriptions", "Box_Id", "dbo.Boxes", "Id");
            AddForeignKey("dbo.BoxVideos", "Box_Id", "dbo.Boxes", "Id");
            AddForeignKey("dbo.StoreRatings", "User_Id", "dbo.Users", "Id");
            AddForeignKey("dbo.PaymentCards", "User_ID", "dbo.Users", "Id");
            AddForeignKey("dbo.StoreRatings", "Store_Id", "dbo.Stores", "Id");
            AddForeignKey("dbo.StoreOrders", "Store_Id", "dbo.Stores", "Id");
            AddForeignKey("dbo.StoreDeliveryHours", "Id", "dbo.Stores", "Id");
            AddForeignKey("dbo.DeliveryMen", "Store_Id", "dbo.Stores", "Id");
            AddForeignKey("dbo.Categories", "Store_Id", "dbo.Stores", "Id");
            AddForeignKey("dbo.Favourites", "User_ID", "dbo.Users", "Id");
            AddForeignKey("dbo.DeliveryManRatings", "User_ID", "dbo.Users", "Id");
            AddForeignKey("dbo.UserRatings", "Deliverer_Id", "dbo.DeliveryMen", "Id");
            AddForeignKey("dbo.Notifications", "DeliveryMan_ID", "dbo.DeliveryMen", "Id");
            AddForeignKey("dbo.DeliveryManRatings", "Deliverer_Id", "dbo.DeliveryMen", "Id");
            AddForeignKey("dbo.DelivererAddresses", "DeliveryMan_Id", "dbo.DeliveryMen", "Id");
            AddForeignKey("dbo.DeliveryMan_AvailibilitySchedule", "DeliveryMan_Id", "dbo.DeliveryMen", "Id");
        }
    }
}
