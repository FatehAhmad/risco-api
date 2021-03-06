using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.ViewModels
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

}