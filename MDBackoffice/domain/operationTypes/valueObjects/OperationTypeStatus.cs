using System;
using MDBackoffice.Domain.Shared;
using Newtonsoft.Json.Linq;

namespace MDBackoffice.Domain.OperationTypes.ValueObjects
{
    public class OperationTypeStatus : IValueObject
    {

        public bool Active{ get;  private set; }

        public OperationTypeStatus(){
            // for EF Core
        }

        public OperationTypeStatus(bool status)
        {
            this.Active = status;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (OperationTypeStatus)obj;
            return Active == other.Active;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Active);
        }

        public string AsString()
        {
            return Active ? "true" : "false";
        }

    }
}