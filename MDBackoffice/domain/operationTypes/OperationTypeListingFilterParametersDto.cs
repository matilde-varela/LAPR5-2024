
using System.Collections.Generic;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.OperationTypes
{
    public class OperationTypeListingFilterParametersDto
    {
        internal IEnumerable<OperationTypeListingFilterParametersDto> queryFilters;

        public string Name { get; set; }
        public string Specialization { get; set;}
        public string Status { get; set;}
    }
}