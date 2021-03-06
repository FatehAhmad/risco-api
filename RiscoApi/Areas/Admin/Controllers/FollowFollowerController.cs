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
using System.Text.RegularExpressions;
//using static BasketApi.Utility;
using static DAL.CustomModels.Enumeration;
using BLL.Utility;

namespace BasketApi.Areas.SubAdmin.Controllers
{
    [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User", "Guest")]
    [RoutePrefix("api/FollowFollower")]
    public class FollowFollowerController : ApiController
    {
        [HttpGet]
        [Route("Follow")]
        public async Task<IHttpActionResult> Follow(int FollowUser_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    if (ctx.FollowFollowers.Any(x => x.FirstUser_Id == userId && x.SecondUser_Id == FollowUser_Id && x.IsDeleted == false))
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Conflict",
                            StatusCode = (int)HttpStatusCode.Conflict,
                            Result = new Error { ErrorMessage = "Already Following" }
                        });
                    }


                    var UserExist = ctx.Users.FirstOrDefault(x => x.Id == FollowUser_Id);
                    
                    if (UserExist != null)
                    {
                        FollowFollower followFollower = new FollowFollower
                        {
                            FirstUser_Id = userId,
                            SecondUser_Id = FollowUser_Id,
                            CreatedDate = DateTime.UtcNow,
                        };

                        ctx.FollowFollowers.Add(followFollower);
                        ctx.SaveChanges();

                        SetActivityLog(FirstUser_Id: userId, SecondUser_Id: FollowUser_Id, EntityType: (int)RiscoEntityTypes.Follow, EntityId: FollowUser_Id);
                        Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.Follow, EntityId: userId, ReceivingUser_Id: FollowUser_Id, SendingUser_Id: userId);

