namespace DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Post
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Post()
        {
            Medias = new HashSet<Media>();
            Likes = new HashSet<Like>();
            Comments = new HashSet<Comment>();
            Shares = new HashSet<Share>();
            //TrendLogs = new HashSet<TrendLog>();
            HidePosts = new HashSet<HidePost>();
            TurnOffNotifications = new HashSet<TurnOffNotification>();
            ReportPosts = new HashSet<ReportPost>();
            PostCategories = new List<PostCategories>();
            //PostLanguageCodes = new List<PostLanguageCodes>();
            PollOptions = new HashSet<PollOptions>();            
        }

        public int Id { get; set; }

        public string Text { get; set; }

        public int Visibility { get; set; }

        public int RiskLevel { get; set; }

        public string Location { get; set; }

        public string Language { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public int User_Id { get; set; }

        public virtual User User { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public int? Group_Id { get; set; }

        public virtual Group Group { get; set; }

        public bool IsPoll { get; set; }

        public short PollType { get; set; }

        public short PostType { get; set; }

        public int TotalVotes { get; set; }

        //public DateTime CreatedOn { get; set; }

        public DateTime? PollExpiryTime { get; set; }

        public bool IsExpired { get; set; }

        // for sharing post

        public int? SharePost_Id { get; set; } = 0;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<Post> SharePostParent { get; set; }

        public virtual Post SharedParent { get; set; }

        // sharing post end

        // Threaded Post
        public int? ParentPost_Id { get; set; } = 0;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<Post> ExtendedPostList { get; set; }
        public virtual Post ExtendedPost { get; set; }
        public int? Sequence { get; set; }

        // Threaded Post end

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Media> Medias { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Like> Likes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comments { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Share> Shares { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TrendLog> TrendLogs { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HidePost> HidePosts { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TurnOffNotification> TurnOffNotifications { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<PostCategories> PostCategories { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual List<PostLanguageCodes> PostLanguageCodes { get; set; }        

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReportPost> ReportPosts { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PollOptions> PollOptions { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PollOptionVote> PollOptionVote { get; set; }

        [NotMapped]
        public bool IsLiked { get; set; }

        [NotMapped]
        public int LikesCount { get; set; }

        [NotMapped]
        public int CommentsCount { get; set; }

        [NotMapped]
        public int ShareCount { get; set; }

        [NotMapped]
        public bool IsUserFollow { get; set; }

        [NotMapped]
        public bool IsPostOwner { get; set; }
        [NotMapped]
        public int PollUserCount { get { return (PollOptionVote == null || PollOptionVote.Count == 0) ? 0 : PollOptionVote.Count; } }
        [NotMapped]
        public bool IsNotificationOn { get; set; }
        [NotMapped]
        public bool HasComment { get; set; }
        [NotMapped]
        public bool IsShared { get; set; }
    }
}
