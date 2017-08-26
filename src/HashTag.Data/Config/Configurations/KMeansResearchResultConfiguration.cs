using HashTag.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HashTag.Data.Config.Configurations
{
    public class KMeansResearchResultConfiguration : IEntityConfiguration
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KMeansResearchResult>().ToTable("KMeansResearchResult").HasKey(x => x.Id);
        }
    }
}