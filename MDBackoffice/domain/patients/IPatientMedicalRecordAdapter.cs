using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDBackoffice.Domain.Patients
{
    public interface IPatientMedicalRecordAdapter
    {
        Task<bool> CreateMedicalRecord(MedicalRecordNumber medicalRecordNumber, List<string> medicalConditions,
            List<string> allergies, string description, string token);

        Task<string> ExportMedicalRecordData(MedicalRecordNumber medicalRecordNumber, string filePath, string password);
    }
}