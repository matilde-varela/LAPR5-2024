using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.StaffProfiles
{
    public class StaffListingFilterParametersDto {
    
    public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Specialization { get; set; }
    
    
        public StaffListingFilterParametersDto(string firstName, string lastName, string email, string specialization)
        {
            FirstName= firstName;
            LastName = lastName;
            Email = email;
            Specialization = specialization;
        }
    
    } 

}
