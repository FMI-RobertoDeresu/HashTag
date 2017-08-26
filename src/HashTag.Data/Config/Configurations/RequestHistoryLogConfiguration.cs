using HashTag.Domain.Logging;
using Microsoft.EntityFrameworkCore;

namespace HashTag.Data.Config.Configurations
{
    public class RequestHistoryLogConfiguration : IEntityConfiguration
    {
        public void Configure(ModelBuilder builder)
        {
            builder.Entity<RequestHistoryLog>().ToTable("RequestHistoryLog").HasKey(x => x.Id);
        }
    }
}