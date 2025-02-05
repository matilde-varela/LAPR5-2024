using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.OperationTypes.ValueObjects
{
    public class OperationTypeName : IValueObject
    {

        public string OperationName { get;  private set; }

        public OperationTypeName(){
            //for EF Core
        }

        public OperationTypeName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new BusinessRuleValidationException("Operation name cannot be null or empty.");
            }
            this.OperationName = name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (OperationTypeName)obj;
            return OperationName == other.OperationName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(OperationName);
        }

    }
}