using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.CustomModels
{
    public class NotificationModel
    {
        public string Title { get; set; }

        public string Message { get; set; }

        public int NotificationId { get; set; }

        public int EntityType { get; set; }

        public int EntityId { get; set; }
    }
    public class NotificationWebModel
    {
        public string title { get; set; }

        public string body { get; set; }

        public string click_action { get; set; }

        public int NotificationId { get; set; }

        public bool isread { get; set; }

        public int entityId { get; set; }

        public int entityType { get; set; }

        public int episodeId { get; set; }

    }
    public class NotificationWebsiteParentModel
    {
        public NotificationWebsiteParentModel()
        {
            notification = new NotificationWebModel();
        }

        public string to { get; set; }
        public NotificationWebModel notification { get; set; }
    }
    public class NotificationMessage
    {
        public string[] registration_ids { get; set; }
        public SendNotification notification { get; set; }
        public object data { get; set; }
    }
    public class SendNotification
    {
        public string title { get; set; }
        public string text { get; set; }
    }
    public class DynamicValuesModel
    {
        public int entityid { get; set; }
        public int entitytype { get; set; }
        public int notificationid { get; set; }
        public bool isread { get; set; }
    }
}
