using HashTag.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HashTag.Data.Config.Configurations
{
    public class SamplePhotoConfiguration : IEntityConfiguration
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SamplePhoto>().ToTable("SamplePhoto").HasKey(x => x.Id);
            modelBuilder.Entity<SamplePhoto>().Property(typeof(string), "_prediction").HasColumnName("Prediction");
            modelBuilder.Entity<SamplePhoto>().Ignore(x => x.Prediction);
            modelBuilder.Entity<SamplePhoto>().Ignore("_predictionArray");
        }
    }
}