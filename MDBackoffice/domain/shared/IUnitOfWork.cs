using System.Threading.Tasks;

namespace MDBackoffice.Domain.Shared
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync();
    }
}