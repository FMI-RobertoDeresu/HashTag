using Microsoft.EntityFrameworkCore;

namespace HashTag.Data.Config.Configurations
{
    public class HashTagConfiguration : IEntityConfiguration
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Domain.Models.HashTag>().ToTable("HashTag").HasKey(x => x.Id);
        }
    }
}