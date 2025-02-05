using System;
using System.Text.RegularExpressions;

namespace MDBackoffice.Domain.Shared
{
    public class Phone : IValueObject
    {
        public string CountryCode;
        public string PhoneNumber;


        private Phone() { }
        public Phone(string countryCode, string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(countryCode) || string.IsNullOrWhiteSpace(phoneNumber))
            {
                throw new BusinessRuleValidationException("Phone number cannot be null or empty.");
            }
            if (!Regex.IsMatch(countryCode, @"^\+\d{1,3}$") && (!countryCode.Equals("[REDACTED]")))
            {
                throw new BusinessRuleValidationException("Country code must start with '+' followed by 1 to 3 digits.", nameof(countryCode));
            }
            if (!Regex.IsMatch(phoneNumber, @"^\d{7,15}$") && (!countryCode.Equals("[REDACTED]"))) // Assuming valid phone numbers are between 7 and 15 digits
            {
                throw new BusinessRuleValidationException("Phone number must be between 7 and 15 digits.", nameof(phoneNumber));
            }
            CountryCode = countryCode;
            PhoneNumber = phoneNumber;
        }

        public Phone(string combinedPhoneNumber)
        {
            if (string.IsNullOrEmpty(combinedPhoneNumber))
            {
                throw new BusinessRuleValidationException("Phone number cannot be null or empty.", nameof(combinedPhoneNumber));
            }

            // Use regex to extract country code and phone number allowing for space
            var match = Regex.Match(combinedPhoneNumber, @"^(\+\d{1,3})\s(\d{7,15})$");
            if (!match.Success)
            {
                throw new BusinessRuleValidationException("Invalid phone number format. Use '+[country code] [phone number]'.", nameof(combinedPhoneNumber));
            }

            CountryCode = match.Groups[1].Value;
            PhoneNumber = match.Groups[2].Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is Phone other)
            {
                return CountryCode == other.CountryCode && PhoneNumber == other.PhoneNumber;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CountryCode, PhoneNumber);
        }

        public override string ToString()
        {
            return $"{CountryCode} {PhoneNumber}";
        }
    }
}