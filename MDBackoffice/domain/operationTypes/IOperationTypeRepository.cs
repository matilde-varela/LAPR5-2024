using System.Collections.Generic;
using System.Threading.Tasks;
using MDBackoffice.Domain.OperationTypes.ValueObjects;
using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;
using MDBackoffice.Domain.Shared;


namespace MDBackoffice.Domain.OperationTypes
{
    public interface IOperationTypeRepository:IRepository<OperationType,OperationTypeId>
    {

         public Task<OperationType> GetByIdWithStaffAsync(OperationTypeId id); 
         Task<List<OperationType>> FilterOperationTypes(OperationTypeQueryParametersDto dto);
        Task<OperationType> GetByNameAsync(string name);
        public Task<List<RequiredStaff>> GetRequiredStaffByOperationTypeIdAsync(OperationTypeId operationTypeId);
    }
}