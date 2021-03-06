using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Areas.Admin.ViewModels
{
    public class WebDashboardStatsViewModel
    {
        public int ActiveUsers { get; set; }
        public int ActivePosts { get; set; }
        public int ActiveGroups { get; set; }
        public int TodayOrders { get; set; }
        public List<DeviceStats> DeviceUsage { get; set; }
    }

    public class DeviceStats
    {
        public int Platform { get; set; }
        public int Count { get; set; }
        public int Percentage { get; set; }
    }
}