using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public partial class PollOptions
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string MediaUrl { get; set; }

        public int Votes { get; set; }

        public double Percentage { get; set; }

        public bool IsDeleted { get; set; }

        public int? Post_Id { get; set; }

        [NotMapped]
        public bool IsVoted { get; set; } // if user voted for this poll we will return true. dont map it on db.

        [NotMapped]
        public Post Post { get; set; }

        [JsonIgnore]
        public List<PollOptionVote> PollOptionVote { get; set; }

    }
}
