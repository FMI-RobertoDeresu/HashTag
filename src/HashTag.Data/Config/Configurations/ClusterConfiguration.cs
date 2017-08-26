using HashTag.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HashTag.Data.Config.Configurations
{
    public class ClusterConfiguration : IEntityConfiguration
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cluster>().ToTable("Cluster").HasKey(x => x.Id);
            modelBuilder.Entity<Cluster>().HasMany(x => x.Photos).WithOne(x => x.Cluster).OnDelete(DeleteBehavior.Cascade);        
            modelBuilder.Entity<Cluster>().Property(typeof(string), "_corePrediction").HasColumnName("CorePrediction");
            modelBuilder.Entity<Cluster>().Ignore(x => x.CorePrediction);
            modelBuilder.Entity<Cluster>().Ignore("_corePredictionArray");
        }
    }
}