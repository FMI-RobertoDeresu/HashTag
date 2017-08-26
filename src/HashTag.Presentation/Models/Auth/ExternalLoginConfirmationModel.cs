using System.ComponentModel.DataAnnotations;

namespace HashTag.Presentation.Models.Auth
{
    public class ExternalLoginConfirmationModel
    {
        public string Provider { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }
    }
}