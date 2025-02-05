using System.Collections.Generic;
using System.Threading.Tasks;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.StaffProfiles;

namespace MDBackoffice.Domain.OperationRequests
{
    public interface IOperationRequestRepository : IRepository<OperationRequest, OperationRequestId>
    {
        Task<IEnumerable<OperationRequest>> FindAllConditioned(StaffId staffId, string? name, string? priority, string? operationType, string? status, string? dateOfRequest, string? deadLineDate);
        Task<List<OperationRequest>> GetAllFromDoctorAsync(string id);
    }
}