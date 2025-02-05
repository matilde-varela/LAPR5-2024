using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MDBackoffice.Domain.Specializations;
using MDBackoffice.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace MDBackoffice.Infrastructure.Specializations
{
    public class SpecializationRepository : BaseRepository<Specialization, SpecializationCode>, ISpecializationRepository
    {
        private readonly MDBackofficeDbContext _context;

        public SpecializationRepository(MDBackofficeDbContext context) : base(context.Specializations)
        {
            _context = context;
        }

        public Task<List<Specialization>> FindAllConditioned(string? code, string? denomination, string? description)
        {
            var specializationQuery = _context.Specializations.AsQueryable();

            if (!string.IsNullOrEmpty(code))
            {
                // Ensure code is compared with a nullable Id, handle null properly.
                specializationQuery = specializationQuery
                    .Where(x => x.Id == new SpecializationCode(code));
            }

            if (!string.IsNullOrEmpty(denomination))
            {
                specializationQuery = specializationQuery
                    .Where(x => x.Denomination.Denomination.Contains(denomination));
            }

            if (!string.IsNullOrEmpty(description))
            {
                specializationQuery = specializationQuery
                    .Where(x => x.Description.Description.Contains(description));
            }

            return specializationQuery.ToListAsync();
        }

        public async Task<Specialization> FindByDenomination(string denomination)
        {
            return await _context.Specializations
                .Where(spec =>
                    spec.Id != null &&
                    spec.Id == new SpecializationCode(denomination))
                    .FirstOrDefaultAsync() ?? throw new NullReferenceException("Couldn't find the specialization with the denomination.");
        }
    }
}