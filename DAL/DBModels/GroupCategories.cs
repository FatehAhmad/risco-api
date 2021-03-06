using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class GroupCategories
    {
        public int Id { get; set; }

        public Group Group { get; set; }

        public int Group_Id { get; set; }

        [ForeignKey("Interest_Id")]
        public Interest Interest { get; set; }
 
        public int Interest_Id { get; set; }

        public bool IsDeleted { get; set; }

        [NotMapped]
        public string InterestName { get; set; }
    }
}
