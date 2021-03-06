using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.AppsViewModels
{
    public class MediaListViewModel
    {
        public MediaListViewModel()
        {
            Medias = new List<Media>();
        }
        public int MediaCount { get; set; }
        public List<Media> Medias { get; set; }
    }

    public class ActivityLogListViewModel
    {
        public ActivityLogListViewModel()
        {
            ActivityLogs = new List<ActivityLog>();
        }
        public int ActivityLogCount { get; set; }
        public List<ActivityLog> ActivityLogs { get; set; }
    }
}