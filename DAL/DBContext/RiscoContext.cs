namespace DAL
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using DAL.DBModels;

    public partial class RiscoContext : DbContext
    {
        public RiscoContext()
            : base("name=RiscoContextQA")
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<RiscoContext, DAL.Migrations.Configuration>());
            Configuration.ProxyCreationEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
            this.Database.CommandTimeout = 50;
        }

        //public virtual DbSet<Application> Applications { get; set; }
        //public virtual DbSet<Category> Categories { get; set; }
        //public virtual DbSet<PaymentCard> PaymentCards { get; set; }
        //public virtual DbSet<DeliveryMan> DeliveryMen { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
        //public virtual DbSet<Favourite> Favourites { get; set; }
        public virtual DbSet<ForgotPasswordToken> ForgotPasswordTokens { get; set; }
        //public virtual DbSet<Franchisor> Franchisors { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        //public virtual DbSet<Offer_Products> Offer_Products { get; set; }
        //public virtual DbSet<Offer_Packages> Offer_Packages { get; set; }
        //public virtual DbSet<Offer> Offers { get; set; }
        //public virtual DbSet<Order_Items> Order_Items { get; set; }
        //public virtual DbSet<OrderPayment> OrderPayments { get; set; }
        //public virtual DbSet<StoreOrder> StoreOrders { get; set; }
        //public virtual DbSet<Package_Products> Package_Products { get; set; }
        ////public virtual DbSet<Package> Packages { get; set; }
        //public virtual DbSet<ProductRating> ProductRatings { get; set; }
        //public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        //public virtual DbSet<StorePayment> StorePayments { get; set; }
        public virtual DbSet<Store> Stores { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<User> Users { get; set; }
        //public virtual DbSet<DeliveryManRatings> DeliveryManRatings { get; set; }
        public virtual DbSet<UserRatings> UserRatings { get; set; }
        public virtual DbSet<AppRatings> AppRatings { get; set; }
        public virtual DbSet<UserAddress> UserAddresses { get; set; }
        //public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<ContactUs> ContactUs { get; set; }
        public virtual DbSet<UserDevice> UserDevices { get; set; }
        //public virtual DbSet<StoreDeliveryHours> StoreDeliveryHours { get; set; }
        public virtual DbSet<RefreshTokens> RefreshTokens { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }
        //public virtual DbSet<DeliveryMan_AvailibilitySchedule> DeliveryManAvailibilitySchedules { get; set; }
        //public virtual DbSet<Box> Boxes { get; set; }
        //public virtual DbSet<BoxVideos> BoxVideos { get; set; }
        public virtual DbSet<UserSubscriptions> UserSubscriptions { get; set; }
        public virtual DbSet<AdminNotifications> AdminNotifications { get; set; }
        //public virtual DbSet<Product_Images> ProductImages { get; set; }
        //public virtual DbSet<Banner_Images> BannerImages { get; set; }
        public virtual DbSet<AdminTokens> AdminTokens { get; set; }
        public virtual DbSet<VerifyNumberCodes> VerifyNumberCodes { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Media> Medias { get; set; }
        public virtual DbSet<Interest> Interests { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Like> Likes { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Share> Shares { get; set; }
        public virtual DbSet<TrendLog> TrendLogs { get; set; }
        public virtual DbSet<HidePost> HidePosts { get; set; }
        public virtual DbSet<HideAllPost> HideAllPosts { get; set; }
        public virtual DbSet<FollowFollower> FollowFollowers { get; set; }
        public virtual DbSet<TurnOffNotification> TurnOffNotifications { get; set; }
        public virtual DbSet<ReportPost> ReportPosts { get; set; }
        public virtual DbSet<TopFollowerLog> TopFollowerLogs { get; set; }
        public virtual DbSet<GroupMember> GroupMembers { get; set; }
        public virtual DbSet<ActivityLog> ActivityLogs { get; set; }
        public virtual DbSet<BlockUser> BlockUsers { get; set; }
        public virtual DbSet<ReportUser> ReportUsers { get; set; }
        public virtual DbSet<UserProfiles> UserProfiles { get; set; }
        public virtual DbSet<PostCategories> PostCategories { get; set; }
        //public virtual DbSet<PostLanguageCodes> PostLanguageCodes { get; set; }
        public virtual DbSet<PollOptions> PollOptions { get; set; }
        public virtual DbSet<PollOptionVote> PollOptionVote { get; set; }
        public virtual DbSet<FAQ> FAQ { get; set; }
        public virtual DbSet<MediaUserViews> MediaUserViews { get; set; }
        public virtual DbSet<MuteUser> MuteUser { get; set; }
        public virtual DbSet<RiskLevelRange> RiskLevelRange { get; set; }
        public virtual DbSet<Languages> Languages { get; set; }
        public virtual DbSet<NotificationCategory> NotificationCategory { get; set; }
        public virtual DbSet<NotificationType> NotificationType { get; set; }
        public virtual DbSet<UserNotificationSetting> UserNotificationSetting { get; set; }
        public virtual DbSet<NotificationTypeScreenMapping> NotificationTypeScreenMapping { get; set; }
        public virtual DbSet<UserLanguageMapping> UserLanguageMappings { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {            

            modelBuilder.Entity<Comment>()
                .HasMany(x => x.Medias)
                .WithOptional(x => x.Comment)
                .HasForeignKey(x => x.Comment_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Post>()
                .HasMany(x => x.ExtendedPostList)
                .WithOptional(x => x.ExtendedPost)
                .HasForeignKey(x => x.ParentPost_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Post>()
                .HasMany(x => x.SharePostParent)
                .WithOptional(x => x.SharedParent)
                .HasForeignKey(x => x.SharePost_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Post>()
                .HasMany(x => x.PollOptionVote)
                .WithRequired(x => x.Post)
                .HasForeignKey(x => x.Post_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
              .HasMany(x => x.PollOptionVote)
              .WithRequired(x => x.User)
              .HasForeignKey(x => x.User_Id)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<PollOptions>()
              .HasMany(x => x.PollOptionVote)
              .WithRequired(x => x.PollOptions)
              .HasForeignKey(x => x.PollOption_Id)
              .WillCascadeOnDelete(false);


            modelBuilder.Entity<Post>()
              .HasMany(x => x.PollOptions)
              .WithOptional(x => x.Post)
              .HasForeignKey(x => x.Post_Id)
              .WillCascadeOnDelete(false);


            modelBuilder.Entity<Post>()
              .HasMany(x => x.PostCategories)
              .WithRequired(x => x.Post)
              .HasForeignKey(x => x.Post_Id)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<Group>()
              .HasMany(x => x.GroupCategories)
              .WithRequired(x => x.Group)
              .HasForeignKey(x => x.Group_Id)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<Post>()
              .HasMany(x => x.TrendLogs)
              .WithRequired(x => x.Post)
              .HasForeignKey(x => x.Post_Id)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
              .HasMany(x => x.UserProfiles)
              .WithRequired(x => x.User)
              .HasForeignKey(x => x.User_Id)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
              .HasMany(x => x.VerifyNumberCodes)
              .WithRequired(x => x.User)
              .HasForeignKey(x => x.User_Id)
              .WillCascadeOnDelete(false);

            modelBuilder.Entity<Admin>()
                .HasMany(x => x.AdminTokens)
                .WithRequired(x => x.Admin)
                .HasForeignKey(x => x.Admin_Id)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Product>()
            //   .HasMany(x => x.ProductImages)
            //   .WithRequired(x => x.Product)
            //   .HasForeignKey(x => x.Product_Id)
            //   .WillCascadeOnDelete(false);

            modelBuilder.Entity<AdminNotifications>()
               .HasMany(e => e.Notifications)
               .WithOptional(e => e.AdminNotification)
               .HasForeignKey(e => e.AdminNotification_Id)
               .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Box>()
            //    .HasMany(x => x.UserSubscriptions)
            //    .WithRequired(x => x.Box)
            //    .HasForeignKey(x => x.Box_Id)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Store>()
             .HasMany(x => x.Feedback)
             .WithOptional(x => x.Store)
             .HasForeignKey(x => x.Store_Id)
             .WillCascadeOnDelete(false);



            modelBuilder.Entity<User>()
                .HasMany(x => x.UserSubscriptions)
                .WithRequired(x => x.User)
                .HasForeignKey(x => x.User_Id)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Box>()
            //    .HasMany(x => x.BoxVideos)
            //    .WithRequired(x => x.Box)
            //    .HasForeignKey(x => x.Box_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<DeliveryMan>()
            //    .HasMany(e => e.AvailibilitySchedules)
            //    .WithRequired(e => e.DeliveryMan)
            //    .HasForeignKey(e => e.DeliveryMan_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Store>()
            //   .HasMany(e => e.Packages)
            //   .WithRequired(e => e.Store)
            //   .HasForeignKey(e => e.Store_Id)
            //   .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Order_Items>()
            //   .HasOptional(x => x.Offer_Product);

            //modelBuilder.Entity<Order_Items>()
            //    .HasOptional(x => x.Offer_Package);

            //modelBuilder.Entity<Order_Items>()
            //    .HasOptional(x => x.Product);

            //modelBuilder.Entity<Order_Items>()
            //    .HasOptional(x => x.Package);

            //modelBuilder.Entity<Store>()
            //    .HasOptional(s => s.StoreDeliveryHours)
            //    .WithRequired(ad => ad.Store);


            //modelBuilder.Entity<DeliveryMan>()
            //    .HasMany(x => x.DelivererAddresses)
            //    .WithRequired(x => x.DeliveryMan)
            //    .HasForeignKey(x => x.DeliveryMan_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Order_Items>

            //modelBuilder.Entity<Store>()
            //    .HasMany(e => e.StoreDeliveryHours)
            //    .WithRequired(e => e.Store)
            //    .HasForeignKey(e => e.Store_Id)
            //    .WillCascadeOnDelete(false);


            //modelBuilder.Entity<StoreDeliveryHours>()
            //    .HasRequired(s => s.Store)
            //    .WithOptional(ad => ad.StoreDeliveryHours);

            //modelBuilder.Entity<Store>()
            //    .HasMany(e => e.StoreRatings)
            //    .WithRequired(e => e.Store)
            //    .HasForeignKey(e => e.Store_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<User>()
            //    .HasMany(e => e.StoreRatings)
            //    .WithRequired(e => e.User)
            //    .HasForeignKey(e => e.User_Id)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserDevices)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Order>()
            //    .HasMany(e => e.StoreOrders)
            //    .WithRequired(e => e.Order)
            //    .HasForeignKey(e => e.Order_Id)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserRatings)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_ID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserAddresses)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_ID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
              .HasMany(e => e.AppRatings)
              .WithRequired(e => e.User)
              .HasForeignKey(e => e.User_ID)
              .WillCascadeOnDelete(false);

            //modelBuilder.Entity<User>()
            //   .HasMany(e => e.DeliveryManRatings)
            //   .WithRequired(e => e.User)
            //   .HasForeignKey(e => e.User_ID)
            //   .WillCascadeOnDelete(false);

            //modelBuilder.Entity<DeliveryMan>()
            //    .HasMany(e => e.DeliveryManRatings)
            //    .WithRequired(e => e.DeliveryMan)
            //    .HasForeignKey(e => e.Deliverer_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<DeliveryMan>()
            //     .HasMany(e => e.UserRatings)
            //     .WithRequired(e => e.DeliveryMan)
            //     .HasForeignKey(e => e.Deliverer_Id)
            //     .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Application>()
            //    .HasMany(e => e.OrderPayments)
            //    .WithRequired(e => e.Application)
            //    .HasForeignKey(e => e.Application_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Category>()
            //    .HasMany(e => e.Products)
            //    .WithRequired(e => e.Category)
            //    .HasForeignKey(e => e.Category_Id)
            //    .WillCascadeOnDelete(false);

            ////modelBuilder.Entity<Category>()
            ////    .HasMany(e => e.SubCategories)
            ////    .WithRequired(e => e.Category)
            ////    .HasForeignKey(e => e.Category_Id)
            ////    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<DeliveryMan>()
            //    .HasMany(e => e.OrderPayments)
            //    .WithOptional(e => e.DeliveryMan)
            //    .HasForeignKey(e => e.DeliveryMan_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<DeliveryMan>()
            //    .HasMany(e => e.Orders)
            //    .WithOptional(e => e.DeliveryMan)
            //    .HasForeignKey(e => e.DeliveryMan_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Offer>()
            //    .HasMany(e => e.Offer_Products)
            //    .WithRequired(e => e.Offer)
            //    .HasForeignKey(e => e.Offer_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Offer>()
            //    .HasMany(e => e.Offer_Packages)
            //    .WithRequired(e => e.Offer)
            //    .HasForeignKey(e => e.Offer_Id)
            //    .WillCascadeOnDelete(false);


            //modelBuilder.Entity<Package>()
            //    .HasMany(e => e.Offer_Packages)
            //    .WithRequired(e => e.Package)
            //    .HasForeignKey(e => e.Package_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Package>()
            //   .HasMany(e => e.Order_Items)
            //   .WithOptional(e => e.Package)
            //   .HasForeignKey(e => e.Package_Id)
            //   .WillCascadeOnDelete(false);

            //modelBuilder.Entity<OrderPayment>()
            //    .HasRequired(e => e.Order)
            //    .WithRequiredDependent(e => e.OrderPayment)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<OrderPayment>()
            //   .HasMany(e => e.Orders)
            //   .WithRequired(e => e.OrderPayment)
            //   .HasForeignKey(e => e.OrderPayments_Id)


            //modelBuilder.Entity<StoreOrder>()
            //    .HasMany(e => e.Order_Items)
            //    .WithRequired(e => e.StoreOrder)
            //    .HasForeignKey(e => e.StoreOrder_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Package>()
            //    .HasMany(e => e.Offer_Products)
            //    .WithRequired(e => e.Package)
            //    .HasForeignKey(e => e.Package_Id)
            //    .WillCascadeOnDelete(false);



            //modelBuilder.Entity<Package>()
            //    .HasMany(e => e.Package_Products)
            //    .WithRequired(e => e.Package)
            //    .HasForeignKey(e => e.Package_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Product>()
            //    .HasMany(e => e.Favourites)
            //    .WithRequired(e => e.Product)
            //    .HasForeignKey(e => e.Product_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Product>()
            //    .HasMany(e => e.Offer_Products)
            //    .WithRequired(e => e.Product)
            //    .HasForeignKey(e => e.Product_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Package>()
            //   .HasMany(e => e.Offer_Packages)
            //   .WithRequired(e => e.Package)
            //   .HasForeignKey(e => e.Package_Id)
            //   .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Product>()
            //    .HasMany(e => e.Order_Items)
            //    .WithOptional(e => e.Product)
            //    .HasForeignKey(e => e.Product_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Product>()
            //    .HasMany(e => e.Package_Products)
            //    .WithRequired(e => e.Product)
            //    .HasForeignKey(e => e.Product_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Product>()
            //    .HasMany(e => e.ProductRatings)
            //    .WithRequired(e => e.Product)
            //    .HasForeignKey(e => e.Product_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Store>()
            //    .Property(e => e.Latitude);

            //modelBuilder.Entity<Store>()
            //    .Property(e => e.Longitude);

            //modelBuilder.Entity<Store>()
            //    .HasMany(e => e.Categories)
            //    .WithRequired(e => e.Store)
            //    .HasForeignKey(e => e.Store_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Store>()
            //    .HasMany(e => e.DeliveryMen)
            //    .WithRequired(e => e.Store)
            //    .HasForeignKey(e => e.Store_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Store>()
            //    .HasMany(e => e.Offers)
            //    .WithRequired(e => e.Store)
            //    .HasForeignKey(e => e.Store_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Store>()
            //    .HasMany(e => e.StoreOrders)
            //    .WithRequired(e => e.Store)
            //    .HasForeignKey(e => e.Store_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Store>()
            //    .HasMany(e => e.Products)
            //    .WithRequired(e => e.Store)
            //    .HasForeignKey(e => e.Store_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Store>()
            //    .HasMany(e => e.Admins)
            //    .WithOptional(e => e.Store)
            //    .HasForeignKey(e => e.Store_Id)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.SendingUserNotifications)
                .WithOptional(e => e.SendingUser)
                .HasForeignKey(e => e.SendingUser_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ReceivingUserNotifications)
                .WithOptional(e => e.ReceivingUser)
                .HasForeignKey(e => e.ReceivingUser_Id)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<DeliveryMan>()
            //    .HasMany(e => e.Notifications)
            //    .WithOptional(e => e.DeliveryMan)
            //    .HasForeignKey(e => e.DeliveryMan_ID)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Store>()
            //    .HasMany(e => e.SubCategories)
            //    .WithRequired(e => e.Store)
            //    .HasForeignKey(e => e.Store_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<SubCategory>()
            //    .HasMany(e => e.Products)
            //    .WithRequired(e => e.SubCategory)
            //    .HasForeignKey(e => e.SubCategory_Id)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<User>()
            //    .Property(e => e.FirstName)
            //    .IsUnicode(false);

            //modelBuilder.Entity<User>()
            //    .Property(e => e.LastName)
            //    .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.FullName)
                .IsUnicode(false);

            //modelBuilder.Entity<User>()
            //    .HasMany(e => e.PaymentCards)
            //    .WithRequired(e => e.User)
            //    .HasForeignKey(e => e.User_ID)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<User>()
            //    .HasMany(e => e.Favourites)
            //    .WithRequired(e => e.User)
            //    .HasForeignKey(e => e.User_ID)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ForgotPasswordTokens)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_ID)
                .WillCascadeOnDelete(false);



            //modelBuilder.Entity<User>()
            //    .HasMany(e => e.Orders)
            //    .WithRequired(e => e.User)
            //    .HasForeignKey(e => e.User_ID)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<User>()
            //    .HasMany(e => e.ProductRatings)
            //    .WithRequired(e => e.User)
            //    .HasForeignKey(e => e.User_ID)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Posts)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserGroups)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Likes)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Comments)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Shares)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.TrendLogs)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.FirstUserHidePosts)
                .WithRequired(e => e.FirstUser)
                .HasForeignKey(e => e.FirstUser_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.SecondUserHidePosts)
                .WithRequired(e => e.SecondUser)
                .HasForeignKey(e => e.SecondUser_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.FirstUserHideAllPosts)
                .WithRequired(e => e.FirstUserAll)
                .HasForeignKey(e => e.FirstUserAll_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.SecondUserHideAllPosts)
                .WithRequired(e => e.SecondUserAll)
                .HasForeignKey(e => e.SecondUserAll_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.FirstUserFollowFollower)
                .WithRequired(e => e.FirstUser)
                .HasForeignKey(e => e.FirstUser_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.SecondUserFollowFollower)
                .WithRequired(e => e.SecondUser)
                .HasForeignKey(e => e.SecondUser_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.MuteUsers_First)
                .WithRequired(e => e.FirstUser)
                .HasForeignKey(e => e.FirstUser_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.MuteUsers_Second)
                .WithRequired(e => e.SecondUser)
                .HasForeignKey(e => e.SecondUser_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.TurnOffNotifications)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ReportPosts)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.FirstUserTopFollowerLog)
                .WithRequired(e => e.FirstUser)
                .HasForeignKey(e => e.FirstUser_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.SecondUserTopFollowerLog)
                .WithRequired(e => e.SecondUser)
                .HasForeignKey(e => e.SecondUser_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.GroupMembers)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Post>()
                .HasMany(e => e.Medias)
                .WithRequired(e => e.Post)
                .HasForeignKey(e => e.Post_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.Medias)
                .WithOptional(e => e.User)
                .HasForeignKey(e => e.User_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Post>()
                .HasMany(e => e.Likes)
                .WithOptional(e => e.Post)
                .HasForeignKey(e => e.Post_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Post>()
                .HasMany(e => e.Comments)
                .WithRequired(e => e.Post)
                .HasForeignKey(e => e.Post_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Post>()
                .HasMany(e => e.Shares)
                .WithRequired(e => e.Post)
                .HasForeignKey(e => e.Post_Id)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Post>()
            //    .HasMany(e => e.TrendLogs)
            //    .WithOptional(e => e.Post)
            //    .HasForeignKey(e => e.Post_Id)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Post>()
                .HasMany(e => e.HidePosts)
                .WithRequired(e => e.Post)
                .HasForeignKey(e => e.Post_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Post>()
                .HasMany(e => e.TurnOffNotifications)
                .WithRequired(e => e.Post)
                .HasForeignKey(e => e.Post_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Post>()
                .HasMany(e => e.ReportPosts)
                .WithRequired(e => e.Post)
                .HasForeignKey(e => e.Post_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Comment>()
                .HasMany(e => e.Likes)
                .WithOptional(e => e.Comment)
                .HasForeignKey(e => e.Comment_Id)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Comment>()
            //    .HasMany(e => e.TrendLogs)
            //    .WithOptional(e => e.Comment)
            //    .HasForeignKey(e => e.Comment_Id)
            //    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Group>()
                .HasMany(e => e.Posts)
                .WithOptional(e => e.Group)
                .HasForeignKey(e => e.Group_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Group>()
                .HasMany(e => e.GroupMembers)
                .WithRequired(e => e.Group)
                .HasForeignKey(e => e.Group_Id)
                .WillCascadeOnDelete(false);         

            modelBuilder.Entity<User>()
                .HasMany(e => e.FirstUserActivityLogs)
                .WithRequired(e => e.FirstUser)
                .HasForeignKey(e => e.FirstUser_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.SecondUserActivityLogs)
                .WithOptional(e => e.SecondUser)
                .HasForeignKey(e => e.SecondUser_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.FirstUserBlockUsers)
                .WithRequired(e => e.FirstUser)
                .HasForeignKey(e => e.FirstUser_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.SecondUserBlockUsers)
                .WithRequired(e => e.SecondUser)
                .HasForeignKey(e => e.SecondUser_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.FirstUserReportUsers)
                .WithRequired(e => e.FirstUser)
                .HasForeignKey(e => e.FirstUser_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(e => e.SecondUserReportUsers)
                .WithRequired(e => e.SecondUser)
                .HasForeignKey(e => e.SecondUser_Id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Media>()
               .HasMany(e => e.MediaUserViews)
               .WithRequired(e => e.Media)
               .HasForeignKey(e => e.MediaId)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<Interest>()
             .HasOptional(x => x.ParentInterest);

            modelBuilder.Entity<User>()
                .HasMany(e => e.UserNotificationSetting)
                .WithRequired(e => e.User)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<User>()
                .HasMany(x => x.UserLanguageMappings)
                .WithRequired()
                .HasForeignKey(x => x.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Languages>()
                .HasMany(x => x.UserLanguageMappings)
                .WithRequired()
                .HasForeignKey(x => x.LanguageId)
                .WillCascadeOnDelete(false);
        }
    }
}
