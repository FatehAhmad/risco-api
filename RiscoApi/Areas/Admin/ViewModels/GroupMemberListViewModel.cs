using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Areas.Admin.ViewModels
{
    public class GroupMemberListViewModel
    {
        public GroupMemberListViewModel()
        {
            GroupMember = new List<GroupMember>();
        }
        public int GroupMemberCount { get; set; }
        public List<GroupMember> GroupMember { get; set; }
    }
    public class GroupDataViewModel
    {
        public GroupDataViewModel()
        {
            Group = new Group();
            GroupMembers = new List<User>();
        }
        public Group Group { get; set; }
        public List<User> GroupMembers { get; set; }
    }
}