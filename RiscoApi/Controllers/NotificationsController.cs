using BasketApi.CustomAuthorization;
using BasketApi.Models;
using BasketApi.ViewModels;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using System.Data.Entity;
using WebApplication1.BindingModels;
using static BasketApi.Global;
using static DAL.CustomModels.Enumeration;
using BLL.Utility;

namespace BasketApi.Controllers
{
    [BasketApi.Authorize("User", "Guest", "Deliverer")]
    [RoutePrefix("api/User")]
    public class NotificationsController : ApiController
    {

        [HttpGet]
        [Route("GetNotifications")]
        public async Task<IHttpActionResult> GetNotifications(int UserId, int SignInType, int? Page = 0, int? Items = 10)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    NotificationsViewModel notificationsViewModel = new NotificationsViewModel();
                    var BlockerIds = ctx.BlockUsers.Where(x => x.SecondUser_Id == UserId).Select(x => x.FirstUser_Id).ToList();
                    BlockerIds.AddRange(ctx.BlockUsers.Where(x => x.FirstUser_Id == UserId).Select(x => x.SecondUser_Id).ToList());



                    if (SignInType == (int)RoleTypes.User)
                    {
                        var Days = DateTime.UtcNow;
                        //notificationsViewModel.Notifications = ctx.Notifications.Include(x => x.SendingUser).Where(x => x.ReceivingUser_Id.HasValue && x.ReceivingUser_Id.Value == UserId && (x.CreatedDate.Year == Days.Year && (((x.CreatedDate.Month - 1) * 30) + x.CreatedDate.Day)-((Days.Month - 1) * 30 + Days.Day)<=60)  && !x.IsDeleted).OrderByDescending(x => x.Id).ToList();

                        var Notifications = ctx.Notifications.Include(x => x.SendingUser).Where(x => x.ReceivingUser_Id.HasValue && x.ReceivingUser_Id.Value == UserId && (x.CreatedDate.Year == Days.Year && (((x.CreatedDate.Month - 1) * 30) + x.CreatedDate.Day) - ((Days.Month - 1) * 30 + Days.Day) <= 60) && !x.IsDeleted).OrderByDescending(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                        if (Notifications != null)
                        {
                            for (int i = 0; i < Notifications.Count; i++)
                            {
                                var Notification = Notifications[i];
                                //if (!ctx.BlockUsers.Any(x => x.FirstUser_Id == Notification.SendingUser_Id || x.SecondUser_Id == Notification.SendingUser_Id && !x.IsDeleted))
                                if (!BlockerIds.Contains(Notification.SendingUser_Id.Value))
                                    notificationsViewModel.Notifications.Add(Notification);

                                //SendGroupRequest                  //AcceptGroupRequest                //RejectGroupRequest            //CancelGroupRequest
                                if (Notification.EntityType == 16 || Notification.EntityType == 17 || Notification.EntityType == 31 || Notification.EntityType == 32)          //noti is send group request
                                {
                                    Notification.GroupMemberStatus = ctx.GroupMembers.FirstOrDefault(x => x.Group_Id == Notification.EntityId && x.User_Id == UserId).Status;
                                }

                            }
                            notificationsViewModel.TotalRecords = ctx.Notifications.Count(x => x.ReceivingUser_Id == UserId && (x.CreatedDate.Year == Days.Year && (((x.CreatedDate.Month - 1) * 30) + x.CreatedDate.Day) - ((Days.Month - 1) * 30 + Days.Day) <= 60) && !x.IsDeleted);
                        }

                    }
                    //else if (SignInType == (int)RoleTypes.Deliverer)
                    //    notificationsViewModel.Notifications = ctx.Notifications.Where(x => x.DeliveryMan_ID.HasValue && x.DeliveryMan_ID.Value == UserId && !x.IsDeleted).OrderByDescending(x => x.Id).ToList();

                    CustomResponse<NotificationsViewModel> response = new CustomResponse<NotificationsViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = notificationsViewModel };

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("DeleteNotification")]
        public async Task<IHttpActionResult> DeleteNotification(int Notification_Id)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {

                    var Notification = ctx.Notifications.FirstOrDefault(x => x.Id == Notification_Id && !x.IsDeleted);
                    if (Notification != null)
                        Notification.IsDeleted = true;

                    ctx.SaveChanges();
                    CustomResponse<string> response = new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = "Notification removed successfully." };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [HttpGet]
        [Route("MarkNotificationAsRead")]
        public async Task<IHttpActionResult> MarkNotificationAsRead(int? NotificationId = 0)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {

                    if (NotificationId.HasValue && NotificationId != 0)
                    {
                        var notification = ctx.Notifications.FirstOrDefault(x => x.Id == NotificationId);

                        if (notification != null)
                        {
                            notification.Status = (int)NotificationStatus.Read;

                        }
                    }
                    else {

                        await ctx.Notifications.ForEachAsync(x=>x.Status=(int)NotificationStatus.Read);
                        ctx.SaveChanges();

                    }
                    ctx.SaveChanges();
                    CustomResponse<string> response = new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = "Notification marked as read." };
                    return Ok(response);
                    //else
                    //{
                    //    CustomResponse<Error> response = new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "Invalid notificationid" } };
                    //    return Ok(response);
                    //}
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        /// <summary>
        /// Turn user notifications on or off
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="On"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("UserNoticationsOnOff")]
        public async Task<IHttpActionResult> UserNoticationsOnOff(int UserId, int SignInType, bool On)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    if (SignInType == (int)RoleTypes.User)
                    {

                        var user = ctx.Users.FirstOrDefault(x => x.Id == UserId);
                        if (user != null)
                        {
                            user.IsNotificationsOn = On;
                            ctx.SaveChanges();
                        }
                    }
                }
                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [HttpGet]
        [Route("GetNotificationTypes")]
        public async Task<IHttpActionResult> GetNotificationTypes(int? ScreenId = 0)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    List<UserNotificationSetting> nSetting = ctx.UserNotificationSetting.Where(x => x.UserId == userId && !x.IsDeleted).ToList();

                    List<NotificationCategory> notificationCategories = new List<NotificationCategory>();
                    List<NotificationType> notificationTypes = new List<NotificationType>();
                    notificationCategories = ctx.NotificationCategory.OrderBy(x => x.Id).ToList();
                    notificationTypes = ctx.NotificationType
                        .Where(x=> ctx.NotificationTypeScreenMapping.Where(s=> s.ScreenId == ScreenId && !s.IsDeleted).Select(s=>s.NotificationTypeId).ToList().Contains(x.Id) || ScreenId == 0)
                        .OrderBy(x => x.Id).ThenBy(x => x.NotificationCategoryId).ToList();

                    foreach (var nCat in notificationCategories)
                    {
                        nCat.NotificationTypeList.AddRange(notificationTypes.Where(x => x.NotificationCategoryId == nCat.Id).ToList());
                        nCat.IsCategory = true;

                        foreach (var nType in nCat.NotificationTypeList)
                        {
                            nType.Status = false;
                            if (nSetting.Any(x => x.NotificationTypeId == nType.Id))
                                nType.Status = true;
                        }
                    }

                    notificationCategories = notificationCategories.Where(x => x.NotificationTypeList.Count() > 0).ToList();

                    var response = new
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new 
                        {
                            List = notificationCategories
                        }
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("UpdateUserNotificationSetting")]
        public async Task<IHttpActionResult> UpdateUserNotificationSetting(int NotificationTypeId, bool Status)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userNotif =  ctx.UserNotificationSetting.Where(x => x.UserId == userId && x.NotificationTypeId == NotificationTypeId).SingleOrDefault();
                    if (userNotif != null)
                    {
                        userNotif.IsDeleted = !Status;
                        userNotif.UpdatedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        UserNotificationSetting userNotification = new UserNotificationSetting
                        {
                            UserId = userId,
                            NotificationTypeId = NotificationTypeId,
                            CreatedDate = DateTime.UtcNow,
                            IsDeleted = false
                            //UpdatedDate = DateTime.UtcNow
                        };

                        ctx.UserNotificationSetting.Add(userNotification);
                    }

                    ctx.SaveChanges();

                    var response = new
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

    }
}
