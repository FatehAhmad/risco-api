using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public partial class PollOptionVote
    {
        public int Id { get; set; }

        public int User_Id { get; set; }

        public bool IsDeleted { get; set; }
        
        public int Post_Id { get; set; }

        public virtual Post Post { get; set; }

        public virtual User User { get; set; }

        public int PollOption_Id { get; set; }
        
        public PollOptions PollOptions { get; set; }


    }
}
