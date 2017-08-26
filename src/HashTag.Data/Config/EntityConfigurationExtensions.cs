using HashTag.Data.Config.Configurations;
using HashTag.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace HashTag.Data.Config
{
    public static class EntityConfigurationExtensions
    {
        public static void ConfigureAllEntities(this ModelBuilder modelBuilder)
        {
            new UserConfiguration().Configure(modelBuilder);

            new ApplicationLogConfiguration().Configure(modelBuilder);
            new RequestHistoryLogConfiguration().Configure(modelBuilder);

            new PhotoConfiguration().Configure(modelBuilder);
            new HashTagConfiguration().Configure(modelBuilder);
            new PhotoHashTagConfiguration().Configure(modelBuilder);

            new ClusterConfiguration().Configure(modelBuilder);
            new ClusterSamplePhotoConfiguration().Configure(modelBuilder);

            new SamplePhotoConfiguration().Configure(modelBuilder);
            new PredictionClassCofiguration().Configure(modelBuilder);

            new KMeansResearchResultConfiguration().Configure(modelBuilder);
        }
    }
}