using System.Threading.Tasks;

namespace HashTag.Contracts
{
    public interface IUnitOfWork
    {
        bool WasCommited { get; }

        bool WasRollBacked { get; }

        bool IsCompleted { get; }

        Task CommitAsync();

        Task RollbackAsync();
    }
}