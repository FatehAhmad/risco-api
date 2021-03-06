using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Areas.Admin.ViewModels;

namespace BasketApi.ViewModels
{    
    public class ContactListViewModel
    {
        public ContactListViewModel()
        {
            ContactList = new List<ContactUs>();
        }
        public List<ContactUs> ContactList { get; set; }
    }
}