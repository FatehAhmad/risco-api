namespace DAL
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public int EntityType { get; set; }
        public int EntityId { get; set; }
        public int? SendingUser_Id { get; set; }
        public int? ReceivingUser_Id { get; set; }
        [Required]
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
        [JsonIgnore]
        //public int? DeliveryMan_ID { get; set; }
        public bool IsDeleted { get; set; }
        public int? AdminNotification_Id { get; set; }
        public virtual User SendingUser { get; set; }
        [JsonIgnore]
        public virtual User ReceivingUser { get; set; }
        [JsonIgnore]
        //public virtual DeliveryMan DeliveryMan { get; set; }
        public virtual AdminNotifications AdminNotification { get; set; }
        public bool IsEmailSent { get; set; }
        public bool IsSMSSent { get; set; }

        [NotMapped]
        public int GroupMemberStatus { get; set; }            //use this to identify whether a request is accepted or not
    }
}
