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
//using static BasketApi.Utility;
using static DAL.CustomModels.Enumeration;
using BLL.Utility;

namespace BasketApi.Areas.SubAdmin.Controllers
{
    [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User", "Guest")]
    [RoutePrefix("api/Group")]
    public class GroupController : ApiController
    {
        [HttpPost]
        [Route("ImageUpload")]
        public async Task<IHttpActionResult> ImageUpload()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;
                string folderPath = string.Empty;

                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multipart data is not included in request." }
                    });
                }
                else if (httpRequest.Files.Count == 0)
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "NotFound",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Result = new Error { ErrorMessage = "Please upload an attachment." }
                    });
                }
                else if (httpRequest.Files.Count > 1)
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multiple attachments are not allowed. Please upload one attachment." }
                    });
                }
                else
                {
                    var postedFile = httpRequest.Files[0];

                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        int MaxContentLength = 1024 * 1024 * 10; //Size = 1 MB  

                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".jpeg", ".gif", ".png" };
                        if (!AllowedFileExtensions.Contains(extension))
                        {
                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "UnsupportedMediaType",
                                StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                Result = new Error { ErrorMessage = "Please Upload image of type .jpg, .jpeg, .gif, .png." }
                            });
                        }
                        if (postedFile.ContentLength > MaxContentLength)
                        {

                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "UnsupportedMediaType",
                                StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                Result = new Error { ErrorMessage = "Please Upload a file upto 1 mb." }
                            });
                        }
                        else
                        {
                            folderPath = ConfigurationManager.AppSettings["GroupMediaFolderPath"] + DateTime.Now.Ticks.ToString() + extension;
                            newFullPath = HttpContext.Current.Server.MapPath("~/" + folderPath);
                            postedFile.SaveAs(newFullPath);

                            return Content(HttpStatusCode.OK, new CustomResponse<string>
                            {
                                Message = Global.ResponseMessages.Success,
                                StatusCode = (int)HttpStatusCode.OK,
                                Result = folderPath
                            });
                        }
                    }
                    else
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = Global.ResponseMessages.BadRequest,
                            StatusCode = (int)HttpStatusCode.BadRequest,
                            Result = new Error { ErrorMessage = "Something went wrong Please try again." }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpPost]
        [Route("CreateGroup")]
        public async Task<IHttpActionResult> CreateGroup(CreateGroupBindingModel model)
        {
            try
            {
                Validate(model);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                List<int> UsersToNotify = new List<int>();
                List<int> GroupCategoryIds = new List<int>();

                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    Group group = new Group
                    {
                        Name = model.Name,
                        Description = model.Description,
                        ImageUrl = model.ImageUrl,
                        User_Id = userId,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow
                    };

                    ctx.Groups.Add(group);
                    ctx.SaveChanges();

                    //insert group admin as a GroupMember
                    var groupMemberAdmin = new GroupMember
                    {
                        User_Id = userId,
                        Group_Id = group.Id,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,
                        Status = (int)GroupMemberStatusTypes.Accepted
                    };
                    ctx.GroupMembers.Add(groupMemberAdmin);

                    // start of setting group's categories
                    if (!string.IsNullOrEmpty(model.Interests))
                    {
                        GroupCategoryIds = model.Interests.Split(',').Select(int.Parse).ToList();
                    }
                    if (GroupCategoryIds.Count > 0)
                    {
                        /// List<PostCategories> Categories = new List<DAL.PostCategories>();
                        foreach (var item in GroupCategoryIds)
                        {
                            group.GroupCategories.Add(new DAL.GroupCategories
                            {
                                Interest_Id = item,
                                Group_Id = group.Id,
                                IsDeleted = false
                            });
                        }
                        //post.PostCategories.AddRange(Categories);
                    }

                    if (!string.IsNullOrEmpty(model.GroupMembersIds))
                    {
                        GroupMember groupMember;
                        var GroupMembersIds = model.GroupMembersIds.Split(',');
                        foreach (var Id in GroupMembersIds)
                        {
                            int GroupMember_Id = Convert.ToInt32(Id);
                            UsersToNotify.Add(GroupMember_Id);
                            //if (ctx.GroupMembers.Any(x => x.Group_Id == group.Id && x.User_Id == GroupMember_Id))
                            //{
                            //    groupMember = ctx.GroupMembers.FirstOrDefault(x => x.Group_Id == group.Id && x.User_Id == GroupMember_Id);
                            //    groupMember.Status = (int)GroupMemberStatusTypes.Accepted;
                            //    groupMember.UpdatedDate = DateTime.UtcNow;
                            //}
                            //else
                            //{
                            //    groupMember = new GroupMember
                            //    {
                            //        User_Id = GroupMember_Id,
                            //        Group_Id = group.Id,
                            //        CreatedDate = DateTime.UtcNow,
                            //        UpdatedDate = DateTime.UtcNow,
                            //        Status = (int)GroupMemberStatusTypes.Accepted
                            //    };
                            //    ctx.GroupMembers.Add(groupMember);


                            //}

                            groupMember = new GroupMember
                            {
                                User_Id = GroupMember_Id,
                                Group_Id = group.Id,
                                CreatedDate = DateTime.UtcNow,
                                UpdatedDate = DateTime.UtcNow,
                                Status = (int)GroupMemberStatusTypes.YetToJoin
                            };
                            ctx.GroupMembers.Add(groupMember);
                        }
                        if (UsersToNotify.IndexOf(userId) != -1)
                        {
                            for (var i = 0; i < UsersToNotify.Count; i++)
                            {
                                if (UsersToNotify[i] == userId)
                                {
                                    UsersToNotify.RemoveAt(i);
                                }
                            }
                            UsersToNotify = UsersToNotify.Distinct().ToList();
                            //UsersToNotify.RemoveAll(userId);
                        }
                        SetActivityLog(FirstUser_Id: userId, SecondUser_Id: 0, EntityType: (int)RiscoEntityTypes.CreateGroup, EntityId: group.Id);
                        Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.Group, EntityId: group.Id, ReceivingUser_Ids: UsersToNotify, SendingUser_Id: userId, ToDifferentUsers: true, ChildEntityType: (int)RiscoEntityTypes.AddInGroup);
                        group.IsAdmin = true;
                    }
                    ctx.SaveChanges();

                    CustomResponse<Group> response = new CustomResponse<Group>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = group
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpPost]
        [Route("EditGroup")]
        public async Task<IHttpActionResult> EditGroup(EditGroupBindingModel model)
        {
            try
            {
                Validate(model);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                List<int> GroupCategoryIds = new List<int>();
                using (RiscoContext ctx = new RiscoContext())
                {
                    Group group = ctx.Groups.FirstOrDefault(x => x.Id == model.Id && x.IsDeleted == false);
                    group.Name = model.Name;
                    group.Description = model.Description;
                    group.ImageUrl = model.ImageUrl;
                    group.UpdatedDate = DateTime.UtcNow;

                    // start of setting group's categories
                    if (!string.IsNullOrEmpty(model.Interests))
                    {
                        GroupCategoryIds = model.Interests.Split(',').Select(int.Parse).ToList();
                    }
                    if (GroupCategoryIds.Count > 0)
                    {
                        // List<PostCategories> Categories = new List<DAL.PostCategories>();
                        foreach (var item in GroupCategoryIds)
                        {
                            group.GroupCategories.Add(new DAL.GroupCategories
                            {
                                Interest_Id = item,
                                Group_Id = group.Id,
                                IsDeleted = false
                            });
                        }
                        //post.PostCategories.AddRange(Categories);
                    }


                    ctx.SaveChanges();

                    if (!string.IsNullOrEmpty(model.GroupMembersIds))
                    {
                        GroupMember groupMember;
                        var GroupMembersIds = model.GroupMembersIds.Split(',');
                        foreach (var Id in GroupMembersIds)
                        {
                            int GroupMember_Id = Convert.ToInt32(Id);
                            if (ctx.GroupMembers.Any(x => x.Group_Id == group.Id && x.User_Id == GroupMember_Id))
                            {
                                groupMember = ctx.GroupMembers.FirstOrDefault(x => x.Group_Id == group.Id && x.User_Id == GroupMember_Id);
                                groupMember.Status = (int)GroupMemberStatusTypes.Accepted;
                                groupMember.UpdatedDate = DateTime.UtcNow;
                            }
                            else
                            {
                                groupMember = new GroupMember
                                {
                                    User_Id = GroupMember_Id,
                                    Group_Id = group.Id,
                                    CreatedDate = DateTime.UtcNow,
                                    UpdatedDate = DateTime.UtcNow,
                                    Status = (int)GroupMemberStatusTypes.Accepted
                                };
                                ctx.GroupMembers.Add(groupMember);
                            }
                        }
                        ctx.SaveChanges();
                    }

                    CustomResponse<Group> response = new CustomResponse<Group>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = group
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpPost]
        [Authorize]
        [Route("UpdateGroupImage")]
        public async Task<IHttpActionResult> UpdateGroupImage()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;

                int Group_Id = Convert.ToInt32(httpRequest.Params["Group_Id"]);

                #region Validations
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (!Request.Content.IsMimeMultipartContent())
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multipart data is not included in request." }
                    });
                }
                else if (httpRequest.Files.Count > 1)
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multiple images are not supported, please upload one image." }
                    });
                }
                #endregion

                using (RiscoContext ctx = new RiscoContext())
                {
                    Group group;

                    HttpPostedFile postedFile = null;
                    string fileExtension = string.Empty;

                    #region ImageSaving
                    if (httpRequest.Files.Count > 0)
                    {
                        postedFile = httpRequest.Files[0];
                        if (postedFile != null && postedFile.ContentLength > 0)
                        {

                            IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".jpeg", ".gif", ".png" };
                            //var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                            var ext = Path.GetExtension(postedFile.FileName);
                            fileExtension = ext.ToLower();
                            if (!AllowedFileExtensions.Contains(fileExtension))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload image of type .jpg, .jpeg, .gif, .png." }
                                });
                            }
                            else if (postedFile.ContentLength > Global.MaximumImageSize)
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "UnsupportedMediaType",
                                    StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                    Result = new Error { ErrorMessage = "Please Upload a file upto " + Global.ImageSize + "." }
                                });
                            }
                        }
                    }
                    #endregion

                    group = ctx.Groups.FirstOrDefault(x => x.Id == Group_Id);

                    if (group == null)
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "NotFound",
                            StatusCode = (int)HttpStatusCode.NotFound,
                            Result = new Error { ErrorMessage = "Group_Id does not exist." }
                        });
                    }
                    else
                    {
                        if (httpRequest.Files.Count > 0)
                        {
                            var FileName = DateTime.UtcNow.Ticks.ToString() + fileExtension;
                            newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["GroupMediaFolderPath"] + FileName);
                            postedFile.SaveAs(newFullPath);
                            group.ImageUrl = ConfigurationManager.AppSettings["GroupMediaFolderPath"] + FileName;
                            group.UpdatedDate = DateTime.UtcNow;
                        }
                        ctx.SaveChanges();

                        CustomResponse<string> response = new CustomResponse<string>
                        {
                            Message = Global.ResponseMessages.Success,
                            StatusCode = (int)HttpStatusCode.OK,
                            Result = group.ImageUrl
                        };
                        return Ok(response);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("GetGroups")]
        public async Task<IHttpActionResult> GetGroups(int MyGroupPageSize = int.MaxValue, int MyGroupPageNo = 0, int SuggestedGroupPageSize = int.MaxValue, int SuggestedGroupPageNo = 0)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                using (RiscoContext ctx = new RiscoContext())
                {
                    #region MyGroups
                    List<Group> myGroups = new List<Group>();

                    List<int> joinedGroupIds = ctx.GroupMembers.Where(x => x.User_Id == userId && x.Status == (int)GroupMemberStatusTypes.Accepted).Select(x => x.Group_Id).ToList();

                    myGroups = ctx.Groups
                        .Include(x => x.User)
                        .Where(x => x.IsDeleted == false && (x.User_Id == userId || joinedGroupIds.Contains(x.Id)))
                        .OrderByDescending(x => x.Id)
                        .ToList();

                    foreach (Group group in myGroups)
                    {
                        group.MembersCount = ctx.GroupMembers.Count(x => x.Status == (int)GroupMemberStatusTypes.Accepted && x.Group_Id == group.Id);
                        group.IsAdmin = (group.User_Id == userId) ? true : false;
                        if (!group.IsAdmin)
                        {
                            if (ctx.GroupMembers.Any(x => x.Group_Id == group.Id && x.User_Id == userId))
                            {
                                group.Status = ctx.GroupMembers.FirstOrDefault(x => x.Group_Id == group.Id && x.User_Id == userId).Status;
                            }
                            else
                            {
                                group.Status = (int)GroupMemberStatusTypes.YetToJoin;
                            }
                        }
                    }

                    int MyGroupCount = myGroups.Count();
                    myGroups = myGroups.Skip(MyGroupPageNo * MyGroupPageSize).Take(MyGroupPageSize).ToList();
                    #endregion

                    #region SuggestedGroups
                    List<Group> suggestedGroups = new List<Group>();
                    int SuggestedGroupCount = 0;

                    User user = ctx.Users.FirstOrDefault(x => x.Id == userId);

                    if (!string.IsNullOrEmpty(user.Interests))
                    {
                        List<int> InterestIds = user.Interests.Split(',').Select(x => int.Parse(x)).ToList();

                        var myFollowerFollowingsId = ctx.FollowFollowers.Where(x => x.FirstUser_Id == userId && x.IsDeleted == false).Select(x => x.SecondUser_Id).ToList();            //getting all followerIds
                        myFollowerFollowingsId.AddRange(ctx.FollowFollowers.Where(x => x.SecondUser_Id == userId && x.IsDeleted == false).Select(x => x.FirstUser_Id).ToList());        //getting all followingIds
                        myFollowerFollowingsId = myFollowerFollowingsId.Distinct().ToList();                                                      //removing all duplicates

                        suggestedGroups = ctx.Groups
                            .Include(x => x.User)
                            .Where(x => x.IsDeleted == false && x.User_Id != userId &&
                            !joinedGroupIds.Contains(x.Id) &&
                            x.GroupMembers.Any(y => y.Status == (int)GroupMemberStatusTypes.Accepted && myFollowerFollowingsId.Contains(y.User_Id) && y.IsBlockedForOtherUser == false)).ToList();

                        foreach (Group group in suggestedGroups)
                        {
                            group.MembersCount = ctx.GroupMembers.Count(x => x.Status == (int)GroupMemberStatusTypes.Accepted && x.Group_Id == group.Id);
                            if (ctx.GroupMembers.Any(x => x.Group_Id == group.Id && x.User_Id == userId))
                            {
                                group.Status = ctx.GroupMembers.FirstOrDefault(x => x.Group_Id == group.Id && x.User_Id == userId).Status;
                            }
                            else
                            {
                                group.Status = (int)GroupMemberStatusTypes.YetToJoin;
                            }
                        }

                        SuggestedGroupCount = suggestedGroups.Count();
                        suggestedGroups = suggestedGroups.Skip(SuggestedGroupPageNo * SuggestedGroupPageSize).Take(SuggestedGroupPageSize).ToList();
                    }
                    #endregion

                    CustomResponse<GroupListViewModel> response = new CustomResponse<GroupListViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new GroupListViewModel
                        {
                            MyGroupCount = MyGroupCount,
                            MyGroups = myGroups,
                            SuggestedGroupCount = SuggestedGroupCount,
                            SuggestedGroups = suggestedGroups
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
        [Route("SearchGroups")]
        public async Task<IHttpActionResult> SearchGroups(string SearchText = "")
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                using (RiscoContext ctx = new RiscoContext())
                {
                    #region MyGroups
                    List<Group> myGroups = new List<Group>();
                    List<int> GroupsIds = new List<int>();
                    List<int> joinedGroupIds = ctx.GroupMembers.Where(x => x.User_Id == userId && x.Status == (int)GroupMemberStatusTypes.Accepted).Select(x => x.Group_Id).ToList();
                    myGroups = ctx.Groups
                        .Where(x => x.IsDeleted == false && (x.User_Id == userId || joinedGroupIds.Contains(x.Id)) && x.Name.Contains(SearchText))
                        .OrderByDescending(x => x.Id)
                        .ToList();

                    foreach (Group group in myGroups)
                    {
                        group.MembersCount = ctx.GroupMembers.Count(x => x.Status == (int)GroupMemberStatusTypes.Accepted && x.Group_Id == group.Id);
                        group.IsAdmin = (group.User_Id == userId) ? true : false;
                    }
                    #endregion

                    #region SuggestedGroups
                    List<Group> suggestedGroups = new List<Group>();

                    User user = ctx.Users.FirstOrDefault(x => x.Id == userId);


                    //commenting interests because interests are not required in group anymore
                    //if (!string.IsNullOrEmpty(user.Interests))
                    //{
                    //    List<int> InterestIds = user.Interests.Split(',').Select(x => int.Parse(x)).ToList();

                    //suggestedGroups = ctx.Groups
                    //    .Where(x => x.IsDeleted == false && x.User_Id != userId && !joinedGroupIds.Contains(x.Id)                             
                    //    && x.Name.Contains(SearchText)
                    //     && (InterestIds.Count > 0 ? x.GroupCategories.Any(y => InterestIds.Contains(y.Interest_Id) && !y.IsDeleted) : true))
                    //    .ToList();

                    var myFollowerFollowingsId = ctx.FollowFollowers.Where(x => x.FirstUser_Id == userId && x.IsDeleted == false).Select(x => x.SecondUser_Id).ToList();            //getting all followerIds
                    myFollowerFollowingsId.AddRange(ctx.FollowFollowers.Where(x => x.SecondUser_Id == userId && x.IsDeleted == false).Select(x => x.FirstUser_Id).ToList());        //getting all followingIds
                    myFollowerFollowingsId = myFollowerFollowingsId.Distinct().ToList();                                                      //removing all duplicates

                    suggestedGroups = ctx.Groups
                        .Include(x => x.User)
                        .Where(x => x.IsDeleted == false && x.User_Id != userId &&
                        !joinedGroupIds.Contains(x.Id) &&
                        x.GroupMembers.Any(y => y.Status == (int)GroupMemberStatusTypes.Accepted && myFollowerFollowingsId.Contains(y.User_Id) && y.IsBlockedForOtherUser == false)).ToList();

                    foreach (Group group in suggestedGroups)
                    {
                        group.MembersCount = ctx.GroupMembers.Count(x => x.Status == (int)GroupMemberStatusTypes.Accepted && x.Group_Id == group.Id);
                        if (ctx.GroupMembers.Any(x => x.Group_Id == group.Id && x.User_Id == userId))
                        {
                            group.Status = ctx.GroupMembers.FirstOrDefault(x => x.Group_Id == group.Id && x.User_Id == userId).Status;
                        }
                        else
                        {
                            group.Status = (int)GroupMemberStatusTypes.YetToJoin;
                        }
                    }
                    //}
                    #endregion

                    CustomResponse<GroupListViewModel> response = new CustomResponse<GroupListViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new GroupListViewModel
                        {
                            MyGroups = myGroups,
                            SuggestedGroups = suggestedGroups
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
        [Route("SearchAllGroups")]
        public async Task<IHttpActionResult> SearchAllGroups(string SearchText = "")
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                using (RiscoContext ctx = new RiscoContext())
                {
                    #region MyGroups
                    List<Group> myGroups = new List<Group>();
                    List<int> GroupsIds = new List<int>();
                    //List<int> joinedGroupIds = ctx.GroupMembers.Where(x => x.User_Id == userId && x.Status == (int)GroupMemberStatusTypes.Accepted).Select(x => x.Group_Id).ToList();
                    myGroups = ctx.Groups
                        .Where(x => x.IsDeleted == false && x.Name.Contains(SearchText))
                        .OrderByDescending(x => x.Id)
                        .ToList();

                    foreach (Group group in myGroups)
                    {
                        group.MembersCount = ctx.GroupMembers.Count(x => x.Status == (int)GroupMemberStatusTypes.Accepted && x.Group_Id == group.Id);
                        group.IsAdmin = (group.User_Id == userId) ? true : false;
                    }
                    #endregion



                    CustomResponse<GroupListViewModel> response = new CustomResponse<GroupListViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new GroupListViewModel
                        {
                            MyGroups = myGroups
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
        [Route("GetGroupById")]
        public async Task<IHttpActionResult> GetGroupById(int Group_Id)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                    Group group = new Group();
                    group = ctx.Groups
                        .Include(x => x.User)
                        .Include(x => x.GroupMembers)
                        .FirstOrDefault(x => x.IsDeleted == false && x.Id == Group_Id);

                    group.PostsCount = ctx.Posts.Count(x => x.IsDeleted == false && x.Group_Id == Group_Id);
                    group.MembersCount = ctx.GroupMembers.Count(x => x.Status == (int)GroupMemberStatusTypes.Accepted && x.Group_Id == Group_Id);
                    group.JoinRequestCount = ctx.GroupMembers.Count(x => x.Status == (int)GroupMemberStatusTypes.Pending && x.Group_Id == Group_Id);
                    group.IsAdmin = (group.User_Id == userId) ? true : false;


                    if (group.User != null)
                    {
                        if (ctx.BlockUsers.Any(x => x.FirstUser_Id == group.User_Id && x.SecondUser_Id == userId && !x.IsDeleted))
                        {
                            group.AdminViewBlocked = true;
                        }
                    }

                    if (!group.IsAdmin)
                    {
                        if (ctx.GroupMembers.Any(x => x.Group_Id == group.Id && x.User_Id == userId))
                        {
                            group.Status = ctx.GroupMembers.FirstOrDefault(x => x.Group_Id == group.Id && x.User_Id == userId).Status;
                        }
                        else
                        {
                            group.Status = (int)GroupMemberStatusTypes.DoesNotExist;
                        }
                    }
                    else
                    {
                        group.Status = (int)GroupMemberStatusTypes.Accepted;
                    }

                    CustomResponse<Group> response = new CustomResponse<Group>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = group
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return Ok(new CustomResponse<string>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = (int)Utility.LogError(ex),
                    Result = "Something went wrong"
                });
            }
        }

        [HttpGet]
        [Route("GetPostsByGroupId")]
        public async Task<IHttpActionResult> GetPostsByGroupId(int Group_Id, int PageSize = int.MaxValue, int PageNo = 0)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                    List<Post> posts = new List<Post>();

                    // Hide All Post Users

                    var HideAllUsersIds = ctx.HideAllPosts.Where(x => x.FirstUserAll_Id == userId && x.IsDeleted == false).Select(x => x.SecondUserAll_Id).Distinct().ToList();

                    var HidePostsIds = ctx.HidePosts.Where(x => x.FirstUser_Id == userId && x.IsDeleted == false).Select(x => x.Post_Id).Distinct().ToList();

                    var ReportPostsIds = ctx.ReportPosts.Where(x => x.User_Id == userId && x.IsDeleted == false).Select(x => x.Post_Id).Distinct().ToList();

                    var BlockedUsers = ctx.BlockUsers.Where(x => x.FirstUser_Id == userId && !x.IsDeleted).Select(x => x.SecondUser_Id).Distinct().ToList();

                    BlockedUsers.AddRange(ctx.BlockUsers.Where(x => x.SecondUser_Id == userId && !x.IsDeleted).Select(x => x.FirstUser_Id).Distinct().ToList());



                    posts = ctx.Posts
                        .Include(x => x.User)
                        .Include(x => x.Medias)
                        .Include(x => x.SharedParent)
                        .Include(x => x.SharedParent.Medias)
                        .Include(x => x.SharedParent.User)
                        .Include(x => x.Medias)
                        .Where(x => x.IsDeleted == false
                        && x.Group_Id == Group_Id
                        && !HideAllUsersIds.Contains(x.User_Id)
                        && !HidePostsIds.Contains(x.Id)
                        && !ReportPostsIds.Contains(x.Id)
                        && !BlockedUsers.Contains(x.User_Id))
                        .OrderByDescending(x => x.Id)
                        .ToList();

                    int PostCount = posts.Count();
                    //posts = posts.Skip((PageNo - 1) * PageSize).Take(PageSize).ToList();
                    posts = posts.Skip(PageSize * PageNo).Take(PageSize).ToList();

                    foreach (Post post in posts)
                    {

                        if (post.Medias.Count > 0)
                            post.Medias = post.Medias.Where(x => x.Comment_Id == null && !x.IsDeleted).ToList();

                        if (post.SharedParent != null)
                        {
                            post.SharedParent.Medias = post.SharedParent.Medias.Where(x => x.Comment_Id == null).ToList();
                            post.SharedParent.IsUserFollow = ctx.FollowFollowers.Any(x => x.FirstUser_Id == userId && x.SecondUser_Id == post.SharedParent.User_Id && x.IsDeleted == false);
                            foreach (var item in post.SharedParent.Medias)
                            {
                                if (item.User != null)
                                    item.User = null;
                            }
                        }

                        if (post.IsPoll)
                        {
                            post.CheckPollExpiry();

                            foreach (var option in post.PollOptions)
                            {
                                option.IsVoted = ctx.PollOptionVote.Any(x => x.User_Id == userId && x.Post_Id == post.Id);
                            }

                        }

                        post.IsLiked = ctx.Likes.Any(x => x.Post_Id == post.Id && x.User_Id == userId && x.IsDeleted == false);
                        post.LikesCount = ctx.Posts.Sum(p => p.Likes.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.CommentsCount = ctx.Posts.Sum(p => p.Comments.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.ShareCount = ctx.Posts.Sum(p => p.Shares.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.IsUserFollow = ctx.FollowFollowers.Any(x => x.FirstUser_Id == userId && x.SecondUser_Id == post.User_Id && x.IsDeleted == false);
                        post.IsPostOwner = (post.User_Id == userId) ? true : false;
                    }

                    CustomResponse<PostListViewModel> response = new CustomResponse<PostListViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new PostListViewModel
                        {
                            Posts = posts,
                            PostCount = PostCount
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
        [Route("GetMembersByGroupId")]
        public async Task<IHttpActionResult> GetMembersByGroupId(int Group_Id, int PageSize = int.MaxValue, int PageNo = 0)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                    List<GroupMember> groupMembers = new List<GroupMember>();

                    groupMembers = ctx.GroupMembers
                        .Include(x => x.User)
                        .Where(x => x.Status == (int)GroupMemberStatusTypes.Accepted && x.Group_Id == Group_Id)
                        .OrderBy(x => x.Id)
                        .ToList();

                    int GroupMembersCount = groupMembers.Count();
                    groupMembers = groupMembers.Skip((PageNo - 1) * PageSize).Take(PageSize).ToList();

                    foreach (GroupMember groupMember in groupMembers)
                    {
                        groupMember.IsUserFollow = ctx.FollowFollowers.Any(x => x.FirstUser_Id == userId && x.SecondUser_Id == groupMember.User_Id && x.IsDeleted == false);
                        if (ctx.BlockUsers.Any(x => x.FirstUser_Id == groupMember.User_Id && x.SecondUser_Id == userId && !x.IsDeleted))
                            groupMember.IsBlockedForOtherUser = true;

                    }

                    CustomResponse<GroupMemberListViewModel> response = new CustomResponse<GroupMemberListViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new GroupMemberListViewModel
                        {
                            GroupMember = groupMembers,
                            GroupMemberCount = GroupMembersCount
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
        [Route("DeleteGroup")]
        public async Task<IHttpActionResult> DeleteGroup(int Group_Id)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    Group group = ctx.Groups.FirstOrDefault(x => x.Id == Group_Id);
                    group.IsDeleted = true;
                    group.UpdatedDate = DateTime.UtcNow;
                    ctx.SaveChanges();

                    CustomResponse<Group> response = new CustomResponse<Group>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = group
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
        [Route("AddGroupMembersByAdmin")]
        public async Task<IHttpActionResult> AddGroupMembersByAdmin(int Group_Id, string GroupMembersIds)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    if (!string.IsNullOrEmpty(GroupMembersIds))
                    {
                        GroupMember groupMember;
                        string[] GroupMembersIdsArray = GroupMembersIds.Split(',');
                        foreach (var Id in GroupMembersIdsArray)
                        {
                            int GroupMember_Id = Convert.ToInt32(Id);
                            if (ctx.GroupMembers.Any(x => x.Group_Id == Group_Id && x.User_Id == GroupMember_Id)) // if this check is about approving request of user whose request was pending then status check is missing
                            {
                                groupMember = ctx.GroupMembers.Include(x => x.Group).FirstOrDefault(x => x.Group_Id == Group_Id && x.User_Id == GroupMember_Id);
                                groupMember.Status = (int)GroupMemberStatusTypes.Accepted;
                                groupMember.UpdatedDate = DateTime.UtcNow;
                            }
                            else
                            {
                                groupMember = new GroupMember
                                {
                                    User_Id = GroupMember_Id,
                                    Group_Id = Group_Id,
                                    CreatedDate = DateTime.UtcNow,
                                    UpdatedDate = DateTime.UtcNow,
                                    Status = (int)GroupMemberStatusTypes.YetToJoin
                                };
                                ctx.GroupMembers.Add(groupMember);
                                ctx.SaveChanges();
                                groupMember = ctx.GroupMembers.Include(x => x.Group).FirstOrDefault(x => x.Id == groupMember.Id);
                            }
                            Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.Group, EntityId: groupMember.Group_Id, ReceivingUser_Id: GroupMember_Id, SendingUser_Id: groupMember.Group.User_Id, ChildEntityType: (int)RiscoEntityTypes.AddInGroup);
                        }
                        ctx.SaveChanges();
                    }

                    CustomResponse<string> response = new CustomResponse<string>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = "Members Added"
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
        [Route("LeftGroupByUser")]
        public async Task<IHttpActionResult> LeftGroupByUser(int Group_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                using (RiscoContext ctx = new RiscoContext())
                {
                    GroupMember groupMember = ctx.GroupMembers.FirstOrDefault(x => x.Group_Id == Group_Id && x.User_Id == userId && x.Status == (int)GroupMemberStatusTypes.Accepted);
                    groupMember.Status = (int)GroupMemberStatusTypes.LeftByUser;
                    groupMember.UpdatedDate = DateTime.UtcNow;
                    ctx.SaveChanges();

                    CustomResponse<GroupMember> response = new CustomResponse<GroupMember>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = groupMember
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
        [Route("RemoveUserByAdmin")]
        public async Task<IHttpActionResult> RemoveUserByAdmin(int Group_Id, int User_Id)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    GroupMember groupMember = ctx.GroupMembers.FirstOrDefault(x => x.Group_Id == Group_Id && x.User_Id == User_Id && x.Status == (int)GroupMemberStatusTypes.Accepted);
                    groupMember.Status = (int)GroupMemberStatusTypes.LeftUserByAdmin;
                    //groupMember.IsDeleted = true;                                             //commenting isdeleted because group memebers are being managed by the GroupMember.Status
                    groupMember.UpdatedDate = DateTime.UtcNow;
                    ctx.SaveChanges();

                    CustomResponse<GroupMember> response = new CustomResponse<GroupMember>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = groupMember
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return Ok(new CustomResponse<string>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = (int)Utility.LogError(ex),
                    Result = "Something went wrong"
                });
            }
        }

        [HttpGet]
        [Route("JoinRequest")]
        public async Task<IHttpActionResult> JoinRequest(int Group_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                using (RiscoContext ctx = new RiscoContext())
                {
                    GroupMember groupMember;
                    int ReceivingUser_Id = ctx.Groups.FirstOrDefault(x => x.Id == Group_Id).User_Id;

                    if (ctx.GroupMembers.Any(x => x.Group_Id == Group_Id && x.User_Id == userId         //if this user was previously a member of this group
                        && (x.Status == (int)GroupMemberStatusTypes.Rejected
                        || x.Status == (int)GroupMemberStatusTypes.CancelRequest
                        || x.Status == (int)GroupMemberStatusTypes.LeftByUser
                        || x.Status == (int)GroupMemberStatusTypes.LeftUserByAdmin)))
                    {
                        groupMember = ctx.GroupMembers.FirstOrDefault(x => x.Group_Id == Group_Id && x.User_Id == userId);
                        groupMember.Status = (int)GroupMemberStatusTypes.Pending;
                        groupMember.UpdatedDate = DateTime.UtcNow;
                    }
                    else if (ctx.GroupMembers.Any(x => x.Group_Id == Group_Id && x.User_Id == userId    //if a request already exists
                        && (x.Status == (int)GroupMemberStatusTypes.YetToJoin
                        || x.Status == (int)GroupMemberStatusTypes.Pending
                        || x.Status == (int)GroupMemberStatusTypes.Accepted)))
                    {
                        CustomResponse<string> responseError = new CustomResponse<string>
                        {
                            Message = Global.ResponseMessages.BadRequest,
                            StatusCode = (int)HttpStatusCode.Conflict,
                            Result = "You are not eligible for this request"
                        };
                        return Ok(responseError);
                    }
                    else
                    {
                        groupMember = new GroupMember                                               //if this user has never been a member of this group and has never sent a request either
                        {
                            User_Id = userId,
                            Group_Id = Group_Id,
                            CreatedDate = DateTime.UtcNow,
                            UpdatedDate = DateTime.UtcNow,
                            Status = (int)GroupMemberStatusTypes.Pending
                        };
                        ctx.GroupMembers.Add(groupMember);
                    }
                    ctx.SaveChanges();
                    Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.SendGroupRequest, EntityId: Group_Id, ReceivingUser_Id: ReceivingUser_Id, SendingUser_Id: userId);
                    CustomResponse<GroupMember> response = new CustomResponse<GroupMember>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = groupMember
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
        [Route("CancelJoinRequest")]
        public async Task<IHttpActionResult> CancelJoinRequest(int Group_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                using (RiscoContext ctx = new RiscoContext())
                {
                    GroupMember groupMember = ctx.GroupMembers.FirstOrDefault(x => x.Group_Id == Group_Id && x.User_Id == userId && x.Status == (int)GroupMemberStatusTypes.Pending);
                    groupMember.Status = (int)GroupMemberStatusTypes.CancelRequest;
                    groupMember.UpdatedDate = DateTime.UtcNow;
                    ctx.SaveChanges();

                    //int ReceivingUser_Id = ctx.Groups.FirstOrDefault(x => x.Id == Group_Id).User_Id;
                    //Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.CancelGroupRequest, EntityId: Group_Id, ReceivingUser_Id: ReceivingUser_Id, SendingUser_Id: userId);                    

                    CustomResponse<GroupMember> response = new CustomResponse<GroupMember>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = groupMember
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return Ok(new CustomResponse<string>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = (int)Utility.LogError(ex),
                    Result = "Something went wrong"
                });
            }
        }

        [HttpGet]
        [Route("GetAllJoinRequests")]
        public async Task<IHttpActionResult> GetAllJoinRequests(int Group_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                using (RiscoContext ctx = new RiscoContext())
                {
                    List<GroupMember> groupMembers = ctx.GroupMembers
                        .Include(x => x.User)
                        .Where(x => x.Group_Id == Group_Id && x.Status == (int)GroupMemberStatusTypes.Pending)
                        .ToList();

                    CustomResponse<List<GroupMember>> response = new CustomResponse<List<GroupMember>>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = groupMembers
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
        [Route("AcceptJoinRequestByAdmin")]
        public async Task<IHttpActionResult> AcceptJoinRequestByAdmin(int Group_Id, int User_Id) // this user id will be off user whose request will be accepted.
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid")); // admin of group
                using (RiscoContext ctx = new RiscoContext())
                {
                    GroupMember groupMember = ctx.GroupMembers.FirstOrDefault(x => x.Group_Id == Group_Id && x.User_Id == User_Id && x.Status == (int)GroupMemberStatusTypes.Pending);
                    groupMember.Status = (int)GroupMemberStatusTypes.Accepted;
                    groupMember.UpdatedDate = DateTime.UtcNow;
                    ctx.SaveChanges();

                    Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.AcceptGroupRequest, EntityId: Group_Id, ReceivingUser_Id: User_Id, SendingUser_Id: userId);

                    CustomResponse<GroupMember> response = new CustomResponse<GroupMember>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = groupMember
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return Ok(new CustomResponse<string>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = (int)Utility.LogError(ex),
                    Result = "Something went wrong"
                });
            }
        }


        [HttpGet]
        [Route("AcceptJoinRequestByUser")]
        public async Task<IHttpActionResult> AcceptJoinRequestByUser(int Group_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                using (RiscoContext ctx = new RiscoContext())
                {
                    GroupMember groupMember = ctx.GroupMembers.Include(x => x.Group).FirstOrDefault(x => x.Group_Id == Group_Id && x.User_Id == userId && x.Status == (int)GroupMemberStatusTypes.YetToJoin);

                    if (groupMember == null)
                    {
                        return Content(HttpStatusCode.NotFound, new CustomResponse<Error>
                        {
                            Message = "NotFound",
                            StatusCode = (int)HttpStatusCode.NotFound,
                            Result = new Error { ErrorMessage = "Join request not found." }
                        });
                    }

                    groupMember.Status = (int)GroupMemberStatusTypes.Accepted;
                    groupMember.UpdatedDate = DateTime.UtcNow;
                    ctx.SaveChanges();

                    int groupAdminUserId = groupMember.Group.User_Id;

                    //closing the noti here because no point of sending a noti to anyone when a user has accepted a group join request
                    //Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.AcceptGroupRequest, EntityId: Group_Id, ReceivingUser_Id: groupAdminUserId, SendingUser_Id: userId);

                    CustomResponse<GroupMember> response = new CustomResponse<GroupMember>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = groupMember
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return Ok(new CustomResponse<string>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = (int)Utility.LogError(ex),
                    Result = "Something went wrong"
                });
            }
        }



        [HttpGet]
        [Route("RejectJoinRequestByAdmin")]
        public async Task<IHttpActionResult> RejectJoinRequestByAdmin(int Group_Id, int User_Id) // this user id will be off user whose request will be rejected.
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid")); // admin of group
                using (RiscoContext ctx = new RiscoContext())
                {
                    GroupMember groupMember = ctx.GroupMembers.FirstOrDefault(x => x.Group_Id == Group_Id && x.User_Id == User_Id && x.Status == (int)GroupMemberStatusTypes.Pending);
                    groupMember.Status = (int)GroupMemberStatusTypes.Rejected;
                    groupMember.UpdatedDate = DateTime.UtcNow;
                    ctx.SaveChanges();

                    CustomResponse<GroupMember> response = new CustomResponse<GroupMember>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = groupMember
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return Ok(new CustomResponse<string>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = (int)Utility.LogError(ex),
                    Result = "Something went wrong"
                });
            }
        }

        [HttpGet]
        [Route("RejectJoinRequestByUser")]
        public async Task<IHttpActionResult> RejectJoinRequestByUser(int Group_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                using (RiscoContext ctx = new RiscoContext())
                {
                    GroupMember groupMember = ctx.GroupMembers.FirstOrDefault(x => x.Group_Id == Group_Id && x.User_Id == userId && x.Status == (int)GroupMemberStatusTypes.YetToJoin);

                    if (groupMember == null)
                    {
                        return Content(HttpStatusCode.NotFound, new CustomResponse<Error>
                        {
                            Message = "NotFound",
                            StatusCode = (int)HttpStatusCode.NotFound,
                            Result = new Error { ErrorMessage = "Join request not found." }
                        });
                    }

                    groupMember.Status = (int)GroupMemberStatusTypes.Rejected;
                    groupMember.UpdatedDate = DateTime.UtcNow;
                    ctx.SaveChanges();

                    //no noti getting generated at this point because it is unethical to send rejection notificaction
                    //Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.RejectGroupRequest, EntityId: Group_Id, ReceivingUser_Id: User_Id, SendingUser_Id: userId);

                    CustomResponse<GroupMember> response = new CustomResponse<GroupMember>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = groupMember
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return Ok(new CustomResponse<string>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = (int)Utility.LogError(ex),
                    Result = "Something went wrong"
                });
            }
        }

        [HttpGet]
        [Route("GetFollowersToAdd")]
        public async Task<IHttpActionResult> GetFollowersToAdd(int? Group_Id = null, int? Page = 0, int? Items = 10)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                    var AlreadyMembersIds = ctx.GroupMembers.Where(x => x.Group_Id == Group_Id.Value && x.Status == (int)GroupMemberStatusTypes.Accepted).Select(x => x.User_Id).ToList();

                    List<FollowFollower> followFollowers = new List<FollowFollower>();

                    //followFollowers = ctx.FollowFollowers
                    //    .Include(x => x.FirstUser)
                    //    .Where(x => x.IsDeleted == false && x.SecondUser_Id == userId && !AlreadyMembersIds.Contains(x.FirstUser_Id))
                    //    .ToList();
                    var IBlocked = ctx.BlockUsers.Where(x => x.FirstUser_Id == userId).Select(x => x.SecondUser_Id).ToList();
                    var TheyBlocked = ctx.BlockUsers.Where(x => x.SecondUser_Id == userId).Select(x => x.FirstUser_Id).ToList();

                    var Users = ctx.Users.Where(x => !x.IsDeleted && !IBlocked.Contains(x.Id) && !TheyBlocked.Contains(x.Id) && !AlreadyMembersIds.Contains(x.Id)).OrderByDescending(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();

                    if (Users != null)
                    {
                        foreach (var user in Users)
                        {
                            followFollowers.Add(new FollowFollower
                            {
                                FirstUser = user,
                                FirstUser_Id = user.Id
                            });
                        }
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
        [Route("GetFollowFollowersToAdd")]
        public async Task<IHttpActionResult> GetFollowFollowersToAdd(int? Group_Id = null, int? Page = 0, int? Items = 10)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    ctx.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                    //these are users which are already group members or their group status is YetToJoin or Pending, so we should not send them JointRequest again
                    var AlreadyMembersIds = ctx.GroupMembers
                        .Where(x => x.Group_Id == Group_Id.Value &&
                        (x.Status == (int)GroupMemberStatusTypes.Accepted || x.Status == (int)GroupMemberStatusTypes.Pending || x.Status == (int)GroupMemberStatusTypes.YetToJoin)).Select(x => x.User_Id).ToList();

                    List<FollowFollower> followFollowers = new List<FollowFollower>();
                    //List<User> lstUsers = new List<User>();

                    var IBlocked = ctx.BlockUsers.Where(x => x.FirstUser_Id == userId).Select(x => x.SecondUser_Id).ToList();
                    var TheyBlocked = ctx.BlockUsers.Where(x => x.SecondUser_Id == userId).Select(x => x.FirstUser_Id).ToList();
                    followFollowers = ctx.FollowFollowers
                        .Include(x => x.FirstUser)
                        .Include(x => x.SecondUser)
                        .Where(x => x.IsDeleted == false && (x.SecondUser_Id == userId || x.FirstUser_Id == userId) //&& !AlreadyMembersIds.Contains(x.FirstUser_Id)
                            && !IBlocked.Contains(x.Id) && !TheyBlocked.Contains(x.Id))
                            .OrderByDescending(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();

                    //var Users = ctx.Users.Where(x => !x.IsDeleted && !IBlocked.Contains(x.Id) && !TheyBlocked.Contains(x.Id) && !AlreadyMembersIds.Contains(x.Id)).OrderByDescending(x => x.Id).Skip(Page.Value * Items.Value).Take(Items.Value).ToList();
                    //if (Users != null)
                    //{
                    //    foreach (var user in Users)
                    //    {
                    //        followFollowers.Add(new FollowFollower
                    //        {
                    //            FirstUser = user,
                    //            FirstUser_Id = user.Id
                    //        });
                    //    }
                    //}

                    var myFollowFollowerIds = followFollowers.Where(x => x.SecondUser_Id == userId).Select(x => x.FirstUser_Id).ToList();               //followers
                    myFollowFollowerIds.AddRange(followFollowers.Where(x => x.FirstUser_Id == userId).Select(x => x.SecondUser_Id).ToList());           //followings
                    myFollowFollowerIds = myFollowFollowerIds.Distinct().ToList();                                                                      //removing all duplicates
                    myFollowFollowerIds = myFollowFollowerIds.Where(x => !AlreadyMembersIds.Contains(x)).ToList();                                      //removing users which are already group members


                    var users = ctx.Users.Where(x => myFollowFollowerIds.Contains(x.Id)).ToList();

                    //foreach (FollowFollower followFoller in followFollowers)
                    //{
                    //    //followFoller.IsFollowing = ctx.FollowFollowers.Any(x => x.FirstUser_Id == userId && x.SecondUser_Id == followFoller.FirstUser_Id && x.IsDeleted == false);
                    //    if (followFoller.FirstUser_Id == userId)
                    //    {
                    //        followFoller.FirstUser = followFoller.SecondUser;
                    //    }
                    //    followFoller.SecondUser = null;
                    //}
                    //lstUsers = followFollowers.Select(x => x.FirstUser).Distinct().ToList();

                    CustomResponse<List<User>> response = new CustomResponse<List<User>>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = users
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
        [Route("SearchGetFollowFollowersToAdd")]
        public async Task<IHttpActionResult> SearchGetFollowFollowersToAdd(int? Group_Id = null, string SearchText = "")
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                    //these are users which are already group members or their group status is YetToJoin or Pending, so we should not send them JointRequest again
                    var AlreadyMembersIds = ctx.GroupMembers
                        .Where(x => x.Group_Id == Group_Id.Value &&
                        (x.Status == (int)GroupMemberStatusTypes.Accepted || x.Status == (int)GroupMemberStatusTypes.Pending || x.Status == (int)GroupMemberStatusTypes.YetToJoin)).Select(x => x.User_Id).ToList();

                    List<FollowFollower> followFollowers = new List<FollowFollower>();
                    //List<User> lstUsers = new List<User>();

                    var IBlocked = ctx.BlockUsers.Where(x => x.FirstUser_Id == userId).Select(x => x.SecondUser_Id).ToList();
                    var TheyBlocked = ctx.BlockUsers.Where(x => x.SecondUser_Id == userId).Select(x => x.FirstUser_Id).ToList();
                    followFollowers = ctx.FollowFollowers
                       .Include(x => x.FirstUser)
                       .Include(x => x.SecondUser)
                       .Where(x => x.IsDeleted == false && !IBlocked.Contains(x.Id) && !TheyBlocked.Contains(x.Id)
                       && (x.SecondUser_Id == userId || x.FirstUser_Id == userId))
                       //&& (x.FirstUser.FullName.Contains(SearchText) || x.FirstUser.Phone.Contains(SearchText) || x.FirstUser.Email.Contains(SearchText)))
                       .ToList();
                    //var Users = ctx.Users.Where(x => !x.IsDeleted && !IBlocked.Contains(x.Id) && !TheyBlocked.Contains(x.Id) && !AlreadyMembersIds.Contains(x.Id) && (x.FullName.Contains(SearchText) || x.Phone.Contains(SearchText) || x.Email.Contains(SearchText))).ToList();

                    //if (Users != null)
                    //{
                    //    foreach (var user in Users)
                    //    {
                    //        followFollowers.Add(new FollowFollower
                    //        {
                    //            FirstUser = user,
                    //            FirstUser_Id = user.Id
                    //        });
                    //    }
                    //}
                    //List<FollowFollower> followFollowersToRremove = new List<FollowFollower>();

                    var myFollowFollowerIds = followFollowers.Where(x => x.SecondUser_Id == userId
                    && (x.FirstUser.FullName.Contains(SearchText) || x.FirstUser.Email.Contains(SearchText)))
                    .Select(x => x.FirstUser_Id).ToList();               //followers

                    myFollowFollowerIds.AddRange(followFollowers.Where(x => x.FirstUser_Id == userId
                    && (x.SecondUser.FullName.Contains(SearchText) || x.SecondUser.Email.Contains(SearchText))).Select(x => x.SecondUser_Id).ToList());           //followings
                    myFollowFollowerIds = myFollowFollowerIds.Distinct().ToList();                                                                      //removing all duplicates
                    myFollowFollowerIds = myFollowFollowerIds.Where(x => !AlreadyMembersIds.Contains(x)).ToList();                                      //removing users which are already group members

                    var users = ctx.Users.Where(x => myFollowFollowerIds.Contains(x.Id)).ToList();

                    //foreach (FollowFollower followFoller in followFollowers)
                    //{
                    //    //followFoller.IsFollowing = ctx.FollowFollowers.Any(x => x.FirstUser_Id == userId && x.SecondUser_Id == followFoller.FirstUser_Id && x.IsDeleted == false);
                    //    if (followFoller.FirstUser_Id == userId)
                    //    {
                    //        followFoller.FirstUser = followFoller.SecondUser;
                    //    }
                    //    followFoller.SecondUser = null;
                    //}

                    //lstUsers = followFollowers.Select(x => x.FirstUser).Distinct().ToList();

                    CustomResponse<List<User>> response = new CustomResponse<List<User>>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = users
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
        [Route("SearchGetFollowersToAdd")]
        public async Task<IHttpActionResult> SearchGetFollowersToAdd(int? Group_Id = null, string SearchText = "")
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                    var AlreadyMembersIds = ctx.GroupMembers.Where(x => x.Group_Id == Group_Id.Value && x.Status == (int)GroupMemberStatusTypes.Accepted).Select(x => x.User_Id).ToList();

                    List<FollowFollower> followFollowers = new List<FollowFollower>();

                    //followFollowers = ctx.FollowFollowers
                    //    .Include(x => x.FirstUser)
                    //    .Where(x => x.IsDeleted == false && x.SecondUser_Id == userId && !AlreadyMembersIds.Contains(x.FirstUser_Id) && (x.FirstUser.FullName.Contains(SearchText) || x.FirstUser.Phone.Contains(SearchText) || x.FirstUser.Email.Contains(SearchText)))
                    //    .ToList();
                    var IBlocked = ctx.BlockUsers.Where(x => x.FirstUser_Id == userId).Select(x => x.SecondUser_Id).ToList();
                    var TheyBlocked = ctx.BlockUsers.Where(x => x.SecondUser_Id == userId).Select(x => x.FirstUser_Id).ToList();

                    var Users = ctx.Users.Where(x => !x.IsDeleted && !IBlocked.Contains(x.Id) && !TheyBlocked.Contains(x.Id) && !AlreadyMembersIds.Contains(x.Id) && (x.FullName.Contains(SearchText) || x.Phone.Contains(SearchText) || x.Email.Contains(SearchText))).ToList();

                    if (Users != null)
                    {
                        foreach (var user in Users)
                        {
                            followFollowers.Add(new FollowFollower
                            {
                                FirstUser = user,
                                FirstUser_Id = user.Id
                            });
                        }
                    }
                    //List<FollowFollower> followFollowersToRremove = new List<FollowFollower>();

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

    }
}
