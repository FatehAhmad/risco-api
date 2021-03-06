namespace DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class GroupMember
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GroupMember()
        {
        }

        public int Id { get; set; }

        public int User_Id { get; set; }

        public virtual User User { get; set; }

        public int Group_Id { get; set; }

        public virtual Group Group { get; set; }

        public int Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public bool IsBlockedForOtherUser { get; set; }

        public bool IsDeleted { get; set; }

        [NotMapped]
        public bool IsUserFollow { get; set; }
    }
}
