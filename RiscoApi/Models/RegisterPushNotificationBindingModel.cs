using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BasketApi.Models
{
    public class RegisterPushNotificationBindingModel
    {
        public string DeviceName { get; set; }

        [Required]
        public string UDID { get; set; }

        [Required, Range(1,3)]
        public int Platform { get; set; }

        [Required]
        public bool IsPlayStore { get; set; }

        [Required]
        public int User_Id { get; set; }
        
        [Required]
        public bool IsProduction { get; set; }

        public string AuthToken { get; set; }
    }
}