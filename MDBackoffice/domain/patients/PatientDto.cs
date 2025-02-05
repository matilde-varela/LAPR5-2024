using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Patients
{
    public class PatientDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string DateBirth { get; set;}
        public string PatientId { get; set; }

        public PatientDto(string name, string phone, string email, string address, string dateBirth, string patientId)
        {
            Name = name;
            Phone = phone;
            Email = email;
            Address = address;
            DateBirth = dateBirth;
            PatientId = patientId;
            
        }

        public PatientDto(string name, string phone, string email, string patientId)
        {
            Name = name;
            Phone = phone;
            Email = email;
            PatientId = patientId;
        }
    }
}
