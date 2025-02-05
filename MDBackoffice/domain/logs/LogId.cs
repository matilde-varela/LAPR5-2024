using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Logs
{
    public class LogId : EntityId
    {
        public LogId(string seqNumber, bool isSeqNumber)
            : base($"{DateTime.Now.Year}{seqNumber}")
        {
            if (seqNumber.Length != 5 || !int.TryParse(seqNumber, out _))
            {
                throw new BusinessRuleValidationException("Sequential number must be a 5-digit number.");
            }
        }

        public LogId(string value) : base(value)
        {
    
        }

        protected override object createFromString(string text)
        {
            if (string.IsNullOrEmpty(text) || text.Length < 9)
            {
                throw new ArgumentException("Invalid Log ID format.");
            }

            string year = text.Substring(0, 4); 
            string seqNumber = text.Substring(4); 

            // Validate year and sequence number
            if (!int.TryParse(year, out _) || !int.TryParse(seqNumber, out _))
            {
                throw new ArgumentException("Invalid year or sequential number format.");
            }

            return text;
        }

        public override string AsString()
        {
            return Value;
        }
    }
}
