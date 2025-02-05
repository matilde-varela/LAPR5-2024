using System;

namespace MDBackoffice.Domain.Shared
{
    public class Name : IValueObject
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string FullName { get; }
        public Name(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                throw new BusinessRuleValidationException("Name cannot be null or empty.");
            }
            this.FirstName = firstName;
            this.LastName = lastName;
            this.FullName = $"{firstName} {lastName}";
        }
        public Name(string firstName, string lastName, string fullName)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(fullName))
            {
                throw new BusinessRuleValidationException("Name cannot be null or empty.");
            }
            this.FirstName = firstName;
            this.LastName = lastName;
            this.FullName = fullName;
        }

        public Name(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                throw new BusinessRuleValidationException("Name cannot be null or empty.");
            }

            var nameParts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (nameParts.Length < 2)
            {
                throw new BusinessRuleValidationException("Full name must contain at least a first and last name.");
            }


            this.FirstName = nameParts[0];
            this.LastName = nameParts[nameParts.Length - 1];
            this.FullName = fullName;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (Name)obj;
            return FirstName == other.FirstName && LastName == other.LastName && FullName == other.FullName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FirstName, LastName, FullName);
        }

        public override string ToString()
        {
            return FullName;
        }
        public bool Contains(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(value));
            }

            return FullName.Contains(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}