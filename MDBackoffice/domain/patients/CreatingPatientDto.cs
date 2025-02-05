using System.Collections.Generic;

namespace MDBackoffice.Domain.Patients
{
        public class CreatingPatientDto(string firstName, string lastName, string address, string phone, string email, string emergencyContact, string gender, string date, List<string> medicalConditions, List<string> allergies, string description)
    {

        public string FirstName { get; set; } = firstName;
        public string LastName { get; set; } = lastName;
        public string Phone { get; set; } = phone;
        public string Email { get; set; } = email;
        public string Address { get; set; } = address;
        public string EmergencyContact { get; set; } = emergencyContact;
        public string Gender { get; set; } = gender;
        public string DateBirth { get; set; } = date;
        public List<string> MedicalConditions { get; set; } = medicalConditions;
        public List<string> Allergies { get; set; } = allergies;
        public string Description { get; set; } = description;
    }
}
