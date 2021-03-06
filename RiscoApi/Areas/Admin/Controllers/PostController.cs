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
using WebApplication1.ViewModels;
using Z.EntityFramework.Plus;
using System.Drawing;
using System.Drawing.Imaging;
using GleamTech.VideoUltimate;
using BasketApi.ViewModels;
using NReco.VideoConverter;
using static DAL.CustomModels.Enumeration;
using BLL.Utility;
using System.Diagnostics;

namespace BasketApi.Areas.SubAdmin.Controllers
{
    [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User", "Guest")]
    [RoutePrefix("api/Post")]
    public class PostController : ApiController
    {
        //private MemoryStream SaveThumbnail(SPFile videoFile)
        //{
        //    MemoryStream ms;
        //    try
        //    {
        //        //Creating Temp File Path to be used by Nreco


        //        ms = new MemoryStream();
        //        SPSecurity.RunWithElevatedPrivileges(delegate () {

        //            string destinationFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + videoFile.Name);

        //            //Copying the content the content of the file at temp loaction from stream
        //            using (FileStream fileStream = File.Create(destinationFile))
        //            {
        //                Stream lStream = videoFile.OpenBinaryStream();
        //                byte[] contents = new byte[lStream.Length];
        //                lStream.Read(contents, 0, (int)lStream.Length);
        //                lStream.Close();

        //                // Use write method to write to the file specified above
        //                fileStream.Write(contents, 0, contents.Length);
        //                fileStream.Close();
        //            }


        //            var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
        //            ffMpeg.GetVideoThumbnail(destinationFile, ms, 10);

        //            System.IO.File.Delete(destinationFile);
        //        });


        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return ms;
        //}


        #region HttpGet

        [HttpGet]
        [Route("GetPosts")]
        public async Task<IHttpActionResult> GetPosts(int PageSize = 20, int PageNo = 0)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    ctx.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);

                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                    List<int> UserInterests = new List<int>();
                    List<int> UserLanguageIds = new List<int>();
                    List<string> userlangCodes = new List<string>();
                    List<int> postIds = new List<int>();

                    List<Post> posts = new List<Post>();

                    // Hide All Post Users

                    var HideAllUsersIds = ctx.HideAllPosts.Where(x => x.FirstUserAll_Id == userId && x.IsDeleted == false).Select(x => x.SecondUserAll_Id).Distinct().ToList();

                    var HidePostsIds = ctx.HidePosts.Where(x => x.FirstUser_Id == userId && x.IsDeleted == false).Select(x => x.Post_Id).Distinct().ToList();

                    var ReportPostsIds = ctx.ReportPosts.Where(x => x.User_Id == userId && x.IsDeleted == false).Select(x => x.Post_Id).Distinct().ToList();

                    var userModel = ctx.Users.Include(x => x.UserLanguageMappings).FirstOrDefault(x => x.Id == userId);
                    if (userModel != null)
                    {
                        if (!string.IsNullOrEmpty(userModel.Interests))
                        {
                            UserInterests = userModel.Interests.Split(',').Select(int.Parse).ToList();
                        }

                        if (!string.IsNullOrEmpty(userModel.Language))
                        {
                            userlangCodes = userModel.Language.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x).ToList();
                            UserLanguageIds = userModel.UserLanguageMappings.Select(x => x.LanguageId).ToList();
                        }

                        //if (!string.IsNullOrEmpty(userModel.Language))
                        //{
                        //    UserLanguageCodes = userModel.Language.Split(',').Select(x => x).ToList();
                        //    UserLanguageIds =
                        //        ctx.Languages.Where(y => UserLanguageCodes.Contains(y.Code) && !y.IsDeleted).Select(x => x.Id).ToList();
                        //}
                    }

                    List<FollowFollower> followFollowers = new List<FollowFollower>();

                    var myFollowingsIds = ctx.FollowFollowers                                             //this list contains all the user ids whose posts we want
                        .Where(x => x.IsDeleted == false && x.FirstUser_Id == userId)
                        .Select(x => x.SecondUser_Id).Distinct()
                        .ToList();
                    myFollowingsIds.Add(userId);


                    var BlockedUsers = ctx.BlockUsers.Where(x => x.FirstUser_Id == userId && !x.IsDeleted).Select(x => x.SecondUser_Id).Distinct().ToList();

                    BlockedUsers.AddRange(ctx.BlockUsers.Where(x => x.SecondUser_Id == userId && !x.IsDeleted).Select(x => x.FirstUser_Id).Distinct().ToList());
                    //Predicate<string> isUpper = s => s.s;

                    //posts = ctx.Posts
                    // .Include(x => x.User)
                    // .Include(x => x.User.UserLanguageMappings)
                    // .Include(x => x.Medias)
                    // .Include(x => x.Medias.Select(y => y.MediaUserViews))
                    // .Include(x => x.PollOptions)
                    // .Include(x => x.Comments)
                    // .Include(x => x.SharedParent)
                    // .Include(x => x.SharedParent.User)
                    // //.Include(x => x.ExtendedPostList)
                    // .Where(x => x.IsDeleted == false && x.Group_Id == null &&
                    // !BlockedUsers.Contains(x.User_Id) &&
                    // !HideAllUsersIds.Contains(x.User_Id) &&
                    // !HidePostsIds.Contains(x.Id) &&
                    // !ReportPostsIds.Contains(x.Id) &&
                    // (UserInterests.Count > 0 ? x.PostCategories.Any(y => UserInterests.Contains(y.Interest_Id) && !y.IsDeleted) : true) &&
                    // //x.PostLanguageCodes.Any(y => UserLanguageIds.Contains(y.Language_Id) && !y.IsDeleted) &&
                    // ((x.Visibility == (int)PostVisibilityTypes.Public) || (x.Visibility == (int)PostVisibilityTypes.Public && x.User_Id == userId) || (x.Visibility == (int)PostVisibilityTypes.OnlyMe && x.User_Id == userId) ||
                    // (x.Visibility == (int)PostVisibilityTypes.Follower && (x.User_Id == userId || myFollowingsIds.Contains(x.User_Id)))) &&
                    // (x.IsPoll ? x.PollExpiryTime >= DateTime.UtcNow : true))
                    // .OrderByDescending(x => x.Id).ToList();

                    posts = ctx.Posts
                        .Include(x => x.User)
                        .Include(x => x.User.UserLanguageMappings)
                        .Include(x => x.Medias)
                        .Include(x => x.Medias.Select(y => y.MediaUserViews))
                        .Include(x => x.PollOptions)
                        .Include(x => x.Comments)
                        .Include(x => x.TurnOffNotifications)
                        .Include(x => x.SharedParent)
                        .Include(x => x.SharedParent.User)
                        .Include(x => x.SharedParent.Medias)
                        .Include(x => x.ExtendedPostList.Select(extendedPost => extendedPost.Medias))
                        .Where(x => x.IsDeleted == false && x.Group_Id == null && x.ParentPost_Id == null   //take all posts which are not deleted or group posts
                        && myFollowingsIds.Contains(x.User_Id)                                              //or the post is by a user which I am following
                        //|| x.Visibility == (int)PostVisibilityTypes.Public && UserInterests.Count > 0 ? x.PostCategories.Any(y => UserInterests.Contains(y.Interest_Id) && !y.IsDeleted) : true                             //or post is public and current user's interests and logged in user's interest match
                        || x.Visibility == (int)PostVisibilityTypes.Public && x.IsDeleted == false && x.Group_Id == null && x.ParentPost_Id == null && x.User.UserLanguageMappings.Any(y => UserLanguageIds.Contains(y.LanguageId))                             //or post is public and current user's lanugages and logged in user's lanugages match               userModel.UserLanguageMappings.Any(z => z.LanguageId == y.LanguageId)
                        && !BlockedUsers.Contains(x.User_Id)                                                //make sure the post is not by a user which this user has blocked
                        && !HideAllUsersIds.Contains(x.User_Id)                                             //make sure the post is by a user which is not hidden
                        && !HidePostsIds.Contains(x.Id)                                                     //make sure the post is not hidden
                        && !ReportPostsIds.Contains(x.Id)                                                   //make sure the post is not reported
                        && (x.Visibility != (int)PostVisibilityTypes.OnlyMe || x.Visibility == (int)PostVisibilityTypes.OnlyMe && x.User_Id == userId)      //post is not only me or if it is only me then this is current user's post
                        && (x.IsPoll ? x.PollExpiryTime >= DateTime.UtcNow : true))                       //if post is poll then make sure the poll is not expired
                        .OrderByDescending(x => x.Id).Skip(PageSize * PageNo).Take(PageSize).ToList();
                    //.OrderByDescending(x => x.Id).ToList();



                    //posts = posts.Skip((PageNo - 1) * PageSize).Take(PageSize).ToList();



                    //x.User.UserLanguageMappings.Any(x => userModel.UserLanguageMappings.Any(y => y.LanguageId == x.LanguageId)))

                    //posts = ctx.Posts
                    //    .Include(x => x.User)
                    //    .Include(x => x.Medias)
                    //    .Include(x => x.PollOptions)
                    //    .Include(x => x.PostCategories)
                    //    .Where(x => x.IsDeleted == false && x.Group_Id == null &&
                    //    !BlockedUsers.Contains(x.User_Id) &&
                    //    !HideAllUsersIds.Contains(x.User_Id) &&
                    //    !HidePostsIds.Contains(x.Id) &&
                    //    !ReportPostsIds.Contains(x.Id) &&
                    //    ((x.Visibility == (int)PostVisibilityTypes.Public && x.PostCategories.Any(y => UserInterests.Contains(y.Interest_Id))) || (x.Visibility == (int)PostVisibilityTypes.Public && x.User_Id == userId) || (x.Visibility == (int)PostVisibilityTypes.OnlyMe && x.User_Id == userId) ||
                    //    (x.Visibility == (int)PostVisibilityTypes.Follower && (x.User_Id == userId || myFollowingsIds.Contains(x.User_Id)))))
                    //    .OrderByDescending(x => x.Id)
                    //    .ToList();



                    //             posts = posts.Skip((PageNo -1) * PageSize).Take(PageSize).ToList();

