using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AgriculturalForum.Web.ModelViews
{
    public class RegisterViewModel
    {
        [Key]
        public int UserId { get; set; }

        [DisplayName("FullName")]
        [Required(ErrorMessage = "FullNameRequired")]
        public string FullName { get; set; }

        [MaxLength(150)]
        [Required(ErrorMessage = "EmailRequired")]
        [DataType(DataType.EmailAddress)]
        [Remote(action: "ValidateEmail", controller: "Account", AdditionalFields = nameof(Email))]
        [DisplayName("Email")]
        public string Email { get; set; }

        [MaxLength(11)]
        [Required(ErrorMessage = "PhoneRequired")]
        [DisplayName("Phone")]
        [DataType(DataType.PhoneNumber)]
        [Remote(action: "ValidatePhone", controller: "Account", AdditionalFields = nameof(Phone))]
        public string Phone { get; set; }
  
        [DisplayName("Password")]
        [Required(ErrorMessage = "PasswordRequired")]
        [MinLength(6, ErrorMessage = "PasswordMinLenght")]
        public string Password { get; set; }

       
        [DisplayName("ConfirmPassword")]
        [Compare("Password", ErrorMessage = "ComparePassword")]
        public string ConfirmPassword { get; set; }
    }
}

