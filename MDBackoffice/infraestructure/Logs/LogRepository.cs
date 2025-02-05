using System;
using System.Linq;
using System.Threading.Tasks;
using MDBackoffice.Domain.Logs;
using MDBackoffice.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace MDBackoffice.Infrastructure.Logs
{
    public class LogRepository : BaseRepository<Log, LogId>, ILogRepository
    {
         private readonly MDBackofficeDbContext _context;
      
        public LogRepository(MDBackofficeDbContext context):base(context.Logs)
        {
            _context = context;
        }

        public async Task<LogId> FindLastLogIdAsync()
        {
            var logList = await _context.Logs.ToListAsync(); // Load all staff profiles into memory

            var lastLog = logList
                .OrderByDescending(log => log.Id.AsString().Substring(4)) // Now we can safely use AsString and Substring
                .FirstOrDefault() ?? throw new NullReferenceException();

            return lastLog.Id;
        }


    }
}