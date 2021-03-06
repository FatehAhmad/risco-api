using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Areas.Admin.ViewModels
{
    public class UserDetailsViewModel
    {

        public User User { get; set; }
        public int TotalPosts { get; set; }
        public int TotalGroups { get; set; }
        public int TotalFollowers { get; set; }
        public int TotalFollowings { get; set; }
        public int TotalShares { get; set; }
        public int TotalLikes { get; set; }
        public int TotalIncidents { get; set; }
        public int TotalMedia { get; set; }
    }

    public class ReportUsersViewModel
    {
        public ReportUsersViewModel()
        {
            ReportUsers = new List<ReportUser>();
        }
        public List<ReportUser> ReportUsers { get; set; }
    }

    public class ReportPostsViewModel
    {
        public ReportPostsViewModel()
        {
            ReportPosts = new List<ReportPost>();
        }
        public List<ReportPost> ReportPosts { get; set; }

    }

    public class ReportPostViewModel
    {
        public ReportPostViewModel()
        {
            ReportPost = new ReportPost();
        }
        public ReportPost ReportPost { get; set; }

    }
}