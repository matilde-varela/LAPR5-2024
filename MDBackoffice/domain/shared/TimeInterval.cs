using System;

namespace MDBackoffice.Domain.Shared
{

    public class TimeInterval : IValueObject, IComparable<TimeInterval>
    {
        public TimeSpan Start { get; }
        public TimeSpan End { get; }
        public bool Verification { get; }

        private TimeInterval() { }

        public TimeInterval(string startString, string endString, bool verification)
        {
            if (!TimeSpan.TryParse(startString, out TimeSpan start) || !TimeSpan.TryParse(endString, out TimeSpan end))
                throw new BusinessRuleValidationException("Invalid time format. Use hh:mm.", nameof(startString));

            if (!verification && end < start)
                throw new BusinessRuleValidationException("End time must be greater than or equal to start time.");

            Start = start;
            End = end;
        }

        // Implement IComparable<TimeInterval>
        public int CompareTo(TimeInterval other)
        {
            if (other == null) return 1; // Consider null as greater (optional)
            return Start.CompareTo(other.Start); // Compare based on the Start TimeSpan
        }

        public override string ToString()
        {
            return $"{Start:HH}h{Start:mm}-{End:HH}h{End:mm}";
        }

        public override bool Equals(object obj)
        {
            if (obj is TimeInterval other)
            {
                return Start == other.Start && End == other.End && Verification == other.Verification;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Start, End, Verification);
        }
    }

}