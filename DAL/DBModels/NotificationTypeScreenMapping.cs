using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DBModels
{
    public partial class NotificationTypeScreenMapping
    {
        public int Id { get; set; }
        public int? NotificationTypeId { get; set; }
        public virtual NotificationType NotificationType { get; set; }
        public int ScreenId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
