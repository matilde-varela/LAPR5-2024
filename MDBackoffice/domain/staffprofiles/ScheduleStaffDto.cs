using System;
using System.Collections.Generic;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.StaffProfiles
{
    public class ScheduleStaffDto
    {
        public string Id { get; set; }
        public string Function { get; set; }
        public string SpecializationId { get; set; }
        public List<SlotsDto>? Slots { get; set; }

        public ScheduleStaffDto(string staffId, string function, string specializationDenomination, List<SlotsDto> slots)
        {
            Function = function;
            Slots = slots;
            SpecializationId = specializationDenomination;
            Id = staffId;
        }
    }
}