namespace DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            //PaymentCards = new HashSet<PaymentCard>();
            //Favourites = new HashSet<Favourite>();
            ForgotPasswordTokens = new HashSet<ForgotPasswordToken>();
            SendingUserNotifications = new HashSet<Notification>();
            //Orders = new HashSet<Order>();
            //ProductRatings = new HashSet<ProductRating>();
            UserRatings = new HashSet<UserRatings>();
            //DeliveryManRatings = new HashSet<DeliveryManRatings>();
            AppRatings = new HashSet<AppRatings>();
            UserAddresses = new HashSet<UserAddress>();
            UserDevices = new HashSet<UserDevice>();
            //StoreRatings = new HashSet<StoreRatings>();
            UserSubscriptions = new HashSet<UserSubscriptions>();
            Feedback = new HashSet<ContactUs>();
            VerifyNumberCodes = new HashSet<VerifyNumberCodes>();
            Posts = new HashSet<Post>();
            UserGroups = new HashSet<Group>();
            Likes = new HashSet<Like>();
            Comments = new HashSet<Comment>();
            Shares = new HashSet<Share>();
            TrendLogs = new HashSet<TrendLog>();
            FirstUserHidePosts = new HashSet<HidePost>();
            SecondUserHidePosts = new HashSet<HidePost>();
            FirstUserHideAllPosts = new HashSet<HideAllPost>();
            SecondUserHideAllPosts = new HashSet<HideAllPost>();
            FirstUserFollowFollower = new HashSet<FollowFollower>();
            SecondUserFollowFollower = new HashSet<FollowFollower>();
            TurnOffNotifications = new HashSet<TurnOffNotification>();
            ReportPosts = new HashSet<ReportPost>();
            FirstUserTopFollowerLog = new HashSet<TopFollowerLog>();
            SecondUserTopFollowerLog = new HashSet<TopFollowerLog>();
            GroupMembers = new HashSet<GroupMember>();
            FirstUserActivityLogs = new HashSet<ActivityLog>();
            SecondUserActivityLogs = new HashSet<ActivityLog>();
            FirstUserBlockUsers = new HashSet<BlockUser>();
            SecondUserBlockUsers = new HashSet<BlockUser>();
            FirstUserReportUsers = new HashSet<ReportUser>();
            SecondUserReportUsers = new HashSet<ReportUser>();
            ReceivingUserNotifications = new HashSet<Notification>();
            UserProfiles = new HashSet<UserProfiles>();
            Interest = new List<DAL.Interest>();
            Medias = new HashSet<Media>();
            MuteUsers_First= new HashSet<MuteUser>();
            MuteUsers_Second = new HashSet<MuteUser>();
            UserNotificationSetting = new HashSet<UserNotificationSetting>();
            UserLanguageMappings = new HashSet<UserLanguageMapping>();
        }

        public int Id { get; set; }

        //[StringLength(100)]
        //public string FirstName { get; set; }

        //[StringLength(100)]
        //public string LastName { get; set; }

        [StringLength(200)]
        public string FullName { get; set; }

        public string ProfilePictureUrl { get; set; }

        public string CoverPictureUrl { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        [NotMapped]
        public int UnreadNotifications { get; set; }

        public bool GroupsNotification { get; set; }// false mean you will not receive notifications

        public bool PostsNotification { get; set; }// false mean you will not receive notifications

        public bool MuteYouDontFollow { get; set; }// false mean you will not receive notifications

        public bool MuteDontFollowYou { get; set; }// false mean you will not receive notifications

        public string LastLoginFrom { get; set; }

        public DateTime LastLoginTime { get; set; }

        //public bool FindByEmail { get; set; }

        //public bool FindByPhone { get; set; }

        //public bool AllowAllToSendMessage { get; set; }

        //public bool MessageOfPeopleIFollow { get; set; }

        //public bool MessageOfPeopleFollowMe { get; set; }

        //public bool AllowNoneToSendMessage { get; set; }

        //public bool AllowFollowerFollowingToSendMessage { get; set; }

        //public string ZipCode { get; set; }

        //public string DateofBirth { get; set; }

        public int ReportCount { get; set; }

        public int? SignInType { get; set; }

        public string UserName { get; set; }

        public short? Status { get; set; }

        public bool AccountBlocked { get; set; }

        public DateTime JoinedOn { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<PaymentCard> PaymentCards { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Favourite> Favourites { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserRatings> UserRatings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<DeliveryManRatings> DeliveryManRatings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<ForgotPasswordToken> ForgotPasswordTokens { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Notification> SendingUserNotifications { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Notification> ReceivingUserNotifications { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Order> Orders { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AppRatings> AppRatings { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<ProductRating> ProductRatings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserAddress> UserAddresses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserDevice> UserDevices { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<StoreRatings> StoreRatings { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserSubscriptions> UserSubscriptions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ContactUs> Feedback { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VerifyNumberCodes> VerifyNumberCodes { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserProfiles> UserProfiles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

        [JsonIgnore]
        public virtual ICollection<Post> Posts { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Group> UserGroups { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Like> Likes { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comments { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Share> Shares { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrendLog> TrendLogs { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HidePost> FirstUserHidePosts { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HidePost> SecondUserHidePosts { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HideAllPost> FirstUserHideAllPosts { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HideAllPost> SecondUserHideAllPosts { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FollowFollower> FirstUserFollowFollower { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FollowFollower> SecondUserFollowFollower { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TurnOffNotification> TurnOffNotifications { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReportPost> ReportPosts { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TopFollowerLog> FirstUserTopFollowerLog { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TopFollowerLog> SecondUserTopFollowerLog { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PollOptionVote> PollOptionVote { get; set; }
        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserLanguageMapping> UserLanguageMappings { get; set; }


        public bool IsNotificationsOn { get; set; }

        [NotMapped]
        public Token Token { get; set; }

        [NotMapped]
        public Settings BasketSettings { get; set; }

        public int? Gender { get; set; }

        public string Language { get; set; }

        /// <summary>
        /// Two Way Authentication
        /// </summary>
        public bool IsLoginVerification { get; set; }

        public string CountryCode { get; set; }
        public string CountryName { get; set; }

        public string AboutMe { get; set; }

        public bool IsVideoAutoPlay { get; set; }

        public string Interests { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool PhoneConfirmed { get; set; }

        [NotMapped]
        public bool IsMessagingAllowed { get; set; } = false;

        #region Notifications Settings

        //public bool IsPeopleIDontFollow { get; set; }

        //public bool IsPeopleWhoDontFollowMe { get; set; }

        //public bool IsPeopleWithNewAccount { get; set; }

        //public bool IsPeopleWithDefaultProfilePhoto { get; set; }

        /// <summary>
        /// Mute Notification From People With Unverified Email
        /// </summary>
        public bool MuteUnverifiedEmail { get; set; }

        /// <summary>
        /// Mute Notification From People With Unverified Phone
        /// </summary>
        public bool MuteUnverifiedPhone { get; set; }

        [NotMapped]
        public bool IsMute { get; set; }

        #endregion

        #region Privacy Settings

        #region Post Settings
        public bool IsPostLocation { get; set; }

        #endregion

        #region Post Tagging


        public int TaggingPrivacy { get; set; }

        #endregion

        #region Discoverability

        public bool FindByEmail { get; set; }

        public bool FindByPhone { get; set; }

        #endregion

        #region Direct Messages

        public int MessagePrivacy { get; set; }

        #endregion

        #endregion

        public bool IsDeleted { get; set; }

        [NotMapped]
        public int PostCount { get; set; }

        [NotMapped]
        public int IncidentCount { get; set; }

        [NotMapped]
        public int FollowingCount { get; set; }

        [NotMapped]
        public int LikesCount { get; set; }

        [NotMapped]
        public int RepliesCount { get; set; }

        [NotMapped]
        public int FollowersCount { get; set; }

        [NotMapped]
        public int MediaCount { get; set; }

        /// <summary>
        /// Either am I following him
        /// </summary>
        [NotMapped]
        public bool IsFollowing { get; set; }

        /// <summary>
        /// Either other User is following me?
        /// </summary>
        [NotMapped]
        public bool IsFollower { get; set; }

        public bool DeActive { get; set; }

        [NotMapped]
        public bool IsBlocked { get; set; }

        [NotMapped]
        public List<Interest> Interest { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GroupMember> GroupMembers { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActivityLog> FirstUserActivityLogs { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActivityLog> SecondUserActivityLogs { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BlockUser> FirstUserBlockUsers { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BlockUser> SecondUserBlockUsers { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReportUser> FirstUserReportUsers { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReportUser> SecondUserReportUsers { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Media> Medias { get; set; }
        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MuteUser> MuteUsers_First { get; set; }
        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MuteUser> MuteUsers_Second { get; set; }
        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserNotificationSetting> UserNotificationSetting { get; set; }

        public bool isAllowWatchFollowerFollowings { get; set; }

        [NotMapped]
        public List<Languages> Languages { get; set; }

        [NotMapped]
        public List<Languages> AllLanguages { get; set; }
    }
}
