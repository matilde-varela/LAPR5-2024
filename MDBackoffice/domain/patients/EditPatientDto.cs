namespace MDBackoffice.Domain.Patients
{
    public class EditPatientDto
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? DateBirth { get; set; }

        public EditPatientDto(string name, string phone, string email, string address, string datebirth)
        {
            Name = name;
            Phone = phone;
            Email = email;
            Address = address;
            DateBirth = datebirth;
        }

    }
}