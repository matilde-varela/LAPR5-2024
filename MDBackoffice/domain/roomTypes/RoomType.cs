using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.RoomTypes
{
    public class RoomType : Entity<RoomTypeCode>, IAggregateRoot
    {
        public RoomTypeDesignation Designation { get;}
        public RoomTypeDescription Description { get;}

        public RoomType()
        {
            //for EF Core
        }

        public RoomType(string code, string designation, string description)
        {
            this.Id = new RoomTypeCode(code);
            this.Designation = new RoomTypeDesignation(designation);
            this.Description = new RoomTypeDescription(description);
        }

        public override bool Equals(object obj)
        {
            if (obj is not RoomType other)
            {
                return false;
            }

            return Id.Equals(other.Id) &&
                   Designation.Equals(other.Designation) &&
                   Description.Equals(other.Description);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Designation, Description);
        }

        
    }
}
