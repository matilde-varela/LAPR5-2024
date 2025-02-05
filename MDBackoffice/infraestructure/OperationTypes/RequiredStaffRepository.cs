using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;
using MDBackoffice.Infrastructure.Shared;

namespace MDBackoffice.Infrastructure.OperationTypes{
    public class RequiredStaffRepository : BaseRepository<RequiredStaff, RequiredStaffId>, IRequiredStaffRepository
    {
        private readonly MDBackofficeDbContext _context;
        public RequiredStaffRepository(MDBackofficeDbContext context):base(context.RequiredStaff)
        {
            _context = context;   
        }
    }
}