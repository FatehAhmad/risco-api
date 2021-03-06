using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.BindingModels
{
    public class CreateGroupBindingModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public string GroupMembersIds { get; set; }

        //[Required]
        public string ImageUrl { get; set; }        

        public string Interests { get; set; }
    }

    public class EditGroupBindingModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public string GroupMembersIds { get; set; }

        //[Required]
        public string ImageUrl { get; set; }

        
        public string Interests { get; set; }
    }
}