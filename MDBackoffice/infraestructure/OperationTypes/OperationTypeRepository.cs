using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MDBackoffice.Domain.OperationTypes;
using MDBackoffice.Domain.OperationTypes.ValueObjects;
using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;
using MDBackoffice.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MDBackoffice.Infrastructure.OperationTypes
{
    public class OperationTypeRepository : BaseRepository<OperationType, OperationTypeId>, IOperationTypeRepository
    {
      
      private readonly MDBackofficeDbContext _context;
        public OperationTypeRepository(MDBackofficeDbContext context):base(context.OperationTypes)
        {
            _context = context;   
        }

        public async Task<OperationType> GetByIdWithStaffAsync(OperationTypeId id)
        {
            var operationTypeById =
                _context.OperationTypes
                .Include(p => p.RequiredStaff)
                .Where(p => p.Id.Equals(id));

            return operationTypeById.FirstOrDefault();
        }


        public async Task<List<OperationType>> FilterOperationTypes(OperationTypeQueryParametersDto dto)
        {
            IQueryable<OperationType> combinedQuery = null;

            foreach (OperationTypeListingFilterParametersDto filter in dto.queryFilters)
            {
                var query = _context.OperationTypes
                    .Include(o => o.RequiredStaff)
                    .AsQueryable();

                if(!string.IsNullOrEmpty(filter.Name))
                {
                    query = query.AsEnumerable().Where(s => s.Name.OperationName.Contains(filter.Name)).AsQueryable();
                }

                if (!string.IsNullOrEmpty(filter.Specialization))
                {
                    query = query.AsEnumerable()
                        .Where(o => o.RequiredStaff.Any(rs => rs.SpecializationId.AsString() == filter.Specialization))
                        .AsQueryable();
                }

                if (!string.IsNullOrEmpty(filter.Status))
                {
                    query = query.AsEnumerable().Where(s => s.Status.AsString().Contains(filter.Status)).AsQueryable();
                }

                 if (!query.IsNullOrEmpty())
                {
                    combinedQuery = combinedQuery == null ? query : combinedQuery.Union(query);
                }
            }
            
            if(combinedQuery.IsNullOrEmpty())
                return [];

            return [.. combinedQuery];
        }

    public async Task<OperationType> GetByNameAsync(string name)
    {
        return await _context.OperationTypes
            .Include(p => p.RequiredStaff)
            .Include(p => p.Phases)
            .FirstOrDefaultAsync(p => p.Name.OperationName == name); // Compare the primitive property
    }

    public async Task<List<RequiredStaff>> GetRequiredStaffByOperationTypeIdAsync(OperationTypeId operationTypeId)
    {
        var operationType = await _context.OperationTypes
            .Include(o => o.RequiredStaff) 
            .FirstOrDefaultAsync(o => o.Id.Equals(operationTypeId));

        if (operationType == null)
        {
            throw new KeyNotFoundException("Operation Type not found.");
        }

        return operationType.RequiredStaff; 
    }
    }
}
