using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HashTag.Domain.Models
{
    public class ApplicationUser : IdentityUser<long>
    {
        public ApplicationUser()
        {
            User = new User(this);
        }

        public ApplicationUser(string email, string userName)
            : this()
        {
            Email = email;
            UserName = userName;
        }

        public User User { get; set; }
    }
}