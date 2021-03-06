using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Areas.Admin.ViewModels
{
    public class SearchListViewModel
    {
        public SearchListViewModel()
        {
            Users = new List<User>();
            Groups = new List<Group>();
            Posts = new List<Post>();
        }
        public List<User> Users { get; set; }
        public List<Group> Groups { get; set; }
        public List<Post> Posts { get; set; }
    }
}