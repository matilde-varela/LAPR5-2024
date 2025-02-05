using System;
using MDBackoffice.Domain.Shared;
using Newtonsoft.Json;

namespace MDBackoffice.Domain.Appointments
{
    public class AppointmentId : EntityId
    {
        [JsonConstructor]
        public AppointmentId(Guid value) : base(value)
        {

        }

        public AppointmentId(string value) : base(value)
        {
            if(string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Appointment Id cannot be null or empty.");
            }
        }

        protected override object createFromString(string text)
        {
            return text;
        }

        public override string AsString()
        {
            Guid obj = (Guid)base.ObjValue;
            return obj.ToString();
        }
        public Guid AsGuid()
        {
            var ob = base.Value;
            return new Guid((string)ob);
        }
     }
}