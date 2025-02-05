using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MDBackoffice.Domain.RoomTypes;
using MDBackoffice.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace MDBackoffice.Infrastructure.RoomTypes
{
    public class RoomTypeRepository : BaseRepository<RoomType, RoomTypeCode>, IRoomTypeRepository
    {
        private readonly MDBackofficeDbContext _context;
        public RoomTypeRepository(MDBackofficeDbContext context) : base(context.RoomTypes)
        {
            _context = context;
        }

        public Task<List<RoomType>> GetAllRoomTypesAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<RoomType> GetByDesignationAsync(string designation)
        {
            return await _context.RoomTypes
                .Where(roomType =>
                    roomType.Designation != null &&
                    roomType.Designation.Designation == designation)
                    .FirstOrDefaultAsync() ?? throw new NullReferenceException("Couldn't find the room type with the specified designation.");

        }
    }
}