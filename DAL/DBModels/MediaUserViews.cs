namespace DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    

    public partial class MediaUserViews
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MediaUserViews()
        {
        }
        public int Id { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }

        public int MediaId { get; set; }

        public virtual Media Media { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }

    }
}

