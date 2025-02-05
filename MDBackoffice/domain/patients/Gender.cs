using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Patients
{

    public class Gender : IValueObject
    {
        public string Denomination { get; }

        public static Gender Female { get; } = new Gender("female");
        public static Gender Male { get; } = new Gender("male");

        private Gender(string denomination)
        {
            if (string.IsNullOrEmpty(denomination))
            {
                throw new BusinessRuleValidationException("Gender denomination cannot be null or empty");
            }
            Denomination = denomination;
        }
        public static Gender? GetGenderByDescription(string description)
        {
             if (string.IsNullOrEmpty(description))
            {
                throw new BusinessRuleValidationException("Gender denomination cannot be null or empty");
            }
            
            return string.IsNullOrEmpty(description.ToLower()) ? null : description.ToLower() switch
            {
                var d when string.Equals(d, Female.Denomination) => Female,
                var d when string.Equals(d, Male.Denomination) => Male,
                _ => throw new BusinessRuleValidationException("Gender denomination must be 'female' or 'male'")
            };
        }

        public override bool Equals(object obj)
        {
            return obj is Gender gender && Denomination == gender.Denomination;
        }

        public override int GetHashCode()
        {
            return Denomination.GetHashCode();
        }

    }

}