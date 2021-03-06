using BasketApi.Areas.Admin.ViewModels;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApplication1.Areas.Admin.ViewModels;
using System.Data.Entity;
using WebApplication1.BindingModels;
using System.Web;
using System.Net.Http;
using System.IO;
using System.Configuration;
using static BasketApi.Global;
using System.Data;
using BLL.Utility;

namespace BasketApi.Areas.SubAdmin.Controllers
{
    [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User", "Guest")]
    [RoutePrefix("api/ActivityLog")]
    public class ActivityLogController : ApiController
    {
        [HttpGet]
        [Route("GetActivityLogs")]
        public async Task<IHttpActionResult> GetActivityLogs(int? Page=0,int? Items=10)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    //var IBlocked = ctx.BlockUsers.Where(x => x.FirstUser_Id == userId).Select(x => x.SecondUser_Id).ToList();
                    var BlockerIds = ctx.BlockUsers.Where(x => x.SecondUser_Id== userId).Select(x => x.FirstUser_Id).ToList();
                    var Activities = ctx.ActivityLogs
                         .Include(x => x.SecondUser)
                         .Where(x => x.IsDeleted == false && x.FirstUser_Id == userId && !BlockerIds.Contains(x.SecondUser_Id.Value)).OrderByDescending(x => x.CreatedDate).Skip(Page.Value*Items.Value).Take(Items.Value).ToList();

                    if (Activities != null)
                    {
                        foreach (var activity in Activities)
                        {
                            if (activity.SecondUser_Id != null)
                                activity.SecondUser.IsBlocked = ctx.BlockUsers.Any(x => (x.FirstUser_Id == userId && x.SecondUser_Id == activity.SecondUser_Id) || (x.SecondUser_Id == userId && x.SecondUser_Id == activity.SecondUser_Id) && x.IsDeleted == false);
                        }
                    }

                    CustomResponse<List<ActivityLog>> response = new CustomResponse<List<ActivityLog>>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = Activities
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
        [Route("RemoveActivityLog")]
        public async Task<IHttpActionResult> RemoveActivityLog(int? ActivityLog_Id = null)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {

                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                    if (ActivityLog_Id.HasValue) {

                        ActivityLog activityLog = ctx.ActivityLogs.FirstOrDefault(x => x.Id == ActivityLog_Id);
                        activityLog.IsDeleted = true;

                    }else
                    {
                        List<ActivityLog> activityLogList = ctx.ActivityLogs.Where(x => x.FirstUser_Id == userId).ToList();
                        activityLogList.ForEach(x => x.IsDeleted = true);
                    }
                    ctx.SaveChanges();

                    CustomResponse<string> response = new CustomResponse<string>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = "Operation performed successfully."
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
        [Route("RemoveAllActivityLogs")]
        public async Task<IHttpActionResult> RemoveAllActivityLogs()
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    await ctx.ActivityLogs.Where(x => x.IsDeleted == false && x.FirstUser_Id == userId).ForEachAsync(y => y.IsDeleted = true);
                    ctx.SaveChanges();

                    CustomResponse<string> response = new CustomResponse<string>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = "Activity logs removed."
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
