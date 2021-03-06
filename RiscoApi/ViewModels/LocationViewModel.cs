using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.ViewModels
{
    public class LocationViewModel
    {
        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public int Id { get; set; }

    }

    public class HeatMapViewModel
    {
        public HeatMapViewModel()
        {
            Low = new List<LocationViewModel>();
            Medium = new List<LocationViewModel>();
            High = new List<LocationViewModel>();
        }

        public List<LocationViewModel> Low { get; set; }
        public List<LocationViewModel> Medium { get; set; }
        public List<LocationViewModel> High { get; set; }
        public List<Media> Medias { get; set; }
    }

}