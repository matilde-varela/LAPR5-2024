using System.Collections.Generic;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.StaffProfiles
{
    public class StaffWithFunctionDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Function { get; set; }
        public string Address { get; set; }
        public string SpecializationId { get; set; }
        public List<SlotsDto>? Slots { get; set; }
        public string Status { get; set; }

        public StaffWithFunctionDto(string name, string phone, string email, string address, string function, string staffId, string specializationDenomination)
        {
            Name = name;
            Phone = phone;
            Email = email;
            Address = address;
            Function = function;
            SpecializationId = specializationDenomination;
            Id = staffId;
        }
        public StaffWithFunctionDto(string name, string phone, string email, string address, string function, string staffId, string specializationDenomination, List<SlotsDto> slots)
        {
            Name = name;
            Phone = phone;
            Email = email;
            Address = address;
            Function = function;
            SpecializationId = specializationDenomination;
            Id = staffId;
            Slots = slots;
        }

        public StaffWithFunctionDto(string name, string phone, string email, string address, string function, string staffId, string specializationDenomination, string status)
        {
            Name = name;
            Phone = phone;
            Email = email;
            Address = address;
            SpecializationId = specializationDenomination;
            Id = staffId;
            Function = function;
            Status = status;
        }
    }
}