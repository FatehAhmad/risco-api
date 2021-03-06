using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class PostLanguageCodes
    {
        public int Id { get; set; }
       
        public Post Post { get; set; }

        public int Post_Id { get; set; }

        [ForeignKey("Language_Id")]
        public Languages Languages { get; set; }

        public int Language_Id { get; set; }

        public bool IsDeleted { get; set; }
    }
}
