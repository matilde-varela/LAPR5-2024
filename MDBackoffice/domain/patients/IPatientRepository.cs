using System.Collections.Generic;
using System.Threading.Tasks;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Patients
{
    public interface IPatientRepository : IRepository<Patient, MedicalRecordNumber>
    {
        Task<bool> ExistsPatientWithEmailOrPhone(string email, string v1, string v2);
        Task<bool> ExistsPatientWithId(string MRN);
        Task<List<Patient>> FilterPatientProfiles(PatientQueryParametersDto dto);
        Task<MedicalRecordNumber> FindLastPatientIdAsync();
        Task<Patient> FindPatientWithUserEmail(string email);
        Task<Patient> GetPatientWithEmail(string email);
        Task<Patient> FindPatientWithEmailOrPhone(string email, string v1, string v2);
        Task<Patient> GetByIdWithAppointmentHistoryAsync(MedicalRecordNumber medicalRecordNumber);
        Task<MedicalRecordNumber> GetMedicalRecordNumberOfPatientWithEmail(string email);
    }
}