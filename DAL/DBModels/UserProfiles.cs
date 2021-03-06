using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class UserProfiles
    {
        public int Id { get; set; }

        public string Ip { get; set; }

        public string Platform { get; set; }

        public string PlatformName { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsBlocked { get; set; }

        public int User_Id { get; set; }

        public User User { get; set; }

    }
}
