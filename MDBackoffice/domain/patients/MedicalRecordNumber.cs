using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Patients
{
    public class MedicalRecordNumber : EntityId
    {
        public MedicalRecordNumber(string seqNumber, bool isUnfinished)
            : base($"{DateTime.Now.Year}{DateTime.Now.Month.ToString("D2")}{seqNumber}")
        {
            if (seqNumber.Length != 6 || !int.TryParse(seqNumber, out _))
            {
                throw new BusinessRuleValidationException("Sequential number must be a 5-digit number.");
            }
            if (isUnfinished) 
            {
                return;
            }
        }

        public MedicalRecordNumber(string value)
                 : base(value)
        {

        }
        protected override object createFromString(string text)
        {
            if (string.IsNullOrEmpty(text) || text.Length < 10)
            {
                throw new BusinessRuleValidationException("Invalid Medical Record Number format.");
            }

            string year = text[..4];
            string month = text.Substring(4, 2);
            string seqNumber = text[6..];

            if (!int.TryParse(year, out _) || !int.TryParse(month, out _) || !int.TryParse(seqNumber, out _))
            {
                throw new BusinessRuleValidationException("Invalid year, month, or sequential number format.");
            }

            if (seqNumber.Length != 6)
            {
                throw new BusinessRuleValidationException("Sequential number must be a 6-digit number.");
            }

            // Now, create a new instance using the valid seqNumber
            return text;
        }


        public override string AsString()
        {
            return Value;
        }
    }
}