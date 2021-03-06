namespace DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Group
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Group()
        {
            Posts = new HashSet<Post>();
            GroupMembers = new HashSet<GroupMember>();
            GroupCategories = new List<GroupCategories>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }      

        public int User_Id { get; set; }

        public bool AdminViewBlocked { get; set; }

        public virtual User User { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public virtual List<GroupCategories> GroupCategories { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Post> Posts { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GroupMember> GroupMembers { get; set; }

        [NotMapped]
        public int PostsCount { get; set; }
        [NotMapped]
        public int JoinRequestCount { get; set; }

        [NotMapped]
        public int MembersCount { get; set; }

        [NotMapped]
        public int Status { get; set; }

        [NotMapped]
        public bool IsAdmin { get; set; }
    }
}
