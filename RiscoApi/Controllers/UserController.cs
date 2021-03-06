using DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Nexmo.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using BasketApi.CustomAuthorization;
using BasketApi.Models;
using BasketApi.ViewModels;
using System.IO;
using System.Configuration;
using System.Data.Entity;
using System.Net.Mail;
using static BasketApi.Global;
using BasketApi.Components.Helpers;
using System.Web.Hosting;
using System.Web.Http.Cors;
using WebApplication1.BindingModels;
//using static BasketApi.Utility;
using WebApplication1.ViewModels;
using WebApplication1.Areas.Admin.ViewModels;
using Newtonsoft.Json;
using static DAL.CustomModels.Enumeration;
using BLL.Utility;
using DAL.CustomModels;
using WebApplication1.Models;

namespace BasketApi.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        private ApplicationUserManager _userManager;

        [Route("all")]
        public IHttpActionResult Getall()
        {
            try
            {
                //var nexmoVerifyResponse = NumberVerify.Verify(new NumberVerify.VerifyRequest { brand = "INGIC", number = "+923325345126" });
                //var nexmoCheckResponse = NumberVerify.Check(new NumberVerify.CheckRequest { request_id = nexmoVerifyResponse.request_id, code = "6310"});
                return Ok("Hello");
            }
            catch (Exception ex)
            {
                return Ok("Hello");
            }
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("Login")]
        [HttpPost]
        public async Task<IHttpActionResult> Login(LoginBindingModel model)
        {
            try
            {                
                if (!ModelState.IsValid)
                {
                    return (model.Platform > 0 && model.Platform < 4) ? BadRequest(ModelState) : (IHttpActionResult)BadRequest("Invalid platform!");
                }
                using (RiscoContext ctx = new RiscoContext())
                {
                    HttpRequestMessage request = new HttpRequestMessage();
                    DAL.User userModel;
                    List<int> BlockedUsers = new List<int>();
                    UserProfiles UserProfile = new UserProfiles();
                    IpStackResponseViewmodel lastLogin = new IpStackResponseViewmodel();

                    var hashedPassword = CryptoHelper.Hash(model.Password);
                    userModel = ctx.Users.Include(x => x.UserAddresses)
                        .Include(x => x.UserProfiles)
                        .Include(x => x.UserLanguageMappings.Select(y => y.Language))
                        .FirstOrDefault(x => (x.Email == model.Email || x.Phone == model.Email) && x.Password == hashedPassword);

                    if (userModel != null)
                    {
                        if (!userModel.EmailConfirmed)
                        {

                            var codeInt = new Random().Next(1111, 9999);
                            await ctx.VerifyNumberCodes.Where(x => x.User_Id == userModel.Id).ForEachAsync(x => x.IsDeleted = true);
                            await ctx.SaveChangesAsync();
                            userModel.VerifyNumberCodes.Add(new VerifyNumberCodes { Phone = userModel.Phone, CreatedAt = DateTime.Now, IsDeleted = false, User_Id = userModel.Id, Code = codeInt });
                            ctx.SaveChanges();

                            Utility.SendEmail("Email Verification", EmailTypes.EmailVerification, userModel.Id, codeInt.ToString(), userModel.Email);


                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "Forbidden",
                                StatusCode = (int)HttpStatusCode.Unauthorized,
                                Result = new Error { ErrorMessage = "Your account is not verified." }
                            });
                        }

                        if (userModel.IsDeleted)
                        {
                            if (userModel.DeActive)
                            {
                                var query = @"Update Posts SET IsDeleted='0' Where User_Id='" + userModel.Id + @"';
                                      Update Comments SET IsDeleted='0' Where User_Id='" + userModel.Id + @"';
                                      Update Likes SET IsDeleted='0' Where User_Id='" + userModel.Id + @"';
                                      Update Groups SET IsDeleted='0' Where User_Id='" + userModel.Id + @"';
                                      Update GroupMembers SET IsDeleted='0' Where User_Id='" + userModel.Id + @"';
                                      Update Shares SET IsDeleted='0' Where User_Id='" + userModel.Id + @"';
                                      Update FollowFollowers SET IsDeleted='0' Where FirstUser_Id='" + userModel.Id + @"' OR SecondUser_Id='" + userModel.Id + @"';
                                      Update Notifications SET IsDeleted='0' Where SendingUser_Id='" + userModel.Id + @"' OR ReceivingUser_Id='" + userModel.Id + @"'
                                      Update ActivityLogs SET IsDeleted='0' Where FirstUser_Id='" + userModel.Id + @"' OR SecondUser_Id='" + userModel.Id + @"'
                                      Update Users SET DeActive='0',IsDeleted='0' Where ID='" + userModel.Id + @"'";
                                ctx.Database.ExecuteSqlCommand(query);

                            }
                            else if (userModel.ReportCount >= 10)
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "Forbidden",
                                    StatusCode = (int)HttpStatusCode.Forbidden,
                                    Result = new Error { ErrorMessage = "Your account has been temporarily blocked due to abusive & spam reports.Please contact admin for further details." }
                                });

                            }
                            else
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "Forbidden",
                                    StatusCode = (int)HttpStatusCode.Forbidden,
                                    Result = new Error { ErrorMessage = "Admin has temporarily blocked your account.Kindly contact us for further assistance." }
                                });
                            }
                        }
                        var Ip = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "";

                        if (!string.IsNullOrEmpty(Ip))
                        {
                            lastLogin = Utility.GetIpDetails(Ip);

                            userModel.LastLoginTime = DateTime.UtcNow;
                            userModel.LastLoginFrom = lastLogin.city;

                            UserProfile.GetPlatformData(HttpContext.Current.Request.UserAgent);

                            if (userModel.UserProfiles.Any(x => x.Ip == UserProfile.Ip && x.User_Id == userModel.Id && x.IsBlocked))
                            {
                                return Content(HttpStatusCode.OK, new CustomResponse<Error>
                                {
                                    Message = "Forbidden",
                                    StatusCode = (int)HttpStatusCode.Forbidden,
                                    Result = new Error { ErrorMessage = "Login from this device is blocked by account owner." }
                                });
                            }
                            else if (!userModel.UserProfiles.Any(x => x.Ip == UserProfile.Ip && x.Platform.Contains(UserProfile.Platform) && x.User_Id == userModel.Id))
                            {
                                Utility.SendEmail("Device Authentication", EmailTypes.NewLogin, userModel.Id, "", userModel.Email, lastLogin.city);
                                UserProfile.User_Id = userModel.Id;
                                UserProfile.CreatedDate = DateTime.UtcNow;
                                ctx.UserProfiles.Add(UserProfile);
                            }
                        }
                        ctx.SaveChanges();
                        //var Type = HttpContext.Current.Request.Browser.Type;
                        //var t = HttpContext.Current.Request.Browser.IsMobileDevice;

                        userModel.FollowingCount = ctx.FollowFollowers.Where(x => x.FirstUser_Id == userModel.Id && x.IsDeleted == false).Count();
                        userModel.FollowersCount = ctx.FollowFollowers.Where(x => x.SecondUser_Id == userModel.Id && x.IsDeleted == false).Count();
                        userModel.PostCount = ctx.Posts.Where(x => x.User_Id == userModel.Id && x.IsDeleted == false).Count();

                        await userModel.GenerateToken(Request);
                        BasketSettings.LoadSettings();
                        userModel.BasketSettings = BasketSettings.Settings;
                        if (!String.IsNullOrEmpty(userModel.Interests))
                        {
                            var lstUserIntrests = userModel.Interests.Split(',').Select(int.Parse).ToList();
                            foreach (var intrest in userModel.BasketSettings.Interests)
                                intrest.Checked = lstUserIntrests.Contains(intrest.Id);
                        }
                        userModel.Languages = new List<Languages>();
                        if (userModel.UserLanguageMappings.Count > 0)
                        {
                            userModel.Languages = new List<Languages>();
                            foreach (var userLanguageMapping in userModel.UserLanguageMappings)
                            {
                                Languages language = new Languages
                                {
                                    Id = userLanguageMapping.Language.Id,
                                    Code = userLanguageMapping.Language.Code,
                                    CreatedDate = userLanguageMapping.Language.CreatedDate,
                                    IsDeleted = userLanguageMapping.Language.IsDeleted,
                                    Name = userLanguageMapping.Language.Name,
                                    Selected = userLanguageMapping.Language.Selected
                                };
                                userModel.Languages.Add(language);
                            }
                        }

                        if (userModel != null)
                        {
                            BlockedUsers = ctx.BlockUsers.Where(x => x.FirstUser_Id == userModel.Id).Select(x => x.SecondUser_Id).ToList();
                            BlockedUsers.AddRange(ctx.BlockUsers.Where(x => x.SecondUser_Id == userModel.Id).Select(x => x.FirstUser_Id).ToList());
                            userModel.UnreadNotifications = ctx.Notifications.Count(x => x.ReceivingUser_Id == userModel.Id && !BlockedUsers.Contains(x.SendingUser_Id.Value) && x.Status == (Int32)NotificationStatus.Unread && !x.IsDeleted);
                        }

                        RegisterPushNotificationBindingModel pnModel = new RegisterPushNotificationBindingModel();
                        pnModel.DeviceName = model.DeviceName;
                        pnModel.UDID = model.UDID;
                        pnModel.Platform = model.Platform;
                        pnModel.IsPlayStore = model.IsPlayStore;
                        pnModel.User_Id = model.User_Id;
                        pnModel.IsProduction = model.IsProduction;
                        pnModel.AuthToken = model.AuthToken;
                        UpdateUserDevice(pnModel, userModel.Id);

                        LoginResponseModel responseModel = AutoMapper.Mapper.Map<LoginResponseModel>(userModel);

                        return Ok(new CustomResponse<LoginResponseModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = responseModel });
                    }

                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "Forbidden",
                        StatusCode = (int)HttpStatusCode.Forbidden,
                        Result = new Error { ErrorMessage = "Invalid email or phone or password." }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        private static void UpdateUserDevice(RegisterPushNotificationBindingModel model, int UserId)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var user = ctx.Users.Include(x => x.UserDevices).FirstOrDefault(x => x.Id == UserId);
                    if (user != null)
                    {
                        var existingUserDevice = user.UserDevices.FirstOrDefault(x => x.UDID.Equals(model.UDID));
                        if (existingUserDevice == null)
                        {
                            var userDeviceModel = new UserDevice
                            {
                                Platform = model.Platform,
                                ApplicationType = model.IsPlayStore ? UserDevice.ApplicationTypes.PlayStore : UserDevice.ApplicationTypes.Enterprise,
                                EnvironmentType = model.IsProduction ? UserDevice.ApnsEnvironmentTypes.Production : UserDevice.ApnsEnvironmentTypes.Sandbox,
                                UDID = model.UDID,
                                AuthToken = model.AuthToken,
                                IsActive = true,
                                CreatedDate = DateTime.UtcNow
                            };
                            PushNotificationsUtil.ConfigurePushNotifications(userDeviceModel);
                            user.UserDevices.Add(userDeviceModel);
                            ctx.SaveChanges();
                        }
                        else
                        {
                            existingUserDevice.Platform = model.Platform;
                            existingUserDevice.ApplicationType = model.IsPlayStore ? UserDevice.ApplicationTypes.PlayStore : UserDevice.ApplicationTypes.Enterprise;
                            existingUserDevice.EnvironmentType = model.IsProduction ? UserDevice.ApnsEnvironmentTypes.Production : UserDevice.ApnsEnvironmentTypes.Sandbox;
                            existingUserDevice.UDID = model.UDID;
                            existingUserDevice.AuthToken = model.AuthToken;
                            existingUserDevice.IsActive = true;
                            existingUserDevice.UpdatedDate = DateTime.UtcNow;
                            ctx.SaveChanges();
                            PushNotificationsUtil.ConfigurePushNotifications(existingUserDevice);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Utility.LogError(ex);
            }
        }

        /// <summary>
        /// Login for web admin panel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("WebPanelLogin")]
        [HttpPost]
        public async Task<IHttpActionResult> WebPanelLogin(WebLoginBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                using (RiscoContext ctx = new RiscoContext())
                {
                    DAL.Admin adminModel;
                    var hashedPassword = CryptoHelper.Hash(model.Password);
                    adminModel = ctx.Admins.FirstOrDefault(x => x.Email == model.Email && x.Password == hashedPassword && x.IsDeleted == false);

                    if (adminModel != null)
                    {
                        await adminModel.GenerateToken(Request);
                        CustomResponse<Admin> response = new CustomResponse<Admin> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = adminModel };
                        return Ok(response);
                    }
                    else
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Forbidden",
                            StatusCode = (int)HttpStatusCode.Forbidden,
                            Result = new Error { ErrorMessage = "Invalid Email or Password" }
                        });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        private int SavePicture(HttpRequestMessage request, out string PicturePath)
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                PicturePath = String.Empty;

                if (httpRequest.Files.Count > 1)
                    return 3;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {
                        int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB  

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        var ext = Path.GetExtension(postedFile.FileName);
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {
                            var message = string.Format("Please Upload image of type .jpg,.gif,.png.");
                            return 1;
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {

                            var message = string.Format("Please Upload a file upto 1 mb.");
                            return 2;
                        }
                        else
                        {
                            int count = 1;
                            string fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
                            string newFullPath = HttpContext.Current.Server.MapPath("~/App_Data/" + postedFile.FileName);

                            while (File.Exists(newFullPath))
                            {
                                string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                                newFullPath = HttpContext.Current.Server.MapPath("~/App_Data/" + tempFileName + extension);
                            }

                            postedFile.SaveAs(newFullPath);
                            PicturePath = newFullPath;
                        }
                    }
                }
                return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Logout
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("Logout")]
        public IHttpActionResult Logout(string UDID)
        {
            HttpContext.Current.GetOwinContext().Authentication.SignOut(OAuthDefaults.AuthenticationType);
            // Delete device token from userdevices

            using (RiscoContext ctx = new RiscoContext())
            {
                List<UserDevice> user = ctx.UserDevices.Where(x => x.UDID == UDID).ToList();
                user.ForEach(x => { x.IsActive = false; x.UpdatedDate = DateTime.UtcNow; });
                ctx.SaveChanges();

                return Ok(new CustomResponse<List<UserDevice>> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = user });
            }
        }

        [Authorize]
        [Route("AddNewUserProfile")]
        [HttpGet]
        public IHttpActionResult AddNewUserProfile(string Ip, string Platform, int User_Id)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                using (RiscoContext ctx = new RiscoContext())
                {
                    var User = ctx.Users.FirstOrDefault(x => x.Id == User_Id && !x.IsDeleted);
                    if (User != null)
                    {
                        if (!User.DeActive)
                        {
                            ctx.UserProfiles.Add(new UserProfiles
                            {
                                CreatedDate = DateTime.UtcNow,
                                Ip = Ip,
                                Platform = Platform,
                                IsDeleted = false,
                                User_Id = User_Id
                            });
                            ctx.SaveChanges();
                        }
                    }

                    return Redirect("risco.ingicweb.com");

                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [Authorize]
        [Route("MarkVerified")]
        [HttpPost]
        public IHttpActionResult MarkUserAccountAsVerified(UserModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                using (RiscoContext ctx = new RiscoContext())
                {
                    var userModel = ctx.Users.FirstOrDefault(x => x.Email == model.Email);
                    if (userModel == null)
                        return BadRequest("User account doesn't exist");

                    userModel.Status = (int)DAL.CustomModels.Enumeration.StatusCode.Verified;
                    ctx.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        /// <summary>
        /// Send verification code to user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("SendVerificationSms")]
        public async Task<IHttpActionResult> SendVerificationSms(PhoneBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    var user = ctx.Users.FirstOrDefault(x => x.Id == userId);
                    //user.Phone = model.PhoneNumber;

                    if (ctx.Users.Any(x => x.Id != user.Id && x.Phone == model.PhoneNumber))
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Conflict",
                            StatusCode = (int)HttpStatusCode.Conflict,
                            Result = new Error { ErrorMessage = "User with entered phone number already exists." }
                        });
                    }


                    if (user != null)
                    {
                        var codeInt = new Random().Next(111111, 999999);

                        await ctx.VerifyNumberCodes.Where(x => x.User_Id == user.Id).ForEachAsync(x => x.IsDeleted = true);
                        await ctx.SaveChangesAsync();
                        user.VerifyNumberCodes.Add(new VerifyNumberCodes { Phone = model.PhoneNumber, CreatedAt = DateTime.Now, IsDeleted = false, User_Id = user.Id, Code = codeInt });
                        ctx.SaveChanges();

                        return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = user });

                        //var results = SMS.Send(new SMS.SMSRequest
                        //{
                        //    from = "Skribl",
                        //    title = "Skribl",
                        //    to = model.PhoneNumber,
                        //    text = "Verification Code : " + codeInt
                        //});

                        //if (results.messages.First().status == "0")
                        //{
                        //    return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = user });
                        //}
                        //else
                        //{
                        //    using (StreamWriter sw = File.AppendText(AppDomain.CurrentDomain.BaseDirectory + "/ErrorLog.txt"))
                        //    {
                        //        sw.WriteLine("neximo error : " + DateTime.Now + Environment.NewLine);

                        //        sw.WriteLine(Environment.NewLine + "Status" + results.messages.First().status);
                        //        sw.WriteLine(Environment.NewLine + "RemainingBalance" + results.messages.First().remaining_balance);
                        //        sw.WriteLine(Environment.NewLine + "MessagePrice" + results.messages.First().message_price);
                        //        sw.WriteLine(Environment.NewLine + "ErrorText" + results.messages.First().error_text);
                        //        sw.WriteLine(Environment.NewLine + "ClientRef" + results.messages.First().client_ref);
                        //    }
                        //    return Content(HttpStatusCode.OK, new CustomResponse<Error> { Message = "InternalServerError", StatusCode = (int)HttpStatusCode.InternalServerError, Result = new Error { ErrorMessage = "SMS failed due to some reason." } });
                        //}
                    }
                    else
                        return Ok(new CustomResponse<Error> { Message = "NotFound", StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "User with entered phone number doesn’t exist." } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [Route("ChangeForgotPassword")]
        public async Task<IHttpActionResult> ChangeForgotPassword(SetForgotPasswordBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                using (RiscoContext ctx = new RiscoContext())
                {
                    var user = ctx.Users.FirstOrDefault(x => x.Id == model.User_Id);
                    if (user != null)
                    {
                        user.Password = CryptoHelper.Hash(model.NewPassword);
                        ctx.SaveChanges();
                        return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                    }
                    else
                        return Ok(new CustomResponse<Error> { Message = "Forbidden", StatusCode = (int)HttpStatusCode.Forbidden, Result = new Error { ErrorMessage = "Incorrect old password." } });


                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        /// <summary>
        /// Verify code sent to user. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("VerifySmsCode")]
        public IHttpActionResult VerifySmsCode(PhoneVerificationModel model)
        {
            try
            {
                var userEmail = User.Identity.Name;

                if (string.IsNullOrEmpty(userEmail))
                    throw new Exception("User Email is empty in user.identity.name.");
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                using (RiscoContext ctx = new RiscoContext())
                {
                    var userModel = ctx.Users.Include(x => x.VerifyNumberCodes).FirstOrDefault(x => x.Id == model.UserId);
                    BasketSettings.LoadSettings();
                    userModel.BasketSettings = BasketSettings.Settings;
                    userModel.GenerateToken(Request);

                    userModel.Languages = ctx.Languages.Where(x => x.IsDeleted == false && (x.Code == "en" || x.Code == "en-GB")).ToList();

                    var codeEntry = userModel.VerifyNumberCodes.FirstOrDefault(x => x.Code == model.Code && x.IsDeleted == false && DateTime.Now.Subtract(x.CreatedAt).Minutes < 60);
                    if (codeEntry != null)
                    {
                        userModel.Phone = codeEntry.Phone;
                        userModel.PhoneConfirmed = true;
                        codeEntry.IsDeleted = true;
                        ctx.SaveChanges();
                        return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = userModel });
                    }
                    else
                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.Forbidden, Result = new Error { ErrorMessage = "Invalid code" } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("GetUserDetails")]
        public async Task<IHttpActionResult> GetUserDetails(int User_Id)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    UserDetailsViewModel returnModel = new UserDetailsViewModel();
                    var user = ctx.Users
                        .Include(x => x.Posts)
                        .Include(x => x.TrendLogs)
                        .Include(x => x.Likes)
                        .Include(x => x.UserLanguageMappings.Select(y => y.Language))
                        .Include(x => x.Shares)
                        .Include(x => x.UserGroups)
                        .Include(x => x.Medias)
                        .Include(x => x.FirstUserFollowFollower)
                        .Include(x => x.SecondUserFollowFollower).FirstOrDefault(x => x.Id == User_Id);
                    if (user != null)
                    {
                        user.Languages = new List<Languages>();
                        returnModel.User = user;

                        List<int> UserInterests = new List<int>();
                        if (!string.IsNullOrEmpty(returnModel.User.Interests))
                        {
                            UserInterests = returnModel.User.Interests.Split(',').Select(int.Parse).ToList();
                            foreach (var item in UserInterests)
                            {
                                returnModel.User.Interest.Add(new Interest
                                {
                                    Name = ctx.Interests.FirstOrDefault(x => x.Id == item).Name
                                });
                            }
                        }
                        if (user.UserLanguageMappings.Count > 0)
                        {
                            foreach (var userLanguageMapping in user.UserLanguageMappings)
                            {
                                Languages language = new Languages
                                {
                                    Id = userLanguageMapping.Language.Id,
                                    Code = userLanguageMapping.Language.Code,
                                    CreatedDate = userLanguageMapping.Language.CreatedDate,
                                    IsDeleted = userLanguageMapping.Language.IsDeleted,
                                    Name = userLanguageMapping.Language.Name,
                                    Selected = userLanguageMapping.Language.Selected
                                };
                                user.Languages.Add(language);
                            }
                        }

                        returnModel.TotalPosts = user.Posts.Count(x => !x.IsDeleted && x.PostType == 1);
                        returnModel.TotalIncidents = user.Posts.Count(x => !x.IsDeleted && x.PostType == 2);
                        returnModel.TotalGroups = user.UserGroups.Count(x => !x.IsDeleted);
                        returnModel.TotalFollowers = user.FirstUserFollowFollower.Count(x => !x.IsDeleted);
                        returnModel.TotalFollowings = user.SecondUserFollowFollower.Count(x => !x.IsDeleted);
                        returnModel.TotalShares = user.Shares.Count(x => !x.IsDeleted);
                        returnModel.TotalLikes = user.Likes.Count(x => !x.IsDeleted);
                        returnModel.TotalMedia = user.Medias.Count(x => !x.IsDeleted);
                    }
                    return Ok(new CustomResponse<UserDetailsViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = returnModel });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("VerifyUserCode")]
        public async Task<IHttpActionResult> VerifyUserCode(int userId, int code)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var user = ctx.Users.Include(x => x.ForgotPasswordTokens).FirstOrDefault(x => x.Id == userId);

                    if (user == null)
                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "Invalid UserId" } });

                    user.Languages = ctx.Languages.Where(x => x.IsDeleted == false && (x.Code == "en" || x.Code == "en-GB")).ToList();

                    var token = user.ForgotPasswordTokens.FirstOrDefault(x => x.Code == Convert.ToString(code) && x.IsDeleted == false && DateTime.UtcNow.Subtract(x.CreatedAt).Minutes < 11);

                    if (token != null)
                    {
                        token.IsDeleted = true;
                        ctx.SaveChanges();
                        await user.GenerateToken(Request);
                        return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = user });
                    }
                    else
                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = "Invalid code" } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [Route("Register")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            try
            {
                UserProfiles UserProfile = new UserProfiles();

                

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else if (string.IsNullOrEmpty(model.Email))
                {
                    return Ok(new CustomResponse<Error>
                    {
                        Message = "Bad Request",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Result = new Error { ErrorMessage = "Either provide email or password." }
                    });
                }
                else if(!Utility.findMatch(model.Password, @"(\w*[A-Z]+\w*[0-9]+\w*)+|(\w*[0-9]+\w*[A-Z]+\w*)+"))
                {
                    return Ok(new CustomResponse<Error>
                    {
                        Message = "Bad Request",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Result = new Error { ErrorMessage = "Password must contains atleast one uppercase word and number." }
                    });
                }


                using (RiscoContext ctx = new RiscoContext())
                {
                    if (ctx.Users.Any(x => x.Email == model.Email && x.IsDeleted == false))
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Conflict",
                            StatusCode = (int)HttpStatusCode.Conflict,
                            Result = new Error { ErrorMessage = "User with entered email already exists." }
                        });
                    }
                    else if (model.PhoneNumber != null && ctx.Users.Any(x => x.Phone == model.PhoneNumber && x.IsDeleted == false))
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Conflict",
                            StatusCode = (int)HttpStatusCode.Conflict,
                            Result = new Error { ErrorMessage = "User with entered phone number already exists." }
                        });
                    }
                    else
                    {
                        if (ctx.Users.Any(x => x.UserName == model.UserName && x.IsDeleted == false))
                        {
                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "Conflict",
                                StatusCode = (int)HttpStatusCode.Conflict,
                                Result = new Error { ErrorMessage = "Username already exists." }
                            });
                        }
                    }


                    User userModel = new User
                    {
                        //FirstName = model.FirstName,
                        //LastName = model.LastName,
                        FullName = model.FullName,
                        UserName = model.UserName,
                        Email = String.IsNullOrEmpty(model.Email) ? null : model.Email,
                        Phone = String.IsNullOrEmpty(model.PhoneNumber) ? null : model.PhoneNumber,
                        Password = CryptoHelper.Hash(model.Password),
                        CountryCode = model.CountryCode,
                        Gender = model.Gender,
                        SignInType = (int)RoleTypes.User,
                        IsNotificationsOn = true,
                        GroupsNotification = false,
                        PostsNotification = false,
                        AccountBlocked = false,
                        JoinedOn = DateTime.UtcNow,
                        Interests = model.Interests,
                        LastLoginTime = DateTime.UtcNow,
                        Language = "en",
                        CountryName = model.CountryName
                    };

                    ctx.Users.Add(userModel);
                    ctx.SaveChanges();

                    //adding default languages as Englisg US
                    UserLanguageMapping userLanguageMappingENGUS = new UserLanguageMapping
                    {
                        LanguageId = 14,
                        UserId = userModel.Id
                    };
                    ctx.UserLanguageMappings.Add(userLanguageMappingENGUS);

                    ctx.SaveChanges();

                    userModel.Languages = ctx.Languages.Where(x => x.IsDeleted == false && (x.Code == "en" || x.Code == "en-GB")).ToList();

                    UserProfile.Ip = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : "";
                    if (UserProfile.Ip != null)
                    {
                        var UserData = HttpContext.Current.Request.UserAgent;
                        if (!string.IsNullOrEmpty(UserData))
                        {
                            var UserDataArray = UserData.Split('/');
                            if (UserDataArray.Count() >= 2)
                            {
                                UserProfile.Platform = UserDataArray[0];
                                UserProfile.PlatformName = UserDataArray[1];
                                UserProfile.CreatedDate = DateTime.UtcNow;
                            }
                        }
                        userModel.UserProfiles.Add(UserProfile);
                        ctx.SaveChanges();
                    }

                    BasketSettings.LoadSettings();
                    userModel.BasketSettings = BasketSettings.Settings;
                    await userModel.GenerateToken(Request);

                    var codeInt = new Random().Next(1111, 9999);
                    //await ctx.VerifyNumberCodes.Where(x => x.User_Id == userModel.Id).ForEachAsync(x => x.IsDeleted = true);
                    //userModel.ForgotPasswordTokens.Add(new ForgotPasswordToken { CreatedAt = DateTime.UtcNow, IsDeleted = false, User_ID = userModel.Id, Code = Convert.ToString(codeInt) });
                    //await ctx.SaveChangesAsync();
                    //userModel.VerifyNumberCodes.Add(new VerifyNumberCodes { Phone = model.PhoneNumber, CreatedAt = DateTime.Now, IsDeleted = false, User_Id = userModel.Id, Code = codeInt });
                    //ctx.SaveChanges();
                    userModel.VerifyNumberCodes.Add(new VerifyNumberCodes { CreatedAt = DateTime.Now, IsDeleted = false, User_Id = userModel.Id, Code = codeInt });
                    await ctx.SaveChangesAsync(); 


                    Utility.SendEmail("Email Verification", EmailTypes.EmailVerification, userModel.Id, Convert.ToString(codeInt), userModel.Email);


                    //var codeInt = new Random().Next(1111, 9999);
                    //await ctx.VerifyNumberCodes.Where(x => x.User_Id == userModel.Id).ForEachAsync(x => x.IsDeleted = true);
                    //await ctx.SaveChangesAsync();
                    //userModel.VerifyNumberCodes.Add(new VerifyNumberCodes { Phone = model.PhoneNumber, CreatedAt = DateTime.Now, IsDeleted = false, User_Id = userModel.Id, Code = codeInt });
                    //ctx.SaveChanges();

                    //Utility.SendEmail("Phone Verification", EmailTypes.PhoneVerification, userModel.Id, codeInt.ToString(), userModel.Email);

                    LoginResponseModel responseModel = AutoMapper.Mapper.Map<LoginResponseModel>(userModel);
                    return Ok(new CustomResponse<LoginResponseModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = responseModel });

                    //var results = SMS.Send(new SMS.SMSRequest
                    //{
                    //	from = "Risco",
                    //	title = "Risco",
                    //	to = model.PhoneNumber,
                    //	text = "Verification Code : " + codeInt
                    //});

                    //if (results.messages.First().status == "0")
                    //{
                    //	return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = userModel });
                    //}
                    //else
                    //{
                    //	using (StreamWriter sw = File.AppendText(AppDomain.CurrentDomain.BaseDirectory + "/ErrorLog.txt"))
                    //	{
                    //		sw.WriteLine("neximo error : " + DateTime.Now + Environment.NewLine);

                    //		sw.WriteLine(Environment.NewLine + "Status" + results.messages.First().status);
                    //		sw.WriteLine(Environment.NewLine + "RemainingBalance" + results.messages.First().remaining_balance);
                    //		sw.WriteLine(Environment.NewLine + "MessagePrice" + results.messages.First().message_price);
                    //		sw.WriteLine(Environment.NewLine + "ErrorText" + results.messages.First().error_text);
                    //		sw.WriteLine(Environment.NewLine + "ClientRef" + results.messages.First().client_ref);
                    //	}
                    //	return Content(HttpStatusCode.OK, new CustomResponse<Error> { Message = "InternalServerError", StatusCode = (int)HttpStatusCode.InternalServerError, Result = new Error { ErrorMessage = "SMS failed due to some reason." } });
                    //}

                    //Nexmo Code


                    //var nexmoVerifyResponse = NumberVerify.Verify(new NumberVerify.VerifyRequest { brand = "INGIC", number = model.PhoneNumber, });

                    //if (nexmoVerifyResponse.status == "0")
                    //{

                    //}
                    //return Content(HttpStatusCode.OK, new CustomResponse<NumberVerify.VerifyResponse> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = nexmoVerifyResponse });
                    //else
                    //{
                    //	return Content(HttpStatusCode.OK, new CustomResponse<Error> { Message = "InternalServerError", StatusCode = (int)HttpStatusCode.InternalServerError, Result = new Error { ErrorMessage = "Verification SMS failed due to some reason." } });
                    //}
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [Route("UploadUserImage")]
        [Authorize]
        public async Task<IHttpActionResult> UploadUserImage()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;

                #region Validations
                var userEmail = User.Identity.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    throw new Exception("User Email is empty in user.identity.name.");
                }
                else if (!Request.Content.IsMimeMultipartContent())
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
                        Result = new Error { ErrorMessage = "Image not found, please upload an image." }
                    });
                }
                else if (httpRequest.Files.Count > 1)
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multiple images are not allowed. Please upload 1 image." }
                    });
                }
                #endregion

                var postedFile = httpRequest.Files[0];

                if (postedFile != null && postedFile.ContentLength > 0)
                {

                    int MaxContentLength = 1024 * 1024 * 10; //Size = 1 MB  

                    IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                    var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                    var extension = ext.ToLower();
                    if (!AllowedFileExtensions.Contains(extension))
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "UnsupportedMediaType",
                            StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                            Result = new Error { ErrorMessage = "Please Upload image of type .jpg, .gif, .png." }
                        });
                    }
                    else if (postedFile.ContentLength > MaxContentLength)
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
                        int count = 1;
                        fileNameOnly = Path.GetFileNameWithoutExtension(postedFile.FileName);
                        newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UserImageFolderPath"] + postedFile.FileName);

                        while (File.Exists(newFullPath))
                        {
                            string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                            newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UserImageFolderPath"] + tempFileName + extension);
                        }
                        postedFile.SaveAs(newFullPath);
                    }
                }

                MessageViewModel successResponse = new MessageViewModel { StatusCode = "200 OK", Details = "Image Updated Successfully." };
                //var filePath = Utility.BaseUrl + ConfigurationManager.AppSettings["UserImageFolderPath"] + Path.GetFileName(newFullPath);
                var filePath = ConfigurationManager.AppSettings["UserImageFolderPath"] + Path.GetFileName(newFullPath);
                ImagePathViewModel model = new ImagePathViewModel { Path = filePath };

                using (RiscoContext ctx = new RiscoContext())
                {
                    ctx.Users.FirstOrDefault(x => x.Email == userEmail).ProfilePictureUrl = filePath;
                    ctx.SaveChanges();
                }

                return Content(HttpStatusCode.OK, model);
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [Route("UploadCoverImage")]
        [Authorize]
        public async Task<IHttpActionResult> UploadCoverImage()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;
                var extension = string.Empty;
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                #region Validations
                var userEmail = User.Identity.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    throw new Exception("User Email is empty in user.identity.name.");
                }
                else if (!Request.Content.IsMimeMultipartContent())
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
                        Result = new Error { ErrorMessage = "Image not found, please upload an image." }
                    });
                }
                else if (httpRequest.Files.Count > 1)
                {
                    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                    {
                        Message = "UnsupportedMediaType",
                        StatusCode = (int)HttpStatusCode.UnsupportedMediaType,
                        Result = new Error { ErrorMessage = "Multiple images are not allowed. Please upload 1 image." }
                    });
                }
                #endregion

                using (RiscoContext ctx = new RiscoContext())
                {
                    User userModel;

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

                    userModel = ctx.Users.FirstOrDefault(x => x.Id == userId);

                    if (userModel == null)
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "NotFound",
                            StatusCode = (int)HttpStatusCode.NotFound,
                            Result = new Error { ErrorMessage = "UserId does not exist." }
                        });
                    }
                    else
                    {
                        if (httpRequest.Files.Count > 0)
                        {
                            var FileName = DateTime.UtcNow.Ticks.ToString() + fileExtension;
                            newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UserImageFolderPath"] + FileName);
                            postedFile.SaveAs(newFullPath);
                            userModel.CoverPictureUrl = ConfigurationManager.AppSettings["UserImageFolderPath"] + FileName;
                        }
                        ctx.SaveChanges();

                        SetActivityLog(FirstUser_Id: userId, SecondUser_Id: userId, EntityType: (int)RiscoEntityTypes.UpdateCover, EntityId: userId);

                        CustomResponse<string> response = new CustomResponse<string>
                        {
                            Message = Global.ResponseMessages.Success,
                            StatusCode = (int)HttpStatusCode.OK,
                            Result = userModel.CoverPictureUrl
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

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(SetPasswordBindingModel model)
        {
            try
            {
                var userEmail = User.Identity.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    throw new Exception("User Email is empty in user.identity.name.");
                }
                else if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else if (!Utility.findMatch(model.NewPassword, @"(\w*[A-Z]+\w*[0-9]+\w*)+|(\w*[0-9]+\w*[A-Z]+\w*)+"))
                {
                    return Ok(new CustomResponse<Error>
                    {
                        Message = "Bad Request",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Result = new Error { ErrorMessage = "Password must contains atleast one uppercase word and number." }
                    });
                }


                using (RiscoContext ctx = new RiscoContext())
                {
                    if (model.SignInType == (int)RoleTypes.User)
                    {
                        var hashedPassword = CryptoHelper.Hash(model.OldPassword);
                        var hashedNewPassword = CryptoHelper.Hash(model.NewPassword);
                        var user = ctx.Users.FirstOrDefault(x => x.Email == userEmail && x.Password == hashedPassword);
                        if (hashedPassword == hashedNewPassword)
                        {
                            return Ok(new CustomResponse<Error> { Message = "Forbidden", StatusCode = (int)HttpStatusCode.Forbidden, Result = new Error { ErrorMessage = "New password shouldn't be same as the old password" } });
                        }
                        if (user != null)
                        {
                            user.Password = CryptoHelper.Hash(model.NewPassword);
                            ctx.SaveChanges();
                            SetActivityLog(FirstUser_Id: user.Id, SecondUser_Id: user.Id, EntityType: (int)RiscoEntityTypes.ChangePassword, EntityId: user.Id);
                            return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                        }
                        else
                            return Ok(new CustomResponse<Error> { Message = "Forbidden", StatusCode = (int)HttpStatusCode.Forbidden, Result = new Error { ErrorMessage = "Incorrect old password." } });

                    }
                    else
                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("SignInType") } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

                if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                    && ticket.Properties.ExpiresUtc.HasValue
                    && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
                {
                    return BadRequest("External login failure.");
                }

                ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

                if (externalData == null)
                {
                    return BadRequest("The external login is already associated with an account.");
                }

                IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

                if (!result.Succeeded)
                {
                    //return GetErrorResult(result);
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        /// <summary>
        /// Update user profile with image. This is multipart request. SignInType 0 for user, 1 for deliverer
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("UpdateUserProfileImage")]
        public async Task<IHttpActionResult> UpdateUserProfileImage()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string newFullPath = string.Empty;
                string fileNameOnly = string.Empty;

                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

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
                    User userModel;

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

                    userModel = ctx.Users.FirstOrDefault(x => x.Id == userId);

                    if (userModel == null)
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "NotFound",
                            StatusCode = (int)HttpStatusCode.NotFound,
                            Result = new Error { ErrorMessage = "UserId does not exist." }
                        });
                    }
                    else
                    {
                        if (httpRequest.Files.Count > 0)
                        {
                            var FileName = DateTime.UtcNow.Ticks.ToString() + fileExtension;
                            newFullPath = HttpContext.Current.Server.MapPath("~/" + ConfigurationManager.AppSettings["UserImageFolderPath"] + FileName);
                            postedFile.SaveAs(newFullPath);
                            userModel.ProfilePictureUrl = ConfigurationManager.AppSettings["UserImageFolderPath"] + FileName;
                        }
                        ctx.SaveChanges();

                        SetActivityLog(FirstUser_Id: userId, SecondUser_Id: userId, EntityType: (int)RiscoEntityTypes.ProfileImage, EntityId: userModel.Id);


                        CustomResponse<string> response = new CustomResponse<string>
                        {
                            Message = Global.ResponseMessages.Success,
                            StatusCode = (int)HttpStatusCode.OK,
                            Result = userModel.ProfilePictureUrl
                        };

                        Utility.SendEmail("Update Profile", EmailTypes.UpdateProfile, userId, "", userModel.Email);

                        return Ok(response);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [Authorize]
        [HttpPost]
        [Route("SetAccountSettings")]
        public async Task<IHttpActionResult> SetAccountSettings(AccountSettingsBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    User userModel = ctx.Users.Include(x => x.UserLanguageMappings).FirstOrDefault(x => x.Id == userId);

                    //if (userModel.FullName != model.FullName)
                    //{
                    //	if (ctx.Users.Any(x => x.FullName == model.FullName))
                    //	{
                    //		return Ok(new CustomResponse<Error>
                    //		{
                    //			Message = "Conflict",
                    //			StatusCode = (int)HttpStatusCode.Conflict,
                    //			Result = new Error { ErrorMessage = "User Name already exists." }
                    //		});
                    //	}
                    //}

                    userModel.FullName = model.FullName;
                    userModel.Language = model.Language;
                    userModel.IsLoginVerification = model.IsLoginVerification;
                    userModel.CountryCode = model.CountryCode;
                    userModel.CountryName = model.CountryName;
                    userModel.AboutMe = model.AboutMe;
                    userModel.IsVideoAutoPlay = model.IsVideoAutoPlay;
                    userModel.Interests = model.Interests;
                    userModel.Phone = String.IsNullOrEmpty(model.Phone) ? null : model.Phone;

                    if (model.AllLanguages != null)
                    {
                        if (userModel.UserLanguageMappings != null && userModel.UserLanguageMappings.Count > 0)
                        {
                            //removing all the previous mapping
                            ctx.UserLanguageMappings.RemoveRange(userModel.UserLanguageMappings);
                        }
                        //selected languages
                        var selectedLanguages = model.AllLanguages.Where(x => x.Selected == true);

                        //foreach selected language creating a new mapping
                        foreach (Languages selectedLanguage in selectedLanguages)
                        {
                            UserLanguageMapping userLanguageMapping = new UserLanguageMapping
                            {
                                LanguageId = selectedLanguage.Id,
                                UserId = userId
                            };
                            ctx.UserLanguageMappings.Add(userLanguageMapping);
                        }
                    }

                    ctx.SaveChanges();
                    SetActivityLog(FirstUser_Id: userId, SecondUser_Id: userId, EntityType: (int)RiscoEntityTypes.AccountSettings, EntityId: userId);

                    CustomResponse<User> response = new CustomResponse<User>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = userModel
                    };

                    Utility.SendEmail("Update Profile", EmailTypes.UpdateProfile, userId, "", userModel.Email);

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        /// <summary>
        /// Get All Admins
        /// </summary>
        /// <returns></returns>
        [Route("GetAllAdmin")]
        public async Task<IHttpActionResult> GetAllAdmins()
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var users = ctx.Users.Where(x => x.SignInType == (int)RoleTypes.SubAdmin || x.SignInType == (int)RoleTypes.SuperAdmin).ToList();

                    CustomResponse<IEnumerable<User>> response = new CustomResponse<IEnumerable<User>>
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

        [Authorize]
        [Route("GetAllLanguages")]
        public async Task<IHttpActionResult> GetAllLanguages()
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var languages = ctx.Languages.Where(x => !x.IsDeleted).OrderBy(x => x.Name).ToList();

                    CustomResponse<IEnumerable<Languages>> response = new CustomResponse<IEnumerable<Languages>>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = languages
                    };

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        [Authorize]
        [Route("GetProfileImageUrls")]
        public async Task<IHttpActionResult> GetProfileImageUrls(string userIds)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userIdList = Array.ConvertAll(userIds.Split(','), int.Parse);

                    //var userIdList = userIds.Split(',');


                    var users = ctx.Users.Where(x => userIdList.Contains(x.Id)).ToList();


                    //var languages = ctx.Languages.Where(x => !x.IsDeleted).OrderBy(x => x.Name).ToList();

                    CustomResponse<IEnumerable<User>> response = new CustomResponse<IEnumerable<User>>
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



        //[AllowAnonymous]
        //[HttpGet]
        //[Route("ExternalLogin")]
        //public async Task<IHttpActionResult> ExternalLogin(string accessToken, int? socialLoginType)
        //{
        //    try
        //    {
        //        if (socialLoginType.HasValue && !string.IsNullOrEmpty(accessToken))
        //        {
        //            SocialLogins socialLogin = new SocialLogins();
        //            // send access token and social login type to GetSocialUserData in return it will give you full name email and profile picture of user 
        //            var socialUser = await socialLogin.GetSocialUserData(accessToken, (SocialLogins.SocialLoginType)socialLoginType.Value);

        //            if (socialUser != null)
        //            {
        //                using (RiscoContext ctx = new RiscoContext())
        //                {

        //                    // if user have privacy on his / her email then we will create email address from his user Id which will be send by mobile developer 
        //                    if (string.IsNullOrEmpty(socialUser.email))
        //                    {
        //                        socialUser.email = socialUser.id + "@gmail.com";
        //                    }
        //                    var existingUser = ctx.Users.Include(x => x.UserSubscriptions).Include(x => x.UserAddresses).Include(x => x.PaymentCards).FirstOrDefault(x => x.Email == socialUser.email);

        //                    if (existingUser != null)
        //                    {
        //                        // if user already have registered through social login them wee will always check his picture and name just to get updated values of that user from facebook 
        //                        existingUser.ProfilePictureUrl = socialUser.picture;
        //                        existingUser.FullName = socialUser.name;
        //                        ctx.SaveChanges();
        //                        await existingUser.GenerateToken(Request);
        //                        BasketSettings.LoadSettings();
        //                        existingUser.BasketSettings = BasketSettings.Settings;
        //                        CustomResponse<User> response = new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = existingUser };
        //                        return Ok(response);
        //                    }
        //                    else
        //                    {
        //                        int SignInType = 0;
        //                        if (socialLoginType.Value == (int)BasketApi.SocialLogins.SocialLoginType.Google)
        //                        {
        //                            SignInType = (int)SocialLoginType.Google;
        //                        }
        //                        else if (socialLoginType.Value == (int)BasketApi.SocialLogins.SocialLoginType.Facebook)
        //                            SignInType = (int)SocialLoginType.Facebook;


        //                        var newUser = new User { FullName = socialUser.name, Email = socialUser.email, ProfilePictureUrl = socialUser.picture, SignInType = SignInType, Status = 1, IsNotificationsOn = true };
        //                        ctx.Users.Add(newUser);
        //                        ctx.SaveChanges();
        //                        await newUser.GenerateToken(Request);
        //                        BasketSettings.LoadSettings();
        //                        newUser.BasketSettings = BasketSettings.Settings;

        //                        HostingEnvironment.QueueBackgroundWorkItem(cancellationToken =>
        //                        {
        //                            sendJoiningEmail(socialUser.email);
        //                        });
        //                        return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = newUser });
        //                    }
        //                }
        //            }
        //            else
        //                return BadRequest("Unable to get user info");
        //        }
        //        else
        //            return BadRequest("Please provide access token along with social login type");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        /// <summary>
        /// Contact us
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("ContactUs")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ContactUs(ContactUsBindingModel model)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    //var userSubscriptions = ctx.UserSubscriptions.Include(x => x.Box).Where(x => x.User_Id == model.UserId && x.Box.ReleaseDate.Month == model.Month && x.Box.ReleaseDate.Year == model.Year).ToList();

                    //if (userSubscriptions.Count == 0)
                    //{
                    //    return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.OK, Result = new Error { ErrorMessage = "You can't provide feedback for the selected box." } });
                    //}

                    if (!String.IsNullOrEmpty(model.Description))
                    {
                        if (model.UserId.HasValue)
                            ctx.ContactUs.Add(new ContactUs { UserId = model.UserId.Value, Description = model.Description });
                        else
                            ctx.ContactUs.Add(new ContactUs { Description = model.Description });
                        ctx.SaveChanges();
                        return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                    }
                    else
                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.BadRequest, StatusCode = (int)HttpStatusCode.BadRequest, Result = new Error { ErrorMessage = Global.ResponseMessages.CannotBeEmpty("Description") } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        /// <summary>
        /// Contact us list
        /// </summary>        
        /// <returns></returns>
        [HttpGet]
        [Route("ContactUsList")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ContactUsList()
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {

                    List<ContactUs> lstContactUs = ctx.ContactUs.Include(x => x.User).Where(x => x.UserId != null).ToList();
                    return Ok(new CustomResponse<ContactListViewModel> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new ContactListViewModel { ContactList = lstContactUs } });                    
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        //[Authorize]
        ///// <summary>
        ///// Add user address
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("AddUserAddress")]
        //public async Task<IHttpActionResult> AddUserAddress(AddUserAddressBindingModel model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }

        //        using (RiscoContext ctx = new RiscoContext())
        //        {
        //            User user;
        //            DeliveryMan deliverer;
        //            if (model.SignInType == (int)RoleTypes.User)
        //            {
        //                user = ctx.Users.Include(x => x.UserAddresses).Include(x => x.PaymentCards).FirstOrDefault(x => x.Id == model.UserId);
        //                if (user != null)
        //                {
        //                    if (!user.UserAddresses.Any(
        //                        x => x.Apartment == model.Apartment
        //                        && x.City == model.City
        //                        && x.Country == model.Country
        //                        && x.Floor == model.Floor
        //                        && x.NearestLandmark == model.NearestLandmark
        //                        && x.BuildingName == model.BuildingName
        //                        && x.StreetName == model.StreetName
        //                        && x.Type == model.AddressType
        //                        && x.IsDeleted == false)
        //                        )
        //                    {
        //                        foreach (var address in user.UserAddresses)
        //                            address.IsPrimary = false;

        //                        user.UserAddresses.Add(new UserAddress
        //                        {
        //                            Apartment = model.Apartment,
        //                            City = model.City,
        //                            Country = model.Country,
        //                            Floor = model.Floor,
        //                            NearestLandmark = model.NearestLandmark,
        //                            BuildingName = model.BuildingName,
        //                            StreetName = model.StreetName,
        //                            Type = model.AddressType,
        //                            IsPrimary = true
        //                        });
        //                        ctx.SaveChanges();
        //                        return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = user });
        //                    }
        //                    else
        //                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Conflict, StatusCode = (int)HttpStatusCode.Conflict, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateAlreadyExists("Address") } });
        //                }
        //                else
        //                    return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateNotFound("Address") } });
        //            }
        //            else
        //            {
        //                deliverer = ctx.DeliveryMen.Include(x => x.DelivererAddresses).FirstOrDefault(x => x.Id == model.UserId);
        //                if (deliverer != null)
        //                {
        //                    if (!deliverer.DelivererAddresses.Any(
        //                        x => x.Apartment == model.Apartment
        //                        && x.City == model.City
        //                        && x.Country == model.Country
        //                        && x.Floor == model.Floor
        //                        && x.NearestLandmark == model.NearestLandmark
        //                        && x.BuildingName == model.BuildingName
        //                        && x.StreetName == model.StreetName
        //                        && x.Type == model.AddressType)
        //                        )
        //                    {

        //                        foreach (var address in deliverer.DelivererAddresses)
        //                            address.IsPrimary = false;

        //                        deliverer.DelivererAddresses.Add(new DelivererAddress
        //                        {
        //                            Apartment = model.Apartment,
        //                            City = model.City,
        //                            Country = model.Country,
        //                            Floor = model.Floor,
        //                            NearestLandmark = model.NearestLandmark,
        //                            BuildingName = model.BuildingName,
        //                            StreetName = model.StreetName,
        //                            Type = model.AddressType,
        //                            IsPrimary = true
        //                        });
        //                        ctx.SaveChanges();
        //                        return Ok(new CustomResponse<DeliveryMan> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = deliverer });
        //                    }
        //                    else
        //                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Conflict, StatusCode = (int)HttpStatusCode.Conflict, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateAlreadyExists("Address") } });
        //                }
        //                else
        //                    return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateNotFound("Address") } });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        [HttpPost]
        [Route("MarkUserVerified")]
        [AllowAnonymous]
        public async Task<IHttpActionResult> MarkUserVerified(int User_Id)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var User = ctx.Users.FirstOrDefault(x => x.Id == User_Id);
                    Utility.SendEmail("Welcome To Risco", EmailTypes.Welcome, User_Id, "", User.Email);
                    return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        //[Authorize]
        //[HttpPost]
        //[Route("EditUserAddress")]
        //public async Task<IHttpActionResult> EditUserAddress(EditUserAddressBindingModel model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }
        //        using (RiscoContext ctx = new RiscoContext())
        //        {
        //            User user;
        //            DeliveryMan deliverer;

        //            if (model.SignInType == (int)RoleTypes.User)
        //            {
        //                user = ctx.Users.Include(x => x.UserAddresses).Include(x => x.PaymentCards).FirstOrDefault(x => x.Id == model.UserId);

        //                if (user != null)
        //                {
        //                    var address = user.UserAddresses.FirstOrDefault(
        //                        x => x.Id == model.AddressId && x.IsDeleted == false
        //                        );
        //                    if (address != null)
        //                    {
        //                        address.Apartment = model.Apartment;
        //                        address.City = model.City;
        //                        address.Country = model.Country;
        //                        address.Floor = model.Floor;
        //                        address.NearestLandmark = model.NearestLandmark;
        //                        address.BuildingName = model.BuildingName;
        //                        address.StreetName = model.StreetName;
        //                        address.Type = model.AddressType;
        //                        address.IsPrimary = model.IsPrimary;

        //                        ctx.SaveChanges();

        //                        return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = user });
        //                    }
        //                    else
        //                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("AddressId") } });
        //                }
        //                else
        //                    return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("UserId") } });
        //            }
        //            else
        //            {
        //                deliverer = ctx.DeliveryMen.Include(x => x.DelivererAddresses).FirstOrDefault(x => x.Id == model.UserId);

        //                if (deliverer != null)
        //                {
        //                    var address = deliverer.DelivererAddresses.FirstOrDefault(
        //                        x => x.Id == model.AddressId && x.IsDeleted == false
        //                        );
        //                    if (address != null)
        //                    {
        //                        address.Apartment = model.Apartment;
        //                        address.City = model.City;
        //                        address.Country = model.Country;
        //                        address.Floor = model.Floor;
        //                        address.NearestLandmark = model.NearestLandmark;
        //                        address.BuildingName = model.BuildingName;
        //                        address.StreetName = model.StreetName;
        //                        address.Type = model.AddressType;
        //                        address.IsPrimary = model.IsPrimary;

        //                        ctx.SaveChanges();

        //                        return Ok(new CustomResponse<DeliveryMan> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = deliverer });
        //                    }
        //                    else
        //                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("AddressId") } });
        //                }
        //                else
        //                    return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateInvalid("UserId") } });
        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        /// <summary>
        /// An email will be sent to user containing password.
        /// </summary>
        /// <param name="Email">Email of user.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("ResetPasswordThroughEmail")]
        public async Task<IHttpActionResult> ResetPasswordThroughEmail(string Email)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var user = ctx.Users.FirstOrDefault(x => x.Email == Email);

                    if (user != null)
                    {
                        //var codeInt = new Random().Next(111111, 999999);

                        //var html = Utility.GetEmailTemplate(EmailTypes.ResetPassword, user.Id);
                        Utility.SendEmail("Forget Password", EmailTypes.ForgetPassword, user.Id, "", user.Email);
                        //                  string subject = "Reset your password - " + EmailUtil.FromName;
                        //const string body = "Use this code as current password";

                        //var smtp = new SmtpClient
                        //{
                        //	Host = "smtp.gmail.com",
                        //	Port = 587,
                        //	EnableSsl = true,
                        //	DeliveryMethod = SmtpDeliveryMethod.Network,
                        //	UseDefaultCredentials = false,
                        //	Credentials = new NetworkCredential(EmailUtil.FromMailAddress.Address, EmailUtil.FromPassword)
                        //};

                        //var message = new MailMessage(EmailUtil.FromMailAddress, new MailAddress("data.expert9@gmail.com"))
                        //{
                        //	Subject = subject,
                        //	Body = body,
                        //                      IsBodyHtml=true
                        //};

                        //smtp.Send(message);

                        //user.Password = CryptoHelper.Hash(codeInt.ToString());
                        //user.ForgotPasswordTokens.Add(new ForgotPasswordToken { CreatedAt = DateTime.UtcNow, IsDeleted = false, User_ID = user.Id, Code = Convert.ToString(codeInt) });
                        //ctx.SaveChanges();
                        return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = user });
                    }
                    else
                    {
                        return Ok(new CustomResponse<Error> { Message = "NotFound", StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = "User with entered email doesn’t exist." } });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        /// <summary>
        /// Register for getting push notifications
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("RegisterPushNotification")]
        public async Task<IHttpActionResult> RegisterPushNotification(RegisterPushNotificationBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                using (RiscoContext ctx = new RiscoContext())
                {
                    var user = ctx.Users.Include(x => x.UserDevices).FirstOrDefault(x => x.Id == model.User_Id);
                    if (user != null)
                    {
                        var existingUserDevice = user.UserDevices.FirstOrDefault(x => x.UDID.Equals(model.UDID));
                        if (existingUserDevice == null)
                        {
                            //foreach (var userDevice in user.UserDevices)
                            //    userDevice.IsActive = false;

                            var userDeviceModel = new UserDevice
                            {
                                Platform = model.Platform,
                                ApplicationType = model.IsPlayStore ? UserDevice.ApplicationTypes.PlayStore : UserDevice.ApplicationTypes.Enterprise,
                                EnvironmentType = model.IsProduction ? UserDevice.ApnsEnvironmentTypes.Production : UserDevice.ApnsEnvironmentTypes.Sandbox,
                                UDID = model.UDID,
                                AuthToken = model.AuthToken,
                                IsActive = true,
                                CreatedDate = DateTime.UtcNow
                            };

                            PushNotificationsUtil.ConfigurePushNotifications(userDeviceModel);

                            user.UserDevices.Add(userDeviceModel);
                            ctx.SaveChanges();
                            return Ok(new CustomResponse<UserDevice> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = userDeviceModel });
                        }
                        else
                        {
                            //foreach (var userDevice in user.UserDevices)
                            //    userDevice.IsActive = false;

                            existingUserDevice.Platform = model.Platform;
                            existingUserDevice.ApplicationType = model.IsPlayStore ? UserDevice.ApplicationTypes.PlayStore : UserDevice.ApplicationTypes.Enterprise;
                            existingUserDevice.EnvironmentType = model.IsProduction ? UserDevice.ApnsEnvironmentTypes.Production : UserDevice.ApnsEnvironmentTypes.Sandbox;
                            existingUserDevice.UDID = model.UDID;
                            existingUserDevice.AuthToken = model.AuthToken;
                            existingUserDevice.IsActive = true;
                            existingUserDevice.UpdatedDate = DateTime.UtcNow;
                            ctx.SaveChanges();
                            PushNotificationsUtil.ConfigurePushNotifications(existingUserDevice);
                            return Ok(new CustomResponse<UserDevice> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = existingUserDevice });
                        }
                    }
                    else
                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateNotFound("User") } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        //[Authorize]
        //[HttpPost]
        //[Route("AddEditPaymentCard")]
        //public async Task<IHttpActionResult> AddEditPaymentCard(PaymentCardBindingModel model)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }
        //        using (RiscoContext ctx = new RiscoContext())
        //        {
        //            var existingUser = ctx.Users.Include(x => x.UserAddresses).Include(x => x.PaymentCards).FirstOrDefault(x => x.Id == model.UserId && x.IsDeleted == false);

        //            if (existingUser != null)
        //            {
        //                if (model.IsEdit)
        //                {
        //                    var existingCard = existingUser.PaymentCards.FirstOrDefault(x => x.Id == model.Id && x.IsDeleted == false);
        //                    if (existingCard != null)
        //                    {
        //                        ctx.Entry(existingCard).CurrentValues.SetValues(model);
        //                    }
        //                    else
        //                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateNotFound("Card") } });
        //                }
        //                else
        //                {
        //                    existingUser.PaymentCards.Add(new PaymentCard
        //                    {
        //                        CardNumber = model.CardNumber,
        //                        CardType = model.CardType,
        //                        CCV = model.CCV,
        //                        ExpiryDate = model.ExpiryDate,
        //                        NameOnCard = model.NameOnCard,
        //                        User_ID = model.UserId
        //                    });
        //                }
        //                ctx.SaveChanges();
        //                return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = existingUser });
        //            }
        //            else
        //            {
        //                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateNotFound("User") } });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        //[Authorize]
        //[HttpGet]
        //[Route("DeletePaymentCard")]
        //public async Task<IHttpActionResult> DeletePaymentCard(int UserId, int CardId)
        //{
        //    try
        //    {
        //        using (RiscoContext ctx = new RiscoContext())
        //        {
        //            var existingUser = ctx.Users.Include(x => x.UserAddresses).Include(x => x.PaymentCards).FirstOrDefault(x => x.Id == UserId && x.IsDeleted == false);
        //            if (existingUser != null)
        //            {
        //                var existingCard = existingUser.PaymentCards.FirstOrDefault(x => x.Id == CardId && x.IsDeleted == false);
        //                if (existingCard != null)
        //                    existingCard.IsDeleted = true;
        //                else
        //                    return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateNotFound("Card") } });
        //                ctx.SaveChanges();
        //                return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = existingUser });
        //            }
        //            else
        //                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateNotFound("User") } });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        //[Authorize]
        //[HttpGet]
        //[Route("DeleteUserAddress")]
        //public async Task<IHttpActionResult> DeleteUserAddress(int UserId, int AddressId)
        //{
        //    try
        //    {
        //        using (RiscoContext ctx = new RiscoContext())
        //        {
        //            var existingUser = ctx.Users.Include(x => x.UserAddresses).Include(x => x.PaymentCards).FirstOrDefault(x => x.Id == UserId && x.IsDeleted == false);
        //            if (existingUser != null)
        //            {
        //                var existingAddress = existingUser.UserAddresses.FirstOrDefault(x => x.Id == AddressId && x.IsDeleted == false);
        //                if (existingAddress != null)
        //                    existingAddress.IsDeleted = true;
        //                else
        //                    return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateNotFound("Address") } });

        //                ctx.SaveChanges();
        //                return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = existingUser });
        //            }
        //            else
        //                return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.NotFound, StatusCode = (int)HttpStatusCode.NotFound, Result = new Error { ErrorMessage = Global.ResponseMessages.GenerateNotFound("User") } });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(Utility.LogError(ex));
        //    }
        //}

        [Authorize]
        [HttpGet]
        [Route("GetUser")]
        public async Task<IHttpActionResult> GetUser(int UserId)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                List<int> BlockedUsers = new List<int>();
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userModel = ctx.Users
                        .Include(x => x.UserAddresses)
                        .Include(x => x.UserLanguageMappings).FirstOrDefault(x => x.Id == UserId && x.IsDeleted == false);

                    if (userModel != null)
                    {
                        userModel.FollowingCount = ctx.FollowFollowers.Where(x => x.FirstUser_Id == userModel.Id && x.IsDeleted == false).Count();
                        userModel.FollowersCount = ctx.FollowFollowers.Where(x => x.SecondUser_Id == userModel.Id && x.IsDeleted == false).Count();
                        userModel.PostCount = ctx.Posts.Where(x => x.User_Id == userModel.Id && x.IsDeleted == false && x.PostType == 1).Count();
                        userModel.IncidentCount = ctx.Posts.Where(x => x.User_Id == userModel.Id && x.IsDeleted == false && x.PostType == 2).Count();
                        userModel.MediaCount = ctx.Medias.Count(x => x.IsDeleted == false && x.User_Id == UserId);
                        userModel.LikesCount = ctx.Likes.Count(x => x.IsDeleted == false && x.User_Id == UserId);
                        userModel.RepliesCount = ctx.Comments.Where(x => x.User_Id == UserId && x.IsDeleted == false).Select(x => x.Post).Distinct().Count();


                        if (userModel != null)
                        {
                            BlockedUsers = ctx.BlockUsers.Where(x => x.FirstUser_Id == userModel.Id).Select(x => x.SecondUser_Id).ToList();
                            BlockedUsers.AddRange(ctx.BlockUsers.Where(x => x.SecondUser_Id == userModel.Id).Select(x => x.FirstUser_Id).ToList());
                            userModel.UnreadNotifications = ctx.Notifications.Count(x => x.ReceivingUser_Id == userModel.Id && !BlockedUsers.Contains(x.SendingUser_Id.Value) && x.Status == (Int32)NotificationStatus.Unread && !x.IsDeleted);
                        }

                        if (userId != UserId)
                        {
                            userModel.IsFollowing = ctx.FollowFollowers.Any(x => x.FirstUser_Id == userId && x.SecondUser_Id == userModel.Id && x.IsDeleted == false);
                            userModel.IsFollower = ctx.FollowFollowers.Any(x => x.FirstUser_Id == userModel.Id && x.SecondUser_Id == userId && x.IsDeleted == false);
                            userModel.IsBlocked = ctx.BlockUsers.Any(x => (x.FirstUser_Id == userId && x.SecondUser_Id == userModel.Id) || (x.FirstUser_Id == userModel.Id && x.SecondUser_Id == userId) && x.IsDeleted == false);
                        }


                        switch (userModel.MessagePrivacy)
                        {
                            case (int)DirectMessagePrivacyTypes.Anyone:
                                userModel.IsMessagingAllowed = true;
                                break;
                            case (int)DirectMessagePrivacyTypes.Follower: // jo mjha follow kar raha han
                                if (ctx.FollowFollowers.Any(x => x.FirstUser_Id == userId && x.SecondUser_Id == UserId))
                                    userModel.IsMessagingAllowed = true;
                                break;
                            case (int)DirectMessagePrivacyTypes.FollowersAndFollowing:
                                if (ctx.FollowFollowers.Any(x => (x.FirstUser_Id == userId && x.SecondUser_Id == UserId) || (x.FirstUser_Id == UserId && x.SecondUser_Id == userId)))
                                    userModel.IsMessagingAllowed = true;
                                break;
                            case (int)DirectMessagePrivacyTypes.Following: // ma jinko follow kar rah ahu
                                if (ctx.FollowFollowers.Any(x => x.FirstUser_Id == UserId && x.SecondUser_Id == userId))
                                    userModel.IsMessagingAllowed = true;
                                break;
                            case (int)DirectMessagePrivacyTypes.NotAllowed:
                                userModel.IsMessagingAllowed = false;
                                break;
                            default:
                                userModel.IsMessagingAllowed = true;
                                break;
                        }

                        //interests

                        BasketSettings.LoadSettings();
                        await userModel.GenerateToken(Request);
                        userModel.BasketSettings = BasketSettings.Settings;
                        if (!String.IsNullOrEmpty(userModel.Interests))
                        {
                            var lstUserIntrests = userModel.Interests.Split(',').Select(int.Parse).ToList();
                            foreach (var intrest in userModel.BasketSettings.Interests)
                                intrest.Checked = lstUserIntrests.Contains(intrest.Id);
                        }
                        //All Languages
                        userModel.AllLanguages = ctx.Languages.Where(x => !x.IsDeleted).OrderBy(x => x.Name).ToList();

                        //user languages
                        userModel.Languages = new List<Languages>();

                        foreach (var userLanguageMapping in userModel.UserLanguageMappings)
                        {                            
                            if (userModel.AllLanguages.Where(x => x.Id == userLanguageMapping.LanguageId).FirstOrDefault() != null)
                            {
                                //adding user selected language to the Languages array
                                userModel.Languages.Add(userModel.AllLanguages.Where(x => x.Id == userLanguageMapping.LanguageId).FirstOrDefault());

                                //setting selected property of the AllLanguages array to true
                                userModel.AllLanguages.Where(x => x.Id == userLanguageMapping.LanguageId).FirstOrDefault().Selected = true;
                            }
                        }

                        if (userModel != null)
                        {
                            BlockedUsers = ctx.BlockUsers.Where(x => x.FirstUser_Id == userModel.Id).Select(x => x.SecondUser_Id).ToList();
                            BlockedUsers.AddRange(ctx.BlockUsers.Where(x => x.SecondUser_Id == userModel.Id).Select(x => x.FirstUser_Id).ToList());
                            userModel.UnreadNotifications = ctx.Notifications.Count(x => x.ReceivingUser_Id == userModel.Id && !BlockedUsers.Contains(x.SendingUser_Id.Value) && x.Status == (Int32)NotificationStatus.Unread && !x.IsDeleted);
                            userModel.IsMute = ctx.MuteUser.Any(x => x.FirstUser_Id == userId && x.SecondUser_Id == UserId);
                            //userModel.UnreadNotifications = ctx.Notifications.Count(x => x.ReceivingUser_Id == userModel.Id && x.Status == (Int32)NotificationStatus.Unread && !x.IsDeleted);
                        }


                        return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = userModel });
                    }
                    else
                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new Error { ErrorMessage = "Invalid UserId" } });
                }
            }
            catch (Exception ex)
            {               
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("MarkDeviceAsInActive")]
        public async Task<IHttpActionResult> MarkDeviceAsInActive(int UserId, int DeviceId)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var device = ctx.UserDevices.FirstOrDefault(x => x.Id == DeviceId && x.User_Id == UserId);
                    if (device != null)
                    {
                        ctx.UserDevices.Remove(device);
                        ctx.SaveChanges();
                        return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK });
                    }
                    else
                        return Ok(new CustomResponse<Error> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = new Error { ErrorMessage = "Invalid UserId or DeviceId." } });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("GetMediaByUserId")]
        public async Task<IHttpActionResult> GetMediaByUserId(int UserId, int PageSize = int.MaxValue, int PageNo = 1)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    int MediaCount = ctx.Medias.Count(x => x.IsDeleted == false && x.User_Id == UserId);

                    List<Media> Medias = ctx.Medias
                        .Include(x => x.User)
                        .Where(x => x.IsDeleted == false && x.User_Id == UserId)
                        .OrderByDescending(x => x.Id)
                        .Skip((PageNo - 1) * PageSize).Take(PageSize)
                        .ToList();

                    return Ok(new CustomResponse<MediaListViewModel>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = new MediaListViewModel
                        {
                            MediaCount = MediaCount,
                            Medias = Medias
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IHttpActionResult> GetAllUsers(int PageSize = int.MaxValue, int PageNo = 0)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));
                    //List<User> Users = new List<User>();
                    List<FollowFollower> UsersToReturn = new List<FollowFollower>();

                    var IBlocked = ctx.BlockUsers.Where(x => x.FirstUser_Id == userId).Select(x => x.SecondUser_Id).ToList();
                    var TheyBlocked = ctx.BlockUsers.Where(x => x.SecondUser_Id == userId).Select(x => x.FirstUser_Id).ToList();

                    var Users = ctx.Users.Where(x => !x.IsDeleted && !IBlocked.Contains(x.Id) && !TheyBlocked.Contains(x.Id)).OrderByDescending(x => x.Id).Skip(PageSize * PageNo).Take(PageSize).ToList();

                    if (Users != null)
                    {
                        foreach (var user in Users)
                        {
                            UsersToReturn.Add(new FollowFollower
                            {
                                FirstUser = user,
                                FirstUser_Id = user.Id
                            });
                        }
                    }
                    return Ok(new CustomResponse<List<FollowFollower>>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = UsersToReturn
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("ResendCode")]
        public async Task<IHttpActionResult> ResendCode(string Email)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var User = ctx.Users.FirstOrDefault(x => x.Email == Email);
                    if (User != null)
                    {
                        var codeInt = new Random().Next(1111, 9999);

                        var alreadyCode = ctx.VerifyNumberCodes.Where(x => x.User_Id == User.Id).ToList();

                        if (alreadyCode != null)
                        {
                            foreach (var code in alreadyCode)
                            {
                                code.IsDeleted = true;
                            }
                            ctx.SaveChanges();
                        }

                        //await ctx.VerifyNumberCodes.Where(x => x.User_Id == User.Id).ForEachAsync(x => x.IsDeleted = true);
                        //await ctx.SaveChangesAsync();
                        User.VerifyNumberCodes.Add(new VerifyNumberCodes { Phone = User.Phone, CreatedAt = DateTime.Now, IsDeleted = false, User_Id = User.Id, Code = codeInt });
                        ctx.SaveChanges();

                        Utility.SendEmail("Phone Verification", EmailTypes.PhoneVerification, User.Id, codeInt.ToString(), User.Email);
                    }
                    else
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Forbidden",
                            StatusCode = (int)HttpStatusCode.Forbidden,
                            Result = new Error { ErrorMessage = "user not found." }
                        });
                    }
                }
                return Ok(new CustomResponse<string> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = "Code resent successfully." });


            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [HttpGet]
        [Route("VerificationOfCode")]
        public async Task<IHttpActionResult> VerificationOfCode(string Email, int VerificationCode)
        {
            try
            {
                using (RiscoContext ctx = new RiscoContext())
                {
                    var User = ctx.Users.FirstOrDefault(x => x.Email == Email);

                    if (User != null)
                    {
                        User.Languages = ctx.Languages.Where(x => x.IsDeleted == false && (x.Code == "en" || x.Code == "en-GB")).ToList();

                        var code = ctx.VerifyNumberCodes.FirstOrDefault(x => x.User_Id == User.Id && x.Code == VerificationCode && !x.IsDeleted);

                        if (code != null)
                        {
                            //if (DateTime.Now < code.CreatedAt.AddMinutes(2))
                            //{

                            if (User != null)
                            {
                                User.EmailConfirmed = true;
                                User.PhoneConfirmed = true;
                                ctx.SaveChanges();
                                await User.GenerateToken(Request);
                            }
                            return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = User });
                            //}
                            //else
                            //{
                            //    return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            //    {
                            //        Message = "Forbidden",
                            //        StatusCode = (int)HttpStatusCode.Forbidden,
                            //        Result = new Error { ErrorMessage = "Verification code has been expired." }
                            //    });
                            //}
                        }
                        else
                        {
                            return Content(HttpStatusCode.OK, new CustomResponse<Error>
                            {
                                Message = "Forbidden",
                                StatusCode = (int)HttpStatusCode.Forbidden,
                                Result = new Error { ErrorMessage = "Invalid verification code." }
                            });
                        }


                        //Utility.SendEmail("Phone Verification", EmailTypes.PhoneVerification, User.Id, codeInt.ToString(), User.Email);
                    }
                    else
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Forbidden",
                            StatusCode = (int)HttpStatusCode.Forbidden,
                            Result = new Error { ErrorMessage = "user not found." }
                        });
                    }
                    return Ok(new CustomResponse<User> { Message = Global.ResponseMessages.Success, StatusCode = (int)HttpStatusCode.OK, Result = User });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        public async Task sendJoiningEmail(string joinerEmail)
        {
            try
            {
                string subject = "New User Signed Up - " + EmailUtil.FromName;
                string body = "A new user " + joinerEmail + " has just signed up";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(EmailUtil.FromMailAddress.Address, EmailUtil.FromPassword)
                };

                var AdminEmail = BasketSettings.GetAdminEmailForOrders();

                var message = new MailMessage(EmailUtil.FromMailAddress, new MailAddress(AdminEmail))
                {
                    Subject = subject,
                    Body = body
                };

                smtp.Send(message);

                sendJoiningEmailtoUser(joinerEmail);
            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }

        public async Task sendJoiningEmailtoUser(string joinerEmail)
        {
            try
            {
                var body = @"
Hello,

Thank You for signing up on Skribl Box App. 

We’re sure you love art just as much as we do, and this is why we delight in bringing you all the art supplies you will ever need for every art project imaginable. Our Skribl Boxes are overflowing with exciting and new ideas to both inspire your inner craftsman and fuel your need to express!

This is an automated message. Please, do not reply to it. If you have a questions not already answered on FAQs, feel free to drop us an email on info@skriblbox.com.  We’ll get right back to you!

Thanks,
Skribl box App Team
Contact Number: +971-567941517
http://www.skriblbox.com
";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(EmailUtil.FromMailAddress.Address, EmailUtil.FromPassword)
                };

                var message = new MailMessage(EmailUtil.FromMailAddress, new MailAddress(joinerEmail))
                {
                    Subject = "Welcome to Skribl Box",
                    Body = body
                };

                smtp.Send(message);

            }
            catch (Exception ex)
            {
                Utility.LogError(ex);
            }
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        [Authorize]
        [HttpPost]
        [Route("SetPrivacySettings")]
        public async Task<IHttpActionResult> SetPrivacySettings(PrivacySettingsBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                    User UserResult = ctx.Users.FirstOrDefault(x => x.Id == userId);

                    UserResult.IsPostLocation = model.IsPostLocation;
                    UserResult.TaggingPrivacy = model.TaggingPrivacy;
                    UserResult.FindByEmail = model.FindByEmail;
                    UserResult.FindByPhone = model.FindByPhone;
                    UserResult.MessagePrivacy = model.MessagePrivacy;
                    UserResult.isAllowWatchFollowerFollowings = model.isAllowWatchFollowerFollowings;
                    
                    ctx.SaveChanges();
                    SetActivityLog(FirstUser_Id: userId, SecondUser_Id: userId, EntityType: (int)RiscoEntityTypes.PrivacySettings, EntityId: UserResult.Id);

                    CustomResponse<User> response = new CustomResponse<User>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = UserResult
                    };

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [Authorize]
        [HttpPost]
        [Route("SetNotificationSettings")]
        public async Task<IHttpActionResult> SetNotificationSettings(NotificationSettingsBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                using (RiscoContext ctx = new RiscoContext())
                {
                    var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                    User UserResult = ctx.Users.FirstOrDefault(x => x.Id == userId);

                    UserResult.GroupsNotification = model.GroupsNotification;
                    UserResult.PostsNotification = model.PostsNotification;
                    UserResult.MuteYouDontFollow = model.MuteYouDontFollow;
                    UserResult.MuteDontFollowYou = model.MuteDontFollowYou;
                    ctx.SaveChanges();
                    SetActivityLog(FirstUser_Id: userId, SecondUser_Id: userId, EntityType: (int)RiscoEntityTypes.NotificationSettings, EntityId: UserResult.Id);


                    CustomResponse<User> response = new CustomResponse<User>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = UserResult
                    };



                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("BlockUser")]
        public async Task<IHttpActionResult> BlockUser(int User_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    User user = ctx.Users.FirstOrDefault(x => x.Id == userId);

                    var alreadyFollowing = ctx.FollowFollowers.Where(x => (x.FirstUser_Id == userId && x.SecondUser_Id == User_Id) || (x.FirstUser_Id == User_Id && x.SecondUser_Id == userId)).ToList();

                    if (alreadyFollowing != null)
                    {
                        ctx.FollowFollowers.RemoveRange(alreadyFollowing);
                        ctx.SaveChanges();
                    }

                    BlockUser blockUser = new BlockUser
                    {
                        FirstUser_Id = userId,
                        SecondUser_Id = User_Id,
                        CreatedDate = DateTime.UtcNow
                    };

                    ctx.BlockUsers.Add(blockUser);
                    ctx.SaveChanges();

                    SetActivityLog(FirstUser_Id: userId, SecondUser_Id: User_Id, EntityType: (int)RiscoEntityTypes.BlockUser, EntityId: blockUser.Id);

                    CustomResponse<BlockUser> response = new CustomResponse<BlockUser>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = blockUser
                    };

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("UnBlockUser")]
        public async Task<IHttpActionResult> UnBlockUser(int User_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    BlockUser blockUser = ctx.BlockUsers.FirstOrDefault(x => x.FirstUser_Id == userId && x.SecondUser_Id == User_Id);
                    ctx.BlockUsers.Remove(blockUser);
                    //blockUser.IsDeleted = true;
                    ctx.SaveChanges();

                    CustomResponse<BlockUser> response = new CustomResponse<BlockUser>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = blockUser
                    };

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("MuteUser")]
        public async Task<IHttpActionResult> MuteUser(int User_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    MuteUser blockUser = new MuteUser
                    {
                        FirstUser_Id = userId,
                        SecondUser_Id = User_Id,
                        CreatedDate = DateTime.UtcNow
                    };
                    if (!ctx.MuteUser.Any(x => x.FirstUser_Id == userId && x.SecondUser_Id == User_Id))
                    {


                        ctx.MuteUser.Add(blockUser);
                        ctx.SaveChanges();

                        SetActivityLog(FirstUser_Id: userId, SecondUser_Id: User_Id, EntityType: (int)RiscoEntityTypes.NotificationSettings, EntityId: blockUser.Id);
                    }
                    CustomResponse<MuteUser> response = new CustomResponse<MuteUser>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = blockUser
                    };

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("UnMuteUser")]
        public async Task<IHttpActionResult> UnMuteUser(int User_Id)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    MuteUser muteUser = ctx.MuteUser.FirstOrDefault(x => x.FirstUser_Id == userId && x.SecondUser_Id == User_Id);
                    ctx.MuteUser.Remove(muteUser);
                    //blockUser.IsDeleted = true;
                    ctx.SaveChanges();

                    CustomResponse<MuteUser> response = new CustomResponse<MuteUser>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = muteUser
                    };

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("DeactivateAccount")]
        public async Task<IHttpActionResult> DeactivateAccount()
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    var user = ctx.Users.FirstOrDefault(x => x.Id == userId && !x.IsDeleted);
                    if (user != null)
                    {
                        var query = @"Update Posts SET IsDeleted='1' Where User_Id='" + user.Id + @"';
                                      Update Comments SET IsDeleted='1' Where User_Id='" + user.Id + @"';
                                      Update Likes SET IsDeleted='1' Where User_Id='" + user.Id + @"';
                                      Update Groups SET IsDeleted='1' Where User_Id='" + user.Id + @"';
                                      Update GroupMembers SET IsDeleted='1' Where User_Id='" + user.Id + @"';
                                      Update Shares SET IsDeleted='1' Where User_Id='" + user.Id + @"';
                                      Update FollowFollowers SET IsDeleted='1' Where FirstUser_Id='" + user.Id + @"' OR SecondUser_Id='" + user.Id + @"';
                                      Update Notifications SET IsDeleted='1' Where SendingUser_Id='" + user.Id + @"' OR ReceivingUser_Id='" + user.Id + @"'
                                      Update ActivityLogs SET IsDeleted='1' Where FirstUser_Id='" + user.Id + @"' OR SecondUser_Id='" + user.Id + @"'
                                      Update Users SET IsDeleted='1',DeActive='1' Where ID='" + user.Id + @"'";

                        ctx.Database.ExecuteSqlCommand(query);
                    }
                    else
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Forbidden",
                            StatusCode = (int)HttpStatusCode.Forbidden,
                            Result = new Error { ErrorMessage = "user not found." }
                        });
                    }
                    ctx.SaveChanges();

                    CustomResponse<string> response = new CustomResponse<string>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = "Account deactivated successfully."
                    };

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("ReportUser")]
        public async Task<IHttpActionResult> ReportUser(int User_Id, int ReportUserStatus)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    User user = ctx.Users.FirstOrDefault(x => x.Id == User_Id);
                    ReportUser reportUser = new ReportUser();
                    int reportCount = 0;

                    reportCount = ctx.ReportUsers.Count(x => x.SecondUser_Id == User_Id && !x.IsDeleted);

                    var AlreadyExistingReport = ctx.ReportUsers.Include(x => x.SecondUser).FirstOrDefault(x => x.SecondUser_Id == User_Id && x.FirstUser_Id == userId && !x.IsDeleted);
                    if (AlreadyExistingReport != null)
                    {
                        return Content(HttpStatusCode.OK, new CustomResponse<Error>
                        {
                            Message = "Forbidden",
                            StatusCode = (int)HttpStatusCode.Forbidden,
                            Result = new Error { ErrorMessage = "You already reported this user." }
                        });

                    }
                    else
                    {
                        ctx.ReportUsers.Add(new DAL.ReportUser
                        {
                            FirstUser_Id = userId,
                            SecondUser_Id = User_Id,
                            CreatedDate = DateTime.UtcNow,
                            IsDeleted = false,
                            ReportStatus = ReportUserStatus
                        });

                        user.ReportCount++;
                        ctx.SaveChanges();

                        if (user.ReportCount > 5 && user.ReportCount < 10)
                        {
                            // send warning email
                            Utility.SendEmail("Account Suspension Warning", EmailTypes.AccountSuspensionWarning, AlreadyExistingReport.SecondUser_Id, "", AlreadyExistingReport.SecondUser.Email);
                        }
                        else if (user.ReportCount == 10 || user.ReportCount > 10)
                        {
                            // block user
                            user.IsDeleted = true;
                            Utility.SendEmail("Account Suspended", EmailTypes.AccountSuspended, AlreadyExistingReport.SecondUser_Id, "", AlreadyExistingReport.SecondUser.Email);
                        }
                    }

                    SetActivityLog(FirstUser_Id: userId, SecondUser_Id: User_Id, EntityType: (int)RiscoEntityTypes.ReportUser, EntityId: reportUser.Id);
                    CustomResponse<ReportUser> response = new CustomResponse<ReportUser>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = reportUser
                    };

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("GetUsersProfileImage")]
        public async Task<IHttpActionResult> GetUsersProfileImage(string User_Ids)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));


                UserImagesListViewModel userimages = new UserImagesListViewModel();

                using (RiscoContext ctx = new RiscoContext())
                {


                    List<int> UserIds = new List<int>();

                    if (!string.IsNullOrEmpty(User_Ids))
                    {
                        UserIds = User_Ids.Split(',').Select(int.Parse).ToList();


                        if (UserIds.Count > 0)
                        {

                            for (int i = 0; i < UserIds.Count; i++)
                            {
                                var userkiid = UserIds[i];
                                var user = ctx.Users.FirstOrDefault(x => x.Id == userkiid);
                                userimages.Images.Add(new UsersImagesViewModel
                                {
                                    Id = user.Id,
                                    ImageUrl = user.ProfilePictureUrl,
                                    FullName = user.FullName
                                });
                            }
                        }

                        //var users = ctx.Users.Where(x => UserIds.Contains(x.Id)).ToList();
                        //if(users != null)
                        //{
                        //    foreach (var item in users)
                        //    {
                        //        userimages.Images.Add(new UsersImagesViewModel {
                        //            Id=item.Id,
                        //            ImageUrl=item.ProfilePictureUrl
                        //        });
                        //    }
                        //}

                    }
                }
                CustomResponse<UserImagesListViewModel> response = new CustomResponse<UserImagesListViewModel>
                {
                    Message = Global.ResponseMessages.Success,
                    StatusCode = (int)HttpStatusCode.OK,
                    Result = userimages
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("GetBlockUsers")]
        public async Task<IHttpActionResult> GetBlockUsers()
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    var blockUsers = new List<BlockUser>();
                    blockUsers = ctx.BlockUsers.Include(x => x.SecondUser).Where(x => x.FirstUser_Id == userId).ToList();

                    CustomResponse<List<BlockUser>> response = new CustomResponse<List<BlockUser>>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = blockUsers
                    };

                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }


        //this method is getting used to update both interests and languages
        [Authorize]
        [HttpGet]
        [Route("UpdateUserInterest")]
        public async Task<IHttpActionResult> UpdateUserInterest(string interests, string lanugages)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                using (RiscoContext ctx = new RiscoContext())
                {
                    var user = await ctx.Users.FirstOrDefaultAsync(x => x.Id == userId);

                    user.Interests = interests;

                    //set user languages
                    if (!string.IsNullOrEmpty(lanugages))
                    {
                        //selected languages
                        var selectedLanguages = lanugages.Split(',').Select(int.Parse).ToList();

                        //foreach selected language creating a new mapping
                        foreach (int selectedLanguage in selectedLanguages)
                        {
                            UserLanguageMapping userLanguageMapping = new UserLanguageMapping
                            {
                                LanguageId = selectedLanguage,
                                UserId = userId
                            };
                            ctx.UserLanguageMappings.Add(userLanguageMapping);
                        }
                    }

                    await ctx.SaveChangesAsync();
                    user.Token = new Token() { access_token = Request.Headers.Authorization.Parameter.Replace("bearer ", ""), token_type = "bearer" };

                    CustomResponse<User> response = new CustomResponse<User>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = user
                    };
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(Utility.LogError(ex));
            }
        }

        [Authorize]
        [HttpGet]
        [Route("UpdateUserLanguages")]
        public async Task<IHttpActionResult> UpdateUserLanguages(string interests)
        {
            try
            {
                var userId = Convert.ToInt32(User.GetClaimValue("userid"));

                UserImagesListViewModel userimages = new UserImagesListViewModel();

                using (RiscoContext ctx = new RiscoContext())
                {

                    var user = await ctx.Users.FirstOrDefaultAsync(x => x.Id == userId);

                    user.Interests = interests;

                    await ctx.SaveChangesAsync();
                    user.Token = new Token() { access_token = Request.Headers.Authorization.Parameter.Replace("bearer ", ""), token_type = "bearer" };

                    CustomResponse<User> response = new CustomResponse<User>
                    {
                        Message = Global.ResponseMessages.Success,
                        StatusCode = (int)HttpStatusCode.OK,
                        Result = user
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
