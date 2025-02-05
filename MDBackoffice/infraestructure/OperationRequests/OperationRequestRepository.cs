using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MDBackoffice.Domain.OperationRequests;
using MDBackoffice.Domain.OperationTypes;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.StaffProfiles;
using MDBackoffice.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace MDBackoffice.Infrastructure.OperationRequests
{
    public class OperationRequestRepository : BaseRepository<OperationRequest, OperationRequestId>, IOperationRequestRepository
    {
        private readonly MDBackofficeDbContext _context;
        public OperationRequestRepository(MDBackofficeDbContext context) : base(context.OperationRequests)
        {
            _context = context;
        }

        public async Task<IEnumerable<OperationRequest>> FindAllConditioned(
            StaffId staffId,
            string? name,
            string? priority,
            string? operationTypeId,
            string? status,
            string? dateOfRequest,
            string? deadLineDate)
        {
            // Step 1: Get the OperationRequests
            var operationRequestsQuery = _context.OperationRequests.AsQueryable();

            operationRequestsQuery = operationRequestsQuery
                    .Where(x => x.StaffId == staffId);

            // Step 2: Apply filters to OperationRequests first
            if (!string.IsNullOrEmpty(priority))
            {
                operationRequestsQuery = operationRequestsQuery
                    .Where(x => x.Priority.Name == priority);
            }

            if (!string.IsNullOrEmpty(status))
            {
                operationRequestsQuery = operationRequestsQuery
                    .Where(x => x.Status.Description == status);
            }

            if (!string.IsNullOrEmpty(dateOfRequest) && DateTime.TryParse(dateOfRequest, out var dateRequest))
            {
                operationRequestsQuery = operationRequestsQuery
                    .Where(x => x.DateOfRequest.Start == dateRequest.Date);
            }

            if (!string.IsNullOrEmpty(deadLineDate) && DateTime.TryParse(deadLineDate, out var deadLine))
            {
                operationRequestsQuery = operationRequestsQuery
                    .Where(x => x.DeadLineDate.Start == deadLine.Date);
            }

            OperationType? op = null;
            if (!string.IsNullOrEmpty(operationTypeId))
            {
                op = await _context.OperationTypes
                    .FirstOrDefaultAsync(ot => ot.Name.OperationName == operationTypeId)
                    ?? throw new BusinessRuleValidationException("Couldn't find the specified operation type.");
            }

            // Step 3: Fetch the OperationRequests and their related Patients
            var operationRequests = await operationRequestsQuery.ToListAsync();

            // Step 4: Get Patient names
            var patientIds = operationRequests.Select(or => or.PatientId).Distinct().ToList();
            var patients = await _context.Patients
                .Where(p => patientIds.Contains(p.Id))
                .ToListAsync();

            // Step 5: Combine results in memory
            var result = operationRequests
                .Where(or =>
                    (string.IsNullOrEmpty(name) || patients.Any(p => p.Id == or.PatientId && p.Name.Contains(name))) &&
                    (op == null || or.OperationTypeId == op.Id) // Match the OperationTypeId with the fetched OperationType's Id
                )
                .ToList();

            return result;
        }

        public async Task<List<OperationRequest>> GetAllFromDoctorAsync(string id)
        {
            return await _context.OperationRequests
                .Where(opRequest => opRequest.StaffId != null && opRequest.StaffId.ToString().Equals(id))
                .ToListAsync();
        }
    }
}