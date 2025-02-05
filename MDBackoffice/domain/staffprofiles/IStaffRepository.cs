using System.Collections.Generic;
using System.Threading.Tasks;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.StaffProfiles
{
    public interface IStaffRepository : IRepository<Staff, StaffId>
    {
        Task<bool> ExistsStaffWithEmailOrPhone(string email, string CountryCode, string phone);
        Task<Staff> FindStaffWithEmailOrPhone(string email, string CountryCode, string phone);
        Task<List<Staff>> FilterStaffProfiles(StaffQueryParametersDto dto);
        Task<Staff> FindStaffWithUserId(string userId);
        Task<StaffId> FindLastStaffIdAsync();
        Task<Staff> GetStaffWithEmail(string email);
        Task<List<Staff>> GetAllActiveAsync();
        Task<Staff> GetStaffWithIdIncludingSlots(string id);
    }
}