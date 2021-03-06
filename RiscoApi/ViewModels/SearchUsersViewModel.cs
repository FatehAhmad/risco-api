using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Areas.Admin.ViewModels;

namespace BasketApi.ViewModels
{
    public class SearchUsersViewModel
    {
        public SearchUsersViewModel()
        {
            User = new User();
            Followers = new List<User>();
            Followings = new List<User>();
            Groups = new List<Group>();
            Medias = new List<Media>();
            ReportUsers = new List<ReportUser>();
            UserDetails = new UserDetailsViewModel();
            JoinedGroups = new List<Group>();
        }
        public UserDetailsViewModel UserDetails { get; set; }
        public User User { get; set; }
        public List<User> Followers { get; set; }
        public List<User> Followings { get; set; }
        public List<Group> Groups { get; set; }
        public List<Media> Medias { get; set; }
        public List<ReportUser> ReportUsers { get; set; }
        public List<Group> JoinedGroups { get; set; }


    }
    public class SearchUsers
    {
        public SearchUsers()
        {
            Users = new List<User>();
        }
        public List<User> Users { get; set; }
    }
}