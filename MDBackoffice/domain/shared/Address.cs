using System;

namespace MDBackoffice.Domain.Shared
{
    public class ResidentialAddress : IValueObject
    {
        public string Country { get; }
        public string Residence { get; }
        public string PostalCode { get; }

        // Private constructor for ORM tools
        private ResidentialAddress() { }

        public ResidentialAddress(string country, string residence, string postalCode)
        {
            if (string.IsNullOrWhiteSpace(country)) 
                throw new BusinessRuleValidationException("Country cannot be empty.");
            if (string.IsNullOrWhiteSpace(residence)) 
                throw new BusinessRuleValidationException("Residence cannot be empty.");
            if (string.IsNullOrWhiteSpace(postalCode)) 
                throw new BusinessRuleValidationException("Postal code cannot be empty.");

            Country = country;
            Residence = residence;
            PostalCode = postalCode;
        }

        // Constructor that parses a single string in the format "Country, PostalCode, Residence"
        public ResidentialAddress(string ResidentialaddressString)
        {
            if (string.IsNullOrWhiteSpace(ResidentialaddressString))
                throw new BusinessRuleValidationException("ResidentialAddress string cannot be empty.");

            var parts = ResidentialaddressString.Split(',');

            if (parts.Length != 3)
                throw new BusinessRuleValidationException("ResidentialAddress string must be in the format 'Country, PostalCode, Residence'.");

            Country = parts[0].Trim();
            PostalCode = parts[1].Trim();
            Residence = parts[2].Trim();

            if (string.IsNullOrWhiteSpace(Country))
                throw new BusinessRuleValidationException("Country cannot be empty.");
            if (string.IsNullOrWhiteSpace(PostalCode))
                throw new BusinessRuleValidationException("Postal code cannot be empty.");
            if (string.IsNullOrWhiteSpace(Residence))
                throw new BusinessRuleValidationException("Residence cannot be empty.");
        }

        public override string ToString()
        {
            return $"{Country}, {PostalCode}, {Residence}";
        }

        public override bool Equals(object obj)
        {
            if (obj is not ResidentialAddress other)
                return false;

            return Country == other.Country &&
                   Residence == other.Residence &&
                   PostalCode == other.PostalCode;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Country, Residence, PostalCode);
        }
    }
}
