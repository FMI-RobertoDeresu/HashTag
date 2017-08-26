using HashTag.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HashTag.Data.Config.Configurations
{
    public class PhotoConfiguration : IEntityConfiguration
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Photo>().ToTable("Photo").HasKey(x => x.Id);
            modelBuilder.Entity<Photo>().HasOne(x => x.CreatedBy);
            modelBuilder.Entity<Photo>().HasOne(x => x.UpdatedBy);
            modelBuilder.Entity<Photo>().HasOne(x => x.DeletedBy);
            modelBuilder.Entity<Photo>().HasOne(x => x.Cluster);
            modelBuilder.Entity<Photo>().HasMany(x => x.PhotoHashTags);
            modelBuilder.Entity<Photo>().Property(typeof(string), "_prediction").HasColumnName("Prediction");
            modelBuilder.Entity<Photo>().Ignore(x => x.Prediction);
            modelBuilder.Entity<Photo>().Ignore("_predictionArray");
        }
    }
}