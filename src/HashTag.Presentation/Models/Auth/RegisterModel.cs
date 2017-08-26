using System.ComponentModel.DataAnnotations;
using HashTag.Domain.AutoMapping;
using HashTag.Domain.Dtos;
using HashTag.Infrastructure.Attributes;

namespace HashTag.Presentation.Models.Auth
{
    public class RegisterModel : IMapTo<UserCreateDto>
    {
        [EmailAddress]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }

        [Display(Name = "Repeat password")]
        [EqualsTo("Password", "Passwords does not match!")]
        public string RePassword { get; set; }
    }
}