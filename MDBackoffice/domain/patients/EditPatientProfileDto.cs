namespace MDBackoffice.Domain.Patients
{
    public class EditPatientProfileDto
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? EmergencyContact { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }

        public EditPatientProfileDto() { }
        public EditPatientProfileDto(string? name, string? phone, string? emergencyContact, string? email, string? address)
        {
            Name = name;
            Phone = phone;
            EmergencyContact = emergencyContact;
            Email = email;
            Address = address;
        }
    }
}