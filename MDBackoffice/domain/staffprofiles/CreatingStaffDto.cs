namespace MDBackoffice.Domain.StaffProfiles
{
    public class CreatingStaffDto(string licenseNumber, string address, string firstName, string lastName, string phone, string email, string function, string specializationId)
    {

        public string LicenseNumber { get; set; } = licenseNumber;
        public string FirstName { get; set; } = firstName;
        public string LastName { get; set; } = lastName;
        public string Phone { get; set; } = phone;
        public string Email { get; set; } = email;
        public string Address { get; set; } = address;
        public string Function { get; set; } = function;
        public string SpecializationId { get; set; } = specializationId;
    }
}