using System;
using System.Collections.Generic;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.StaffProfiles
{
    public class EditStaffDto
    {
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? SpecializationId { get; set; }

        public EditStaffDto(string phone, string email, string address, string specializationDenomination)
        {
            Phone = phone;
            Email = email;
            Address = address;
            SpecializationId = specializationDenomination;
        }
    }
}