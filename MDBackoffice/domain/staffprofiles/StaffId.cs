using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.StaffProfiles
{
    public class StaffId : EntityId
    {
        public StaffId(string function, string seqNumber)
              : base($"{function}{DateTime.Now.Year}{seqNumber}")
        {
            if (string.IsNullOrEmpty(function) || string.IsNullOrEmpty(seqNumber))
            {
                throw new BusinessRuleValidationException("Function and sequential number cannot be null or empty.");
            }

            if (function != "N" && function != "D" && function != "O")
            {
                throw new BusinessRuleValidationException("Function must be 'N' for nurse, 'D' for doctor, or 'O' for other.");
            }

            if (seqNumber.Length != 5 || !int.TryParse(seqNumber, out _))
            {
                throw new BusinessRuleValidationException("Sequential number must be a 5-digit number.");
            }
        }

        public StaffId(string value) : base(value)
        {

        }

        protected override object createFromString(string text)
        {
            if (string.IsNullOrEmpty(text) || text.Length < 10)
            {
                throw new BusinessRuleValidationException("Invalid Staff ID format.");
            }

            string function = text[..1];
            string year = text.Substring(1, 4);
            string seqNumber = text[5..];

            if (function != "N" && function != "D" && function != "O")
            {
                throw new BusinessRuleValidationException("Function must be 'N' for nurse, 'D' for doctor, or 'O' for other.");
            }

            if (!int.TryParse(year, out _) || !int.TryParse(seqNumber, out _))
            {
                throw new BusinessRuleValidationException("Invalid year or sequential number format.");
            }

            return text;
        }

        public override string AsString()
        {
            return Value;
        }
    }
}