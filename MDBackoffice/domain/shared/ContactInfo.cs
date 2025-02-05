namespace MDBackoffice.Domain.Shared
{
    public class ContactInfo : IValueObject
    {
        public Email EmailAdress { get; }

        public Phone PhoneNumber { get; }

        private ContactInfo() { }
        public ContactInfo(string emailAdress, string countryCode, string phoneNumber)
        {
            this.EmailAdress = new Email(emailAdress);
            this.PhoneNumber = new Phone(countryCode, phoneNumber);
        }

        public ContactInfo(string emailAdress, string phoneNumber)
        {
            this.EmailAdress = new Email(emailAdress);
            this.PhoneNumber = new Phone(phoneNumber);
        }
    }
}
