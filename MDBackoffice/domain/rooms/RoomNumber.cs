using System;
using System.Text.RegularExpressions;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Rooms
{
    public class RoomNumber : EntityId
    {
        private static readonly Regex RoomNumberPattern = new Regex(@"^R\d{3}$");

        public RoomNumber(string value) : base(value)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                throw new BusinessRuleValidationException("Rooms must have a number");
            }

            if (!RoomNumberPattern.IsMatch(value))
            {
                throw new BusinessRuleValidationException("Room number must be in the format 'R' followed by three digits (e.g., R101, R202).");
            }
        }

        protected override object createFromString(string text)
        {
            if (string.IsNullOrEmpty(text) || !RoomNumberPattern.IsMatch(text))
            {
                throw new BusinessRuleValidationException("Invalid Room Number format.");
            }

            return text;
        }

        public override string AsString()
        {
            return Value;
        }
    }
}
    

