namespace DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Interest
    {
        public Interest()
        {
            ChildInterests = new HashSet<Interest>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public bool IsDeleted { get; set; }
        [NotMapped]
        public bool Checked { get; set; }
       
        public int? ParentInterestId { get; set; }
       
        public Interest ParentInterest { get; set; }

        public ICollection<Interest> ChildInterests { get; set; }

        
    }
}