                    foreach (Post post in posts)
                    {
                        //if count is zero then noti is on for this post
                        if (post.TurnOffNotifications.Count == 0)
                        {
                            post.IsNotificationOn = true;
                        }
                        else if (post.TurnOffNotifications.Count > 0)
                        {
                            //if TurnOffNotification is exists then notis are on
                            if (post.TurnOffNotifications.FirstOrDefault().IsDeleted == false)
                            {
                                post.IsNotificationOn = false;
                            }
                            else if (post.TurnOffNotifications.FirstOrDefault().IsDeleted == true)                            //if TurnOffNotification is deleted then notis are off
                            {
                                post.IsNotificationOn = true;
                            }
                        }

                        if (post.User == null)
                            post.User = ctx.Users.FirstOrDefault(x => x.Id == post.User_Id);

                        if (post.IsPoll)
                            post.CheckPollExpiry();

                        foreach (var item in post.PollOptions)
                        {
                            if (ctx.PollOptionVote.Any(x => x.PollOption_Id == item.Id && x.User_Id == userId && !x.IsDeleted))
                                item.IsVoted = true;
                        }

                        if (post.SharedParent != null)
                            post.SharedParent.IsUserFollow = ctx.FollowFollowers.Any(x => x.FirstUser_Id == userId && x.SecondUser_Id == post.SharedParent.User_Id && x.IsDeleted == false);


                        if (BlockedUsers.Contains(post.User_Id))        //not required, this was alread checked when the posts were fetched from database
                        {
                            posts.Remove(post);
                        }
                        else
                        {
                            post.IsLiked = ctx.Likes.Any(x => x.Post_Id == post.Id && x.User_Id == userId && x.IsDeleted == false);
                            post.HasComment = ctx.Comments.Any(x => x.Post_Id == post.Id && x.User_Id == userId && x.IsDeleted == false);
                            post.LikesCount = ctx.Posts.Sum(p => p.Likes.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                            post.CommentsCount = ctx.Posts.Sum(p => p.Comments.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                            post.ShareCount = ctx.Posts.Sum(p => p.Shares.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                            post.IsUserFollow = ctx.FollowFollowers.Any(x => x.FirstUser_Id == userId && x.SecondUser_Id == post.User_Id && x.IsDeleted == false);
                            post.IsPostOwner = (post.User_Id == userId) ? true : false;

                            foreach (var extendedPost in post.ExtendedPostList)
                            {
                                extendedPost.IsLiked = ctx.Likes.Any(x => x.Post_Id == extendedPost.Id && x.User_Id == userId && x.IsDeleted == false);
                                extendedPost.HasComment = ctx.Comments.Any(x => x.Post_Id == extendedPost.Id && x.User_Id == userId && x.IsDeleted == false);
                                extendedPost.LikesCount = ctx.Posts.Sum(p => p.Likes.Where(x => x.Post_Id == extendedPost.Id && x.IsDeleted == false).Count());
                                extendedPost.CommentsCount = ctx.Posts.Sum(p => p.Comments.Where(x => x.Post_Id == extendedPost.Id && x.IsDeleted == false).Count());
                                extendedPost.ShareCount = ctx.Posts.Sum(p => p.Shares.Where(x => x.Post_Id == extendedPost.Id && x.IsDeleted == false).Count());
                                extendedPost.IsUserFollow = ctx.FollowFollowers.Any(x => x.FirstUser_Id == userId && x.SecondUser_Id == extendedPost.User_Id && x.IsDeleted == false);
                            }

                            //if this post is a shared post
                            if (post.SharedParent != null)
                            {
                                post.IsShared = (post.SharedParent.User_Id == userId) ? true : false;
                            }
                        }

                        //int count = post.LikesCount + post.CommentsCount + post.ShareCount;


                        post.Medias = post.Medias.Where(x => x.Comment_Id == null).ToList();

                        //if(!string.IsNullOrEmpty(post.Language))
                        //{
                        //    if(!UserLanguages.Any(x => post.Language.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Contains(x)))
                        //    {
                        //        posts.Remove(post);
                        //    }
                        //}
                        //else
                        //{
                        //    posts.Remove(post);
                        //}
                        //if (!string.IsNullOrEmpty(post.Language))
                        //{
                        //    var langCodes = post.Language.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x).ToList();
                        //    if (langCodes.Any(x => userlangCodes.Contains(x)))
                        //    {
                        //        postIds.Add(post.Id);
                        //    }
                        //    //langCodes
                        //}
                    }

                    //posts = posts.Where(x => postIds.Contains(x.Id)).ToList();

                    int PostCount = posts.Count();
                    //posts = posts.Skip((PageNo - 1) * PageSize).Take(PageSize).ToList();

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
        [Route("GetPostsByLanguages")]
        public async Task<IHttpActionResult> GetPostsByLanguages(string strLanguages = "")
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                    List<int> UserInterests = new List<int>();
                    List<int> UserLanguageIds = new List<int>();
                    List<string> userlangCodes = new List<string>();
                    List<int> postIds = new List<int>();

                    List<Post> posts = new List<Post>();

                    // Hide All Post Users

                    var HideAllUsersIds = ctx.HideAllPosts.Where(x => x.FirstUserAll_Id == userId && x.IsDeleted == false).Select(x => x.SecondUserAll_Id).Distinct().ToList();

                    var HidePostsIds = ctx.HidePosts.Where(x => x.FirstUser_Id == userId && x.IsDeleted == false).Select(x => x.Post_Id).Distinct().ToList();

                    var ReportPostsIds = ctx.ReportPosts.Where(x => x.User_Id == userId && x.IsDeleted == false).Select(x => x.Post_Id).Distinct().ToList();

                    var userModel = ctx.Users.FirstOrDefault(x => x.Id == userId);
                    if (userModel != null)
                    {
                        if (!string.IsNullOrEmpty(userModel.Interests))
                        {
                            UserInterests = userModel.Interests.Split(',').Select(int.Parse).ToList();
                        }

                        //if (!string.IsNullOrEmpty(userModel.Language))
                        //{
                        //    userlangCodes = userModel.Language.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x).ToList();
                        //}
                    }

                    userlangCodes = strLanguages.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x).ToList();

                    List<FollowFollower> followFollowers = new List<FollowFollower>();

                    var myFollowingsIds = ctx.FollowFollowers
                        .Where(x => x.IsDeleted == false && x.FirstUser_Id == userId)
                        .Select(x => x.SecondUser_Id).Distinct()
                        .ToList();

                    var BlockedUsers = ctx.BlockUsers.Where(x => x.FirstUser_Id == userId && !x.IsDeleted).Select(x => x.SecondUser_Id).Distinct().ToList();

                    BlockedUsers.AddRange(ctx.BlockUsers.Where(x => x.SecondUser_Id == userId && !x.IsDeleted).Select(x => x.FirstUser_Id).Distinct().ToList());
                    //Predicate<string> isUpper = s => s.s;

                    posts = ctx.Posts
                     .Include(x => x.User)
                     .Include(x => x.Medias)
                     .Include(x => x.Medias.Select(y => y.MediaUserViews))
                     .Include(x => x.PollOptions)
                     .Include(x => x.Comments)
                     .Include(x => x.SharedParent)
                     .Include(x => x.SharedParent.User)
                     .Include(x => x.ExtendedPostList)
                     .Where(x => x.IsDeleted == false && x.Group_Id == null &&
                     !BlockedUsers.Contains(x.User_Id) &&
                     !HideAllUsersIds.Contains(x.User_Id) &&
                     !HidePostsIds.Contains(x.Id) &&
                     !ReportPostsIds.Contains(x.Id) &&
                     (UserInterests.Count > 0 ? x.PostCategories.Any(y => UserInterests.Contains(y.Interest_Id) && !y.IsDeleted) : true) &&
                     //x.PostLanguageCodes.Any(y => UserLanguageIds.Contains(y.Language_Id) && !y.IsDeleted) &&
                     ((x.Visibility == (int)PostVisibilityTypes.Public) || (x.Visibility == (int)PostVisibilityTypes.Public && x.User_Id == userId) || (x.Visibility == (int)PostVisibilityTypes.OnlyMe && x.User_Id == userId) ||
                     (x.Visibility == (int)PostVisibilityTypes.Follower && (x.User_Id == userId || myFollowingsIds.Contains(x.User_Id))))
                     && (x.IsPoll ? x.PollExpiryTime >= DateTime.UtcNow : true))
                     .OrderByDescending(x => x.Id).ToList();

                    //posts = ctx.Posts
                    //    .Include(x => x.User)
                    //    .Include(x => x.Medias)
                    //    .Include(x => x.PollOptions)
                    //    .Include(x => x.PostCategories)
                    //    .Where(x => x.IsDeleted == false && x.Group_Id == null &&
                    //    !BlockedUsers.Contains(x.User_Id) &&
                    //    !HideAllUsersIds.Contains(x.User_Id) &&
                    //    !HidePostsIds.Contains(x.Id) &&
                    //    !ReportPostsIds.Contains(x.Id) &&
                    //    ((x.Visibility == (int)PostVisibilityTypes.Public && x.PostCategories.Any(y => UserInterests.Contains(y.Interest_Id))) || (x.Visibility == (int)PostVisibilityTypes.Public && x.User_Id == userId) || (x.Visibility == (int)PostVisibilityTypes.OnlyMe && x.User_Id == userId) ||
                    //    (x.Visibility == (int)PostVisibilityTypes.Follower && (x.User_Id == userId || myFollowingsIds.Contains(x.User_Id)))))
                    //    .OrderByDescending(x => x.Id)
                    //    .ToList();



                    //             posts = posts.Skip((PageNo -1) * PageSize).Take(PageSize).ToList();

                    foreach (Post post in posts)
                    {

                        if (post.User == null)
                            post.User = ctx.Users.FirstOrDefault(x => x.Id == post.User_Id);

                        if (post.IsPoll)
                            post.CheckPollExpiry();

                        foreach (var item in post.PollOptions)
                        {
                            if (ctx.PollOptionVote.Any(x => x.PollOption_Id == item.Id && x.User_Id == userId && !x.IsDeleted))
                                item.IsVoted = true;
                        }

                        if (post.SharedParent != null)
                            post.SharedParent.IsUserFollow = ctx.FollowFollowers.Any(x => x.FirstUser_Id == userId && x.SecondUser_Id == post.SharedParent.User_Id && x.IsDeleted == false);


                        if (BlockedUsers.Contains(post.User_Id))
                        {
                            posts.Remove(post);
                        }
                        else
                        {
                            post.IsLiked = ctx.Likes.Any(x => x.Post_Id == post.Id && x.User_Id == userId && x.IsDeleted == false);
                            post.LikesCount = ctx.Posts.Sum(p => p.Likes.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                            post.CommentsCount = ctx.Posts.Sum(p => p.Comments.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                            post.ShareCount = ctx.Posts.Sum(p => p.Shares.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                            post.IsUserFollow = ctx.FollowFollowers.Any(x => x.FirstUser_Id == userId && x.SecondUser_Id == post.User_Id && x.IsDeleted == false);
                            post.IsPostOwner = (post.User_Id == userId) ? true : false;
                            post.HasComment = ctx.Comments.Any(x => x.Post_Id == post.Id && x.User_Id == userId && x.IsDeleted == false);

                            //if this post is a shared post
                            if (post.SharedParent != null)
                            {
                                post.IsShared = (post.SharedParent.User_Id == userId) ? true : false;
                            }
                        }

                        //int count = post.LikesCount + post.CommentsCount + post.ShareCount;


                        post.Medias = post.Medias.Where(x => x.Comment_Id == null).ToList();

                        //if(!string.IsNullOrEmpty(post.Language))
                        //{
                        //    if(!UserLanguages.Any(x => post.Language.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Contains(x)))
                        //    {
                        //        posts.Remove(post);
                        //    }
                        //}
                        //else
                        //{
                        //    posts.Remove(post);
                        //}
                        if (!string.IsNullOrEmpty(post.Language))
                        {
                            var langCodes = post.Language.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x).ToList();
                            if (langCodes.Any(x => userlangCodes.Contains(x)))
                            {
                                postIds.Add(post.Id);
                            }
                            //langCodes
                        }
                    }

                    posts = posts.Where(x => x.IsPoll ? x.PollExpiryTime >= DateTime.UtcNow : true).ToList();
                    posts = posts.Where(x => postIds.Contains(x.Id)).ToList();
                    int PostCount = posts.Count();
                    //posts = posts.Skip((PageNo -1) * PageSize).Take(PageSize).ToList();

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
        [Route("GetPostsByUserId")]
        public async Task<IHttpActionResult> GetPostsByUserId(int User_Id, int PageSize = int.MaxValue, int PageNo = 0)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                using (RiscoContext ctx = new RiscoContext())
                {
                    List<Post> posts = new List<Post>();

                    var myFollowingsIds = ctx.FollowFollowers
                       .Where(x => x.IsDeleted == false && x.FirstUser_Id == userId)
                       .Select(x => x.SecondUser_Id).Distinct()
                       .ToList();

                    posts = ctx.Posts
                        .Include(x => x.User)
                        .Include(x => x.PollOptions)
                        .Include(x => x.Medias)
                        .Include(x => x.Medias.Select(y => y.MediaUserViews))
                        .Include(x => x.Comments)
                        .Include(x => x.TurnOffNotifications)
                        .Include(x => x.SharedParent)
                        .Include(x => x.SharedParent.User)
                        .Include(x => x.SharedParent.Medias)
                        .Include(x => x.ExtendedPostList.Select(extendedPost => extendedPost.Medias))
                        .Where(x => x.User_Id == User_Id && x.IsDeleted == false && x.Group_Id == null && x.ParentPost_Id == null &&
                        ((x.Visibility == (int)PostVisibilityTypes.Public) ||
                        (x.Visibility == (int)PostVisibilityTypes.Follower && (x.User_Id == userId || myFollowingsIds.Contains(x.User_Id))))
                        ).OrderByDescending(x => x.Id)
                        .ToList();

                    int PostCount = posts.Count();
                    posts = posts.Skip((PageNo - 1) * PageSize).Take(PageSize).ToList();

                    foreach (Post post in posts)
                    {
                        //if count is zero then noti is on for this post
                        if (post.TurnOffNotifications.Count == 0)
                        {
                            post.IsNotificationOn = true;
                        }
                        else if (post.TurnOffNotifications.Count > 0)
                        {
                            //if TurnOffNotification is exists then notis are on
                            if (post.TurnOffNotifications.FirstOrDefault().IsDeleted == false)
                            {
                                post.IsNotificationOn = false;
                            }
                            else if (post.TurnOffNotifications.FirstOrDefault().IsDeleted == true)                            //if TurnOffNotification is deleted then notis are off
                            {
                                post.IsNotificationOn = true;
                            }
                        }

                        if (post.User == null)
                            post.User = ctx.Users.FirstOrDefault(x => x.Id == post.User_Id);
                        if (post.SharedParent != null)
                        {
                            post.SharedParent.Medias = post.SharedParent.Medias.Where(x => x.Comment_Id == null).ToList();
                            foreach (var item in post.SharedParent.Medias)
                            {
                                if (item.User != null)
                                    item.User = null;
                            }
                        }
                        if (post.IsPoll)
                        {
                            post.CheckPollExpiry();
                            var vote = ctx.PollOptionVote.FirstOrDefault(x => x.User_Id == userId && x.Post_Id == post.Id);
                            if (vote != null)
                                post.PollOptions.FirstOrDefault(x => x.Id == vote.PollOption_Id).IsVoted = true;
                        }

                        post.Medias = post.Medias.Where(x => x.Comment_Id == null).ToList();

                        post.IsLiked = ctx.Likes.Any(x => x.Post_Id == post.Id && x.User_Id == userId && x.IsDeleted == false);
                        post.LikesCount = ctx.Posts.Sum(p => p.Likes.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.CommentsCount = ctx.Posts.Sum(p => p.Comments.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.ShareCount = ctx.Posts.Sum(p => p.Shares.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.IsUserFollow = true;
                        post.IsPostOwner = (post.User_Id == userId) ? true : false;
                        post.HasComment = ctx.Comments.Any(x => x.Post_Id == post.Id && x.User_Id == userId && x.IsDeleted == false);

                        //if this post is a shared post
                        if (post.SharedParent != null)
                        {
                            post.IsShared = (post.SharedParent.User_Id == userId) ? true : false;
                        }
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
        [Route("GetPostsDetailByUserId")]
        public async Task<IHttpActionResult> GetPostsDetailByUserId(int User_Id, int PageSize = 20, int PageNo = 0)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                using (RiscoContext ctx = new RiscoContext())
                {
                    List<Post> posts = new List<Post>();

                    var myFollowingsIds = ctx.FollowFollowers
                       .Where(x => x.IsDeleted == false && x.FirstUser_Id == userId)
                       .Select(x => x.SecondUser_Id).Distinct()
                       .ToList();

                    posts = ctx.Posts
                        .Include(x => x.User)
                        .Include(x => x.PollOptions)
                        .Include(x => x.Medias)
                        .Include(x => x.Medias.Select(y => y.MediaUserViews))
                        .Include(x => x.Comments)
                        .Include(x => x.TurnOffNotifications)
                        .Include(x => x.SharedParent)
                        .Include(x => x.SharedParent.User)
                        .Include(x => x.SharedParent.Medias)
                        .Include(x => x.ExtendedPostList.Select(extendedPost => extendedPost.Medias))
                        .Where(x => x.User_Id == User_Id && x.IsDeleted == false && x.Group_Id == null && x.PostType == 1 && x.ParentPost_Id == null &&
                        ((x.Visibility == (int)PostVisibilityTypes.Public) || (x.Visibility == (int)PostVisibilityTypes.OnlyMe && x.User_Id == userId) ||
                        (x.Visibility == (int)PostVisibilityTypes.Follower && (x.User_Id == userId || myFollowingsIds.Contains(x.User_Id))))
                        ).OrderByDescending(x => x.Id).Skip(PageSize * PageNo).Take(PageSize).ToList();

                    int PostCount = posts.Count();
                    //posts = posts.Skip((PageNo - 1) * PageSize).Take(PageSize).ToList();

                    foreach (Post post in posts)
                    {
                        //if count is zero then noti is on for this post
                        if (post.TurnOffNotifications.Count == 0)
                        {
                            post.IsNotificationOn = true;
                        }
                        else if (post.TurnOffNotifications.Count > 0)
                        {
                            //if TurnOffNotification is exists then notis are on
                            if (post.TurnOffNotifications.FirstOrDefault().IsDeleted == false)
                            {
                                post.IsNotificationOn = false;
                            }
                            else if (post.TurnOffNotifications.FirstOrDefault().IsDeleted == true)                            //if TurnOffNotification is deleted then notis are off
                            {
                                post.IsNotificationOn = true;
                            }
                        }

                        if (post.User == null)
                            post.User = ctx.Users.FirstOrDefault(x => x.Id == post.User_Id);
                        if (post.SharedParent != null)
                        {
                            post.SharedParent.Medias = post.SharedParent.Medias.Where(x => x.Comment_Id == null).ToList();
                            foreach (var item in post.SharedParent.Medias)
                            {
                                if (item.User != null)
                                    item.User = null;
                            }
                        }
                        if (post.IsPoll)
                        {
                            post.CheckPollExpiry();
                            var vote = ctx.PollOptionVote.FirstOrDefault(x => x.User_Id == userId && x.Post_Id == post.Id);
                            if (vote != null)
                                post.PollOptions.FirstOrDefault(x => x.Id == vote.PollOption_Id).IsVoted = true;
                        }

                        post.Medias = post.Medias.Where(x => x.Comment_Id == null).ToList();

                        post.IsLiked = ctx.Likes.Any(x => x.Post_Id == post.Id && x.User_Id == userId && x.IsDeleted == false);
                        post.LikesCount = ctx.Posts.Sum(p => p.Likes.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.CommentsCount = ctx.Posts.Sum(p => p.Comments.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.ShareCount = ctx.Posts.Sum(p => p.Shares.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.IsUserFollow = true;
                        post.IsPostOwner = (post.User_Id == userId) ? true : false;
                        post.HasComment = ctx.Comments.Any(x => x.Post_Id == post.Id && x.User_Id == userId && x.IsDeleted == false);

                        //if this post is a shared post
                        if (post.SharedParent != null)
                        {
                            post.IsShared = (post.SharedParent.User_Id == userId) ? true : false;
                        }
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
        [Route("GetIncidentsDetailByUserId")]
        public async Task<IHttpActionResult> GetIncidentsDetailByUserId(int User_Id, int PageSize = int.MaxValue, int PageNo = 0)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                using (RiscoContext ctx = new RiscoContext())
                {
                    List<Post> posts = new List<Post>();

                    var myFollowingsIds = ctx.FollowFollowers
                       .Where(x => x.IsDeleted == false && x.FirstUser_Id == userId)
                       .Select(x => x.SecondUser_Id).Distinct()
                       .ToList();

                    posts = ctx.Posts
                        .Include(x => x.User)
                        .Include(x => x.PollOptions)
                        .Include(x => x.Medias)
                        .Include(x => x.Medias.Select(y => y.MediaUserViews))
                        .Include(x => x.Comments)
                        .Include(x => x.TurnOffNotifications)
                        .Include(x => x.SharedParent)
                        .Include(x => x.SharedParent.User)
                        .Include(x => x.SharedParent.Medias)
                        .Include(x => x.ExtendedPostList)
                        .Where(x => x.User_Id == User_Id && x.IsDeleted == false && x.Group_Id == null && x.PostType == 2 &&
                        ((x.Visibility == (int)PostVisibilityTypes.Public) ||
                        (x.Visibility == (int)PostVisibilityTypes.Follower && (x.User_Id == userId || myFollowingsIds.Contains(x.User_Id))))
                        ).OrderByDescending(x => x.Id).Skip(PageSize * PageNo).Take(PageSize)
                        .ToList();

                    int PostCount = posts.Count();
                    //posts = posts.Skip((PageNo - 1) * PageSize).Take(PageSize).ToList();

                    foreach (Post post in posts)
                    {
                        //if count is zero then noti is on for this post
                        if (post.TurnOffNotifications.Count == 0)
                        {
                            post.IsNotificationOn = true;
                        }
                        else if (post.TurnOffNotifications.Count > 0)
                        {
                            //if TurnOffNotification is exists then notis are on
                            if (post.TurnOffNotifications.FirstOrDefault().IsDeleted == false)
                            {
                                post.IsNotificationOn = false;
                            }
                            else if (post.TurnOffNotifications.FirstOrDefault().IsDeleted == true)                            //if TurnOffNotification is deleted then notis are off
                            {
                                post.IsNotificationOn = true;
                            }
                        }

                        if (post.User == null)
                            post.User = ctx.Users.FirstOrDefault(x => x.Id == post.User_Id);
                        if (post.SharedParent != null)
                        {
                            post.SharedParent.Medias = post.SharedParent.Medias.Where(x => x.Comment_Id == null).ToList();
                            foreach (var item in post.SharedParent.Medias)
                            {
                                if (item.User != null)
                                    item.User = null;
                            }
                        }
                        if (post.IsPoll)
                        {
                            post.CheckPollExpiry();
                            var vote = ctx.PollOptionVote.FirstOrDefault(x => x.User_Id == userId && x.Post_Id == post.Id);
                            if (vote != null)
                                post.PollOptions.FirstOrDefault(x => x.Id == vote.PollOption_Id).IsVoted = true;
                        }

                        post.Medias = post.Medias.Where(x => x.Comment_Id == null).ToList();

                        post.IsLiked = ctx.Likes.Any(x => x.Post_Id == post.Id && x.User_Id == userId && x.IsDeleted == false);
                        post.LikesCount = ctx.Posts.Sum(p => p.Likes.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.CommentsCount = ctx.Posts.Sum(p => p.Comments.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.ShareCount = ctx.Posts.Sum(p => p.Shares.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.IsUserFollow = true;
                        post.IsPostOwner = (post.User_Id == userId) ? true : false;
                        post.HasComment = ctx.Comments.Any(x => x.Post_Id == post.Id && x.User_Id == userId && x.IsDeleted == false);

                        //if this post is a shared post
                        if (post.SharedParent != null)
                        {
                            post.IsShared = (post.SharedParent.User_Id == userId) ? true : false;
                        }
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
        [Route("GetLikesPostDetailByUserId")]
        public async Task<IHttpActionResult> GetLikesPostDetailByUserId(int User_Id, int PageSize = int.MaxValue, int PageNo = 0)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                using (RiscoContext ctx = new RiscoContext())
                {
                    List<Post> posts = new List<Post>();

                    var myFollowingsIds = ctx.FollowFollowers
                       .Where(x => x.IsDeleted == false && x.FirstUser_Id == userId)
                       .Select(x => x.SecondUser_Id).Distinct()
                       .ToList();

                    posts =
                        ctx.Likes.Where(x => x.User_Id == User_Id && x.IsDeleted == false)
                        .Include(x => x.Post)
                        .Select(x => x.Post).Distinct()
                        .Include(x => x.User)
                        .Include(x => x.PollOptions)
                        .Include(x => x.Medias)
                        .Include(x => x.Medias.Select(y => y.MediaUserViews))
                        .Include(x => x.Comments)
                        .Include(x => x.SharedParent)
                        .Include(x => x.SharedParent.User)
                        .Include(x => x.SharedParent.Medias)
                        .Include(x => x.ExtendedPostList)
                        .Where(x => x.IsDeleted == false && x.Group_Id == null &&
                        ((x.Visibility == (int)PostVisibilityTypes.Public) ||
                        (x.Visibility == (int)PostVisibilityTypes.Follower && myFollowingsIds.Contains(x.User_Id)))
                        ).OrderByDescending(x => x.Id)
                        .ToList();

                    int PostCount = posts.Count();
                    posts = posts.Skip((PageNo - 1) * PageSize).Take(PageSize).ToList();

                    foreach (Post post in posts)
                    {
                        if (post.User == null)
                            post.User = ctx.Users.FirstOrDefault(x => x.Id == post.User_Id);
                        if (post.SharedParent != null)
                        {
                            post.SharedParent.Medias = post.SharedParent.Medias.Where(x => x.Comment_Id == null).ToList();
                            foreach (var item in post.SharedParent.Medias)
                            {
                                if (item.User != null)
                                    item.User = null;
                            }
                        }
                        if (post.IsPoll)
                        {
                            post.CheckPollExpiry();
                            var vote = ctx.PollOptionVote.FirstOrDefault(x => x.User_Id == userId && x.Post_Id == post.Id);
                            if (vote != null)
                                post.PollOptions.FirstOrDefault(x => x.Id == vote.PollOption_Id).IsVoted = true;
                        }

                        post.Medias = post.Medias.Where(x => x.Comment_Id == null).ToList();

                        post.IsLiked = ctx.Likes.Any(x => x.Post_Id == post.Id && x.User_Id == userId && x.IsDeleted == false);
                        post.LikesCount = ctx.Posts.Sum(p => p.Likes.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.CommentsCount = ctx.Posts.Sum(p => p.Comments.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.ShareCount = ctx.Posts.Sum(p => p.Shares.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.IsUserFollow = true;
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
        [Route("GetMediasPostDetailByUserId")]
        public async Task<IHttpActionResult> GetMediasPostDetailByUserId(int User_Id, int PageSize = int.MaxValue, int PageNo = 0)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                using (RiscoContext ctx = new RiscoContext())
                {
                    List<Post> posts = new List<Post>();

                    var myFollowingsIds = ctx.FollowFollowers
                       .Where(x => x.IsDeleted == false && x.FirstUser_Id == userId)
                       .Select(x => x.SecondUser_Id).Distinct()
                       .ToList();

                    posts =
                        ctx.Medias.Where(x => x.User_Id == User_Id && x.IsDeleted == false)
                                        .Include(x => x.Post)
                                        .Select(x => x.Post).Distinct()
                        .Include(x => x.User)
                        .Include(x => x.PollOptions)
                        .Include(x => x.Medias)
                        .Include(x => x.Medias.Select(y => y.MediaUserViews))
                        .Include(x => x.Comments)
                        .Include(x => x.SharedParent)
                        .Include(x => x.SharedParent.User)
                        .Include(x => x.SharedParent.Medias)
                        .Include(x => x.ExtendedPostList)
                        .Where(x => x.User_Id == User_Id && x.IsDeleted == false && x.Group_Id == null &&
                        ((x.Visibility == (int)PostVisibilityTypes.Public) ||
                        (x.Visibility == (int)PostVisibilityTypes.Follower && (x.User_Id == userId || myFollowingsIds.Contains(x.User_Id))))
                        ).OrderByDescending(x => x.Id)
                        .ToList();

                    int PostCount = posts.Count();
                    posts = posts.Skip((PageNo - 1) * PageSize).Take(PageSize).ToList();

                    foreach (Post post in posts)
                    {
                        if (post.User == null)
                            post.User = ctx.Users.FirstOrDefault(x => x.Id == post.User_Id);
                        if (post.SharedParent != null)
                        {
                            post.SharedParent.Medias = post.SharedParent.Medias.Where(x => x.Comment_Id == null).ToList();
                            foreach (var item in post.SharedParent.Medias)
                            {
                                if (item.User != null)
                                    item.User = null;
                            }
                        }
                        if (post.IsPoll)
                        {
                            post.CheckPollExpiry();
                            var vote = ctx.PollOptionVote.FirstOrDefault(x => x.User_Id == userId && x.Post_Id == post.Id);
                            if (vote != null)
                                post.PollOptions.FirstOrDefault(x => x.Id == vote.PollOption_Id).IsVoted = true;
                        }

                        post.Medias = post.Medias.Where(x => x.Comment_Id == null).ToList();

                        post.IsLiked = ctx.Likes.Any(x => x.Post_Id == post.Id && x.User_Id == userId && x.IsDeleted == false);
                        post.LikesCount = ctx.Posts.Sum(p => p.Likes.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.CommentsCount = ctx.Posts.Sum(p => p.Comments.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.ShareCount = ctx.Posts.Sum(p => p.Shares.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.IsUserFollow = true;
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
        [Route("GetRepliesPostDetailByUserId")]
        public async Task<IHttpActionResult> GetRepliesPostDetailByUserId(int User_Id, int PageSize = int.MaxValue, int PageNo = 0)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                List<Comment> comments = new List<Comment>();
                List<Comment> childcomments = new List<Comment>();
                using (RiscoContext ctx = new RiscoContext())
                {
                    List<Post> posts = new List<Post>();

                    var myFollowingsIds = ctx.FollowFollowers
                       .Where(x => x.IsDeleted == false && x.FirstUser_Id == userId)
                       .Select(x => x.SecondUser_Id).Distinct()
                       .ToList();

                    //comments = ctx.Comments.Where(x => x.User_Id != User_Id && x.IsDeleted == false).ToList();
                    //childcomments = ctx.Comments.Where(x => x.User_Id == User_Id && x.IsDeleted == false).ToList();
                    //foreach (Comment comment in comments)
                    //{
                    //    comment.ChildComments = ctx.Comments
                    //        .Where(x => x.ParentComment_Id == comment.Id && x.User_Id == User_Id && x.IsDeleted == false).ToList();
                    //}
                    //foreach (Comment comment in childcomments)
                    //{
                    //    comment.ChildComments = ctx.Comments
                    //        .Where(x => x.ParentComment_Id == comment.Id && x.User_Id == User_Id && x.IsDeleted == false).ToList();
                    //}
                    //comments = comments.Concat(childcomments).ToList();
                    //List<int?> lstIds = new List<int?>();
                    //lstIds.AddRange(comments.Select(x => x.Post_Id).ToList());
                    //foreach (Comment com in comments)
                    //{
                    //    lstIds.AddRange(com.ChildComments.Select(x => x.Post_Id).ToList());
                    //}

                    //lstIds = lstIds.Distinct().ToList();                    

                    posts =
                        ctx.Comments.Where(x => x.User_Id == User_Id && x.IsDeleted == false)
                                        .Include(x => x.Post)
                                        .Select(x => x.Post).Distinct()
                        .Include(x => x.User)
                        .Include(x => x.PollOptions)
                        .Include(x => x.Medias)
                        .Include(x => x.Medias.Select(y => y.MediaUserViews))
                        .Include(x => x.SharedParent)
                        .Include(x => x.SharedParent.User)
                        .Include(x => x.SharedParent.Medias)
                        .Include(x => x.ExtendedPostList)
                        .Where(x => x.IsDeleted == false && x.Group_Id == null &&
                        ((x.Visibility == (int)PostVisibilityTypes.Public) ||
                        (x.Visibility == (int)PostVisibilityTypes.Follower && myFollowingsIds.Contains(x.User_Id)))
                        ).OrderByDescending(x => x.Id)
                        .ToList();

                    int PostCount = posts.Count();
                    posts = posts.Skip((PageNo - 1) * PageSize).Take(PageSize).ToList();

                    foreach (Post post in posts)
                    {
                        if (post.User == null)
                            post.User = ctx.Users.FirstOrDefault(x => x.Id == post.User_Id);
                        if (post.SharedParent != null)
                        {
                            post.SharedParent.Medias = post.SharedParent.Medias.Where(x => x.Comment_Id == null).ToList();
                            foreach (var item in post.SharedParent.Medias)
                            {
                                if (item.User != null)
                                    item.User = null;
                            }
                        }
                        if (post.IsPoll)
                        {
                            post.CheckPollExpiry();
                            var vote = ctx.PollOptionVote.FirstOrDefault(x => x.User_Id == userId && x.Post_Id == post.Id);
                            if (vote != null)
                                post.PollOptions.FirstOrDefault(x => x.Id == vote.PollOption_Id).IsVoted = true;
                        }

                        post.Medias = post.Medias.Where(x => x.Comment_Id == null).ToList();

                        post.IsLiked = ctx.Likes.Any(x => x.Post_Id == post.Id && x.User_Id == userId && x.IsDeleted == false);
                        post.LikesCount = ctx.Posts.Sum(p => p.Likes.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.CommentsCount = ctx.Posts.Sum(p => p.Comments.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.ShareCount = ctx.Posts.Sum(p => p.Shares.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                        post.IsUserFollow = true;
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
        [Route("GetPostByPostId")]
        public async Task<IHttpActionResult> GetPostByPostId(int Post_Id)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {

                    Post post = new Post();
                    post = ctx.Posts
                        .Include(x => x.User)
                        .Include(x => x.Medias)
                        .Include(x => x.Medias.Select(y => y.MediaUserViews))
                        .Include(x => x.PollOptions)
                        .Include(x => x.Comments)
                        .Include(x => x.SharedParent)
                        .Include(x => x.SharedParent.User)
                        .Include(x => x.SharedParent.Medias)
                        .Include(x => x.SharedParent.Comments)
                        .Include(x => x.SharedParent.Medias.Select(y => y.MediaUserViews))
                        .Include(x => x.SharedParent.PollOptions)
                        .Include(x => x.ExtendedPostList)
                        .FirstOrDefault(x => x.Id == Post_Id && x.IsDeleted == false);

                    //if (post.SharedParent != null)
                    //{
                    //    post.SharedParent.Medias = post.SharedParent.Medias.Where(x => x.Comment_Id == null).ToList();                        
                    //    foreach (var item in post.SharedParent.Medias)
                    //    {
                    //        if (item.User != null)
                    //            item.User = null;
                    //    }
                    //}

                    if (post != null)
                    {
                        post = includePostDetail(ctx, post);
                        if (post.SharedParent != null)
                            post.SharedParent = includePostDetail(ctx, post.SharedParent);

                        CustomResponse<Post> response = new CustomResponse<Post>
                        {
                            Message = Global.ResponseMessages.Success,
                            StatusCode = (int)HttpStatusCode.OK,
                            Result = post
                        };
                        return Ok(response);
                    }
                    else
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "NotFound",
                            StatusCode = (int)HttpStatusCode.NotFound,
                            Result = new Error { ErrorMessage = "This content is currently unavailable." }
                        });
                    }
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
        [Route("LikePost")]
        public async Task<IHttpActionResult> LikePost(int Post_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                //List<UserDevice> MyFollowingsDevices = new List<UserDevice>();
                //List<Notification> NotifyMyFollowings = new List<Notification>();
                Like like = new Like();


                using (RiscoContext ctx = new RiscoContext())
                {

                    var existingLike = ctx.Likes.FirstOrDefault(x => x.Post_Id == Post_Id && x.User_Id == userId && x.IsDeleted);

                    if (existingLike != null)
                    {
                        existingLike.IsDeleted = false;
                        existingLike.CreatedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        like = new Like
                        {
                            Post_Id = Post_Id,
                            User_Id = userId,
                            CreatedDate = DateTime.UtcNow
                        };
                        ctx.Likes.Add(like);
                    }

                    // code for risk level --- START ---
                    Post post = ctx.Posts.Where(x => x.Id == Post_Id && !x.IsDeleted).First();
                    if (post.PostType == (int)PostTypes.Incident)
                    {
                        List<RiskLevelRange> RiskLevelRanges = ctx.RiskLevelRange.Where(x => !x.IsDeleted).ToList();
                        var LikesCount = ctx.Likes.Where(x => x.Post_Id == Post_Id && x.IsDeleted == false).Count();
                        var CommentsCount = ctx.Comments.Where(x => x.Post_Id == Post_Id && x.IsDeleted == false).Count();
                        var ShareCount = ctx.Shares.Where(x => x.Post_Id == Post_Id && x.IsDeleted == false).Count();
                        int total = LikesCount + CommentsCount + ShareCount + 1;

                        if (RiskLevelRanges.Where(x => x.From <= total && x.Text.ToLower() == "high").Count() > 0)
                        {
                            post.RiskLevel = (int)RiskLevelTypes.High;
                        }
                        else if (RiskLevelRanges.Where(x => x.From <= total && x.To >= total && x.Text.ToLower() == "medium").Count() > 0)
                        {
                            post.RiskLevel = (int)RiskLevelTypes.Medium;
                        }
                        else
                        {
                            post.RiskLevel = (int)RiskLevelTypes.Low;
                        }
                    }
                    // code for risk level --- END ---

                    ctx.SaveChanges();

                    //Dictionary<string, int> MyFollowings = new Dictionary<string, int>();
                    if (existingLike == null)
                    {
                        //var post = ctx.Posts.Include(x => x.User).FirstOrDefault(x => x.Id == Post_Id);
                        //var MyFollowers = ctx.FollowFollowers.Include(x => x.SecondUser).Include(x => x.FirstUser).Where(x => x.SecondUser_Id == userId).ToList();

                        //if (MyFollowers != null)
                        //{
                        //    foreach (var SingleFollower in MyFollowers)
                        //    {
                        //        //                            MyFollowings.Add(SingleFollower.FirstUser.FullName, SingleFollower.FirstUser_Id); // jinho na mjha follow kea unka ids and names to avoid db hits while sending notification
                        //        MyFollowingsDevices.AddRange(ctx.UserDevices.Where(x => x.User_Id == SingleFollower.FirstUser_Id && x.IsActive));
                        //        NotifyMyFollowings.Add(new Notification
                        //        {
                        //            CreatedDate = DateTime.UtcNow,
                        //            EntityId = like.Post_Id.Value,
                        //            EntityType = (int)RiscoEntityTypes.LikeOnPost,
                        //            Description = SingleFollower.SecondUser.FullName + " liked " + post.User.FullName + "'s post.",
                        //            IsDeleted = false,
                        //            SendingUser_Id = like.User_Id,
                        //            ReceivingUser_Id = SingleFollower.FirstUser_Id,
                        //            Status = (int)NotificationStatus.Unread,
                        //            Title = "Risco",
                        //        });
                        //    }
                        //    ctx.SaveChanges();
                        //}

                        var LikeObj = ctx.Likes.Include(x => x.Post).Include(x => x.Post.Group).FirstOrDefault(x => x.Id == like.Id);
                        int SecondUser_Id = ctx.Posts.FirstOrDefault(x => x.Id == Post_Id && x.IsDeleted == false).User_Id;
                        SetActivityLog(FirstUser_Id: userId, SecondUser_Id: SecondUser_Id, EntityType: (int)RiscoEntityTypes.LikeOnPost, EntityId: Post_Id);
                        if (LikeObj.Post.User_Id != userId)
                        {
                            SetTopFollowerLog(FirstUser_Id: userId, SecondUser_Id: SecondUser_Id);
                            if (LikeObj.Post.Group_Id.HasValue)
                                Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.Group, EntityId: Post_Id, ReceivingUser_Id: SecondUser_Id, SendingUser_Id: userId, ChildEntityType: (int)RiscoEntityTypes.LikeOnPost);
                            else
                                Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.LikeOnPost, EntityId: Post_Id, ReceivingUser_Id: SecondUser_Id, SendingUser_Id: userId);
                        }


                        //NotifyFollowers(MyFollowingsDevices, NotifyMyFollowings.FirstOrDefault(), RiscoEntityTypes.LikeOnPost, like.Post_Id.Value);
                    }
                    CustomResponse<Like> response = new CustomResponse<Like>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = like
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
        [Route("UnLikePost")]
        public async Task<IHttpActionResult> UnLikePost(int Post_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {

                    Like like = ctx.Likes.FirstOrDefault(x => x.Post_Id == Post_Id && x.User_Id == userId && x.IsDeleted == false);
                    like.IsDeleted = true;

                    // code for risk level --- START ---
                    Post post = ctx.Posts.Where(x => x.Id == Post_Id && !x.IsDeleted).First();
                    if (post.PostType == (int)PostTypes.Incident)
                    {
                        List<RiskLevelRange> RiskLevelRanges = ctx.RiskLevelRange.Where(x => !x.IsDeleted).ToList();
                        var LikesCount = ctx.Likes.Where(x => x.Post_Id == Post_Id && x.IsDeleted == false).Count();
                        var CommentsCount = ctx.Comments.Where(x => x.Post_Id == Post_Id && x.IsDeleted == false).Count();
                        var ShareCount = ctx.Shares.Where(x => x.Post_Id == Post_Id && x.IsDeleted == false).Count();
                        int total = LikesCount + CommentsCount + ShareCount - 1;
                        if (total >= 0)
                        {

                            if (RiskLevelRanges.Where(x => x.From <= total && x.Text.ToLower() == "high").Count() > 0)
                            {
                                post.RiskLevel = (int)RiskLevelTypes.High;
                            }
                            else if (RiskLevelRanges.Where(x => x.From <= total && x.To >= total && x.Text.ToLower() == "medium").Count() > 0)
                            {
                                post.RiskLevel = (int)RiskLevelTypes.Medium;
                            }
                            else
                            {
                                post.RiskLevel = (int)RiskLevelTypes.Low;
                            }
                        }
                    }
                    // code for risk level --- END ---

                    ctx.SaveChanges();

                    CustomResponse<Like> response = new CustomResponse<Like>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = like
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
        [Route("LikeComment")]
        public async Task<IHttpActionResult> LikeComment(int Comment_Id, int? Post_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                //List<UserDevice> MyFollowingsDevices = new List<UserDevice>();
                //List<Notification> NotifyMyFollowings = new List<Notification>();
                Like like = new Like();
                List<int> UsersToNotify = new List<int>();
                using (RiscoContext ctx = new RiscoContext())
                {

                    var existingLike = ctx.Likes.FirstOrDefault(x => x.Comment_Id == Comment_Id && x.User_Id == userId && x.IsDeleted);

                    if (existingLike != null)
                    {
                        existingLike.IsDeleted = false;
                    }
                    else
                    {
                        like = new Like
                        {
                            Comment_Id = Comment_Id,
                            User_Id = userId,
                            CreatedDate = DateTime.UtcNow
                        };

                        ctx.Likes.Add(like);
                    }

                    ctx.SaveChanges();

                    if (existingLike == null)
                    {
                        //var post = ctx.Posts.Include(x => x.User).FirstOrDefault(x => x.Id == Post_Id);
                        //var MyFollowers = ctx.FollowFollowers.Include(x => x.SecondUser).Include(x => x.FirstUser).Where(x => x.SecondUser_Id == userId).ToList();

                        //if (MyFollowers != null)
                        //{
                        //    foreach (var SingleFollower in MyFollowers)
                        //    {
                        //        //                            MyFollowings.Add(SingleFollower.FirstUser.FullName, SingleFollower.FirstUser_Id); // jinho na mjha follow kea unka ids and names to avoid db hits while sending notification
                        //        MyFollowingsDevices.AddRange(ctx.UserDevices.Where(x => x.User_Id == SingleFollower.FirstUser_Id && x.IsActive));
                        //        NotifyMyFollowings.Add(new Notification
                        //        {
                        //            CreatedDate = DateTime.UtcNow,
                        //            EntityId = like.Post_Id.Value,
                        //            EntityType = (int)RiscoEntityTypes.LikeOnPost,
                        //            Description = SingleFollower.SecondUser.FullName + " liked " + post.User.FullName + "'s post.",
                        //            IsDeleted = false,
                        //            SendingUser_Id = like.User_Id,
                        //            ReceivingUser_Id = SingleFollower.FirstUser_Id,
                        //            Status = (int)NotificationStatus.Unread,
                        //            Title = "Risco",
                        //        });
                        //    }
                        //    ctx.SaveChanges();
                        //}

                        var comment = ctx.Comments.Include(x => x.Post).FirstOrDefault(x => x.Id == like.Comment_Id);

                        if (comment.User_Id != userId)
                            UsersToNotify.Add(comment.User_Id);

                        int SecondUser_Id = ctx.Posts.FirstOrDefault(x => x.Id == Post_Id && x.IsDeleted == false).User_Id;

                        if (SecondUser_Id != userId && !UsersToNotify.Contains(SecondUser_Id))
                            UsersToNotify.Add(SecondUser_Id);

                        SetTopFollowerLog(FirstUser_Id: userId, SecondUser_Id: SecondUser_Id);
                        SetActivityLog(FirstUser_Id: userId, SecondUser_Id: SecondUser_Id, EntityType: (int)RiscoEntityTypes.LikeOnPost, EntityId: Post_Id.Value);

                        if (UsersToNotify.Count > 0)
                        {

                            if (comment.Post.Group_Id.HasValue)
                                Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.Group, EntityId: Post_Id.Value, ReceivingUser_Ids: UsersToNotify, SendingUser_Id: userId, ChildEntityType: (int)RiscoEntityTypes.LikeOnComment);
                            //Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.Group, EntityId: Post_Id.Value, ReceivingUser_Id: SecondUser_Id, SendingUser_Id: userId, ChildEntityType: (int)RiscoEntityTypes.LikeOnComment);
                            else
                                Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.LikeOnComment, EntityId: Post_Id.Value, ReceivingUser_Ids: UsersToNotify, SendingUser_Id: userId);

                            //Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.LikeOnComment, EntityId: Post_Id.Value, ReceivingUser_Id: SecondUser_Id, SendingUser_Id: userId);
                        }

                    }



                    CustomResponse<Like> response = new CustomResponse<Like>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = like
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
        [Route("UnLikeComment")]
        public async Task<IHttpActionResult> UnLikeComment(int Comment_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {

                    Like like = ctx.Likes.FirstOrDefault(x => x.Comment_Id == Comment_Id && x.User_Id == userId && x.IsDeleted == false);
                    like.IsDeleted = true;
                    ctx.SaveChanges();

                    CustomResponse<Like> response = new CustomResponse<Like>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = like
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
        [Route("DeleteComment")]
        public async Task<IHttpActionResult> DeleteComment(int Comment_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {

                    Comment Comment = ctx.Comments.FirstOrDefault(x => x.Id == Comment_Id && x.User_Id == userId && x.IsDeleted == false);
                    Comment.IsDeleted = true;

                    await ctx.Comments.Where(x => x.ParentComment_Id == Comment.Id).ForEachAsync(x => x.IsDeleted = true);

                    ctx.SaveChanges();
                    int? Post_Id = Comment.Post_Id;
                    Post post = ctx.Posts.Where(x => x.Id == Post_Id && !x.IsDeleted).First();
                    if (post.PostType == (int)PostTypes.Incident)
                    {
                        List<RiskLevelRange> RiskLevelRanges = ctx.RiskLevelRange.Where(x => !x.IsDeleted).ToList();
                        var LikesCount = ctx.Likes.Where(x => x.Post_Id == Post_Id && x.IsDeleted == false).Count();
                        var CommentsCount = ctx.Comments.Where(x => x.Post_Id == Post_Id && x.IsDeleted == false).Count();
                        var ShareCount = ctx.Shares.Where(x => x.Post_Id == Post_Id && x.IsDeleted == false).Count();
                        int total = LikesCount + CommentsCount + ShareCount;

                        if (RiskLevelRanges.Where(x => x.From <= total && x.Text.ToLower() == "high").Count() > 0)
                        {
                            post.RiskLevel = (int)RiskLevelTypes.High;
                        }
                        else if (RiskLevelRanges.Where(x => x.From <= total && x.To >= total && x.Text.ToLower() == "medium").Count() > 0)
                        {
                            post.RiskLevel = (int)RiskLevelTypes.Medium;
                        }
                        else
                        {
                            post.RiskLevel = (int)RiskLevelTypes.Low;
                        }
                    }

                    ctx.SaveChanges();

                    CustomResponse<Comment> response = new CustomResponse<Comment>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = Comment
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



        //[HttpGet]
        //[Route("Repost")]
        //public async Task<IHttpActionResult> Repost(int Post_Id, string Location, int Visibility = (int)PostVisibilityTypes.Public, int? Group_Id = 0)
        //{
        //    try
        //    {
        //        var userId = Convert.ToInt32(User.GetClaimValue("userid"));
        //        List<int> UsersToNotify = new List<int>();

        //        using (RiscoContext ctx = new RiscoContext())
        //        {

        //            Post post = ctx.Posts
        //                .Include(x => x.Medias).AsNoTracking()
        //                .FirstOrDefault(x => x.Id == Post_Id);

        //            post.User_Id = userId;
        //            post.Location = Location;
        //            post.Visibility = Visibility;
        //            post.CreatedDate = DateTime.UtcNow;
        //            if (post.Group_Id.HasValue)
        //            {
        //                post.Group_Id = null;
        //            }
        //            if (Group_Id.HasValue && Group_Id != 0)
        //            {
        //                post.Group_Id = Group_Id;
        //            }



        //            foreach (Media media in post.Medias)
        //            {
        //                media.CreatedDate = DateTime.UtcNow;
        //            }

        //            ctx.Posts.Add(post);
        //            ctx.SaveChanges();

        //            SetTrends(Text: post.Text, User_Id: userId, Post_Id: post.Id);

        //            Share share = new Share
        //            {
        //                Post_Id = Post_Id,
        //                User_Id = userId,
        //                CreatedDate = DateTime.UtcNow,
        //            };

        //            ctx.Shares.Add(share);
        //            ctx.SaveChanges();

        //            // need to verify in case user have no followers

        //            if (post.Visibility == (int)PostVisibilityTypes.OnlyMe)
        //            {
        //                //UsersToNotify.AddRange(ctx.FollowFollowers.Where(x => x.SecondUser_Id == userId).Select(x => x.FirstUser_Id).ToList());
        //                //Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.SharePost, EntityId: post.Id, ReceivingUser_Ids: UsersToNotify, SendingUser_Id: userId, ToDifferentUsers: true);
        //            }
        //            else if (post.Visibility == (int)PostVisibilityTypes.Follower)
        //            {
        //                UsersToNotify.AddRange(ctx.FollowFollowers.Where(x => x.SecondUser_Id == userId).Select(x => x.FirstUser_Id).ToList());

        //                if (UsersToNotify.IndexOf(userId) != -1)
        //                {
        //                    for (var i = 0; i < UsersToNotify.Count; i++)
        //                    {
        //                        if (UsersToNotify[i] == userId)
        //                        {
        //                            UsersToNotify.RemoveAt(i);
        //                        }
        //                    }
        //                    UsersToNotify = UsersToNotify.Distinct().ToList();
        //                }
        //                Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.SharePost, EntityId: post.Id, ReceivingUser_Ids: UsersToNotify, SendingUser_Id: userId, ToDifferentUsers: true);
        //            }
        //            else
        //            {
        //                UsersToNotify.AddRange(ctx.FollowFollowers.Where(x => x.SecondUser_Id == userId).Select(x => x.FirstUser_Id).ToList());
        //                UsersToNotify.AddRange(ctx.FollowFollowers.Where(x => x.FirstUser_Id == userId).Select(x => x.SecondUser_Id).ToList());

        //                if (UsersToNotify.IndexOf(userId) != -1)
        //                {
        //                    for (var i = 0; i < UsersToNotify.Count; i++)
        //                    {
        //                        if (UsersToNotify[i] == userId)
        //                        {
        //                            UsersToNotify.RemoveAt(i);
        //                        }
        //                    }
        //                    UsersToNotify = UsersToNotify.Distinct().ToList();
        //                    //UsersToNotify.RemoveAll(userId);
        //                }
        //                Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.SharePost, EntityId: post.Id, ReceivingUser_Ids: UsersToNotify, SendingUser_Id: userId, ToDifferentUsers: true);
        //            }

        //            //Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.SharePost, EntityId: Post_Id, ReceivingUser_Id: post.User_Id, SendingUser_Id: userId);
        //            SetActivityLog(FirstUser_Id: userId, SecondUser_Id: post.User_Id, EntityType: (int)RiscoEntityTypes.SharePost, EntityId: Post_Id);

        //            CustomResponse<Post> response = new CustomResponse<Post>
        //            {
        //                Message = Global.ResponseMessages.Success,
        //                StatusCode = (int)HttpStatusCode.OK,
        //                Result = post
        //            };
        //            return Ok(response);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new CustomResponse<string>
        //        {
        //            Message = Global.ResponseMessages.Success,
        //            StatusCode = (int)Utility.LogError(ex),
        //            Result = "Something went wrong"
        //        });
        //    }
        //}

        [HttpGet]
        [Route("HidePost")]
        public async Task<IHttpActionResult> HidePost(int Post_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    Post post = ctx.Posts.FirstOrDefault(x => x.Id == Post_Id);

                    HidePost hidePost = new HidePost
                    {
                        FirstUser_Id = userId,
                        SecondUser_Id = post.User_Id,
                        Post_Id = Post_Id,
                        CreatedDate = DateTime.UtcNow
                    };

                    ctx.HidePosts.Add(hidePost);
                    ctx.SaveChanges();

                    CustomResponse<HidePost> response = new CustomResponse<HidePost>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = hidePost
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
        [Route("HideAllPost")]
        public async Task<IHttpActionResult> HideAllPost(int HideAllPostsUser_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    HideAllPost hideAllPost = new HideAllPost
                    {
                        FirstUserAll_Id = userId,
                        SecondUserAll_Id = HideAllPostsUser_Id,
                        CreatedDate = DateTime.UtcNow
                    };

                    ctx.HideAllPosts.Add(hideAllPost);
                    ctx.SaveChanges();

                    CustomResponse<HideAllPost> response = new CustomResponse<HideAllPost>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = hideAllPost
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
        [Route("GetTrends")]
        public async Task<IHttpActionResult> GetTrends()
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                    string query = "select top 10 Text, count(Id) as [Count] from trendlogs  group by Text order by Count desc";
                    List<TrendBindingModel> trends = ctx.Database.SqlQuery<TrendBindingModel>(query).ToList();

                    CustomResponse<TrendListViewModel> response = new CustomResponse<TrendListViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new TrendListViewModel
                        {
                            Trends = trends
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
        [Route("TurnOffNotifications")]
        public async Task<IHttpActionResult> TurnOffNotifications(int Post_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                TurnOffNotification turnOffNotification = new TurnOffNotification();


                using (RiscoContext ctx = new RiscoContext())
                {
                    var turnOffNotifications = ctx.TurnOffNotifications.Where(x => x.Post_Id == Post_Id).ToList();

                    //if TurnOffNotification count is 0 then create TurnOffNotification and turn noti off for this post
                    if (turnOffNotifications.Count == 0)
                    {
                        turnOffNotification.User_Id = userId;
                        turnOffNotification.Post_Id = Post_Id;
                        turnOffNotification.CreatedDate = DateTime.UtcNow;

                        ctx.TurnOffNotifications.Add(turnOffNotification);
                        ctx.SaveChanges();
                    }
                    else                //count is bigger than 0 then take the already created object and set its isDeleted property to opposite value
                    {
                        turnOffNotification = turnOffNotifications.FirstOrDefault();

                        turnOffNotification.IsDeleted = turnOffNotification.IsDeleted == true ? false : true;

                        ctx.SaveChanges();
                    }

                    CustomResponse<TurnOffNotification> response = new CustomResponse<TurnOffNotification>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = turnOffNotification
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
        [Route("ReportPost")]
        public async Task<IHttpActionResult> ReportPost(int Post_Id, int ReportType, string Text)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    ReportPost reportPost = new ReportPost();


                    var AlreadyReported = ctx.ReportPosts.Include(x => x.User).FirstOrDefault(x => x.Post_Id == Post_Id);
                    if (AlreadyReported != null)
                    {
                        if (AlreadyReported.ReportCount > 5 && AlreadyReported.ReportCount < 10)
                        {
                            Utility.SendEmail("Post Deletion Warning", EmailTypes.PostDeletionWarning, AlreadyReported.User_Id, "", AlreadyReported.User.Email);
                        }
                        else if (AlreadyReported.ReportCount > 10)
                        {
                            Utility.SendEmail("Post Deleted", EmailTypes.PostDeleted, AlreadyReported.User_Id, "", AlreadyReported.User.Email);
                        }

                        AlreadyReported.ReportCount++;

                        reportPost = AlreadyReported;
                        ctx.SaveChanges();

                    }
                    else
                    {
                        reportPost = new ReportPost
                        {
                            User_Id = userId,
                            Post_Id = Post_Id,
                            ReportType = ReportType,
                            Text = Text,
                            CreatedDate = DateTime.UtcNow,
                            ReportCount = 1
                        };
                        ctx.ReportPosts.Add(reportPost);
                        ctx.SaveChanges();
                        var NewReported = ctx.ReportPosts.Include(x => x.User).FirstOrDefault(x => x.Post_Id == reportPost.Post_Id);

                        Utility.SendEmail("Post Deletion Warning", EmailTypes.PostDeletionWarning, NewReported.User_Id, "", NewReported.User.Email);

                    }
                    SetActivityLog(FirstUser_Id: userId, SecondUser_Id: ctx.Posts.FirstOrDefault(x => x.Id == Post_Id).User_Id, EntityType: (int)RiscoEntityTypes.ReportPost, EntityId: reportPost.Id);

                    CustomResponse<ReportPost> response = new CustomResponse<ReportPost>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = reportPost
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
        [Route("GetLocationsForHeatMap")]
        public async Task<IHttpActionResult> GetLocationsForHeatMap(string HashTag = "")
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                HeatMapViewModel returnModel = new HeatMapViewModel();
                using (RiscoContext ctx = new RiscoContext())
                {
                    //var Low = ctx.TrendLogs.Include(x => x.Post).Where(x => x.Post.RiskLevel == (int)RiskLevelTypes.Low && !x.IsDeleted).ToList();
                    string LowQuery = @"select distinct p.Id, p.Latitude,p.Longitude 
                                from TrendLogs t
                                join Posts p 
                                on 
                                p.Id=t.Post_Id 
                                where p.RiskLevel=" + (int)RiskLevelTypes.Low + @" And t.IsDeleted=0 And p.IsDeleted=0 and p.posttype = " + (int)PostTypes.Incident + @"-- and t.Text ='" + HashTag + "'";

                    string MedQuery = @"select distinct p.Id, p.Latitude,p.Longitude 
                                from TrendLogs t
                                join Posts p 
                                on 
                                p.Id=t.Post_Id 
                                where p.RiskLevel=" + (int)RiskLevelTypes.Medium + @" And t.IsDeleted=0 And p.IsDeleted=0 and p.posttype = " + (int)PostTypes.Incident + @"-- and t.Text ='" + HashTag + "'";

                    string HighQuery = @"select distinct p.Id, p.Latitude,p.Longitude 
                                from TrendLogs t
                                join Posts p 
                                on 
                                p.Id=t.Post_Id 
                                where p.RiskLevel=" + (int)RiskLevelTypes.High + @" And t.IsDeleted=0 And p.IsDeleted=0 and p.posttype = " + (int)PostTypes.Incident + @"-- and t.Text ='" + HashTag + "'";


                    returnModel.Low = ctx.Database.SqlQuery<LocationViewModel>(LowQuery).Where(x => (x.Latitude != 0 && x.Longitude != 0) && (x.Latitude != null && x.Longitude != null)).ToList();
                    returnModel.Medium = ctx.Database.SqlQuery<LocationViewModel>(MedQuery).Where(x => (x.Latitude != 0 && x.Longitude != 0) && (x.Latitude != null && x.Longitude != null)).ToList();
                    returnModel.High = ctx.Database.SqlQuery<LocationViewModel>(HighQuery).Where(x => (x.Latitude != 0 && x.Longitude != 0) && (x.Latitude != null && x.Longitude != null)).ToList();
                    var posts = new List<Media>();

                    //if (!string.IsNullOrEmpty(HashTag))
                    //{
                    //    HashTag = HashTag.Substring(0, 1) == "#" ? HashTag : $"#{HashTag}";
                    //    returnModel.Medias = ctx.TrendLogs.Include(x => x.Post).Where(x => x.Text == HashTag && x.Post.Medias.Count() > 0).SelectMany(x => x.Post.Medias).Distinct().ToList();
                    returnModel.Medias = ctx.TrendLogs.Include(x => x.Post).Where(x => x.Post.PostType == (int)PostTypes.Incident && x.Post.Medias.Count() > 0).SelectMany(x => x.Post.Medias).Distinct().ToList();
                    //}


                }
                CustomResponse<HeatMapViewModel> response = new CustomResponse<HeatMapViewModel>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = returnModel
                };
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [HttpGet]
        [Route("GetLocationsForHeatMapV2")]
        public async Task<IHttpActionResult> GetLocationsForHeatMapV2()
        {
            try
            {

                HeatMapViewModel returnModel = new HeatMapViewModel();
                using (RiscoContext ctx = new RiscoContext())
                {
                    string LowQuery = @"select distinct p.Id, p.Latitude,p.Longitude 
                    from Posts p 
                    where p.RiskLevel=" + (int)RiskLevelTypes.Low + @" And p.IsDeleted=0 and p.posttype = " + (int)PostTypes.Incident;


                    string MedQuery = @"select distinct p.Id, p.Latitude,p.Longitude 
                    from Posts p 
                    where p.RiskLevel=" + (int)RiskLevelTypes.Medium + @" And p.IsDeleted=0 and p.posttype = " + (int)PostTypes.Incident;

                    string HighQuery = @"select distinct p.Id, p.Latitude,p.Longitude 
                    from Posts p 
                    where p.RiskLevel=" + (int)RiskLevelTypes.High + @" And p.IsDeleted=0 and p.posttype = " + (int)PostTypes.Incident;

                    returnModel.Low = ctx.Database.SqlQuery<LocationViewModel>(LowQuery).Where(x => (x.Latitude != 0 && x.Longitude != 0) && (x.Latitude != null && x.Longitude != null)).ToList();
                    returnModel.Medium = ctx.Database.SqlQuery<LocationViewModel>(MedQuery).Where(x => (x.Latitude != 0 && x.Longitude != 0) && (x.Latitude != null && x.Longitude != null)).ToList();
                    returnModel.High = ctx.Database.SqlQuery<LocationViewModel>(HighQuery).Where(x => (x.Latitude != 0 && x.Longitude != 0) && (x.Latitude != null && x.Longitude != null)).ToList();
                    var posts = new List<Media>();

                    returnModel.Medias = ctx.TrendLogs.Include(x => x.Post).Where(x => x.Post.PostType == (int)PostTypes.Incident && x.Post.Medias.Count() > 0).SelectMany(x => x.Post.Medias).Distinct().ToList();

                }
                CustomResponse<HeatMapViewModel> response = new CustomResponse<HeatMapViewModel>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = returnModel
                };
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("GetPostByHashTags")]
        public async Task<IHttpActionResult> GetPostByHashTags(string hashTag, int postType = 0)
        {
            var posts = new List<Post>();
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                if (!string.IsNullOrEmpty(hashTag))
                {
                    hashTag = hashTag.Substring(0, 1) == "#" ? hashTag : $"#{hashTag}";

                    using (RiscoContext ctx = new RiscoContext())
                    {

                        //var _posts = ctx.TrendLogs
                        //     .Include(x => x.Post)
                        //     .Where(x => x.Text == hashTag && !x.IsDeleted)
                        //     .Select(x => x.Post)
                        //     .Distinct();

                        posts = ctx.TrendLogs
                             .Include(x => x.Post)
                             .Where(x => x.Text == hashTag && !x.IsDeleted && (postType != 0 ? x.Post.PostType == postType : true))
                             .Select(x => x.Post)
                             .Distinct()
                             .Include(x => x.User)
                             .Include(x => x.Medias)
                             .Include(x => x.Medias.Select(y => y.MediaUserViews))
                             .Include(x => x.PollOptions)
                             .Include(x => x.SharedParent)
                             .Include(x => x.SharedParent.User)
                             .ToList();

                        foreach (var post in posts)
                        {
                            if (post.SharedParent != null)
                                post.SharedParent.IsUserFollow = ctx.FollowFollowers.Any(x => x.FirstUser_Id == userId && x.SecondUser_Id == post.SharedParent.User_Id && x.IsDeleted == false);

                            post.IsLiked = ctx.Likes.Any(x => x.Post_Id == post.Id && x.User_Id == userId && x.IsDeleted == false);
                            post.LikesCount = ctx.Posts.Sum(p => p.Likes.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                            post.CommentsCount = ctx.Posts.Sum(p => p.Comments.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                            post.ShareCount = ctx.Posts.Sum(p => p.Shares.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                            post.IsUserFollow = ctx.FollowFollowers.Any(x => x.FirstUser_Id == userId && x.SecondUser_Id == post.User_Id && x.IsDeleted == false);
                            post.IsPostOwner = (post.User_Id == userId) ? true : false;

                            //int count = post.LikesCount + post.CommentsCount + post.ShareCount;
                            post.Medias = post.Medias.Where(x => x.Comment_Id == null).ToList();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
            CustomResponse<List<Post>> response = new CustomResponse<List<Post>>
            {
                Message = Global.ResponseMessages.Success,
                StatusCode = (int)HttpStatusCode.OK,
                Result = posts
            };
            return Ok(response);
        }

        [HttpGet]
        [Route("GetMediaByHashTags")]
        public async Task<IHttpActionResult> GetMediaByHashTags(string hashTag)
        {
            var posts = new List<Media>();
            try
            {

                if (!string.IsNullOrEmpty(hashTag))
                {
                    hashTag = hashTag.Substring(0, 1) == "#" ? hashTag : $"#{hashTag}";

                    using (RiscoContext ctx = new RiscoContext())
                    {

                        posts = ctx.TrendLogs.Include(x => x.Post).Where(x => x.Text == hashTag && x.Post.Medias.Count() > 0).SelectMany(x => x.Post.Medias).Distinct().ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
            CustomResponse<List<Media>> response = new CustomResponse<List<Media>>
            {
                Message = Global.ResponseMessages.Success,
                StatusCode = (int)HttpStatusCode.OK,
                Result = posts
            };
            return Ok(response);
        }

        [HttpGet]
        [Route("GetAllTopMedia")]
        public async Task<IHttpActionResult> GetAllTopMedia(int postCount = 0)
        {

            try
            {

                using (RiscoContext ctx = new RiscoContext())
                {

                    var posts = ctx.TrendLogs.Include(x => x.Post).Where(x => x.Post.Medias.Count() > 0)
                         .GroupBy(x => x.Text).Select(x => new { trend = x.Key, count = x.Count(), Medias = x.SelectMany(y => y.Post.Medias) })
                          .Where(x => (postCount > 0 ? x.count >= postCount : true))
                         .Select(x => x)
                        .OrderByDescending(x => x.count).ToList();

                    CustomResponse<object> response = new CustomResponse<object>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = posts
                    };
                    return Ok(response);
                }

            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
            CustomResponse<List<Media>> response2 = new CustomResponse<List<Media>>
            {
                Message = Global.ResponseMessages.Success,
                StatusCode = (int)HttpStatusCode.OK,
                Result = null
            };
            return Ok(response2);
        }

        [HttpGet]
        [Route("SearchTrends")]
        public async Task<IHttpActionResult> SearchTrends(string hashTag = "", int PageSize = 20, int PageNo = 1, int postCount = 0, int postType = 0)
        {

            try
            {
                hashTag = !string.IsNullOrEmpty(hashTag) ? (hashTag.Substring(0, 1) == "#" ? hashTag : $"#{hashTag}") : "";

                using (RiscoContext ctx = new RiscoContext())
                {

                    var obj = ctx.TrendLogs
                        .Include(x => x.Post)
                        //.Where(x => !string.IsNullOrEmpty(hashTag) ? x.Text == hashTag : true)
                        .Where(x => (!string.IsNullOrEmpty(hashTag) ? x.Text.Contains(hashTag) : true) && (postType != 0 ? x.Post.PostType == postType : true))
                        .GroupBy(x => x.Text).Select(x => new { Text = x.Key, Count = x.Count() })
                        .Where(x => (postCount > 0 ? x.Count >= postCount : true))
                        .OrderByDescending(x => x.Count).Skip((PageNo - 1) * PageSize).Take(PageSize).ToList();
                    CustomResponse<object> response = new CustomResponse<object>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = obj
                    };
                    return Ok(response);
                }

            }
            catch (Exception ex)
            {
                Utility.LogError(ex);

            }
            CustomResponse<object> response2 = new CustomResponse<object>
            {
                Message = Global.ResponseMessages.Success,
                StatusCode = (int)HttpStatusCode.OK,
                Result = null
            };
            return Ok(response2);
        }

        [HttpGet]
        [Route("SetMediaViews")]
        public async Task<IHttpActionResult> SetMediaViews(int mediaId, int userId)
        {

            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    if (!ctx.MediaUserViews.Any(x => x.MediaId == mediaId && x.UserId == userId))
                    {
                        MediaUserViews muv = new MediaUserViews();
                        muv.MediaId = mediaId;
                        muv.UserId = userId;
                        muv.CreatedDate = DateTime.UtcNow;
                        muv.IsDeleted = false;
                        ctx.MediaUserViews.Add(muv);
                        await ctx.SaveChangesAsync();
                    }
                    var count = ctx.MediaUserViews.Count(x => x.MediaId == mediaId);
                    CustomResponse<int> response = new CustomResponse<int>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = count
                    };
                    return Ok(response);
                }

            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
            CustomResponse<object> response2 = new CustomResponse<object>
            {
                Message = Global.ResponseMessages.Success,
                StatusCode = (int)HttpStatusCode.OK,
                Result = null
            };
            return Ok(response2);
        }

        [HttpGet]
        [Route("GetMediaViews")]
        public async Task<IHttpActionResult> GetMediaViews(int mediaId)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var count = ctx.MediaUserViews.Count(x => x.MediaId == mediaId);
                    CustomResponse<int> response = new CustomResponse<int>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = count
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
            CustomResponse<object> response2 = new CustomResponse<object>
            {
                Message = Global.ResponseMessages.Success,
                StatusCode = (int)HttpStatusCode.OK,
                Result = null
            };
            return Ok(response2);
        }

        [HttpGet]
        [Route("GetHighRiskLevelIncidents")]
        public async Task<IHttpActionResult> GetHighRiskLevelIncidents(int PageSize = 20, int PageNo = 0)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                    List<int> UserInterests = new List<int>();
                    List<int> UserLanguageIds = new List<int>();
                    List<string> userlangCodes = new List<string>();
                    List<int> postIds = new List<int>();

                    List<Post> posts = new List<Post>();

                    // Hide All Post Users

                    var HideAllUsersIds = ctx.HideAllPosts.Where(x => x.FirstUserAll_Id == userId && x.IsDeleted == false).Select(x => x.SecondUserAll_Id).Distinct().ToList();

                    var HidePostsIds = ctx.HidePosts.Where(x => x.FirstUser_Id == userId && x.IsDeleted == false).Select(x => x.Post_Id).Distinct().ToList();

                    var ReportPostsIds = ctx.ReportPosts.Where(x => x.User_Id == userId && x.IsDeleted == false).Select(x => x.Post_Id).Distinct().ToList();

                    var userModel = ctx.Users.FirstOrDefault(x => x.Id == userId);
                    if (userModel != null)
                    {
                        if (!string.IsNullOrEmpty(userModel.Interests))
                        {
                            UserInterests = userModel.Interests.Split(',').Select(int.Parse).ToList();
                        }

                        if (!string.IsNullOrEmpty(userModel.Language))
                        {
                            userlangCodes = userModel.Language.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x).ToList();
                        }
                    }

                    List<FollowFollower> followFollowers = new List<FollowFollower>();

                    var myFollowingsIds = ctx.FollowFollowers
                        .Where(x => x.IsDeleted == false && x.FirstUser_Id == userId)
                        .Select(x => x.SecondUser_Id).Distinct()
                        .ToList();

                    var BlockedUsers = ctx.BlockUsers.Where(x => x.FirstUser_Id == userId && !x.IsDeleted).Select(x => x.SecondUser_Id).Distinct().ToList();

                    BlockedUsers.AddRange(ctx.BlockUsers.Where(x => x.SecondUser_Id == userId && !x.IsDeleted).Select(x => x.FirstUser_Id).Distinct().ToList());

                    posts = ctx.Posts
                     .Include(x => x.User)
                     .Include(x => x.Medias)
                     .Include(x => x.Medias.Select(y => y.MediaUserViews))
                     .Include(x => x.PollOptions)
                     .Include(x => x.SharedParent)
                     .Include(x => x.SharedParent.User)
                     .Where(x => x.IsDeleted == false && x.Group_Id == null &&
                     !BlockedUsers.Contains(x.User_Id) &&
                     !HideAllUsersIds.Contains(x.User_Id) &&
                     !HidePostsIds.Contains(x.Id) &&
                     !ReportPostsIds.Contains(x.Id) &&
                     (UserInterests.Count > 0 ? x.PostCategories.Any(y => UserInterests.Contains(y.Interest_Id) && !y.IsDeleted) : true) &&
                     ((x.Visibility == (int)PostVisibilityTypes.Public) || (x.Visibility == (int)PostVisibilityTypes.Public && x.User_Id == userId) || (x.Visibility == (int)PostVisibilityTypes.OnlyMe && x.User_Id == userId) ||
                     (x.Visibility == (int)PostVisibilityTypes.Follower && (x.User_Id == userId || myFollowingsIds.Contains(x.User_Id))))
                     && (x.IsPoll ? x.PollExpiryTime >= DateTime.UtcNow : true) && x.RiskLevel == (int)RiskLevelTypes.High && x.PostType == (int)PostTypes.Incident)
                     .OrderByDescending(x => x.Id).Skip((PageNo - 1) * PageSize).Take(PageSize).ToList();

                    foreach (Post post in posts)
                    {

                        if (post.User == null)
                            post.User = ctx.Users.FirstOrDefault(x => x.Id == post.User_Id);

                        if (post.IsPoll)
                            post.CheckPollExpiry();

                        foreach (var item in post.PollOptions)
                        {
                            if (ctx.PollOptionVote.Any(x => x.PollOption_Id == item.Id && x.User_Id == userId && !x.IsDeleted))
                                item.IsVoted = true;
                        }

                        if (post.SharedParent != null)
                            post.SharedParent.IsUserFollow = ctx.FollowFollowers.Any(x => x.FirstUser_Id == userId && x.SecondUser_Id == post.SharedParent.User_Id && x.IsDeleted == false);


                        if (BlockedUsers.Contains(post.User_Id))
                        {
                            posts.Remove(post);
                        }
                        else
                        {
                            post.IsLiked = ctx.Likes.Any(x => x.Post_Id == post.Id && x.User_Id == userId && x.IsDeleted == false);
                            post.LikesCount = ctx.Posts.Sum(p => p.Likes.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                            post.CommentsCount = ctx.Posts.Sum(p => p.Comments.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                            post.ShareCount = ctx.Posts.Sum(p => p.Shares.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
                            post.IsUserFollow = ctx.FollowFollowers.Any(x => x.FirstUser_Id == userId && x.SecondUser_Id == post.User_Id && x.IsDeleted == false);
                            post.IsPostOwner = (post.User_Id == userId) ? true : false;
                        }

                        post.Medias = post.Medias.Where(x => x.Comment_Id == null).ToList();

                        if (!string.IsNullOrEmpty(post.Language))
                        {
                            var langCodes = post.Language.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x).ToList();
                            if (langCodes.Any(x => userlangCodes.Contains(x)))
                            {
                                postIds.Add(post.Id);
                            }
                        }
                    }

                    posts = posts.Where(x => x.IsPoll ? x.PollExpiryTime >= DateTime.UtcNow : true).ToList();
                    posts = posts.Where(x => postIds.Contains(x.Id)).ToList();
                    CustomResponse<PostListViewModel> response = new CustomResponse<PostListViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new PostListViewModel
                        {
                            Posts = posts,
                            PostCount = posts.Count()
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

        #endregion


        #region HttpPost
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
                        //int MaxContentLength = 1024 * 1024 * 10; //Size = 1 MB  

                        int MaxContentLength = Convert.ToInt32(ConfigurationManager.AppSettings["ImageUploadSize"]); // 3mb size

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
                                Result = new Error { ErrorMessage = "Please Upload a file upto 3 mb." }
                            });
                        }
                        else
                        {
                            folderPath = ConfigurationManager.AppSettings["PostMediaFolderPath"] + DateTime.Now.Ticks.ToString() + extension;
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
        [Authorize]
        [Route("VideoUpload")]
        public async Task<IHttpActionResult> VideoUpload()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string folderPath = string.Empty;
                //List<string> responseObject = new List<string>();

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
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();

                        //int MaxContentLength = 1024 * 1024 * 100; //Size = 10 MB  
                        int MaxContentLength = Convert.ToInt32(ConfigurationManager.AppSettings["VideoUploadSize"]); //Size = 20 MB  

                        if (postedFile.ContentLength > MaxContentLength)
                        {
                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "UnsupportedMediaType",
                                StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                                Result = new Error { ErrorMessage = "Please Upload a file upto 20 mb." }
                            });
                        }
                        else
                        {
                            folderPath = ConfigurationManager.AppSettings["PostMediaFolderPath"] + DateTime.Now.Ticks.ToString() + extension;
                            newFullPath = HttpContext.Current.Server.MapPath("~/" + folderPath);
                            //responseObject.Add(folderPath);
                            postedFile.SaveAs(newFullPath);

                            //using (var videoThumbnailer = new VideoThumbnailer(newFullPath))
                            //using (var thumbnail = videoThumbnailer.GenerateThumbnail(500))
                            //{
                            //    folderPath = ConfigurationManager.AppSettings["PostMediaFolderPath"] + DateTime.Now.Ticks.ToString() + ".jpg";
                            //    responseObject.Add(folderPath);
                            //    newFullPath = HttpContext.Current.Server.MapPath("~/" + folderPath);
                            //    thumbnail.RotateFlip(RotateFlipType.Rotate90FlipX);
                            //    thumbnail.Save(newFullPath, ImageFormat.Jpeg);
                            //}

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
        [Route("VoteOnPollOption")]
        public async Task<IHttpActionResult> VoteOnPollOption(VotePollBindingModel model)
        {
            try
            {
                model.User_Id = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    var post = ctx.Posts.Include(x => x.PollOptions).Include(x => x.User).FirstOrDefault(x => x.Id == model.Post_Id && !x.IsDeleted);
                    if (post != null)
                    {
                        var alreadyVotes = ctx.PollOptionVote.Include(x => x.PollOptions).FirstOrDefault(x => x.User_Id == model.User_Id && x.Post_Id == model.Post_Id && !x.IsDeleted);
                        var PollOption = ctx.PollOptions.FirstOrDefault(x => x.Id == model.PollOption_Id);

                        if (alreadyVotes == null)
                        {
                            alreadyVotes = new PollOptionVote
                            {
                                IsDeleted = false,
                                PollOption_Id = model.PollOption_Id,
                                User_Id = model.User_Id,
                                Post_Id = model.Post_Id
                            };

                            ctx.PollOptionVote.Add(alreadyVotes);
                            post.TotalVotes++;
                            PollOption.Votes++;
                        }
                        else
                        {

                            if (alreadyVotes.PollOption_Id == model.PollOption_Id)
                            {

                            }
                            else
                            {
                                --alreadyVotes.PollOptions.Votes;
                                alreadyVotes.PollOption_Id = model.PollOption_Id;
                                ctx.SaveChanges();
                                var updateStatsPollOption = ctx.PollOptions.FirstOrDefault(x => x.Id == model.PollOption_Id);
                                ++updateStatsPollOption.Votes;
                            }
                        }


                        post.IsUserFollow = ctx.FollowFollowers.Any(x => x.FirstUser_Id == model.User_Id && x.SecondUser_Id == post.User_Id && x.IsDeleted == false);
                        post.CalculatePollPercentage();

                        // if (ctx.PollOptionVote.Any(x => x.PollOption_Id == model.PollOption_Id && x.User_Id == model.User_Id && !x.IsDeleted))
                        post.PollOptions.FirstOrDefault(x => x.Id == model.PollOption_Id).IsVoted = true;

                    }
                    ctx.SaveChanges();

                    if (post.SharePost_Id == null)
                        post.SharePost_Id = 0;

                    CustomResponse<Post> response = new CustomResponse<Post>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = post
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

        [HttpPost]
        [Route("CreatePost")]
        public async Task<IHttpActionResult> CreatePost(ThreadedPostBindingModel model)
        {
            try
            {
                Validate(model);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                List<int> UsersToNotify = new List<int>();
                List<int> PostCategories = new List<int>();
                List<Languages> Languages = new List<Languages>();
                List<string> UserLanguageCodes = new List<string>();

                int MainPostId = 0;
                int ParentPostId = model.Posts[0].ParentPost_Id ?? 0;

                using (RiscoContext ctx = new RiscoContext())
                {
                    foreach (var _post in model.Posts)
                    {
                        Post post = new Post
                        {
                            Text = _post.Text,
                            RiskLevel = _post.RiskLevel,
                            Location = _post.Location,
                            Visibility = _post.Visibility,
                            User_Id = userId,
                            CreatedDate = DateTime.UtcNow,
                            Latitude = _post.Latitude,
                            Longitude = _post.Longitude,
                            IsExpired = false,
                            PostType = _post.PostType,
                            Language = _post.Language
                        };

                        post.ParentPost_Id = ParentPostId > 0 ? ParentPostId : null as int?;

                        if (_post.ExpireAfterHours != 0)
                            post.PollExpiryTime = DateTime.UtcNow.AddHours(_post.ExpireAfterHours);


                        if (_post.IsPoll)
                        {
                            post.IsPoll = _post.IsPoll;
                            post.PollType = _post.PollType;
                            foreach (var item in _post.PollOptions)
                            {
                                if (string.IsNullOrEmpty(item.Title) && string.IsNullOrEmpty(item.MediaUrl))
                                    continue;

                                post.PollOptions.Add(new DAL.PollOptions
                                {
                                    IsDeleted = false,
                                    MediaUrl = item.MediaUrl,
                                    Percentage = 0,
                                    Votes = 0,
                                    Title = item.Title
                                });
                            }
                        }

                        if (_post.Group_Id != 0 && _post.Group_Id.HasValue)
                            post.Group_Id = _post.Group_Id;


                        // start of setting post's categories
                        if (!string.IsNullOrEmpty(_post.Interests))
                        {
                            PostCategories = _post.Interests.Split(',').Select(int.Parse).ToList();
                        }
                        if (PostCategories.Count > 0)
                        {
                            // List<PostCategories> Categories = new List<DAL.PostCategories>();
                            foreach (var item in PostCategories)
                            {
                                post.PostCategories.Add(new DAL.PostCategories
                                {
                                    Interest_Id = item,
                                    Post_Id = post.Id,
                                    IsDeleted = false
                                });
                            }
                            //post.PostCategories.AddRange(Categories);
                        }
                        if (post.SharePost_Id == 0)
                            post.SharePost_Id = null;

                        ctx.Posts.Add(post);
                        ctx.SaveChanges();

                        // end of setting post's categories

                        MainPostId = MainPostId > 0 ? MainPostId : post.Id;

                        if (ParentPostId == 0)
                        {
                            ParentPostId = post.Id;
                        }

                        SetTrends(Text: post.Text, User_Id: userId, Post_Id: post.Id);

                        if (!string.IsNullOrEmpty(_post.ImageUrls))
                        {
                            var ImageUrls = _post.ImageUrls.Split(',');
                            foreach (var ImageUrl in ImageUrls)
                            {
                                Media media = new Media
                                {
                                    Type = (int)MediaTypes.Image,
                                    Url = ImageUrl,
                                    Post_Id = post.Id,
                                    CreatedDate = DateTime.UtcNow,
                                    User_Id = userId
                                };
                                ctx.Medias.Add(media);
                                ctx.SaveChanges();
                            }
                        }

                        if (!string.IsNullOrEmpty(_post.VideoUrls))
                        {
                            var VideoUrls = _post.VideoUrls.Split(',');
                            foreach (var VideoUrl in VideoUrls)
                            {
                                string videoPath = VideoUrl;
                                string videoFullPath = HttpContext.Current.Server.MapPath("~/" + videoPath);

                                var ffMpeg = new NReco.VideoConverter.FFMpegConverter();

                                string thumbNailMovieImagePath = ConfigurationManager.AppSettings["PostMediaFolderPath"] + DateTime.Now.Ticks.ToString() + ".jpg";
                                string thumbNailMovieFullImagePath = HttpContext.Current.Server.MapPath("~/" + thumbNailMovieImagePath);

                                ffMpeg.GetVideoThumbnail(videoFullPath, thumbNailMovieFullImagePath, 1);

                                Media media = new Media
                                {
                                    Type = (int)MediaTypes.Video,
                                    Url = videoPath,
                                    ThumbnailUrl = thumbNailMovieImagePath,
                                    Post_Id = post.Id,
                                    CreatedDate = DateTime.UtcNow,
                                    User_Id = userId
                                };
                                ctx.Medias.Add(media);
                            }
                            ctx.SaveChanges();
                        }

                        SetActivityLog(FirstUser_Id: userId, SecondUser_Id: post.User_Id, EntityType: (int)RiscoEntityTypes.Post, EntityId: post.Id);
                    }

                    Post postResponse = new Post();
                    postResponse = ctx.Posts
                        .Include(x => x.User)
                        .Include(x => x.Medias)
                        .Include(x => x.PostCategories)
                        .Include(x => x.ExtendedPostList)
                        .Include(x => x.ExtendedPostList.Select(extendedPost => extendedPost.Medias))
                            .FirstOrDefault(x => x.Id == MainPostId);

                    /* start - send notification to users */
                    if (postResponse.Visibility == (int)PostVisibilityTypes.Follower || postResponse.Visibility == (int)PostVisibilityTypes.Public)
                    {
                        var utn = from u in ctx.Users
                                  join ff in ctx.FollowFollowers on u.Id equals ff.FirstUser_Id
                                  join fu in ctx.Users on ff.SecondUser_Id equals fu.Id
                                  join bbu in ctx.BlockUsers on u.Id equals bbu.FirstUser_Id into bbu1
                                  from bbu in bbu1.DefaultIfEmpty()
                                  join bu in ctx.BlockUsers on u.Id equals bu.SecondUser_Id into bu1
                                  from bu in bu1.DefaultIfEmpty()
                                  join mu in ctx.MuteUser on u.Id equals mu.SecondUser_Id into mu1
                                  from mu in mu1.DefaultIfEmpty()
                                  where
                                  u.Id == userId &&
                                  bbu.SecondUser_Id != ff.SecondUser_Id &&
                                  bu.FirstUser_Id != ff.SecondUser_Id &&
                                  mu.FirstUser_Id != fu.Id &&
                                  !ff.IsDeleted
                                  //!bbu.IsDeleted &&
                                  //!bu.IsDeleted &&
                                  //!mu.IsDeleted
                                  select fu.Id;

                        UsersToNotify.AddRange(utn);

                        Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.Post, EntityId: postResponse.Id, ReceivingUser_Ids: UsersToNotify, SendingUser_Id: userId, ToDifferentUsers: true);
                    }
                    /* end - send notification to users */

                    CustomResponse<Post> response = new CustomResponse<Post>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = postResponse
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
        [Route("DeletePost")]
        public async Task<IHttpActionResult> DeletePost(int Post_Id)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    Post post = ctx.Posts
                        .Include(x => x.Medias)
                        .Include(x => x.ExtendedPostList)
                        .FirstOrDefault(x => x.Id == Post_Id);

                    post.ExtendedPostList.ForEach(p => p.ParentPost_Id = null);

                    post.IsDeleted = true;

                    foreach (var media in post.Medias)
                    {
                        media.IsDeleted = true;
                    }

                    List<TrendLog> trendLogs = ctx.TrendLogs.Where(x => x.Post_Id == Post_Id).ToList();

                    foreach (TrendLog trendLog in trendLogs)
                    {
                        trendLog.IsDeleted = true;
                    }

                    ctx.SaveChanges();

                    CustomResponse<string> response = new CustomResponse<string>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = "Post has been deleted."
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
        [Route("Comment")]
        public async Task<IHttpActionResult> Comment(CreateCommentBindingModel model)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    Comment comment = new Comment
                    {
                        Text = model.Text,
                        Post_Id = model.Post_Id,
                        User_Id = userId,
                        CreatedDate = DateTime.UtcNow
                    };

                    ctx.Comments.Add(comment);

                    // code for risk level --- START ---
                    int Post_Id = model.Post_Id;
                    Post post = ctx.Posts.Where(x => x.Id == Post_Id && !x.IsDeleted).First();
                    if (post.PostType == (int)PostTypes.Incident)
                    {

                        List<RiskLevelRange> RiskLevelRanges = ctx.RiskLevelRange.Where(x => !x.IsDeleted).ToList();
                        var LikesCount = ctx.Likes.Where(x => x.Post_Id == Post_Id && x.IsDeleted == false).Count();
                        var CommentsCount = ctx.Comments.Where(x => x.Post_Id == Post_Id && x.IsDeleted == false).Count();
                        var ShareCount = ctx.Shares.Where(x => x.Post_Id == Post_Id && x.IsDeleted == false).Count();
                        int total = LikesCount + CommentsCount + ShareCount + 1;

                        if (RiskLevelRanges.Where(x => x.From <= total && x.Text.ToLower() == "high").Count() > 0)
                        {
                            post.RiskLevel = (int)RiskLevelTypes.High;
                        }
                        else if (RiskLevelRanges.Where(x => x.From <= total && x.To >= total && x.Text.ToLower() == "medium").Count() > 0)
                        {
                            post.RiskLevel = (int)RiskLevelTypes.Medium;
                        }
                        else
                        {
                            post.RiskLevel = (int)RiskLevelTypes.Low;
                        }
                    }
                    // code for risk level --- END ---

                    ctx.SaveChanges();


                    if (!string.IsNullOrEmpty(model.ImageUrls))
                    {
                        var ImageUrls = model.ImageUrls.Split(',');
                        foreach (var ImageUrl in ImageUrls)
                        {
                            Media media = new Media
                            {
                                Type = (int)MediaTypes.Image,
                                Url = ImageUrl,
                                Post_Id = model.Post_Id,
                                CreatedDate = DateTime.UtcNow,
                                User_Id = userId,
                                Comment_Id = comment.Id
                            };
                            ctx.Medias.Add(media);
                            ctx.SaveChanges();
                        }
                    }

                    if (!string.IsNullOrEmpty(model.VideoUrls))
                    {
                        var VideoUrls = model.VideoUrls.Split(',');
                        foreach (var VideoUrl in VideoUrls)
                        {
                            Media media = new Media
                            {
                                Type = (int)MediaTypes.Video,
                                Url = VideoUrl,
                                Post_Id = model.Post_Id,
                                CreatedDate = DateTime.UtcNow,
                                User_Id = userId,
                                Comment_Id = comment.Id
                            };
                            ctx.Medias.Add(media);
                        }
                        ctx.SaveChanges();
                    }


                    //comment=ctx.Comments.Include(x=>x.Medias).FirstOrDefault(x=>x.Id==)

                    SetTrends(Text: comment.Text, User_Id: userId, Comment_Id: comment.Id);

                    int SecondUser_Id = ctx.Posts.FirstOrDefault(x => x.Id == model.Post_Id && x.IsDeleted == false).User_Id;
                    SetTopFollowerLog(FirstUser_Id: userId, SecondUser_Id: SecondUser_Id);
                    SetActivityLog(FirstUser_Id: userId, SecondUser_Id: SecondUser_Id, EntityType: (int)RiscoEntityTypes.CommentOnPost, EntityId: model.Post_Id);

                    var CommentPost = ctx.Comments.Include(x => x.Post).FirstOrDefault(x => x.Id == comment.Id);

                    //only send comment noti when logged in user and post creator are not same
                    if (CommentPost != null && userId != CommentPost.Post.User_Id)
                    {
                        if (CommentPost.Post.Group_Id.HasValue)
                            Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.Group, EntityId: model.Post_Id, ReceivingUser_Id: SecondUser_Id, SendingUser_Id: userId, ChildEntityType: (int)RiscoEntityTypes.CommentOnPost);
                        else
                            Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.CommentOnPost, EntityId: model.Post_Id, ReceivingUser_Id: SecondUser_Id, SendingUser_Id: userId);
                    }

                    comment.User = ctx.Users.FirstOrDefault(x => x.Id == comment.User_Id);

                    CustomResponse<Comment> response = new CustomResponse<Comment>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = comment
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

        [HttpPost]
        [Route("CommentReply")]
        public async Task<IHttpActionResult> CommentReply(CreateCommentBindingModel model)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                List<int> UsersToNotify = new List<int>();

                using (RiscoContext ctx = new RiscoContext())
                {
                    Comment ParentComment = new Comment();

                    Comment comment = new Comment
                    {
                        Text = model.Text,
                        Post_Id = model.Post_Id,
                        ParentComment_Id = model.ParentComment_Id,
                        User_Id = userId,
                        CreatedDate = DateTime.UtcNow,
                    };

                    ctx.Comments.Add(comment);

                    // code for risk level --- START ---
                    int Post_Id = model.Post_Id;
                    Post post = ctx.Posts.Where(x => x.Id == Post_Id && !x.IsDeleted).First();
                    if (post.PostType == (int)PostTypes.Incident)
                    {
                        List<RiskLevelRange> RiskLevelRanges = ctx.RiskLevelRange.Where(x => !x.IsDeleted).ToList();
                        var LikesCount = ctx.Likes.Where(x => x.Post_Id == Post_Id && x.IsDeleted == false).Count();
                        var CommentsCount = ctx.Comments.Where(x => x.Post_Id == Post_Id && x.IsDeleted == false).Count();
                        var ShareCount = ctx.Shares.Where(x => x.Post_Id == Post_Id && x.IsDeleted == false).Count();
                        int total = LikesCount + CommentsCount + ShareCount + 1;

                        if (RiskLevelRanges.Where(x => x.From <= total && x.Text.ToLower() == "high").Count() > 0)
                        {
                            post.RiskLevel = (int)RiskLevelTypes.High;
                        }
                        else if (RiskLevelRanges.Where(x => x.From <= total && x.To >= total && x.Text.ToLower() == "medium").Count() > 0)
                        {
                            post.RiskLevel = (int)RiskLevelTypes.Medium;
                        }
                        else
                        {
                            post.RiskLevel = (int)RiskLevelTypes.Low;
                        }
                    }
                    // code for risk level --- END ---

                    ctx.SaveChanges();



                    if (!string.IsNullOrEmpty(model.ImageUrls))
                    {
                        var ImageUrls = model.ImageUrls.Split(',');
                        foreach (var ImageUrl in ImageUrls)
                        {
                            Media media = new Media
                            {
                                Type = (int)MediaTypes.Image,
                                Url = ImageUrl,
                                Post_Id = model.Post_Id,
                                CreatedDate = DateTime.UtcNow,
                                User_Id = userId,
                                Comment_Id = comment.Id
                            };
                            ctx.Medias.Add(media);
                            ctx.SaveChanges();
                        }
                    }

                    if (!string.IsNullOrEmpty(model.VideoUrls))
                    {
                        var VideoUrls = model.VideoUrls.Split(',');
                        foreach (var VideoUrl in VideoUrls)
                        {
                            Media media = new Media
                            {
                                Type = (int)MediaTypes.Video,
                                Url = VideoUrl,
                                Post_Id = model.Post_Id,
                                CreatedDate = DateTime.UtcNow,
                                User_Id = userId,
                                Comment_Id = comment.Id
                            };
                            ctx.Medias.Add(media);
                        }
                        ctx.SaveChanges();
                    }


                    SetTrends(Text: comment.Text, User_Id: userId, Comment_Id: comment.Id);

                    int SecondUser_Id = ctx.Posts.FirstOrDefault(x => x.Id == model.Post_Id && x.IsDeleted == false).User_Id;
                    SetTopFollowerLog(FirstUser_Id: userId, SecondUser_Id: SecondUser_Id);

                    UsersToNotify.Add(SecondUser_Id);

                    //   Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.ReplyOnComment, EntityId: model.Post_Id, ChildCommentId: comment.Id, ReceivingUser_Id: comment.Post.User_Id, SendingUser_Id: userId);

                    if (comment.ParentComment_Id != 0)
                    {
                        ParentComment = ctx.Comments.Include(x => x.Post).FirstOrDefault(x => x.Id == comment.ParentComment_Id && !x.IsDeleted);

                        if (userId != ParentComment.User_Id)
                        {
                            if (!UsersToNotify.Contains(ParentComment.User_Id))
                                UsersToNotify.Add(ParentComment.User_Id);

                            if (comment.Post.Group_Id.HasValue)
                                Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.Group, EntityId: model.Post_Id, ChildCommentId: comment.Id, ReceivingUser_Id: ParentComment.User_Id, SendingUser_Id: userId, ChildEntityType: (int)RiscoEntityTypes.ReplyOnComment);
                            else
                                Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.ReplyOnComment, EntityId: model.Post_Id, ChildCommentId: comment.Id, ReceivingUser_Ids: UsersToNotify, SendingUser_Id: userId, CommentId: comment.Id);

                        }

                    }
                    comment.User = ctx.Users.FirstOrDefault(x => x.Id == comment.User_Id);

                    CustomResponse<Comment> response = new CustomResponse<Comment>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = comment
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

        [HttpPost]
        [Route("Repost")]
        public async Task<IHttpActionResult> Repost(SharePostBindingModel model)
        {
            try
            {
                Validate(model);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                List<int> UsersToNotify = new List<int>();
                List<int> PostCategories = new List<int>();
                using (RiscoContext ctx = new RiscoContext())
                {

                    Post _post = ctx.Posts.Where(x => x.Id == model.SharePost_Id && !x.IsDeleted).Include(x => x.PostCategories).FirstOrDefault();

                    Post post = new Post
                    {
                        Text = model.Text,
                        RiskLevel = model.RiskLevel,
                        Location = model.Location,
                        Visibility = model.Visibility,
                        User_Id = userId,
                        CreatedDate = DateTime.UtcNow,
                        Latitude = model.Latitude,
                        Longitude = model.Longitude,
                        IsExpired = false,
                        SharePost_Id = model.SharePost_Id,
                        PostType = _post.PostType,
                        Language = _post.Language,
                        ParentPost_Id = null
                    };



                    if (model.Group_Id != 0 && model.Group_Id.HasValue)
                        post.Group_Id = model.Group_Id;

                    ctx.Posts.Add(post);
                    ctx.SaveChanges();

                    // start of setting post's categories
                    if (!string.IsNullOrEmpty(model.Interests))
                    {
                        PostCategories = model.Interests.Split(',').Select(int.Parse).ToList();
                    }
                    foreach (var item in _post.PostCategories)
                    {

                        if (!item.IsDeleted)
                        {
                            if (PostCategories.Contains(item.Id))
                                PostCategories.Remove(item.Id);
                            post.PostCategories.Add(new DAL.PostCategories
                            {
                                Interest_Id = item.Interest_Id,
                                Post_Id = post.Id,
                                IsDeleted = false
                            });
                        }
                    }
                    foreach (var item in PostCategories)
                    {
                        post.PostCategories.Add(new DAL.PostCategories
                        {
                            Interest_Id = item,
                            Post_Id = post.Id,
                            IsDeleted = false
                        });
                    }

                    ctx.SaveChanges();

                    // end of setting post's categories


                    Share share = new Share
                    {
                        Post_Id = model.SharePost_Id,
                        User_Id = userId,
                        CreatedDate = DateTime.UtcNow,
                    };

                    ctx.Shares.Add(share);

                    // code for risk level --- START ---            

                    if (model.PostType == (int)PostTypes.Incident)
                    {

                        int Post_Id = model.SharePost_Id;
                        List<RiskLevelRange> RiskLevelRanges = ctx.RiskLevelRange.Where(x => !x.IsDeleted).ToList();
                        var LikesCount = ctx.Likes.Where(x => x.Post_Id == Post_Id && x.IsDeleted == false).Count();
                        var CommentsCount = ctx.Comments.Where(x => x.Post_Id == Post_Id && x.IsDeleted == false).Count();
                        var ShareCount = ctx.Shares.Where(x => x.Post_Id == Post_Id && x.IsDeleted == false).Count();
                        int total = LikesCount + CommentsCount + ShareCount + 1;
                        post = ctx.Posts.Where(x => x.Id == Post_Id && !x.IsDeleted).First();
                        if (RiskLevelRanges.Where(x => x.From <= total && x.Text.ToLower() == "high").Count() > 0)
                        {
                            post.RiskLevel = (int)RiskLevelTypes.High;
                        }
                        else if (RiskLevelRanges.Where(x => x.From <= total && x.To >= total && x.Text.ToLower() == "medium").Count() > 0)
                        {
                            post.RiskLevel = (int)RiskLevelTypes.Medium;
                        }
                        else
                        {
                            post.RiskLevel = (int)RiskLevelTypes.Low;
                        }

                    }
                    // code for risk level --- END ---

                    ctx.SaveChanges();

                    SetTrends(Text: post.Text, User_Id: userId, Post_Id: post.Id);

                    if (!string.IsNullOrEmpty(model.ImageUrls))
                    {
                        var ImageUrls = model.ImageUrls.Split(',');
                        foreach (var ImageUrl in ImageUrls)
                        {
                            Media media = new Media
                            {
                                Type = (int)MediaTypes.Image,
                                Url = ImageUrl,
                                Post_Id = post.Id,
                                CreatedDate = DateTime.UtcNow,
                                User_Id = userId
                            };
                            ctx.Medias.Add(media);
                            ctx.SaveChanges();
                        }
                    }

                    if (!string.IsNullOrEmpty(model.VideoUrls))
                    {
                        var VideoUrls = model.VideoUrls.Split(',');
                        foreach (var VideoUrl in VideoUrls)
                        {
                            Media media = new Media
                            {
                                Type = (int)MediaTypes.Video,
                                Url = VideoUrl,
                                Post_Id = post.Id,
                                CreatedDate = DateTime.UtcNow,
                                User_Id = userId
                            };
                            ctx.Medias.Add(media);
                        }
                        ctx.SaveChanges();
                    }

                    if (post.Visibility == (int)PostVisibilityTypes.OnlyMe)
                    {

                    }
                    else if (post.Visibility == (int)PostVisibilityTypes.Follower)
                    {
                        UsersToNotify.AddRange(ctx.FollowFollowers.Where(x => x.SecondUser_Id == userId).Select(x => x.FirstUser_Id).ToList());

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
                        }
                        Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.SharePost, EntityId: post.Id, ReceivingUser_Ids: UsersToNotify, SendingUser_Id: userId, ToDifferentUsers: true);
                    }
                    else
                    {
                        UsersToNotify.AddRange(ctx.FollowFollowers.Where(x => x.SecondUser_Id == userId).Select(x => x.FirstUser_Id).ToList());
                        UsersToNotify.AddRange(ctx.FollowFollowers.Where(x => x.FirstUser_Id == userId).Select(x => x.SecondUser_Id).ToList());

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
                        }
                        Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.SharePost, EntityId: post.Id, ReceivingUser_Ids: UsersToNotify, SendingUser_Id: userId, ToDifferentUsers: true);
                    }

                    SetActivityLog(FirstUser_Id: userId, SecondUser_Id: post.User_Id, EntityType: (int)RiscoEntityTypes.SharePost, EntityId: model.SharePost_Id);

                    // post = ctx.Posts.Include(x=>x.User).Include(x => x.PostCategories).Include(x => x.Shares).Include(x => x.Medias).Include(x => x.SharedParent).FirstOrDefault(x => x.Id == post.Id);
                    post = ctx.Posts
                        .Include(x => x.User)
                        .Include(x => x.Medias)
                        .Include(x => x.PostCategories)
                        .Include(x => x.Shares)
                        .Include(x => x.SharedParent)
                        .Include(x => x.SharedParent.Medias)
                        .FirstOrDefault(x => x.Id == post.Id);

                    CustomResponse<Post> response = new CustomResponse<Post>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = post
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        #endregion




        /*
        [HttpPost]
        [Route("CreatePost")]
        public async Task<IHttpActionResult> CreatePost(CreatePostBindingModel model)
        {
            try
            {
                Validate(model);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                List<int> UsersToNotify = new List<int>();
                List<int> PostCategories = new List<int>();
                List<Languages> Languages = new List<Languages>();
                List<string> UserLanguageCodes = new List<string>();

                using (RiscoContext ctx = new RiscoContext())
                {
                    Post post = new Post
                    {
                        Text = model.Text[0],
                        RiskLevel = 0,
                        Location = model.Location,
                        Visibility = model.Visibility,
                        User_Id = userId,
                        CreatedDate = DateTime.UtcNow,
                        Latitude = model.Latitude,
                        Longitude = model.Longitude,
                        IsExpired = false,
                        PostType = model.PostType,
                        Language = model.Language
                    };

                    if (model.ExpireAfterHours != 0)
                        post.PollExpiryTime = DateTime.UtcNow.AddHours(model.ExpireAfterHours);

                    if (model.IsPoll)
                    {
                        post.IsPoll = model.IsPoll;
                        post.PollType = model.PollType;
                        foreach (var item in model.PollOptions)
                        {
                            if (string.IsNullOrEmpty(item.Title) && string.IsNullOrEmpty(item.MediaUrl))
                                continue;

                            post.PollOptions.Add(new DAL.PollOptions
                            {
                                IsDeleted = false,
                                MediaUrl = item.MediaUrl,
                                Percentage = 0,
                                Votes = 0,
                                Title = item.Title
                            });
                        }
                    }

                    if (model.Group_Id != 0 && model.Group_Id.HasValue)
                        post.Group_Id = model.Group_Id;

                    // start of setting post's categories
                    if (!string.IsNullOrEmpty(model.Interests))
                    {
                        PostCategories = model.Interests.Split(',').Select(int.Parse).ToList();
                    }
                    if (PostCategories.Count > 0)
                    {
                        /// List<PostCategories> Categories = new List<DAL.PostCategories>();
                        foreach (var item in PostCategories)
                        {
                            post.PostCategories.Add(new DAL.PostCategories
                            {
                                Interest_Id = item,
                                Post_Id = post.Id,
                                IsDeleted = false
                            });
                        }
                        //post.PostCategories.AddRange(Categories);
                    }

                    // start of setting post's languages                    
                    //if (!string.IsNullOrEmpty(model.Language))
                    //{
                    //    UserLanguageCodes = model.Language.Split(',').Select(x => x).ToList();
                    //    Languages =
                    //        ctx.Languages.Where(y => UserLanguageCodes.Contains(y.Code) && !y.IsDeleted).ToList();

                    //}
                    //if (Languages.Count > 0)
                    //{
                    //    foreach (var item in Languages)
                    //    {
                    //        post.PostLanguageCodes.Add(new DAL.PostLanguageCodes
                    //        {
                    //            Language_Id = item.Id,
                    //            Post_Id = post.Id,
                    //            IsDeleted = false
                    //        });
                    //    }
                    //}


                    if (post.SharePost_Id == 0)
                        post.SharePost_Id = null;

                    ctx.Posts.Add(post);
                    ctx.SaveChanges();

                    // end of setting post's categories

                    SetTrends(Text: post.Text, User_Id: userId, Post_Id: post.Id);

                    if (!string.IsNullOrEmpty(model.ImageUrls))
                    {
                        var ImageUrls = model.ImageUrls.Split(',');
                        foreach (var ImageUrl in ImageUrls)
                        {
                            Media media = new Media
                            {
                                Type = (int)MediaTypes.Image,
                                Url = ImageUrl,
                                Post_Id = post.Id,
                                CreatedDate = DateTime.UtcNow,
                                User_Id = userId
                            };
                            ctx.Medias.Add(media);
                            ctx.SaveChanges();
                        }
                    }                    

                    if (!string.IsNullOrEmpty(model.VideoUrls))
                    {
                        var VideoUrls = model.VideoUrls.Split(',');
                        foreach (var VideoUrl in VideoUrls)
                        {
                            string videoPath = VideoUrl;
                            string videoFullPath = HttpContext.Current.Server.MapPath("~/" + videoPath);                                                                                    

                            var ffMpeg = new NReco.VideoConverter.FFMpegConverter();

                            string thumbNailMovieImagePath = ConfigurationManager.AppSettings["PostMediaFolderPath"] + DateTime.Now.Ticks.ToString() + ".jpg";
                            string thumbNailMovieFullImagePath = HttpContext.Current.Server.MapPath("~/" + thumbNailMovieImagePath);

                            ffMpeg.GetVideoThumbnail(videoFullPath, thumbNailMovieFullImagePath, 1);
                            
                            Media media = new Media
                            {
                                Type = (int)MediaTypes.Video,
                                Url = videoPath,
                                ThumbnailUrl = thumbNailMovieImagePath,
                                Post_Id = post.Id,
                                CreatedDate = DateTime.UtcNow,
                                User_Id = userId
                            };
                            ctx.Medias.Add(media);
                        }
                        ctx.SaveChanges();
                    }

                    // send notification to your followings ( jinho na apko follow kea ha unha ) 

                    if (post.Visibility == (int)PostVisibilityTypes.OnlyMe)
                    {
                        //UsersToNotify.AddRange(ctx.FollowFollowers.Where(x => x.SecondUser_Id == userId).Select(x => x.FirstUser_Id).ToList());
                        //Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.SharePost, EntityId: post.Id, ReceivingUser_Ids: UsersToNotify, SendingUser_Id: userId, ToDifferentUsers: true);
                    }
                    else if (post.Visibility == (int)PostVisibilityTypes.Follower)
                    {
                        UsersToNotify.AddRange(ctx.FollowFollowers.Where(x => x.SecondUser_Id == userId).Select(x => x.FirstUser_Id).ToList());

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
                        Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.Post, EntityId: post.Id, ReceivingUser_Ids: UsersToNotify, SendingUser_Id: userId, ToDifferentUsers: true);
                    }
                    else
                    {
                        UsersToNotify.AddRange(ctx.FollowFollowers.Where(x => x.SecondUser_Id == userId).Select(x => x.FirstUser_Id).ToList());
                        UsersToNotify.AddRange(ctx.FollowFollowers.Where(x => x.FirstUser_Id == userId).Select(x => x.SecondUser_Id).ToList());

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
                        Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.Post, EntityId: post.Id, ReceivingUser_Ids: UsersToNotify, SendingUser_Id: userId, ToDifferentUsers: true);
                    }

                    //Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.Post, EntityId: post.Id, ReceivingUser_Ids: UsersToNotify, SendingUser_Id: userId, ToDifferentUsers: true);


                    SetActivityLog(FirstUser_Id: userId, SecondUser_Id: post.User_Id, EntityType: (int)RiscoEntityTypes.Post, EntityId: post.Id);
                    //var MyFollowers = ctx.FollowFollowers.Where(x => x.SecondUser_Id == userId).Select(x => x.FirstUser_Id).ToList();

                    //if (MyFollowers != null)
                    //{
                    //    UsersToNotify.AddRange(MyFollowers);

                    //    Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.Post, EntityId: post.Id, ReceivingUser_Ids: UsersToNotify, SendingUser_Id: userId, ToDifferentUsers: true);
                    //}
                    post = ctx.Posts.Include(x => x.User).Include(x => x.Medias).Include(x => x.PostCategories).FirstOrDefault(x => x.Id == post.Id);
                    CustomResponse<Post> response = new CustomResponse<Post>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = post
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }
        */
        /*
        [HttpPost]
        [Route("CreatePost")]
        public async Task<IHttpActionResult> CreatePost(CreatePostBindingModel model)
        {
            try
            {
                Validate(model);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                List<int> UsersToNotify = new List<int>();
                List<int> PostCategories = new List<int>();
                List<Languages> Languages = new List<Languages>();
                List<string> UserLanguageCodes = new List<string>();

                int ParentPostId = 0;
                int Sequence = 1;
                using (RiscoContext ctx = new RiscoContext())
                {
                    foreach (var text in model.Texts)
                    {
                        Post post = new Post
                        {
                            Text = text,
                            RiskLevel = model.RiskLevel,
                            Location = model.Location,
                            Visibility = model.Visibility,
                            User_Id = userId,
                            CreatedDate = DateTime.UtcNow,
                            Latitude = model.Latitude,
                            Longitude = model.Longitude,
                            IsExpired = false,
                            PostType = model.PostType,
                            Language = model.Language
                        };

                        post.ParentPost_Id = ParentPostId > 0 ? ParentPostId : null as int?;
                        post.Sequence = model.Text.Count() > 1 ? Sequence : null as int?;

                        if (model.ExpireAfterHours != 0)
                            post.PollExpiryTime = DateTime.UtcNow.AddHours(model.ExpireAfterHours);


                        if (model.IsPoll)
                        {
                            post.IsPoll = model.IsPoll;
                            post.PollType = model.PollType;
                            foreach (var item in model.PollOptions)
                            {
                                if (string.IsNullOrEmpty(item.Title) && string.IsNullOrEmpty(item.MediaUrl))
                                    continue;

                                post.PollOptions.Add(new DAL.PollOptions
                                {
                                    IsDeleted = false,
                                    MediaUrl = item.MediaUrl,
                                    Percentage = 0,
                                    Votes = 0,
                                    Title = item.Title
                                });
                            }
                        }

                        if (model.Group_Id != 0 && model.Group_Id.HasValue)
                            post.Group_Id = model.Group_Id;


                        // start of setting post's categories
                        if (!string.IsNullOrEmpty(model.Interests))
                        {
                            PostCategories = model.Interests.Split(',').Select(int.Parse).ToList();
                        }
                        if (PostCategories.Count > 0)
                        {
                            /// List<PostCategories> Categories = new List<DAL.PostCategories>();
                            foreach (var item in PostCategories)
                            {
                                post.PostCategories.Add(new DAL.PostCategories
                                {
                                    Interest_Id = item,
                                    Post_Id = post.Id,
                                    IsDeleted = false
                                });
                            }
                            //post.PostCategories.AddRange(Categories);
                        }
                        if (post.SharePost_Id == 0)
                            post.SharePost_Id = null;

                        ctx.Posts.Add(post);
                        ctx.SaveChanges();

                        // end of setting post's categories

                        if (Sequence == 1)
                        {
                            ParentPostId = post.Id;
                            SetTrends(Text: post.Text, User_Id: userId, Post_Id: post.Id);

                            if (!string.IsNullOrEmpty(model.ImageUrls))
                            {
                                var ImageUrls = model.ImageUrls.Split(',');
                                foreach (var ImageUrl in ImageUrls)
                                {
                                    Media media = new Media
                                    {
                                        Type = (int)MediaTypes.Image,
                                        Url = ImageUrl,
                                        Post_Id = post.Id,
                                        CreatedDate = DateTime.UtcNow,
                                        User_Id = userId
                                    };
                                    ctx.Medias.Add(media);
                                    ctx.SaveChanges();
                                }
                            }

                            if (!string.IsNullOrEmpty(model.VideoUrls))
                            {
                                var VideoUrls = model.VideoUrls.Split(',');
                                foreach (var VideoUrl in VideoUrls)
                                {
                                    string videoPath = VideoUrl;
                                    string videoFullPath = HttpContext.Current.Server.MapPath("~/" + videoPath);

                                    var ffMpeg = new NReco.VideoConverter.FFMpegConverter();

                                    string thumbNailMovieImagePath = ConfigurationManager.AppSettings["PostMediaFolderPath"] + DateTime.Now.Ticks.ToString() + ".jpg";
                                    string thumbNailMovieFullImagePath = HttpContext.Current.Server.MapPath("~/" + thumbNailMovieImagePath);

                                    ffMpeg.GetVideoThumbnail(videoFullPath, thumbNailMovieFullImagePath, 1);

                                    Media media = new Media
                                    {
                                        Type = (int)MediaTypes.Video,
                                        Url = videoPath,
                                        ThumbnailUrl = thumbNailMovieImagePath,
                                        Post_Id = post.Id,
                                        CreatedDate = DateTime.UtcNow,
                                        User_Id = userId
                                    };
                                    ctx.Medias.Add(media);
                                }
                                ctx.SaveChanges();
                            }

                            if (post.Visibility == (int)PostVisibilityTypes.Follower || post.Visibility == (int)PostVisibilityTypes.Public)
                            {
                                UsersToNotify.AddRange(
                                            from u in ctx.Users
                                            join ff in ctx.FollowFollowers on u.Id equals ff.FirstUser_Id
                                            join fu in ctx.Users on ff.SecondUser_Id equals fu.Id
                                            join bbu in ctx.BlockUsers on u.Id equals bbu.FirstUser_Id into bbu1
                                            from bbu in bbu1.DefaultIfEmpty()
                                            join bu in ctx.BlockUsers on u.Id equals bu.SecondUser_Id into bu1
                                            from bu in bu1.DefaultIfEmpty()
                                            join mu in ctx.MuteUser on u.Id equals mu.SecondUser_Id into mu1
                                            from mu in mu1.DefaultIfEmpty()
                                            where
                                            u.Id == userId &&
                                            bbu.SecondUser_Id != ff.SecondUser_Id &&
                                            bu.FirstUser_Id != ff.SecondUser_Id &&
                                            mu.FirstUser_Id != fu.Id
                                            select fu.Id);
                                Global.GenerateNotification(EntityType: (int)RiscoEntityTypes.Post, EntityId: post.Id, ReceivingUser_Ids: UsersToNotify, SendingUser_Id: userId, ToDifferentUsers: true);
                            }

                            SetActivityLog(FirstUser_Id: userId, SecondUser_Id: post.User_Id, EntityType: (int)RiscoEntityTypes.Post, EntityId: post.Id);
                        }
                        Sequence++;
                    }

                    Post postResponse = new Post();
                    postResponse = ctx.Posts.Include(x => x.User).Include(x => x.Medias).Include(x => x.PostCategories).FirstOrDefault(x => x.Id == ParentPostId);
                    CustomResponse<Post> response = new CustomResponse<Post>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = postResponse
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }
        */




        private Post includePostDetail(RiscoContext ctx, Post post)
        {
            List<int> TheyBlocked = new List<int>();
            int Post_Id = post.Id;
            var userId = Convert.ToInt32(User.GetClaimValue("userid"));
            if (post.User == null)
            {
                post.User = ctx.Users.FirstOrDefault(x => x.Id == post.User_Id);
            }

            TheyBlocked = ctx.BlockUsers.Where(x => x.SecondUser_Id == userId).Select(x => x.FirstUser_Id).ToList();

            if (post.IsPoll)
                post.CheckPollExpiry();

            foreach (var poll in post.PollOptions)
            {
                if (ctx.PollOptionVote.Any(x => x.PollOption_Id == poll.Id && x.User_Id == userId && !x.IsDeleted))
                    poll.IsVoted = true;
            }


            post.IsLiked = ctx.Likes.Any(x => x.Post_Id == Post_Id && x.User_Id == userId && x.IsDeleted == false);
            post.LikesCount = ctx.Posts.Sum(p => p.Likes.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
            post.CommentsCount = ctx.Posts.Sum(p => p.Comments.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
            post.ShareCount = ctx.Posts.Sum(p => p.Shares.Where(x => x.Post_Id == post.Id && x.IsDeleted == false).Count());
            post.IsUserFollow = ctx.FollowFollowers.Any(x => x.FirstUser_Id == userId && x.SecondUser_Id == post.User_Id && x.IsDeleted == false);
            post.IsPostOwner = (post.User_Id == userId) ? true : false;

            // For comments and their child comments including self-like

            List<Comment> comments = ctx.Comments
            .Include(x => x.Medias)
            .Include(x => x.User)
            .Include(x => x.Likes)
                .Where(x => x.Post_Id == Post_Id && x.ParentComment_Id == 0 && !TheyBlocked.Contains(x.User_Id) && x.IsDeleted == false).ToList();

            foreach (Comment comment in comments)
            {
                comment.LikesCount = comment.Likes.Where(o => !o.IsDeleted).Count();
                comment.Likes = null;
                comment.ChildComments = ctx.Comments
                    .Include(x => x.User)
                    .Include(x => x.Medias)
                    .Include(x => x.Likes)
                    .Where(x => x.ParentComment_Id == comment.Id && !TheyBlocked.Contains(x.User_Id) && x.IsDeleted == false).ToList();
                comment.ReplyCount = comment.ChildComments.Count;
                comment.IsLiked = ctx.Likes.Any(x => x.Comment_Id == comment.Id && x.User_Id == userId && x.IsDeleted == false);
                foreach (Comment childComment in comment.ChildComments)
                {
                    childComment.IsLiked = ctx.Likes.Any(x => x.Comment_Id == childComment.Id && x.User_Id == userId && x.IsDeleted == false);
                    childComment.LikesCount = childComment.Likes.Where(o => !o.IsDeleted).Count();
                    childComment.Likes = null;
                }
            }

            post.Comments = comments;

            post.Medias = post.Medias.Where(x => x.Comment_Id == null).ToList();

            return post;
        }











        #region Private Regions

        private void SetTrends(string Text, int User_Id, int Post_Id = 0, int Comment_Id = 0)
        {
            try
            {
                var regex = new Regex(@"(?<=#)\w+");
                var matches = regex.Matches(Text);

                using (RiscoContext ctx = new RiscoContext())
                {
                    TrendLog trendLog;
                    foreach (Match m in matches)
                    {
                        trendLog = new TrendLog
                        {
                            Text = "#" + m.Value,
                            User_Id = User_Id,
                            Post_Id = Post_Id,
                            Comment_Id = Comment_Id,
                            CreatedDate = DateTime.UtcNow,
                        };
                        ctx.TrendLogs.Add(trendLog);
                    }

                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }

        private void SetTopFollowerLog(int FirstUser_Id, int SecondUser_Id)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    if (FirstUser_Id != SecondUser_Id)
                    {
                        TopFollowerLog topFollowerLog = new TopFollowerLog
                        {
                            FirstUser_Id = FirstUser_Id,
                            SecondUser_Id = SecondUser_Id,
                            CreatedDate = DateTime.UtcNow
                        };

                        ctx.TopFollowerLogs.Add(topFollowerLog);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }

        #endregion
    }
}
