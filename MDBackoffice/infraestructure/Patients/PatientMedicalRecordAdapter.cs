using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MDBackoffice.Domain.Patients;
using Microsoft.IdentityModel.Tokens;

namespace MDBackoffice.Infrastructure.Patients
{
    public class PatientMedicalRecordAdapter : IPatientMedicalRecordAdapter
    {
        public async Task<bool> CreateMedicalRecord(MedicalRecordNumber medicalRecordNumber, List<string> medicalConditions,
            List<string> allergies, string description, string token)
        {
            string json = createMDRJson(medicalRecordNumber, medicalConditions, allergies, description);
            Console.WriteLine(json);

            string url = "http://localhost:4000/api/medicalRecord/create";

            using (var httpClient = new HttpClient())
            {
                 httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                try
                {
                    var response = httpClient.PostAsync(url, content).Result;
                    response.EnsureSuccessStatusCode();
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(responseBody);

                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred: {ex.Message}");
                    throw;
                }
            }
        }

        public async Task<string> ExportMedicalRecordData(MedicalRecordNumber medicalRecordNumber, string filePath, string password)
        {
            object allData = new
            {
                medicalRecordNumber = medicalRecordNumber.AsString(),
                filepath = filePath,
                pass = password
            };

            string json = JsonSerializer.Serialize(allData, new JsonSerializerOptions { WriteIndented = true });
            Console.WriteLine(json);

            string url = "http://localhost:4000/api/medicalRecord/export";

            using (var httpClient = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                try
                {
                    var response = httpClient.PostAsync(url, content).Result;
                    response.EnsureSuccessStatusCode();
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(responseBody);

                    return responseBody;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred: {ex.Message}");
                    throw;
                }
            }
        }

        private string createMDRJson(MedicalRecordNumber medicalRecordNumber, List<string> medicalConditions,
            List<string> allergies, string description)
        {
            var mrnString = medicalRecordNumber.AsString();
            object allData;

            if ((medicalConditions == null || medicalConditions.Count == 0) &&
                (allergies == null || allergies.Count == 0) &&
                string.IsNullOrWhiteSpace(description))
            {
                allData = new
                {
                    id = mrnString,
                    medicalRecordNumber = mrnString
                };
            }
            else
            {
                allData = new
                {
                    id = mrnString,
                    medicalRecordNumber = mrnString,
                    medicalConditions,
                    allergies,
                    description = string.IsNullOrWhiteSpace(description) ? null : description
                };
            }

            return JsonSerializer.Serialize(allData, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}