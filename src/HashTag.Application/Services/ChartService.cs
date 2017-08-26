using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HashTag.Contracts.Services;
using HashTag.Domain;
using HashTag.Domain.DependencyInjection;
using HashTag.Domain.Dtos;

namespace HashTag.Application.Services
{
    [TransientDependency(ServiceType = typeof(IChartService))]
    internal class ChartService : IChartService
    {
        public async Task<IEnumerable<ClustersChartGroupDto>> GetClustersChartData()
        {
            const int r = 1;
            var result = Enumerable.Range(0, 10)
                .Select(x => x * 2m)
                .Select((x, i) =>
                    new ClustersChartGroupDto
                    {
                        Id = i,
                        Label = $"Cluster {i + 1}",
                        Color = Colors.All().ElementAt(i),
                        CoreLocation = Tuple.Create(x, x),
                        ItemsLocations = Enumerable
                            .Range(0, 15)
                            .Select(y => y * 2 * Math.PI / 15)
                            .Select(theta =>
                                Tuple.Create(
                                    x + r * Convert.ToDecimal(Math.Cos(theta)),
                                    x + r * Convert.ToDecimal(Math.Sin(theta))
                                ))
                    });

            return result;
        }
    }
}