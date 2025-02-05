using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.OperationTypesRecords
{
    public class OperationTypeRecordVersion : IValueObject
    {

        public int Version { get;  private set; }

        public OperationTypeRecordVersion(){
            // for ORM
        }

        public OperationTypeRecordVersion(int version)
        {
            if (int.IsNegative(version))
            {
                throw new ArgumentException("Operation Type Record version cannot be negative.");
            }
            this.Version = version;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (OperationTypeRecordVersion)obj;
            return Version == other.Version;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Version);
        }

    }
}