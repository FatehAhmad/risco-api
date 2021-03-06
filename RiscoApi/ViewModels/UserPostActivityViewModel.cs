using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.ViewModels
{
    public class UserPostActivityViewModel
    {
        public List<Post> Posts { get; set; }
    }

    public class UserIncidentActivityViewModel
    {
        public List<Post> Incidents { get; set; }
    }

    public class UserLikesPostActivityViewModel
    {
        public List<Post> Likes { get; set; }
    }

    public class UserMediasPostActivityViewModel
    {
        public List<Post> Medias { get; set; }
    }

    public class UserRepliesPostActivityViewModel
    {
        public List<Post> Replies { get; set; }
    }
}