using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.BindingModels
{
    public class NotificationSettingsBindingModel
    {
        [Required]
        public bool GroupsNotification { get; set; }

        [Required]
        public bool PostsNotification { get; set; }

        [Required]
        public bool MuteYouDontFollow { get; set; }
        [Required]
        public bool MuteDontFollowYou { get; set; }
    }
}