using System.Linq;
using System.Threading.Tasks;
using MDBackoffice.Domain.OperationTypes;
using MDBackoffice.Domain.OperationTypesRecords;
using MDBackoffice.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;


namespace MDBackoffice.Infrastructure.OperationTypeRecords
{
    public class OperationTypeRecordRepository : BaseRepository<OperationTypeRecord, OperationTypeRecordId>, IOperationTypeRecordRepository
    {
        private readonly MDBackofficeDbContext _context;
      
        public OperationTypeRecordRepository(MDBackofficeDbContext context):base(context.OperationTypeRecords)
        {
            _context = context;
        }

        public async Task<OperationTypeRecord> GetLastFromOpType(OperationTypeId id)
        {
            var operationTypeRecord = await _context.OperationTypeRecords
                .Where(p => p.OperationTypeId.OpID == id.ToString())
                .OrderByDescending(p => p.Version.Version) 
                .FirstOrDefaultAsync();

            return operationTypeRecord;
        }

    }
}
