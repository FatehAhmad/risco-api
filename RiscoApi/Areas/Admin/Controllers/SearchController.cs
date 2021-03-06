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
using BLL.Utility;

namespace BasketApi.Areas.SubAdmin.Controllers
{
    [BasketApi.Authorize("SubAdmin", "SuperAdmin", "ApplicationAdmin", "User", "Guest")]
    [RoutePrefix("api/Search")]
    public class SearchController : ApiController
    {
        [HttpGet]
        [Route("Search")]
        public async Task<IHttpActionResult> Search(string SearchText = "", int PageSize = 10, int PageNo = 0)
        {
            try
            {
                SearchText = SearchText.Replace(" ", "");
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                using (RiscoContext ctx = new RiscoContext())
                {

                    bool IsPhoneSearch = false;
                    bool IsEmailSearch = false;
                    bool SearchAll = false;
                    List<User> users = new List<User>();

                    System.Text.RegularExpressions.Match NameRegexMatch = System.Text.RegularExpressions.Regex.Match(SearchText, @"^[a-zA-Z]+(([',. -][a-zA-Z ])?[a-zA-Z]*)*$");
                    if (NameRegexMatch.Success)
                        SearchAll = true;

                    //System.Text.RegularExpressions.Match EmailRegexMatch = System.Text.RegularExpressions.Regex.Match(SearchText, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    System.Text.RegularExpressions.Match EmailRegexMatch = System.Text.RegularExpressions.Regex.Match(SearchText, @"^([\w\.\-]+)@([\w\-]+)((|\.(\w){2,3})+)$");
                    if (EmailRegexMatch.Success)
                        IsEmailSearch = true;

                    if (!SearchAll && !IsEmailSearch)
                        IsPhoneSearch = true;


                    var BlockedListUsers = ctx.BlockUsers.Where(x => x.SecondUser_Id == userId && !x.IsDeleted).Select(x => x.FirstUser_Id).Distinct().ToList();

                    if (BlockedListUsers == null)
                        BlockedListUsers = new List<int>();

                    if (IsEmailSearch)
                    {
                        users = ctx.Users
                           .Where(x => x.IsDeleted == false && !BlockedListUsers.Contains(x.Id) &&  x.Id != userId && (x.FindByEmail && x.Email.Trim().ToLower().Contains(SearchText.Trim().ToLower())))
                           .ToList();

                    }
                    else if (IsPhoneSearch)
                    {

                        SearchText = SearchText.Replace("+", "");
                        //string query = "select * from Users Where IsDeleted=='0' AND ";
                        //List<TrendBindingModel> trends = ctx.Database.SqlQuery<TrendBindingModel>(query).ToList();
                        users = ctx.Users
                          .Where(x => x.IsDeleted == false && x.Id != userId && !BlockedListUsers.Contains(x.Id) && (x.FindByPhone && (x.Phone.Replace("+","").Contains(SearchText) || x.Phone == SearchText)))
                          .ToList();
                    }
                    else
                    {
                        users = ctx.Users
                           .Where(x => x.IsDeleted == false && x.Id != userId && !BlockedListUsers.Contains(x.Id) && (x.FullName.Contains(SearchText) || (x.FindByEmail && x.Email.Contains(SearchText)) || (x.FindByPhone && x.Phone.Contains(SearchText))))
                           .ToList();
                    }


                    List<Group> groups = ctx.Groups
                        .Where(x => x.IsDeleted == false && x.Name.Contains(SearchText))
                        .ToList();

                    List<int> postIds = ctx.TrendLogs.Where(x => x.Text.Contains(SearchText)).Select(x => x.Post_Id).ToList();

                    List<Post> posts = ctx.Posts
                        .Where(x => x.IsDeleted == false && postIds.Contains(x.Id))
                        .ToList();

                    users = users.Skip((PageNo - 1) * PageSize).Take(PageSize).ToList();
                    groups = groups.Skip((PageNo - 1) * PageSize).Take(PageSize).ToList();
                    posts = posts.Skip((PageNo - 1) * PageSize).Take(PageSize).ToList();

                    CustomResponse<SearchListViewModel> response = new CustomResponse<SearchListViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new SearchListViewModel
                        {
                            Users = users,
                            Groups = groups,
                            Posts = posts
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
    }
}
