using System.ComponentModel.DataAnnotations;
using HashTag.Infrastructure.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace HashTag.Presentation.Models.Manage
{
    public class EditUserModel
    {
        public EditUserModel()
        {
            WithPasswordChange = false;
        }

        public EditUserModel(long userId, string userName) 
            :this()
        {
            UserId = userId;
            UserName = userName;
        }

        [HiddenInput]
        public long UserId { get; set; }

        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Display(Name = "With password change")]
        public bool WithPasswordChange { get; set; }

        [Display(Name = "Current password")]
        [Required(ErrorMessage = "Current password is required.")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Display(Name = "New password")]
        [Required(ErrorMessage = "New password is required.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "Repeat new password")]
        [EqualsTo("NewPassword", "Passwords does not match!")]
        [DataType(DataType.Password)]
        public string NewPasswordRepeat { get; set; }
    }
}