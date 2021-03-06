using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.Models
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
}