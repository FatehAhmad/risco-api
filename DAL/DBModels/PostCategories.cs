using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class PostCategories
    {
        public int Id { get; set; }

        public Post Post { get; set; }

        public int Post_Id { get; set; }

        [ForeignKey("Interest_Id")]
        public Interest Interest { get; set; }
 
        public int Interest_Id { get; set; }

        public bool IsDeleted { get; set; }
    }
}
