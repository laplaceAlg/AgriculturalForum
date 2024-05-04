using System.ComponentModel.DataAnnotations;

namespace AgriculturalForum.Web.ModelViews
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = ("EmailRequired"))]
        [EmailAddress(ErrorMessage = "EmailAddress")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "PasswordRequired")]
        public string Password { get; set; }
    }
}
