using System.Threading.Tasks;
using MDBackoffice.Domain.OperationTypes;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.OperationTypesRecords
{
    public interface IOperationTypeRecordRepository : IRepository<OperationTypeRecord, OperationTypeRecordId>
    {
        Task<OperationTypeRecord> GetLastFromOpType(OperationTypeId id);
    }
}