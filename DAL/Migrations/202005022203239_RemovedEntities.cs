namespace DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedEntities : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderPayments", "Application_Id", "dbo.Applications");
            DropForeignKey("dbo.Order_Items", "Box_Id", "dbo.Boxes");
            DropForeignKey("dbo.Offer_Packages", "Offer_Id", "dbo.Offers");
            DropForeignKey("dbo.Products", "Category_Id", "dbo.Categories");
            DropForeignKey("dbo.Offers", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.Offer_Packages", "Package_Id", "dbo.Packages");
            DropForeignKey("dbo.Package_Products", "Package_Id", "dbo.Packages");
            DropForeignKey("dbo.Packages", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.Products", "Store_Id", "dbo.Stores");
            DropForeignKey("dbo.Favourites", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.Offer_Products", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.Package_Products", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.Product_Images", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.ProductRatings", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.Offer_Products", "Offer_Id", "dbo.Offers");
            DropForeignKey("dbo.Order_Items", "Offer_Package_Id", "dbo.Offer_Packages");
            DropForeignKey("dbo.Order_Items", "Offer_Product_Id", "dbo.Offer_Products");
            DropForeignKey("dbo.Order_Items", "Package_Id", "dbo.Packages");
            DropForeignKey("dbo.Order_Items", "Product_Id", "dbo.Products");
            DropForeignKey("dbo.Order_Items", "StoreOrder_Id", "dbo.StoreOrders");
            DropForeignKey("dbo.StoreOrders", "Order_Id", "dbo.Orders");
            DropForeignKey("dbo.OrderPayments", "Id", "dbo.Orders");
            DropForeignKey("dbo.OrderPayments", "DeliveryMan_Id", "dbo.DeliveryMen");
            DropForeignKey("dbo.Orders", "DeliveryMan_Id", "dbo.DeliveryMen");
            DropForeignKey("dbo.Orders", "User_ID", "dbo.Users");
            DropForeignKey("dbo.ProductRatings", "User_ID", "dbo.Users");
            DropForeignKey("dbo.Banner_Images", "Product_Id", "dbo.Products");
            DropIndex("dbo.OrderPayments", new[] { "Id" });
            DropIndex("dbo.OrderPayments", new[] { "DeliveryMan_Id" });
            DropIndex("dbo.OrderPayments", new[] { "Application_Id" });
            DropIndex("dbo.Orders", new[] { "User_ID" });
            DropIndex("dbo.Orders", new[] { "DeliveryMan_Id" });
            DropIndex("dbo.StoreOrders", new[] { "Order_Id" });
            DropIndex("dbo.Order_Items", new[] { "Product_Id" });
            DropIndex("dbo.Order_Items", new[] { "Box_Id" });
            DropIndex("dbo.Order_Items", new[] { "Package_Id" });
            DropIndex("dbo.Order_Items", new[] { "Offer_Product_Id" });
            DropIndex("dbo.Order_Items", new[] { "Offer_Package_Id" });
            DropIndex("dbo.Order_Items", new[] { "StoreOrder_Id" });
            DropIndex("dbo.Offer_Packages", new[] { "Offer_Id" });
            DropIndex("dbo.Offer_Packages", new[] { "Package_Id" });
            DropIndex("dbo.Offers", new[] { "Store_Id" });
            DropIndex("dbo.Offer_Products", new[] { "Offer_Id" });
            DropIndex("dbo.Offer_Products", new[] { "Product_Id" });
            DropIndex("dbo.Products", new[] { "Category_Id" });
            DropIndex("dbo.Products", new[] { "Store_Id" });
            DropIndex("dbo.Packages", new[] { "Store_Id" });
            DropIndex("dbo.Package_Products", new[] { "Product_Id" });
            DropIndex("dbo.Package_Products", new[] { "Package_Id" });
            DropIndex("dbo.Favourites", new[] { "Product_Id" });
            DropIndex("dbo.Product_Images", new[] { "Product_Id" });
            DropIndex("dbo.ProductRatings", new[] { "Product_Id" });
            DropIndex("dbo.ProductRatings", new[] { "User_ID" });
            DropIndex("dbo.Banner_Images", new[] { "Product_Id" });
            DropColumn("dbo.StoreOrders", "Order_Id");
            DropColumn("dbo.Favourites", "Product_Id");
            DropTable("dbo.OrderPayments");
            DropTable("dbo.Orders");
            DropTable("dbo.Order_Items");
            DropTable("dbo.Offer_Packages");
            DropTable("dbo.Offers");
            DropTable("dbo.Offer_Products");
            DropTable("dbo.Products");
            DropTable("dbo.Packages");
            DropTable("dbo.Package_Products");
            DropTable("dbo.Product_Images");
            DropTable("dbo.ProductRatings");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ProductRatings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Rating = c.Int(nullable: false),
                        Product_Id = c.Int(nullable: false),
                        User_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Product_Images",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                        ThumbnailUrl = c.String(),
                        IsVideo = c.Boolean(nullable: false),
                        Product_Id = c.Int(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Package_Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Qty = c.Int(nullable: false),
                        Product_Id = c.Int(nullable: false),
                        Package_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Packages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Status = c.Short(nullable: false),
                        Price = c.Double(nullable: false),
                        Description = c.String(),
                        ImageUrl = c.String(),
                        Store_Id = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Price = c.Double(nullable: false),
                        Description = c.String(),
                        WeightUnit = c.Int(nullable: false),
                        WeightInGrams = c.Double(nullable: false),
                        WeightInKiloGrams = c.Double(nullable: false),
                        ImageUrl = c.String(),
                        VideoUrl = c.String(),
                        Status = c.Short(nullable: false),
                        Category_Id = c.Int(nullable: false),
                        Store_Id = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Size = c.String(),
                        IsPopular = c.Boolean(nullable: false),
                        OrderedCount = c.Int(nullable: false),
                        AverageRating = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Offer_Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Offer_Id = c.Int(nullable: false),
                        Product_Id = c.Int(nullable: false),
                        Description = c.String(),
                        DiscountedPrice = c.Double(nullable: false),
                        DiscountPercentage = c.Int(nullable: false),
                        SlashPrice = c.Double(nullable: false),
                        ImageUrl = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Offers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ValidFrom = c.DateTime(nullable: false),
                        ValidUpto = c.DateTime(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        Status = c.Int(nullable: false),
                        ImageUrl = c.String(),
                        Store_Id = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Offer_Packages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Offer_Id = c.Int(nullable: false),
                        Package_Id = c.Int(nullable: false),
                        Description = c.String(),
                        DiscountedPrice = c.Double(nullable: false),
                        DiscountPercentage = c.Int(nullable: false),
                        SlashPrice = c.Double(nullable: false),
                        ImageUrl = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Order_Items",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Qty = c.Int(nullable: false),
                        Product_Id = c.Int(),
                        Box_Id = c.Int(),
                        Package_Id = c.Int(),
                        Offer_Product_Id = c.Int(),
                        Offer_Package_Id = c.Int(),
                        StoreOrder_Id = c.Int(nullable: false),
                        Name = c.String(),
                        Price = c.Double(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderNo = c.String(nullable: false),
                        Status = c.Int(nullable: false),
                        OrderDateTime = c.DateTime(nullable: false),
                        DeliveryTime_From = c.DateTime(nullable: false),
                        DeliveryTime_To = c.DateTime(nullable: false),
                        AdditionalNote = c.String(),
                        PaymentMethod = c.Int(nullable: false),
                        Subtotal = c.Double(nullable: false),
                        ServiceFee = c.Double(nullable: false),
                        DeliveryFee = c.Double(nullable: false),
                        Total = c.Double(nullable: false),
                        User_ID = c.Int(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        OrderPayment_Id = c.Int(),
                        PaymentStatus = c.Short(nullable: false),
                        DeliveryAddress = c.String(),
                        DeliveryMan_Id = c.Int(),
                        RemoveFromDelivererHistory = c.Boolean(nullable: false),
                        RemoveFromUserHistory = c.Boolean(nullable: false),
                        Tax = c.Double(nullable: false),
                        PaymentTransactionId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderPayments",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Amount = c.String(nullable: false),
                        PaymentType = c.String(nullable: false),
                        CashCollected = c.String(nullable: false),
                        Status = c.String(nullable: false),
                        Order_Id = c.String(nullable: false),
                        AccountNo = c.String(nullable: false),
                        DeliveryMan_Id = c.Int(),
                        Application_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Favourites", "Product_Id", c => c.Int(nullable: false));
            AddColumn("dbo.StoreOrders", "Order_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Banner_Images", "Product_Id");
            CreateIndex("dbo.ProductRatings", "User_ID");
            CreateIndex("dbo.ProductRatings", "Product_Id");
            CreateIndex("dbo.Product_Images", "Product_Id");
            CreateIndex("dbo.Favourites", "Product_Id");
            CreateIndex("dbo.Package_Products", "Package_Id");
            CreateIndex("dbo.Package_Products", "Product_Id");
            CreateIndex("dbo.Packages", "Store_Id");
            CreateIndex("dbo.Products", "Store_Id");
            CreateIndex("dbo.Products", "Category_Id");
            CreateIndex("dbo.Offer_Products", "Product_Id");
            CreateIndex("dbo.Offer_Products", "Offer_Id");
            CreateIndex("dbo.Offers", "Store_Id");
            CreateIndex("dbo.Offer_Packages", "Package_Id");
            CreateIndex("dbo.Offer_Packages", "Offer_Id");
            CreateIndex("dbo.Order_Items", "StoreOrder_Id");
            CreateIndex("dbo.Order_Items", "Offer_Package_Id");
            CreateIndex("dbo.Order_Items", "Offer_Product_Id");
            CreateIndex("dbo.Order_Items", "Package_Id");
            CreateIndex("dbo.Order_Items", "Box_Id");
            CreateIndex("dbo.Order_Items", "Product_Id");
            CreateIndex("dbo.StoreOrders", "Order_Id");
            CreateIndex("dbo.Orders", "DeliveryMan_Id");
            CreateIndex("dbo.Orders", "User_ID");
            CreateIndex("dbo.OrderPayments", "Application_Id");
            CreateIndex("dbo.OrderPayments", "DeliveryMan_Id");
            CreateIndex("dbo.OrderPayments", "Id");
            AddForeignKey("dbo.Banner_Images", "Product_Id", "dbo.Products", "Id");
            AddForeignKey("dbo.ProductRatings", "User_ID", "dbo.Users", "Id");
            AddForeignKey("dbo.Orders", "User_ID", "dbo.Users", "Id");
            AddForeignKey("dbo.Orders", "DeliveryMan_Id", "dbo.DeliveryMen", "Id");
            AddForeignKey("dbo.OrderPayments", "DeliveryMan_Id", "dbo.DeliveryMen", "Id");
            AddForeignKey("dbo.OrderPayments", "Id", "dbo.Orders", "Id");
            AddForeignKey("dbo.StoreOrders", "Order_Id", "dbo.Orders", "Id");
            AddForeignKey("dbo.Order_Items", "StoreOrder_Id", "dbo.StoreOrders", "Id");
            AddForeignKey("dbo.Order_Items", "Product_Id", "dbo.Products", "Id");
            AddForeignKey("dbo.Order_Items", "Package_Id", "dbo.Packages", "Id");
            AddForeignKey("dbo.Order_Items", "Offer_Product_Id", "dbo.Offer_Products", "Id");
            AddForeignKey("dbo.Order_Items", "Offer_Package_Id", "dbo.Offer_Packages", "Id");
            AddForeignKey("dbo.Offer_Products", "Offer_Id", "dbo.Offers", "Id");
            AddForeignKey("dbo.ProductRatings", "Product_Id", "dbo.Products", "Id");
            AddForeignKey("dbo.Product_Images", "Product_Id", "dbo.Products", "Id");
            AddForeignKey("dbo.Package_Products", "Product_Id", "dbo.Products", "Id");
            AddForeignKey("dbo.Offer_Products", "Product_Id", "dbo.Products", "Id");
            AddForeignKey("dbo.Favourites", "Product_Id", "dbo.Products", "Id");
            AddForeignKey("dbo.Products", "Store_Id", "dbo.Stores", "Id");
            AddForeignKey("dbo.Packages", "Store_Id", "dbo.Stores", "Id");
            AddForeignKey("dbo.Package_Products", "Package_Id", "dbo.Packages", "Id");
            AddForeignKey("dbo.Offer_Packages", "Package_Id", "dbo.Packages", "Id");
            AddForeignKey("dbo.Offers", "Store_Id", "dbo.Stores", "Id");
            AddForeignKey("dbo.Products", "Category_Id", "dbo.Categories", "Id");
            AddForeignKey("dbo.Offer_Packages", "Offer_Id", "dbo.Offers", "Id");
            AddForeignKey("dbo.Order_Items", "Box_Id", "dbo.Boxes", "Id");
            AddForeignKey("dbo.OrderPayments", "Application_Id", "dbo.Applications", "Id");
        }
    }
}
