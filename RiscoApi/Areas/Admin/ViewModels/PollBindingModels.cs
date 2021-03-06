using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Areas.Admin.ViewModels
{
    public class PollBindingModels
    {

    }
    public class VotePollBindingModel
    {
        public int Post_Id { get; set; }

        public int PollOption_Id { get; set; }

        public int User_Id { get; set; }

    }
}