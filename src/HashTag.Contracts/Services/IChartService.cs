using System.Collections.Generic;
using System.Threading.Tasks;
using HashTag.Domain.Dtos;

namespace HashTag.Contracts.Services
{
    public interface IChartService
    {
        Task<IEnumerable<ClustersChartGroupDto>> GetClustersChartData();
    }
}