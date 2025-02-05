using System.Threading.Tasks;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MDBackofficeDbContext _context;

        public UnitOfWork(MDBackofficeDbContext context)
        {
            this._context = context;
        }

        public async Task<int> CommitAsync()
        {
            return await this._context.SaveChangesAsync();
        }
    }
}