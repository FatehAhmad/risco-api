namespace DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ActivityLog
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ActivityLog()
        {

        }

        public int Id { get; set; }

        public string Text { get; set; }

        public int FirstUser_Id { get; set; }

        public virtual User FirstUser { get; set; }

        public int? SecondUser_Id { get; set; }

        public virtual User SecondUser { get; set; }

        public int EntityType { get; set; }

        public int EntityId { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
