using System.Collections.Generic;
using MDBackoffice.Domain.OperationTypes.ValueObjects.Phase;
using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;


namespace MDBackoffice.Domain.OperationTypes
{
    public class OperationTypeDto
    {
        public string Name { get; set; }
        public int EstimatedDuration {get; set;}
        public bool Status {get; set;}
        public List<RequiredStaffDto> RequiredStaff {get; set;}
        public List<PhaseDto> Phases {get; set;} 

    }
}