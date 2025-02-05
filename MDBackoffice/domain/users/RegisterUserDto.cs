namespace MDBackoffice.Domain.Users
{

    public class RegisterUserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string Phone { get; set; }
    }
}