using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Areas.Admin.ViewModels
{
    public class SendNotificationModel
    {
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