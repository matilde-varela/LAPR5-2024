using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.StaffProfiles;
using MDBackoffice.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MDBackoffice.Infrastructure.StaffProfiles
{
    public class StaffRepository : BaseRepository<Staff, StaffId>, IStaffRepository
    {
        private readonly MDBackofficeDbContext _context;
        public StaffRepository(MDBackofficeDbContext context) : base(context.StaffProfiles)
        {
            _context = context;
        }
        public async Task<bool> ExistsStaffWithEmailOrPhone(string email, string countryCode, string phone)
        {
            return await _context.StaffProfiles
              .AnyAsync(staff =>
                  (staff.Email != null && staff.Email.EmailAddress == email) ||
                  (staff.Phone != null &&
                   staff.Phone.CountryCode == countryCode &&
                   staff.Phone.PhoneNumber == phone));

        }

        public async Task<List<Staff>> FilterStaffProfiles(StaffQueryParametersDto dto)
        {
            IQueryable<Staff> combinedQuery = null;

            foreach (StaffListingFilterParametersDto filter in dto.queryFilters)
            {

                var query = _context.StaffProfiles.AsQueryable();

                // if both names are in the filter then create the full name, else search by the provided name
                if (!string.IsNullOrEmpty(filter.FirstName) && !string.IsNullOrEmpty(filter.LastName))
                {
                    query = query.AsEnumerable().Where(s => s.Name.FullName.Contains($"{filter.FirstName} {filter.LastName}")).AsQueryable();
                }
                else
                {

                    if (!string.IsNullOrEmpty(filter.FirstName))
                    {
                        query = query.AsEnumerable().Where(s => s.Name.FirstName.Contains(filter.FirstName)).AsQueryable();
                    }

                    if (!string.IsNullOrEmpty(filter.LastName))
                    {
                        query = query.AsEnumerable().Where(s => s.Name.LastName.Contains(filter.LastName)).AsQueryable();
                    }

                }

                if (!string.IsNullOrEmpty(filter.Email))
                {
                    query = query.AsEnumerable().Where(s => s.Email.EmailAddress.Contains(filter.Email)).AsQueryable();
                }

                if (!string.IsNullOrEmpty(filter.Specialization))
                {
                    query = query.AsEnumerable()
                    .Where(s => s.SpecializationId.AsString() == filter.Specialization)
                    .AsQueryable();
                }

                if (!query.IsNullOrEmpty())
                {
                    combinedQuery = combinedQuery == null ? query : combinedQuery.Union(query);
                }

            }

            if (combinedQuery.IsNullOrEmpty())
                return [];

            return [.. combinedQuery];
        }

        public async Task<StaffId> FindLastStaffIdAsync()
        {
            var staffList = await _context.StaffProfiles.ToListAsync(); // Load all staff profiles into memory

            var lastStaff = staffList
                .OrderByDescending(staff => staff.Id.AsString().Substring(5)) // Now we can safely use AsString and Substring
                .FirstOrDefault() ?? throw new NullReferenceException();

            return lastStaff.Id;
        }

        public async Task<Staff> FindStaffWithEmailOrPhone(string email, string countryCode, string phone)
        {
            return await _context.StaffProfiles
                .Where(staff =>
                    (staff.Email != null && staff.Email.EmailAddress == email) ||
                    (staff.Phone != null &&
                    staff.Phone.CountryCode == countryCode &&
                    staff.Phone.PhoneNumber == phone))
                .FirstOrDefaultAsync(); // Get the first match or return null if none found
        }

        public async Task<Staff> FindStaffWithUserId(string userId)
        {
            return await _context.StaffProfiles
                .Where(staff =>
                    staff.UserReference != null &&
                    staff.UserReference == userId)
                    .FirstOrDefaultAsync() ?? throw new NullReferenceException("Couldn't find the staff with the specified user reference.");
        }

        public async Task<List<Staff>> GetAllActiveAsync()
        {
            return await _context.StaffProfiles
                .Where(staff => staff.Status == true)
                .ToListAsync();
        }


        public async Task<Staff> GetStaffWithEmail(string email)
        {
            return await _context.StaffProfiles
                .Where(staff =>
                    staff.Email != null &&
                    staff.Email.EmailAddress == email)
                    .FirstOrDefaultAsync() ?? throw new NullReferenceException("Couldn't find the staff with the specified email.");
        }

        public async Task<Staff> GetStaffWithIdIncludingSlots(string id)
        {
            var staffList = await _context.StaffProfiles
                .Include(staff => staff.Slots)
                .ToListAsync(); // Asynchronously fetch all staff with slots

            return staffList
                .FirstOrDefault(staff => staff.Id.AsString() == id); // Perform client-side filtering
        }

    }
}


