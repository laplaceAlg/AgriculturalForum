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
        [DisplayName("Email")]
        [EmailAddress(ErrorMessage ="EmailFormat")]
        public string Email { get; set; }

        [MaxLength(11)]
        [Required(ErrorMessage = "PhoneRequired")]
        [DisplayName("Phone")]
		[RegularExpression(@"(03|05|07|08|09)+([0-9]{8})$", ErrorMessage = "PhoneNumberFormat")]
		[DataType(DataType.PhoneNumber)]
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

