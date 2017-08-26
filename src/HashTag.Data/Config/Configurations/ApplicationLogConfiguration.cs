using HashTag.Domain.Logging;
using Microsoft.EntityFrameworkCore;

namespace HashTag.Data.Config.Configurations
{
    public class ApplicationLogConfiguration : IEntityConfiguration
    {
        public void Configure(ModelBuilder builder)
        {
            builder.Entity<ApplicationLog>().ToTable("ApplicationLog").HasKey(x => x.Id);
        }
    }
}