using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public partial class NotificationType
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public int? NotificationCategoryId { get; set; }
        public virtual NotificationCategory NotificationCategory { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        [NotMapped]
        public bool Status { get; set; }
    }
}
