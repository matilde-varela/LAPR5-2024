using System.Collections.Generic;
using MDBackoffice.Domain.Patients;

namespace MDBackoffice.Domain.Shared
{
    public class PatientQueryParametersDto
    {

        public List<PatientListingFilterParametersDto> QueryFilters { get; set; }

        public PatientQueryParametersDto(List<PatientListingFilterParametersDto> _queryFilters)
        {
            QueryFilters = _queryFilters;  
        }

    }
}
