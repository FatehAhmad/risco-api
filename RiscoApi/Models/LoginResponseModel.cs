using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class LoginResponseModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string CoverPictureUrl { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int UnreadNotifications { get; set; }
        public bool GroupsNotification { get; set; }// false mean you will not receive notifications
        public bool PostsNotification { get; set; }// false mean you will not receive notifications
        public bool MuteYouDontFollow { get; set; }// false mean you will not receive notifications
        public bool MuteDontFollowYou { get; set; }// false mean you will not receive notifications
        public string LastLoginFrom { get; set; }
        public DateTime LastLoginTime { get; set; }
        public int ReportCount { get; set; }
        public int? SignInType { get; set; }
        public string UserName { get; set; }
        public short? Status { get; set; }
        public bool AccountBlocked { get; set; }
        public DateTime JoinedOn { get; set; }
        public bool IsNotificationsOn { get; set; }
        public Token Token { get; set; }
        public int? Gender { get; set; }
        public string Language { get; set; }
        public List<Languages> Languages { get; set; }
        public bool IsLoginVerification { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string AboutMe { get; set; }
        public bool IsVideoAutoPlay { get; set; }
        public string Interests { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneConfirmed { get; set; }
        public bool IsMessagingAllowed { get; set; } = false;
        public bool MuteUnverifiedEmail { get; set; }
        public bool MuteUnverifiedPhone { get; set; }
        public bool IsMute { get; set; }
        public bool IsPostLocation { get; set; }
        public int TaggingPrivacy { get; set; }
        public bool FindByEmail { get; set; }
        public bool FindByPhone { get; set; }
        public int MessagePrivacy { get; set; }
        public bool IsDeleted { get; set; }
        public int PostCount { get; set; }
        public int FollowingCount { get; set; }
        public int FollowersCount { get; set; }
        public int MediaCount { get; set; }
        public bool IsFollowing { get; set; }
        public bool IsFollower { get; set; }
        public bool DeActive { get; set; }
        public bool IsBlocked { get; set; }
        public bool isAllowWatchFollowerFollowings { get; set; }
    }
}