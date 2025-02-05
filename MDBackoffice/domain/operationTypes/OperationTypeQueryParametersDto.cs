
using System.Collections.Generic;

namespace MDBackoffice.Domain.OperationTypes
{
    public class OperationTypeQueryParametersDto
    {
        public required List<OperationTypeListingFilterParametersDto> queryFilters { get; set;}
    }
}