using DAL;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BasketApi.ViewModels
{
    public class EditUserProfileBindingModel
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public string FullName { get; set; }

        //[Required]
        //[DataType(DataType.PhoneNumber)]
        //public string PhoneNumber { get; set; }

        //[Required]
        //[DataType(DataType.EmailAddress)]
        //public string Email { get; internal set; }

        [Required]
        public string Language { get; set; }

        [Required]
        public bool IsLoginVerification { get; set; }

        [Required]
        public string CountryCode { get; set; }

        [Required]
        public bool IsVideoAutoPlay { get; set; }

        [Required]
        public string Interests { get; set; }


    }
    
    public class UserViewModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get; set; }

        public string ProfilePictureUrl { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
        
        public string AccountType { get; set; }

        public string ZipCode { get; set; }

        public string DateofBirth { get; set; }

        public short? SignInType { get; set; }

        public string UserName { get; set; }

        public short? Status { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool PhoneConfirmed { get; set; }
    }

    public class UsersViewModel
    {
        public UsersViewModel()
        {
            Users = new List<User>();
        }
        public List<User> Users { get; set; }
    }


    public class UserImagesListViewModel
    {
        public UserImagesListViewModel()
        {
            Images = new List<UsersImagesViewModel>();
        }
        public List<UsersImagesViewModel> Images { get; set; }
    }

    public class UsersImagesViewModel
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string ImageUrl { get; set; }
    }
}