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
using BasketApi;

namespace WebApplication1.Controllers
{
    [RoutePrefix("api/Language")]
    public class LanguageController : ApiController
    {
        [BasketApi.Authorize]
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
    }
}
