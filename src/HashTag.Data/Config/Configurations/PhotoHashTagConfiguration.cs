using HashTag.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HashTag.Data.Config.Configurations
{
    public class PhotoHashTagConfiguration : IEntityConfiguration
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PhotoHashTag>().ToTable("PhotoHashTagAssoc").HasKey("PhotoId", "HashTagId");
            modelBuilder.Entity<PhotoHashTag>().HasOne(x => x.Photo).WithMany(x => x.PhotoHashTags).HasForeignKey("PhotoId");
            modelBuilder.Entity<PhotoHashTag>().HasOne(x => x.HashTag).WithMany(x => x.PhotoHashTags).HasForeignKey("HashTagId");
        }
    }
}