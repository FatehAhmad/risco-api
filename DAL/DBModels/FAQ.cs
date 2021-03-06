using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public partial class FAQ
    {

        public int Id { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        public bool IsDeleted { get; set; }

    }
}
