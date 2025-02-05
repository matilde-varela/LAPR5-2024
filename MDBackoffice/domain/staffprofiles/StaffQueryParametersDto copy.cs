using System.Collections.Generic;
using MDBackoffice.Domain.StaffProfiles;

namespace MDBackoffice.Domain.Shared
{
    public class StaffQueryParametersDto
    {

        public List<StaffListingFilterParametersDto> queryFilters {get; set;}

        public StaffQueryParametersDto(List<StaffListingFilterParametersDto> QueryFilters)
        {
            queryFilters = QueryFilters;
        }

    }
}
