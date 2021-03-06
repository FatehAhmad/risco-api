using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public partial class NotificationCategory
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
  
        [NotMapped]
        public bool IsCategory { get; set; }
        [NotMapped]
        public List<NotificationType> NotificationTypeList { get; set; }
        public NotificationCategory()
        {
            NotificationTypeList = new List<NotificationType>();
        }
    }
}
