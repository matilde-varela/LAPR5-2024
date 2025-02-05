using System;

namespace MDBackoffice.Domain.Shared
{
    public class Date : IValueObject
    {
        public DateTime Start { get; }
        public DateTime End { get; }

        public Date() { }

        public Date(string start, string end = null)
        {
            if (string.IsNullOrWhiteSpace(start))
                throw new BusinessRuleValidationException("Start date cannot be null or empty.");

            if (!DateTime.TryParse(start, out DateTime startDate))
                throw new BusinessRuleValidationException("Invalid start date format.", nameof(start));

            if (string.IsNullOrWhiteSpace(end))
            {
                End = startDate;
            }
            else
            {
                if (!DateTime.TryParse(end, out DateTime endDate))
                    throw new BusinessRuleValidationException("Invalid end date format.", nameof(end));

                if (endDate < startDate)
                    throw new BusinessRuleValidationException("End date cannot be earlier than start date.", nameof(end));

                End = endDate;
            }
            Start = startDate;
        }

        public override string ToString()
        {
            // Check if End date is DateTime.MinValue or if Start equals End
            if (End == DateTime.MinValue || Start.Equals(End))
            {
                return $"{Start:yyyy-MM-dd}";
            }
            else
            {
                return $"{Start:yyyy-MM-dd}/{End:yyyy-MM-dd}";
            }
        }



        public override bool Equals(object obj)
        {
            return obj is Date other && Start == other.Start && End == other.End;
        }

        public override int GetHashCode() { return HashCode.Combine(Start, End); }

    }
}
