using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.ViewModels
{
    public class NotificationsViewModel
    {
        public NotificationsViewModel()
        {
            Notifications = new List<Notification>();
        }
        public List<Notification> Notifications { get; set; }
        public int TotalRecords { get; set; }
    }
}