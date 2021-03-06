using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Newtonsoft.Json;

namespace DAL
{
    public class Media
    {
        public Media()
        {
            MediaUserViews = new HashSet<MediaUserViews>();
        }
        public int Id { get; set; }

        public int Type { get; set; }

        public string Url { get; set; }

        [ForeignKey("Comment")]
        public int? Comment_Id { get; set; }

        public Comment Comment { get; set; }

        public int Post_Id { get; set; }

        public virtual Post Post { get; set; }

        public int? User_Id { get; set; }

        public virtual User User { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }

        [JsonIgnore]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MediaUserViews> MediaUserViews { get; set; }

        [NotMapped]
        public int ViewsCount { get { return MediaUserViews.Count; } }

        public string ThumbnailUrl { get; set; }


    }
}
