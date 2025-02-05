using System;
using System.Linq;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.RoomTypes
{
    public class RoomTypeCode : EntityId
    {

        public RoomTypeCode(string code) : base(code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new BusinessRuleValidationException("Room Types must have a code.");
            }
            if (code.Length != 8)
            {
                throw new BusinessRuleValidationException("Room Types code must be exactly 8 characters long.");
            }
            if (code.Any(char.IsWhiteSpace))
            {
                throw new BusinessRuleValidationException("Room Types code must not contain spaces.");
            }
            if (!code.All(c => char.IsLetterOrDigit(c) || c == '-'))
            {
                throw new BusinessRuleValidationException("Room Types code must contain only letters, numbers, and dashes ('-').");
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