                        CustomResponse<FollowFollower> response = new CustomResponse<FollowFollower>
                        {
                            Message = Global.ResponseMessages.Success,
                            StatusCode = (int)HttpStatusCode.OK,
                            Result = followFollower
                        };
                        return Ok(response);
                    }else
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "NotFound",
                            StatusCode = (int)HttpStatusCode.NotFound,
                            Result = new Error { ErrorMessage = "User not found." }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("UnFollow")]
        public async Task<IHttpActionResult> UnFollow(int UnFollowUser_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    FollowFollower followFollower = ctx.FollowFollowers.FirstOrDefault(x => x.FirstUser_Id == userId && x.SecondUser_Id == UnFollowUser_Id && x.IsDeleted == false);
                    if (followFollower != null)
                    {
                        followFollower.IsDeleted = true;
                        ctx.SaveChanges();
                        CustomResponse<FollowFollower> response = new CustomResponse<FollowFollower>
                        {
                            Message = Global.ResponseMessages.Success,
                            StatusCode = (int)HttpStatusCode.OK,
                            Result = followFollower
                        };
                        return Ok(response);
                    }
                    else
                    {
                        return Ok(new CustomResponse<Error> { Message = "Conflict", StatusCode = (int)HttpStatusCode.Conflict, Result = new Error { ErrorMessage = "Already unfollowing" } });
                    }


                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("GetFollowers")]
        public async Task<IHttpActionResult> GetFollowers(string SearchString = "", int? Page = 0, int? Items = 10,int secondUserId = 0)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                    List<FollowFollower> followFollowers = new List<FollowFollower>();

                    if(secondUserId > 0 && ctx.Users.Where(x => x.Id == secondUserId && !x.IsDeleted && x.isAllowWatchFollowerFollowings).Count() == 0)
                    {
                        CustomResponse<List<FollowFollower>> emptyResponse = new CustomResponse<List<FollowFollower>>
                        {
                            Message = "Please allow permissions for watching followers.",
                            StatusCode = (int)HttpStatusCode.Unauthorized,
                            Result = followFollowers
                        };
                        return Ok(emptyResponse);
                    }
                    if (secondUserId > 0)
                        userId = secondUserId;
                    if (string.IsNullOrEmpty(SearchString))
                    {
                        followFollowers = ctx.FollowFollowers
                            .Include(x => x.FirstUser)                            
                            .Where(x => x.IsDeleted == false && x.SecondUser_Id == userId)
                            .OrderByDescending(x => x.Id)
                            .Skip(Page.Value * Items.Value)
                            .Take(Items.Value)
                            .ToList();
                    }else
                    {
                        followFollowers = ctx.FollowFollowers
                           .Include(x => x.FirstUser)                            
                           .Where(x => x.IsDeleted == false && (x.FirstUser.FullName.ToLower().Contains(SearchString.ToLower()) || x.FirstUser.UserName.ToLower().Contains(SearchString.ToLower())) && x.SecondUser_Id == userId)
                           .OrderByDescending(x => x.Id)
                           .Skip(Page.Value * Items.Value)
                           .Take(Items.Value)
                           .ToList();
                    }
                    foreach (FollowFollower followFoller in followFollowers)
                    {
                        followFoller.IsFollowing = ctx.FollowFollowers.Any(x => x.FirstUser_Id == userId && x.SecondUser_Id == followFoller.FirstUser_Id && x.IsDeleted == false);
                    }

                    CustomResponse<List<FollowFollower>> response = new CustomResponse<List<FollowFollower>>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = followFollowers
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
        [Route("GetFollowings")]
        public async Task<IHttpActionResult> GetFollowings(string SearchString = "", int? Page = 0, int? Items = 10, int firstUserId = 0)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                    List<FollowFollower> followFollowers = new List<FollowFollower>();
                    if (firstUserId > 0 && ctx.Users.Where(x => x.Id == firstUserId && !x.IsDeleted && x.isAllowWatchFollowerFollowings).Count() == 0)
                    {
                        CustomResponse<List<FollowFollower>> emptyResponse = new CustomResponse<List<FollowFollower>>
                        {
                            Message = "Please allow permissions for watch followings.",
                            StatusCode = (int)HttpStatusCode.Unauthorized,
                            Result = followFollowers
                        };
                        return Ok(emptyResponse);
                    }
                    if (firstUserId > 0)
                        userId = firstUserId;
                    if (string.IsNullOrEmpty(SearchString))
                    {
                        followFollowers = ctx.FollowFollowers
                           .Include(x => x.SecondUser)
                           .Where(x => x.IsDeleted == false && x.FirstUser_Id == userId)
                            .OrderByDescending(x => x.Id)
                           .Skip(Page.Value * Items.Value)
                           .Take(Items.Value)
                           .ToList();
                    }
                    else
                    {
                        followFollowers = ctx.FollowFollowers
                        .Include(x => x.SecondUser)
                        .Where(x => x.IsDeleted == false && (x.SecondUser.FullName.ToLower().Contains(SearchString.ToLower()) || x.SecondUser.UserName.ToLower().Contains(SearchString.ToLower())) && x.FirstUser_Id == userId)
                         .OrderByDescending(x => x.Id)
                        .Skip(Page.Value * Items.Value)
                        .Take(Items.Value)
                        .ToList();
                    }

                    CustomResponse<List<FollowFollower>> response = new CustomResponse<List<FollowFollower>>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = followFollowers
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
        [Route("GetFollowerFollowing")]
        public async Task<IHttpActionResult> GetFollowerFollowings()
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                    List<User> lstUsers = new List<User>();
                    lstUsers = ctx.FollowFollowers
                        .Include(x => x.SecondUser)
                        .Where(x => x.IsDeleted == false && x.FirstUser_Id == userId)
                        .Select(x => x.SecondUser)
                        .ToList();
                    lstUsers.AddRange(ctx.FollowFollowers
                            .Include(x => x.FirstUser)
                            .Where(x => x.IsDeleted == false && x.SecondUser_Id == userId)
                            .Select(x => x.FirstUser)                            
                            .ToList());

                                                                    
                    CustomResponse<List<User>> response = new CustomResponse<List<User>>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = lstUsers.Distinct().OrderByDescending(x => x.Id).ToList()
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
        [Route("GetTopFollowers")]
        public async Task<IHttpActionResult> GetTopFollowers()
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                    string query = @"select top 3 FirstUser_Id, count(Id) as [Count] from TopFollowerLogs where  SecondUser_Id = " + userId +
@" and FirstUser_Id in (select FirstUser_Id from FollowFollowers f where f.IsDeleted = 0 and f.SecondUser_Id = " + userId +
@") group by FirstUser_Id  
order by Count desc";
                    List<TopFollowersBindingModel> topFollowers = ctx.Database.SqlQuery<TopFollowersBindingModel>(query).ToList();
                    var topFollowersIds = topFollowers.Select(y => y.FirstUser_Id).ToList();
                    List<FollowFollower> followFollowers = new List<FollowFollower>();

                    followFollowers = ctx.FollowFollowers
                        .Include(x => x.FirstUser)
                        .Where(x => x.IsDeleted == false && x.SecondUser_Id == userId && topFollowersIds.Contains(x.FirstUser_Id))
                        .ToList();

                    CustomResponse<List<FollowFollower>> response = new CustomResponse<List<FollowFollower>>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = followFollowers
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        #region Private Regions
        #endregion
    }
}
