using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.OperationTypesRecords
{
    public class OperationTypeParentId : IValueObject
    {

        public string OpID{ get;  private set; }

        public OperationTypeParentId(){
            // for ORM
        }

        public OperationTypeParentId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Operation Type Id cannot be null or empty.");
            }
            this.OpID  = id;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (OperationTypeParentId)obj;
            return OpID == other.OpID;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(OpID);
        }

    }
}