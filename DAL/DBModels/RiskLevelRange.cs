namespace DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RiskLevelRange
    {
        public RiskLevelRange()
        {

        }
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public int From { get; set; }

        [Required]
        public int To { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedDate { get; set; }                
    }
}
