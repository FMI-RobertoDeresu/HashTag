using System.ComponentModel.DataAnnotations;

namespace HashTag.Presentation.Models.Auth
{
    public class SignInModel
    {
        public SignInModel()
        {
            RememberMe = true;
        }

        [Display(Name = "Username or Email")]
        [Required(ErrorMessage = "Username or Email is required.")]
        public string NameOrEmail { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}