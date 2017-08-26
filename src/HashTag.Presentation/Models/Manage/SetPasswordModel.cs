using System.ComponentModel.DataAnnotations;
using HashTag.Infrastructure.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace HashTag.Presentation.Models.Manage
{
    public class SetPasswordModel
    {
        public SetPasswordModel() { }

        public SetPasswordModel(long userId)
        {
            UserId = userId;
        }

        [HiddenInput]
        public long UserId { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Repeat password")]
        [EqualsTo("Password", "Passwords does not match!")]
        [DataType(DataType.Password)]
        public string RePassword { get; set; }
    }
}