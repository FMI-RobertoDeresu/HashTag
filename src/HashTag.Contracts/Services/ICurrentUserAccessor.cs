using HashTag.Domain.Models;

namespace HashTag.Contracts.Services
{
    public interface ICurrentUserAccessor
    {
        string UserName { get; }
        User User { get; }
        ApplicationUser ApplicationUser { get; }
    }
}