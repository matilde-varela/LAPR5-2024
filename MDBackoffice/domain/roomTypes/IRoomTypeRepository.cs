using System.Collections.Generic;
using System.Threading.Tasks;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.RoomTypes
{
    public interface IRoomTypeRepository : IRepository<RoomType, RoomTypeCode>
    {
        Task<List<RoomType>>  GetAllRoomTypesAsync();
        Task<RoomType>  GetByDesignationAsync(string designation);
    }
}