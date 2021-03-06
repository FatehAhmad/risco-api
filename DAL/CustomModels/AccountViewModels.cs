using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.CustomModels
{
    class AccountViewModels
    {
    }
    // Models returned by AccountController actions.

    public class ExternalLoginViewModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string State { get; set; }
    }
    public class IpStackResponseViewmodel
    {
        public string ip { get; set; }

        public string hostname { get; set; }

        public string continent_code { get; set; }

        public string continent_name { get; set; }

        public string country_code { get; set; }

        public string country_name { get; set; }

        public string region_code { get; set; }

        public string city { get; set; }

        public string zip { get; set; }

    }
    public class ManageInfoViewModel
    {
        public string LocalLoginProvider { get; set; }

        public string Email { get; set; }

        public IEnumerable<UserLoginInfoViewModel> Logins { get; set; }

        public IEnumerable<ExternalLoginViewModel> ExternalLoginProviders { get; set; }
    }

    public class UserInfoViewModel
    {
        public string Email { get; set; }

        public bool HasRegistered { get; set; }

        public string LoginProvider { get; set; }
    }

    public class UserLoginInfoViewModel
    {
        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }
    }
}
