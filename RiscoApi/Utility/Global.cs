using DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
//using static BasketApi.Utility;
using System.Data.Entity;
using static DAL.CustomModels.Enumeration;
using BLL.Utility;

namespace BasketApi
{
    public static class Global
    {
        public static PushNotifications objPushNotifications = new PushNotifications(false);
        public static int MaximumImageSize = 1024 * 1024 * 10; // 10 Mb
        public static string ImageSize = "10 MB";

        private static int searchStoreRadius = Convert.ToInt32(ConfigurationManager.AppSettings["NearByStoreRadius"]);
        public static double NearbyStoreRadius = searchStoreRadius * 1609.344;

        public class ResponseMessages
        {
            public const string Success = "Success";
            public const string NotFound = "NotFound";
            public const string BadRequest = "BadRequest";
            public const string Conflict = "Conflict";

            public static string CannotBeEmpty(params string[] args)
            {
                try
                {
                    string returnString = "";
                    for (int i = 0; i < args.Length; i++)
                        returnString += args[i] + ", ";
                    returnString = returnString.Remove(returnString.LastIndexOf(','), 1);
                    return returnString + "cannot be empty";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            public static string GenerateInvalid(params string[] args)
            {
                try
                {
                    string returnString = "";
                    for (int i = 0; i < args.Length; i++)
                        returnString += args[i] + ", ";
                    returnString = returnString.Remove(returnString.LastIndexOf(','), 1);
                    return "Invalid " + returnString;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            public static string GenerateAlreadyExists(string arg)
            {
                try
                {
                    return arg + " already exists";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            public static string GenerateNotFound(string arg)
            {
                try
                {
                    return arg + " not found";
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public static string GetActionText(int FirstUser_Id, int SecondUser_Id, int EntityType, int EntityId)
        {
            string text = "";
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    if (FirstUser_Id != SecondUser_Id)
                    {
                        User _SecondUser = new User();

                        if (SecondUser_Id != 0)
                            _SecondUser = ctx.Users.FirstOrDefault(x => x.Id == SecondUser_Id);

                        if (EntityType == (int)RiscoEntityTypes.LikeOnPost)
                        {
                            text = "likes " + _SecondUser.FullName + "'s post.";
                        }
                        else if (EntityType == (int)RiscoEntityTypes.CommentOnPost)
                        {
                            text = "commented " + _SecondUser.FullName + "'s post.";
                        }
                        else if (EntityType == (int)RiscoEntityTypes.Follow)
                        {
                            text = "followed " + _SecondUser.FullName + ".";
                        }
                        else if (EntityType == (int)RiscoEntityTypes.Post)
                        {
                            text = "posted on your wall.";
                        }
                        else if (EntityType == (int)RiscoEntityTypes.SharePost)
                        {
                            text = "shared a post.";
                        }
                        else if (EntityType == (int)RiscoEntityTypes.BlockUser)
                            text = "blocked " + _SecondUser.FullName + ".";
                        else if (EntityType == (int)RiscoEntityTypes.ReportUser)
                            text = "report " + _SecondUser.FullName + "'s account.";
                        else if (EntityType == (int)RiscoEntityTypes.ReportPost)
                            text = "report " + _SecondUser.FullName + "'s post.";
                        else if (EntityType == (int)RiscoEntityTypes.CreateGroup)
                        {
                            var group = ctx.Groups.FirstOrDefault(x => x.Id == EntityId);
                            text = "created " + group.Name + ".";
                        }
                    }
                    else
                    {
                        switch (EntityType)
                        {
                            case (int)RiscoEntityTypes.CommentOnPost:
                                text = "commented on his own post.";
                                break;
                            case (int)RiscoEntityTypes.LikeOnPost:
                                text = "likes his own post.";
                                break;
                            case (int)RiscoEntityTypes.ProfileImage:
                                text = "updated his profile picture.";
                                break;
                            case (int)RiscoEntityTypes.NotificationSettings:
                                text = "updated his notification settings.";
                                break;
                            case (int)RiscoEntityTypes.AccountSettings:
                                text = "updated his account info.";
                                break;
                            case (int)RiscoEntityTypes.PrivacySettings:
                                text = "updated his privacy settings.";
                                break;
                            case (int)RiscoEntityTypes.UpdateCover:
                                text = "updated his cover photo.";
                                break;
                            case (int)RiscoEntityTypes.ChangePassword:
                                text = "updated his password.";
                                break;
                            case (int)RiscoEntityTypes.SharePost:
                                text = "shared a post.";
                                break;
                            case (int)RiscoEntityTypes.Post:
                                text = "posted on his wall.";
                                break;
                            default:
                                break;
                        }
                    }
                }
                return text;
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
                return text;
            }
        }
        public static void SetActivityLog(int FirstUser_Id, int SecondUser_Id, int EntityType, int EntityId)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    string text = GetActionText(FirstUser_Id, SecondUser_Id, EntityType, EntityId);

                    if (FirstUser_Id != SecondUser_Id)
                    {
                        ActivityLog activityLog = new ActivityLog
                        {
                            FirstUser_Id = FirstUser_Id,
                            SecondUser_Id = SecondUser_Id,
                            EntityType = EntityType,
                            EntityId = EntityId,
                            Text = text,
                            CreatedDate = DateTime.UtcNow
                        };

                        ctx.ActivityLogs.Add(activityLog);
                        ctx.SaveChanges();
                    }
                    else
                    {
                        ActivityLog activityLog = new ActivityLog
                        {
                            FirstUser_Id = FirstUser_Id,
                            EntityType = EntityType,
                            EntityId = EntityId,
                            Text = text,
                            CreatedDate = DateTime.UtcNow
                        };

                        ctx.ActivityLogs.Add(activityLog);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }
        public static bool isInterestBasedNotification(int? sendingUser_Id,int? receivingUser_Id)
        {
            bool isNotify = false;
            Post sendingUserPost = null;
            User receivingUser = null;

            using (RiscoContext ctx = new RiscoContext())
            {
                sendingUserPost = ctx.Posts.Where(x => x.User_Id == sendingUser_Id && !x.IsDeleted).FirstOrDefault();
                receivingUser = ctx.Users.Where(x => x.Id == receivingUser_Id && !x.IsDeleted).FirstOrDefault();

                List<int> lstPostInterestIds = sendingUserPost.PostCategories.Select(x => x.Interest_Id).ToList();
                string strUserInterests = receivingUser.Interests;
                List<int> lstUserInterestIds = strUserInterests.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();

                if (lstPostInterestIds.Any(x => lstUserInterestIds.Contains(x)))
                    isNotify = true;
                else
                    isNotify = false;

            }

            return isNotify;
        }
        public static void GenerateNotification(int EntityType, int EntityId, int SendingUser_Id, int? ReceivingUser_Id = 0, int? ChildCommentId = 0, bool ToDifferentUsers = false, List<int> ReceivingUser_Ids = null, int ChildEntityType = 0, int? CommentId = 0)
        {
            using (RiscoContext ctx = new RiscoContext())
            {
                List<UserDevice> usersToPushAndroid = new List<UserDevice>();
                List<UserDevice> usersToPushIOS = new List<UserDevice>();
                List<UserDevice> usersToPushWeb = new List<UserDevice>();

                User ReceivingUser = new User();
                Notification notf = new Notification();

                if (ReceivingUser_Id.HasValue && ReceivingUser_Id != 0)
                {
                    ReceivingUser_Ids = new List<int>();
                    ReceivingUser_Ids.Add(ReceivingUser_Id.Value);
                }

                // remove user's who turned off his/her notification for specific post
                if(EntityType == (int)RiscoEntityTypes.Post || EntityType == (int)RiscoEntityTypes.LikeOnPost || EntityType == (int)RiscoEntityTypes.CommentOnPost || EntityType == (int)RiscoEntityTypes.ReplyOnComment
                    || EntityType == (int)RiscoEntityTypes.SharePost || EntityType == (int)RiscoEntityTypes.LikeOnComment || EntityType == (int)RiscoEntityTypes.ReportPost)
                {
                    List<int> lstUserIds = ctx.TurnOffNotifications.Where(x => x.Post_Id == EntityId && !x.IsDeleted).Select(x => x.User_Id).Distinct().ToList();
                    for (int i = 0; i < ReceivingUser_Ids.Count; i++)
                    {
                        for (int j = 0; j < lstUserIds.Count; j++)
                        {
                            if (ReceivingUser_Ids[i] == lstUserIds[j])
                                ReceivingUser_Ids.RemoveAt(i);
                        }
                    }
                }

                var SendingUser = ctx.Users.Where(x => x.Id == SendingUser_Id).FirstOrDefault();
                var post = ctx.Posts.Include(x => x.User).FirstOrDefault(x => x.Id == EntityId);

                foreach (var recieverId in ReceivingUser_Ids)
                {
                    if (ReceivingUser.Id != SendingUser.Id /*&& !ReceivingUser.PostsNotification*/) // if sender and receiver are same user then he should not get notification
                    {
                        ReceivingUser = ctx.Users.Include(x => x.UserDevices).Include(x => x.MuteUsers_First).Include(x => x.UserNotificationSetting).FirstOrDefault(x => x.Id == recieverId);

                        //Mute User
                        if (ReceivingUser.MuteUsers_First.Any(x => x.SecondUser_Id == SendingUser.Id))
                        {
                            continue;
                        }
                        //Block User
                        if (ctx.BlockUsers.Any(x => (x.FirstUser_Id == recieverId && x.SecondUser_Id == SendingUser_Id) || (x.FirstUser_Id == SendingUser_Id && x.SecondUser_Id == recieverId) && !x.IsDeleted))
                        {
                            continue;
                        }

                        #region Receiver Notification Setting 

                        //YouDontFollow
                        if (ReceivingUser.UserNotificationSetting.Any(x => x.NotificationTypeId == (int)NotificationTypes.YouDontFollow && !x.IsDeleted))
                        {
                            if (ctx.FollowFollowers.Any(x => x.FirstUser_Id == recieverId && x.SecondUser_Id == SendingUser_Id && !x.IsDeleted))
                            {
                                continue;
                            }
                        }
                        //WhoDontFollowYou
                        if (ReceivingUser.UserNotificationSetting.Any(x => x.NotificationTypeId == (int)NotificationTypes.WhoDontFollowYou && !x.IsDeleted))
                        {
                            if (ctx.FollowFollowers.Any(x => x.FirstUser_Id == SendingUser_Id && x.SecondUser_Id == recieverId && !x.IsDeleted))
                            {
                                continue;
                            }
                        }
                        //WhoHaventConfirmEmail
                        if (SendingUser.EmailConfirmed == false)
                        {
                            if (ReceivingUser.UserNotificationSetting.Any(x => x.NotificationTypeId == (int)NotificationTypes.WhoHaventConfirmEmail && !x.IsDeleted))
                            {
                                continue;
                            }
                        }
                        //WhoHaventConfirmPhoneNumber
                        if (SendingUser.PhoneConfirmed == false)
                        {
                            if (ReceivingUser.UserNotificationSetting.Any(x => x.NotificationTypeId == (int)NotificationTypes.WhoHaventConfirmPhoneNumber && !x.IsDeleted))
                            {
                                continue;
                            }
                        }
                        //IsNotificationsOn (Post, Incidents, Groups, New Posts, New Incidents)

                        //Mute activities Notification: (Posts, Incidents, Groups)
                        if (EntityType == (int)RiscoEntityTypes.LikeOnPost
                            || EntityType == (int)RiscoEntityTypes.CommentOnPost
                            || EntityType == (int)RiscoEntityTypes.ReplyOnComment
                            || EntityType == (int)RiscoEntityTypes.LikeOnComment)
                        {
                            if (post.PostType == (short)PostTypes.Post && !post.Group_Id.HasValue)
                            {
                                if (ReceivingUser.UserNotificationSetting.Any(x => x.NotificationTypeId == (int)NotificationTypes.Posts && !x.IsDeleted))
                                {
                                    continue;
                                }
                            }
                            if (post.PostType == (short)PostTypes.Incident && !post.Group_Id.HasValue)
                            {
                                if (ReceivingUser.UserNotificationSetting.Any(x => x.NotificationTypeId == (int)NotificationTypes.Incidents && !x.IsDeleted))
                                {
                                    continue;
                                }
                            }
                            if (post.Group_Id.HasValue)
                            {
                                if (ReceivingUser.UserNotificationSetting.Any(x => x.NotificationTypeId == (int)NotificationTypes.Groups && !x.IsDeleted))
                                {
                                    continue;
                                }
                            }
                        }
                        //Post / Incidents notifications (New posts, New Incidents)
                        if (EntityType == (int)RiscoEntityTypes.Post)
                        {
                            if(post.PostType == (short)PostTypes.Post && !post.Group_Id.HasValue)
                            {
                                if (!ReceivingUser.UserNotificationSetting.Any(x => x.NotificationTypeId == (int)NotificationTypes.Newposts && !x.IsDeleted))
                                {
                                    continue;
                                }
                            }
                            if (post.PostType == (short)PostTypes.Incident && !post.Group_Id.HasValue)
                            {
                                if (!ReceivingUser.UserNotificationSetting.Any(x => x.NotificationTypeId == (int)NotificationTypes.NewIncidents && !x.IsDeleted))
                                {
                                    continue;
                                }
                            }
                        }

                        #endregion

                        string Description = "";
                        Description = GetNotificationText(EntityType, EntityId, SendingUser_Id, SendingUser, ReceivingUser_Id, ChildCommentId, ChildEntityType, CommentId);

                        notf = new Notification
                        {
                            Title = "Risco",
                            Description = Description,
                            SendingUser_Id = SendingUser.Id,
                            ReceivingUser_Id = recieverId,
                            EntityType = EntityType,
                            EntityId = EntityId,
                            Status = (int)NotificationStatus.Unread,
                            CreatedDate = DateTime.UtcNow
                        };
                        ctx.Notifications.Add(notf);

                        usersToPushAndroid = ctx.UserDevices.Where(x => x.User_Id == recieverId && x.Platform == (int)DeviceTypes.Android && x.User.IsNotificationsOn && x.IsActive == true).ToList();
                        usersToPushIOS = ctx.UserDevices.Where(x => x.User_Id == recieverId && x.Platform == (int)DeviceTypes.IOS && x.User.IsNotificationsOn && x.IsActive == true).ToList();
                        usersToPushWeb = ctx.UserDevices.Where(x => x.User_Id == recieverId && x.Platform == (int)DeviceTypes.Web && x.User.IsNotificationsOn && x.IsActive == true).ToList();

                        Utility.SendPushNotifications(usersToPushAndroid, usersToPushIOS, usersToPushWeb: usersToPushWeb, Notification: notf, EntityType: EntityType, EntityId: EntityId);
                        ctx.SaveChanges();
                    }
                }
            }
        }
        public static string GetNotificationText(int EntityType, int EntityId, int SendingUser_Id, User SendingUser, int? ReceivingUser_Id = 0, int? ChildCommentId = 0, int ChildEntityType = 0, int? Comment_Id = 0)
        {
            string Description = "";
            User ReceivingUser = new User();

            using (RiscoContext ctx = new RiscoContext())
            {
                if (ReceivingUser_Id.HasValue && ReceivingUser_Id != 0)
                    ReceivingUser = ctx.Users.Include(x => x.UserDevices).Where(x => x.Id == ReceivingUser_Id).FirstOrDefault();

                if (EntityType == (int)RiscoEntityTypes.LikeOnPost)
                {
                    Description = SendingUser.FullName + " liked your post.";
                }
                else if (EntityType == (int)RiscoEntityTypes.CommentOnPost)
                {
                    Description = SendingUser.FullName + " commented on your post.";
                }
                else if (EntityType == (int)RiscoEntityTypes.LikeOnComment)
                {
                    var post = ctx.Posts.Include(x => x.User).FirstOrDefault(x => x.Id == EntityId);
                    //var comment = ctx.Comments.FirstOrDefault(x => x.Id == Comment_Id);

                    //if (ReceivingUser_Id == comment.User_Id)
                    //{
                    //    Description = SendingUser.FullName + " liked comment on";
                    //    if (ReceivingUser_Id == post.User_Id)
                    //        Description += "";
                    //}
                    if (ReceivingUser_Id == post.User_Id)
                        Description = SendingUser.FullName + " liked comment on your post.";
                    else
                        Description = SendingUser.FullName + " liked your comment on " + post.User.FullName + "'s post.";
                }
                else if (EntityType == (int)RiscoEntityTypes.Post) // when someone create post
                {
                    Description = SendingUser.FullName + " posted ";
                    var post = ctx.Posts.Include(x => x.Group).FirstOrDefault(x => x.Id == EntityId && !x.IsDeleted);

                    if (post.Group_Id.HasValue)
                        Description += "in " + post.Group.Name + ".";
                    else
                        Description += "on ";

                    if (ReceivingUser.Gender == (int)GenderType.Male)
                        Description += "his wall.";
                    else
                        Description += "her wall.";

                }
                else if (EntityType == (int)RiscoEntityTypes.ReplyOnComment)
                {
                    Comment parentComment = new Comment();

                    var post = ctx.Posts.FirstOrDefault(x => x.Id == EntityId && !x.IsDeleted);
                    var childComment = ctx.Comments.FirstOrDefault(x => x.Id == ChildCommentId);
                    if (childComment != null)
                        parentComment = ctx.Comments.Include(x => x.Post).Include(x => x.Post.User).FirstOrDefault(x => x.Id == childComment.ParentComment_Id);

                    if (post != null)
                    {
                        Description = SendingUser.FullName + " replied to";

                        if (ReceivingUser.Id == parentComment.User_Id)
                            Description += " your comment on";
                        else
                        {
                            var ThirdUser = ctx.Users.FirstOrDefault(x => x.Id == parentComment.User_Id);
                            if (ThirdUser != null)
                                Description += " " + ThirdUser.FullName + "'s comment on";
                        }

                        if (parentComment.User_Id == ReceivingUser.Id)
                            Description += " your post.";
                        else
                        {
                            Description += " " + parentComment.Post.User.FullName + "'s  post.";
                        }
                    }
                }
                else if (EntityType == (int)RiscoEntityTypes.Group)                 //add receiving user group noti check later
                {
                    if (ChildEntityType == (int)RiscoEntityTypes.AddInGroup)
                    {
                        var group = ctx.Groups.Include(x => x.GroupMembers).FirstOrDefault(x => x.Id == EntityId && !x.IsDeleted);
                        var Admin = ctx.Users.FirstOrDefault(x => x.Id == SendingUser_Id);
                        if (group != null)
                        {
                            //if (group.GroupMembers.Count > 0)
                            //{
                            //    switch (group.GroupMembers.FirstOrDefault().Status)
                            //    {
                            //        case (int)GroupMemberStatusTypes.Accepted:
                            Description = Admin.FullName + " has invited you to join the Group, " + group.Name + ".";
                            //                break;
                            //            default:
                            //                break;
                            //        }
                            //    }
                        }
                    }
                    else if (ChildEntityType == (int)RiscoEntityTypes.LikeOnPost)
                    {
                        var group = ctx.Posts.Include(x => x.Group).Include(x => x.User).FirstOrDefault(x => x.Id == EntityId);
                        Description = SendingUser.FullName + " liked your post in " + group.Group.Name + ".";
                    }
                    else if (ChildEntityType == (int)RiscoEntityTypes.CommentOnPost)
                    {
                        var group = ctx.Posts.Include(x => x.Group).FirstOrDefault(x => x.Id == EntityId);
                        Description = SendingUser.FullName + " commented on your post in " + group.Group.Name + ".";
                    }
                    else if (ChildEntityType == (int)RiscoEntityTypes.LikeOnComment)
                    {
                        var group = ctx.Posts.Include(x => x.Group).FirstOrDefault(x => x.Id == EntityId);
                        Description = SendingUser.FullName + " liked you comment in " + group.Group.Name + ".";
                    }
                    else if (ChildEntityType == (int)RiscoEntityTypes.ReplyOnComment)
                    {
                        var group = ctx.Posts.Include(x => x.Group).FirstOrDefault(x => x.Id == EntityId);
                        Description = SendingUser.FullName + " replied to your comment in " + group.Group.Name + ".";
                    }
                }
                else if (EntityType == (int)RiscoEntityTypes.SharePost)
                {
                    var post = ctx.Posts.Include(x => x.Group).FirstOrDefault(x => x.Id == EntityId);
                    if (post.Group_Id.HasValue)
                        Description = SendingUser.FullName + " shared your post in " + post.Group.Name + ".";
                    else
                        Description = SendingUser.FullName + " shared your post.";
                }
                else if (EntityType == (int)RiscoEntityTypes.SendGroupRequest)
                {
                    Description = SendingUser.FullName + " sent you join group request.";
                }
                else if (EntityType == (int)RiscoEntityTypes.AcceptGroupRequest)
                {
                    Description = SendingUser.FullName + " accepted your join group request.";
                }
                else if (EntityType == (int)RiscoEntityTypes.Follow)
                {
                    Description = SendingUser.FullName + " started following you.";
                }
            }
            return Description;
        }
        public static void GenerateNotification_Old(int EntityType, int EntityId, int SendingUser_Id, int? ReceivingUser_Id = 0, int? ChildCommentId = 0, bool ToDifferentUsers = false, List<int> ReceivingUser_Ids = null, int ChildEntityType = 0, int? CommentId = 0)
        {
            using (RiscoContext ctx = new RiscoContext())
            {
                List<UserDevice> usersToPushAndroid = new List<UserDevice>();
                List<UserDevice> usersToPushIOS = new List<UserDevice>();
                List<UserDevice> usersToPushWeb = new List<UserDevice>();

                User ReceivingUser = new User();
                Notification notf = new Notification();
                List<Notification> notifications = new List<Notification>();

                if (ReceivingUser_Id.HasValue && ReceivingUser_Id != 0)
                {
                    ReceivingUser_Ids = new List<int>();
                    ReceivingUser_Ids.Add(ReceivingUser_Id.Value);
                }

                // remove user's who turned off his/her notification
                if (EntityType == (int)RiscoEntityTypes.Post || EntityType == (int)RiscoEntityTypes.LikeOnPost || EntityType == (int)RiscoEntityTypes.CommentOnPost || EntityType == (int)RiscoEntityTypes.ReplyOnComment
                    || EntityType == (int)RiscoEntityTypes.SharePost || EntityType == (int)RiscoEntityTypes.LikeOnComment || EntityType == (int)RiscoEntityTypes.ReportPost)
                {
                    List<int> lstUserIds = ctx.TurnOffNotifications.Where(x => x.Post_Id == EntityId && !x.IsDeleted).Select(x => x.User_Id).Distinct().ToList();
                    for (int i = 0; i < ReceivingUser_Ids.Count; i++)
                    {
                        for (int j = 0; j < lstUserIds.Count; j++)
                        {
                            if (ReceivingUser_Ids[i] == lstUserIds[j])
                                ReceivingUser_Ids.RemoveAt(i);
                        }
                    }
                }

                var SendingUser = ctx.Users.Where(x => x.Id == SendingUser_Id).FirstOrDefault();

                foreach (var recieverId in ReceivingUser_Ids)
                {
                    ReceivingUser = ctx.Users.Include(x => x.UserDevices).Include(x => x.MuteUsers_First).Include(x => x.UserNotificationSetting).FirstOrDefault(x => x.Id == recieverId);

                    if (ReceivingUser.Id != SendingUser.Id && !ReceivingUser.PostsNotification) // if sender and receiver are same user then he should get notification
                    {
                        if (ReceivingUser.MuteUsers_First.Any(x => x.SecondUser_Id == SendingUser.Id))
                        {
                            continue;
                        }
                        if (ReceivingUser.MuteYouDontFollow)
                        {
                            if (!ctx.FollowFollowers.Any(x => x.FirstUser_Id == ReceivingUser.Id && x.SecondUser_Id == SendingUser.Id))
                            {
                                continue;
                            }
                        }

                        if (ReceivingUser.MuteDontFollowYou)
                        {
                            if (!ctx.FollowFollowers.Any(x => x.FirstUser_Id == SendingUser.Id && x.SecondUser_Id == ReceivingUser.Id))
                            {
                                continue;
                            }
                        }
                        //if (ReceivingUser.IsNotificationsOn)
                        //{
                        string Description = "";

                        if (EntityType == (int)RiscoEntityTypes.LikeOnPost)
                        {
                            Description = SendingUser.FullName + " liked your post.";
                        }
                        else if (EntityType == (int)RiscoEntityTypes.CommentOnPost)
                        {
                            Description = SendingUser.FullName + " commented on your post.";
                        }
                        else if (EntityType == (int)RiscoEntityTypes.LikeOnComment)
                        {
                            var post = ctx.Posts.Include(x => x.User).FirstOrDefault(x => x.Id == EntityId);

                            if (recieverId == post.User_Id)
                                Description = SendingUser.FullName + " liked comment on your post.";
                            else
                                Description = SendingUser.FullName + " liked your comment on " + post.User.FullName + "'s post.";
                        }
                        else if (EntityType == (int)RiscoEntityTypes.Post) // when someone create post
                        {
                            Description = SendingUser.FullName + " posted ";
                            var post = ctx.Posts.Include(x => x.Group).FirstOrDefault(x => x.Id == EntityId && !x.IsDeleted);

                            if (post.Group_Id.HasValue)
                                Description += "in " + post.Group.Name + ".";
                            else
                                Description += "on ";

                            if (ReceivingUser.Gender == (int)GenderType.Male)
                                Description += "his wall.";
                            else
                                Description += "her wall.";
                        }
                        else if (EntityType == (int)RiscoEntityTypes.ReplyOnComment)
                        {
                            Comment parentComment = new Comment();

                            var post = ctx.Posts.FirstOrDefault(x => x.Id == EntityId && !x.IsDeleted);
                            var childComment = ctx.Comments.FirstOrDefault(x => x.Id == ChildCommentId);
                            if (childComment != null)
                                parentComment = ctx.Comments.Include(x => x.Post).Include(x => x.Post.User).FirstOrDefault(x => x.Id == childComment.ParentComment_Id);

                            if (post != null)
                            {
                                Description = SendingUser.FullName + " replied to";

                                if (ReceivingUser.Id == parentComment.User_Id)
                                    Description += " your comment on";
                                else
                                {
                                    var ThirdUser = ctx.Users.FirstOrDefault(x => x.Id == parentComment.User_Id);
                                    if (ThirdUser != null)
                                        Description += " " + ThirdUser.FullName + "'s comment on";
                                }

                                if (parentComment.User_Id == ReceivingUser.Id)
                                    Description += " your post.";
                                else
                                {
                                    Description += " " + parentComment.Post.User.FullName + "'s  post.";
                                }
                            }
                        }
                        else if (EntityType == (int)RiscoEntityTypes.Group && !ReceivingUser.GroupsNotification)
                        {
                            if (ChildEntityType == (int)RiscoEntityTypes.AddInGroup)
                            {
                                var group = ctx.Groups.Include(x => x.GroupMembers).FirstOrDefault(x => x.Id == EntityId && !x.IsDeleted);
                                var Admin = ctx.Users.FirstOrDefault(x => x.Id == SendingUser_Id);
                                if (group != null)
                                {
                                    //if (group.GroupMembers.Count > 0)
                                    //{
                                    //    switch (group.GroupMembers.FirstOrDefault().Status)
                                    //    {
                                    //        case (int)GroupMemberStatusTypes.Accepted:
                                    Description = Admin.FullName + " added you to group " + group.Name + ".";
                                    //                break;
                                    //            default:
                                    //                break;
                                    //        }
                                    //    }
                                }
                            }
                            else if (ChildEntityType == (int)RiscoEntityTypes.LikeOnPost)
                            {
                                var group = ctx.Posts.Include(x => x.Group).Include(x => x.User).FirstOrDefault(x => x.Id == EntityId);
                                Description = SendingUser.FullName + " liked your post in " + group.Group.Name + ".";
                            }
                            else if (ChildEntityType == (int)RiscoEntityTypes.CommentOnPost)
                            {
                                var group = ctx.Posts.Include(x => x.Group).FirstOrDefault(x => x.Id == EntityId);
                                Description = SendingUser.FullName + " commented on your post in " + group.Group.Name + ".";
                            }
                            else if (ChildEntityType == (int)RiscoEntityTypes.LikeOnComment)
                            {
                                var group = ctx.Posts.Include(x => x.Group).FirstOrDefault(x => x.Id == EntityId);
                                Description = SendingUser.FullName + " liked you comment in " + group.Group.Name + ".";
                            }
                            else if (ChildEntityType == (int)RiscoEntityTypes.ReplyOnComment)
                            {
                                var group = ctx.Posts.Include(x => x.Group).FirstOrDefault(x => x.Id == EntityId);
                                Description = SendingUser.FullName + " replied to your comment in " + group.Group.Name + ".";
                            }
                        }
                        else if (EntityType == (int)RiscoEntityTypes.SharePost)
                        {
                            var post = ctx.Posts.Include(x => x.Group).FirstOrDefault(x => x.Id == EntityId);
                            if (post.Group_Id.HasValue)
                                Description = SendingUser.FullName + " shared your post in " + post.Group.Name + ".";
                            else
                                Description = SendingUser.FullName + " shared your post.";
                        }
                        else if (EntityType == (int)RiscoEntityTypes.SendGroupRequest)
                        {
                            Description = SendingUser.FullName + " sent you join group request.";
                        }
                        else if (EntityType == (int)RiscoEntityTypes.AcceptGroupRequest)
                        {
                            Description = SendingUser.FullName + " accepted your join group request.";
                        }
                        else if (EntityType == (int)RiscoEntityTypes.RejectGroupRequest)
                        {
                            Description = SendingUser.FullName + " rejected your join group request.";
                        }
                        else if (EntityType == (int)RiscoEntityTypes.CancelGroupRequest)
                        {
                            Description = SendingUser.FullName + " cancelled your join group request.";
                        }
                        else if (EntityType == (int)RiscoEntityTypes.Follow)
                        {
                            Description = SendingUser.FullName + " started following you.";
                        }

                        //if (ToDifferentUsers)
                        //{
                        //    foreach (var receivingUser in ReceivingUser_Ids)
                        //    {
                        //        notifications.Add(new Notification
                        //        {
                        //            Title = "Risco",
                        //            Description = Description,
                        //            SendingUser_Id = SendingUser.Id,
                        //            ReceivingUser_Id = receivingUser,
                        //            EntityType = EntityType,
                        //            EntityId = EntityId,
                        //            Status = (int)NotificationStatus.Unread,
                        //            CreatedDate = DateTime.UtcNow
                        //        });
                        //    }
                        //    ctx.Notifications.AddRange(notifications);

                        //}
                        //else
                        //{
                        notf = new Notification
                        {
                            Title = "Risco",
                            Description = Description,
                            SendingUser_Id = SendingUser.Id,
                            ReceivingUser_Id = recieverId,
                            EntityType = EntityType,
                            EntityId = EntityId,
                            Status = (int)NotificationStatus.Unread,
                            CreatedDate = DateTime.UtcNow
                        };
                        ctx.Notifications.Add(notf);
                        //}


                        // till here


                        //if (ToDifferentUsers && ReceivingUser_Ids.Count > 0)
                        //{
                        //    usersToPushAndroid = ctx.UserDevices.Where(x => ReceivingUser_Ids.Contains(x.User_Id) && x.Platform == (int)DeviceTypes.Android && x.User.IsNotificationsOn).ToList();
                        //    usersToPushIOS = ctx.UserDevices.Where(x => ReceivingUser_Ids.Contains(x.User_Id) && x.Platform == (int)DeviceTypes.IOS && x.User.IsNotificationsOn).ToList();
                        //    usersToPushWeb = ctx.UserDevices.Where(x => ReceivingUser_Ids.Contains(x.User_Id) && x.Platform == (int)DeviceTypes.Web && x.User.IsNotificationsOn).ToList();

                        //}
                        //else
                        //{
                        usersToPushAndroid = ctx.UserDevices.Where(x => x.User_Id == recieverId && x.Platform == (int)DeviceTypes.Android && x.User.IsNotificationsOn).ToList();
                        usersToPushIOS = ctx.UserDevices.Where(x => x.User_Id == recieverId && x.Platform == (int)DeviceTypes.IOS && x.User.IsNotificationsOn).ToList();
                        usersToPushWeb = ctx.UserDevices.Where(x => x.User_Id == recieverId && x.Platform == (int)DeviceTypes.Web && x.User.IsNotificationsOn).ToList();

                        //}
                        //if (ToDifferentUsers)
                        //    Utility.SendPushNotifications(usersToPushAndroid, usersToPushIOS, usersToPushWeb: usersToPushWeb, Notification: notifications.FirstOrDefault(), EntityType: EntityType, EntityId: EntityId);
                        //else

                        Utility.SendPushNotifications(usersToPushAndroid, usersToPushIOS, usersToPushWeb: usersToPushWeb, Notification: notf, EntityType: EntityType, EntityId: EntityId);
                        ctx.SaveChanges();
                    }
                }
            }
        }

        //public static void GenerateNotification(int EntityType, int EntityId, int SendingUser_Id, int? ReceivingUser_Id = 0, int? ChildCommentId = 0, bool ToDifferentUsers = false, List<int> ReceivingUser_Ids = null, int ChildEntityType = 0)
        //{
        //    using (RiscoContext ctx = new RiscoContext())
        //    {
        //        List<UserDevice> usersToPushAndroid = new List<UserDevice>();
        //        List<UserDevice> usersToPushIOS = new List<UserDevice>();
        //        List<UserDevice> usersToPushWeb = new List<UserDevice>();


        //        Notification notf = new Notification();
        //        List<Notification> notifications = new List<Notification>();

        //        //if (ReceivingUser.Id != SendingUser.Id && ReceivingUser.PostsNotification) // if sender and receiver are same user then he should get notification
        //        //{

        //        string Description = string.Empty;

        //        foreach (var receivingUser in ReceivingUser_Ids)
        //        {
        //            Description = GetNotificationText(EntityType, EntityId, SendingUser_Id, ReceivingUser_Id, ChildCommentId, ChildEntityType);
        //            notifications.Add(new Notification
        //            {
        //                Title = "Risco",
        //                Description = Description,
        //                SendingUser_Id = SendingUser_Id,
        //                ReceivingUser_Id = receivingUser,
        //                EntityType = EntityType,
        //                EntityId = EntityId,
        //                Status = (int)NotificationStatus.Unread,
        //                CreatedDate = DateTime.UtcNow
        //            });
        //        }
        //        ctx.Notifications.AddRange(notifications);


        //        if (ToDifferentUsers && ReceivingUser_Ids.Count > 0)
        //        {
        //            usersToPushAndroid = ctx.UserDevices.Where(x => ReceivingUser_Ids.Contains(x.User_Id) && x.Platform == (int)DeviceTypes.Android && x.User.IsNotificationsOn).ToList();
        //            usersToPushIOS = ctx.UserDevices.Where(x => ReceivingUser_Ids.Contains(x.User_Id) && x.Platform == (int)DeviceTypes.IOS && x.User.IsNotificationsOn).ToList();
        //            usersToPushWeb = ctx.UserDevices.Where(x => ReceivingUser_Ids.Contains(x.User_Id) && x.Platform == (int)DeviceTypes.Web && x.User.IsNotificationsOn).ToList();

        //        }
        //        else
        //        {
        //            usersToPushAndroid = ctx.UserDevices.Where(x => x.Platform == (int)DeviceTypes.Android && x.User.IsNotificationsOn && x.User_Id == ReceivingUser_Id).ToList();
        //            usersToPushIOS = ctx.UserDevices.Where(x => x.Platform == (int)DeviceTypes.IOS && x.User.IsNotificationsOn && x.User_Id == ReceivingUser_Id).ToList();
        //            usersToPushWeb = ctx.UserDevices.Where(x => x.Platform == (int)DeviceTypes.Web && x.User.IsNotificationsOn && x.User_Id == ReceivingUser_Id).ToList();

        //        }

        //        if (ToDifferentUsers)
        //            Utility.SendPushNotifications(usersToPushAndroid, usersToPushIOS, usersToPushWeb: usersToPushWeb, Notification: notifications.FirstOrDefault(), EntityType: EntityType, EntityId: EntityId);
        //        else
        //            Utility.SendPushNotifications(usersToPushAndroid, usersToPushIOS, usersToPushWeb: usersToPushWeb, Notification: notf, EntityType: EntityType, EntityId: EntityId);
        //        ctx.SaveChanges();


        //        //}
        //    }
        //}
        public static void NotifyFollowers(List<UserDevice> Users, Notification Notification, RiscoEntityTypes EntityType, int EntityId)
        {
            Utility.SendPushNotifications(usersToPushAndroid: Users.Where(x => x.Platform == (int)PlatformTypes.Android).ToList(), usersToPushIOS: Users.Where(x => x.Platform == (int)PlatformTypes.Ios).ToList(), usersToPushWeb: Users.Where(x => x.Platform == (int)PlatformTypes.Web).ToList(), Notification: Notification, EntityType: (int)EntityType, EntityId: EntityId);
        }

    }
}