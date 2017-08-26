using HashTag.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HashTag.Data.Config.Configurations
{
    public class UserConfiguration : IEntityConfiguration
    {
        public void Configure(ModelBuilder builder)
        {
            builder.Entity<User>().ToTable("Users").HasKey(x => x.Id);
            builder.Entity<User>().HasOne(x => x.ApplicationUser).WithOne(x => x.User).HasForeignKey<User>("ApplicationUserId");
            builder.Entity<User>().HasOne(x => x.ProfilePhoto).WithOne().HasForeignKey<User>("ProfilePhotoId");
        }
    }
}