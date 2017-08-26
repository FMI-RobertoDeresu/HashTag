using HashTag.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HashTag.Data.Config.Configurations
{
    public class PredictionClassCofiguration : IEntityConfiguration
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PredictionClass>().ToTable("PredictionClass").HasKey(x => x.Id);
        }
    }
}