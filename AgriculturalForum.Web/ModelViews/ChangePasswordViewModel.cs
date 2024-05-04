using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AgriculturalForum.Web.ModelViews
{
    public class ChangePasswordViewModel
    {
        [Key]
        public int UserId { get; set; }

        [DisplayName("CurrentPassword")]
        [Required(ErrorMessage = "CurrentPasswordRequired")]
        public string CurrentPassword { get; set; }

        [DisplayName("NewPassword")]
        [Required(ErrorMessage = "NewPasswordRequired")]
        [MinLength(6, ErrorMessage = "PasswordMinLenght")]
        public string NewPassword { get; set; }

       
        [DisplayName("ConfirmPassword")]
        [Compare("NewPassword", ErrorMessage = "CompareConfirmAndNewPassword")]
        public string ConfirmPassword { get; set; }
    }
}
