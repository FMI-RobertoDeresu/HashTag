using HashTag.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace HashTag.Data.Config.Configurations
{
    public class ClusterSamplePhotoConfiguration : IEntityConfiguration
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClusterSamplePhoto>().ToTable("ClusterSamplePhoto").HasKey(x => x.Id);
            modelBuilder.Entity<ClusterSamplePhoto>().HasOne(x => x.Cluster).WithMany(x => x.Photos).HasForeignKey("ClusterId").OnDelete(DeleteBehavior.Restrict);
        }
    }
}