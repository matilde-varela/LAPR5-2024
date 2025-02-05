using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Patients
{
    public class AppointmentHistoryId : EntityId
    {
        // Parameterless constructor required by EF Core
        public AppointmentHistoryId(string value) : base(value)
        {
        }

        // Constructor for normal instantiation
        public AppointmentHistoryId(string value, bool v) : base(value)
        {
        }

        // Override to create the object from a string
        protected override object createFromString(string text)
        {
            return text;
        }

        // Override to convert to string
        public override string AsString()
        {
            return (string)base.Value;
        }
    }
}
