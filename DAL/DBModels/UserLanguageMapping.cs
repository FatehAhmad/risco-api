using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public partial class UserLanguageMapping
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int LanguageId { get; set; }
        public virtual User User { get; set; }
        public virtual Languages Language { get; set; }
    }
}