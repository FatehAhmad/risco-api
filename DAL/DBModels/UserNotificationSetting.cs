using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public partial class UserNotificationSetting
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int NotificationTypeId { get; set; }
        public virtual NotificationType NotificationType {get;set;}
        public virtual User User { get; set; }
        public bool IsPush { get; set; }
        public bool IsEmail { get; set; }
        public bool IsSMS { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; } 
        public bool IsDeleted { get; set; }
    }
}
