using System;

namespace MDBackoffice.Domain.Shared
{
    public class Status : IValueObject
    {

        public string StatusName{ get;  private set; }

        public Status() 
        {

        }

        public Status (string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("Status cannot be null or empty.");
            }
            this.StatusName = status;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (Status)obj;
            return StatusName == other.StatusName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(StatusName);
        }

        public override string ToString()
        {
            return "Status: " + StatusName;
        }

    }
}