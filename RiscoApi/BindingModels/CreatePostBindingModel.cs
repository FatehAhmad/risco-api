using DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.BindingModels
{
    public class CreatePostBindingModel
    {
        public string Text { get; set; }
        public string[] Texts { get; set; }

        [Required]
        public int Visibility { get; set; }
        
        public int RiskLevel { get; set; }

        public string Location { get; set; } = "";


        public double? Latitude { get; set; }


        public double? Longitude { get; set; }

        public string ImageUrls { get; set; }

        public string VideoUrls { get; set; }

        public int? Group_Id { get; set; }

        public bool IsPoll { get; set; }

        public string Interests { get; set; }

        public string Language { get; set; }

        public short PollType { get; set; }

        public short PostType { get; set; }
        public int? ParentPost_Id { get; set; }

        public int ExpireAfterHours { get; set; }

        public List<PollOptionsBindingModel> PollOptions { get; set; }

    }
    public class PollOptionsBindingModel
    {
        public string Title { get; set; }

        public string MediaUrl { get; set; }

        public int Post_Id { get; set; }
    }



    public class SharePostBindingModel
    {
        public string Text { get; set; }

        [Required]
        public int Visibility { get; set; }

        public int RiskLevel { get; set; }

        public string Location { get; set; }
        
        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string ImageUrls { get; set; }

        public string VideoUrls { get; set; }

        public int? Group_Id { get; set; }

        public string Interests { get; set; }

        public int SharePost_Id { get; set; }
        public short PostType { get; set; }

    }

    public class ThreadedPostBindingModel
    {
        public List<CreatePostBindingModel> Posts { get; set; }
    }

}