using System;
using System.Linq;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Specializations
{
    public class SpecializationCode : EntityId
    {

        public SpecializationCode(string code) : base(code)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrWhiteSpace(code))
            {
                throw new BusinessRuleValidationException("Specializations must have a code.");
            }
            if (!code.All(char.IsDigit))
            {
                throw new BusinessRuleValidationException("Specializations code must contain only numeric digits.");
            }
            if (code.Length < 6)
            {
                throw new BusinessRuleValidationException("Specializations code must have at least 6 digits.");
            }
        }

        protected override object createFromString(string text)
        {
            return text;
        }

        public override string AsString()
        {
            // Return the value as a string
            return Value;
        }
    }
}
