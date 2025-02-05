using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Patients
{
        public class PatientListingFilterParametersDto {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string DateBirth { get; set; }
        public string MedicalRecordNumber { get; set; }

        public PatientListingFilterParametersDto(string firstName, string lastName, string email, string gender, string date, string medicalRecordNumber)
        {
            FirstName = firstName;
            LastName  = lastName;
            Email  = email;
            Gender  = gender;
            DateBirth = date;
            MedicalRecordNumber = medicalRecordNumber;
        }

    }
    
}
