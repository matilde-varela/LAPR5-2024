using System;
using System.Text.RegularExpressions;

namespace MDBackoffice.Domain.Shared
{
    public class Email : IValueObject
    {
        public string EmailAddress { get; }

        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);


        private Email() {}
        public Email(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                throw new BusinessRuleValidationException("Email EmailAddress cannot be null or empty.");
            }

            if ((!IsValidEmail(emailAddress)) && (!emailAddress.Equals("[REDACTED]")))
            {
                throw new BusinessRuleValidationException("Invalid email EmailAddress format.");
            }

            EmailAddress = emailAddress;
        }

        private static bool IsValidEmail(string email)
        {
            return EmailRegex.IsMatch(email);
        }

        public override string ToString()
        {
            return EmailAddress;
        }

        public override bool Equals(object obj)
        {
            if (obj is Email Email)
            {
                return string.Equals(EmailAddress, Email.EmailAddress, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return EmailAddress.GetHashCode();
        }
    }
}
