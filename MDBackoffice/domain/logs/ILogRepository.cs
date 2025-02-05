using System.Threading.Tasks;
using MDBackoffice.Domain.Logs;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Logs
{
    public interface ILogRepository : IRepository<Log, LogId>
    {
        Task<LogId> FindLastLogIdAsync();
    }
}