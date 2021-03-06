using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Areas.Admin.ViewModels
{
    public class GroupListViewModel
    {
        public GroupListViewModel()
        {
            MyGroups = new List<Group>();

            SuggestedGroups = new List<Group>();
        }
        public int MyGroupCount { get; set; }

        public List<Group> MyGroups { get; set; }

        public int SuggestedGroupCount { get; set; }

        public List<Group> SuggestedGroups { get; set; }
    }
}