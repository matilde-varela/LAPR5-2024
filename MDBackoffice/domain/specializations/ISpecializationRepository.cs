using System.Collections.Generic;
using System.Threading.Tasks;
using MDBackoffice.Domain.Shared;


namespace MDBackoffice.Domain.Specializations
{
    public interface ISpecializationRepository : IRepository<Specialization, SpecializationCode>
    {
        Task<List<Specialization>> FindAllConditioned(string? code, string? denomination, string? description);
        Task<Specialization> FindByDenomination(string denomination);
    }
}