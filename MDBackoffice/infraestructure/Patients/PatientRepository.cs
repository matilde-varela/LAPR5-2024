using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MDBackoffice.Infrastructure.Patients
{
    public class PatientRepository : BaseRepository<Patient, MedicalRecordNumber>, IPatientRepository
    {
        private readonly MDBackofficeDbContext _context;
        public PatientRepository(MDBackofficeDbContext context) : base(context.Patients)
        {
            _context = context;
        }

        public async Task<bool> ExistsPatientWithEmailOrPhone(string email, string countryCode, string phone)
        {
            return await _context.Patients
                .AnyAsync(patient =>
                    (patient.Email != null && patient.Email.EmailAddress == email) ||
                    (patient.PhoneNumber != null &&
                     patient.PhoneNumber.CountryCode == countryCode &&
                     patient.PhoneNumber.PhoneNumber == phone));
        }

        public async Task<bool> ExistsPatientWithId(string MRN)
        {
            List<Patient> reqPatients = _context.Patients
                .Where(s=> s.Id.Equals(new MedicalRecordNumber(MRN))).ToList();

            if(!reqPatients.IsNullOrEmpty()) return true;
            return false;
        }



        public async Task<List<Patient>> FilterPatientProfiles(PatientQueryParametersDto dto)
        {
           IQueryable<Patient> combinedQuery = null;

            foreach (PatientListingFilterParametersDto filter in dto.QueryFilters)
            {

                var query = _context.Patients.AsQueryable();

                // if both names are in the filter then create the full name, else search by the provided name
                if (!string.IsNullOrEmpty(filter.FirstName) && !string.IsNullOrEmpty(filter.LastName))
                {
                    query = query.AsEnumerable().Where(s => s.Name.FullName.Contains($"{filter.FirstName} {filter.LastName}")).AsQueryable();
                }
                else
                {

                    if (!string.IsNullOrEmpty(filter.FirstName))
                    {
                        query = query.AsEnumerable().Where(s => s.Name.FirstName.Contains(filter.FirstName)).AsQueryable();
                    }

                    if (!string.IsNullOrEmpty(filter.LastName))
                    {
                        query = query.AsEnumerable().Where(s => s.Name.LastName.Contains(filter.LastName)).AsQueryable();
                    }

                }

                if (!string.IsNullOrEmpty(filter.Email))
                {
                    query = query.AsEnumerable().Where(s => s.Email.EmailAddress.Contains(filter.Email)).AsQueryable();
                }


                if (!string.IsNullOrEmpty(filter.Gender))
                {
                    query = query.AsEnumerable().Where(s => s.Gender.Denomination.Equals(filter.Gender.ToLower())).AsQueryable();
                }

                if (!string.IsNullOrEmpty(filter.DateBirth))
                {
                    var parsedDate = DateTime.Parse(filter.DateBirth);
                    query = query.AsEnumerable().Where(s => s.DateBirth.Date == parsedDate.Date).AsQueryable();
                }

                if (!string.IsNullOrEmpty(filter.MedicalRecordNumber))
                {
                    query = query.AsEnumerable().Where(s => s.Id.AsString() == filter.MedicalRecordNumber).AsQueryable();
                }

                 if (!query.IsNullOrEmpty()){
                    combinedQuery = combinedQuery == null ? query : combinedQuery.Union(query);
                 }

            }

            if(combinedQuery.IsNullOrEmpty())
                return [];

            return [.. combinedQuery];
        }


        public async Task<MedicalRecordNumber> FindLastPatientIdAsync()
        {
            var patientList = await _context.Patients.ToListAsync(); // Load all staff profiles into memory

            var lastPatient = patientList
                .OrderByDescending(patient => patient.Id.AsString()[6..]) // Now we can safely use AsString and Substring
                .FirstOrDefault() ?? throw new NullReferenceException();

            return lastPatient.Id;
        }

        public async Task<Patient> FindPatientWithUserEmail(string email)
        {
            return await _context.Patients
                .Join(
                    _context.Users,
                    patient => patient.UserReference,
                    user => user.Id,
                    (patient, user) => new { patient, user }
                )
                .Where(joinResult => joinResult.user.Email == email)
                .Select(joinResult => joinResult.patient)
                .FirstOrDefaultAsync() ?? throw new NullReferenceException();
        }

        public async Task<Patient> GetPatientWithEmail(string email)
        {
            return await _context.Patients
                .Where(patient =>
                    patient.Email != null &&
                    patient.Email.EmailAddress == email)
                    .FirstOrDefaultAsync();
        }

         public async Task<Patient> FindPatientWithEmailOrPhone(string email, string countryCode, string phone)
        {
            return await _context.Patients
                .Where(patient =>
                    (patient.Email != null && patient.Email.EmailAddress == email) ||
                    (patient.PhoneNumber != null &&
                    patient.PhoneNumber.CountryCode == countryCode &&
                    patient.PhoneNumber.PhoneNumber == phone))
                .FirstOrDefaultAsync(); // Get the first match or return null if none found
        }

        public async Task<Patient> GetByIdWithAppointmentHistoryAsync(MedicalRecordNumber medicalRecordNumber)
        {
             var patient =
                _context.Patients
                .Include(p => p.AppointmentList)
                .Where(p => p.Id.Equals(medicalRecordNumber));

            return patient.FirstOrDefault();
        }

        public async Task<MedicalRecordNumber> GetMedicalRecordNumberOfPatientWithEmail(string email)
        {
            var patientId = await _context.Patients
                .Where(p => p.Email.EmailAddress.Equals(email))
                .Select(p => p.Id) 
                .FirstOrDefaultAsync(); 

            return patientId;
        }
    }
}