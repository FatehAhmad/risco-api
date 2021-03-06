using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BasketApi.BindingModels
{
    public class ChangeOrderStatusBindingModel
    {
        public int OrderId { get; set; }
        public int StoreOrder_Id { get; set; }
        public int Status { get; set; }
    }

    public class ChangeOrderStatusListBindingModel
    {
        public ChangeOrderStatusListBindingModel()
        {
            Orders = new List<ChangeOrderStatusBindingModel>();
        }
        public List<ChangeOrderStatusBindingModel> Orders { get; set; }
    }


    public class ChangeGroupStatusListBindingModel
    {
        public ChangeGroupStatusListBindingModel()
        {
            Groups = new List<ChangeGroupStatusBindingModel>();
        }
        public List<ChangeGroupStatusBindingModel> Groups { get; set; }
    }

    public class ChangeGroupStatusBindingModel
    {
        public int GroupId { get; set; }
        public bool Status { get; set; }
    }




    // here
    public class ChangeUserStatusBindingModel
    {
        public int UserId { get; set; }
        public bool Status { get; set; }
    }
 
    public class ChangeUserStatusListBindingModel
    {
        public ChangeUserStatusListBindingModel()
        {
            Users = new List<ChangeUserStatusBindingModel>();
        }
        public List<ChangeUserStatusBindingModel> Users { get; set; }
    }

    // change post statuses 


    public class ChangePostStatusModel
    {
        public int UserId { get; set; }
        public bool Status { get; set; }
    }
    public class ChangePostStatusListModel
    {
        public ChangePostStatusListModel()
        {
            Posts = new List<ChangePostStatusModel>();
        }

        public List<ChangePostStatusModel> Posts { get; set; }
    }




    public class ChangeBoxStatusBindingModel
    {
        public int BoxId { get; set; }
        public short Status { get; set; }
    }
    public class ChangeBoxStatusListBindingModel
    {
        public ChangeBoxStatusListBindingModel()
        {
            Boxes = new List<ChangeBoxStatusBindingModel>();
        }
        public List<ChangeBoxStatusBindingModel> Boxes { get; set; }
    }

    public class FaqBindingModel
    {
        public int Id { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }
    }
}