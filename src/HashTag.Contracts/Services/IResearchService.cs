using System.Collections.Generic;
using System.Threading.Tasks;
using HashTag.Domain.Models;

namespace HashTag.Contracts.Services
{
    public interface IResearchService
    {
        Task KMeansResearchAsync();
        Task<IEnumerable<KMeansResearchResult>> GetKMeansResearchResultsAsync();
    }
}