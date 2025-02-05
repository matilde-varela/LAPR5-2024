namespace MDBackoffice.Domain.Patients
{
    public class DownloadMedicalRecordDto(string filePath, string password)
    {

        public string FilePath { get; set; } = filePath;
        public string Password { get; set; } = password;
       
    }
}
