using Microsoft.EntityFrameworkCore;

namespace HashTag.Data.Config
{
    internal interface IEntityConfiguration
    {
        void Configure(ModelBuilder modelBuilder);
    }
}