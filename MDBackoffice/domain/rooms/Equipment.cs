using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Rooms
{
    public class Equipment : IValueObject
    {
        public string EquipmentName { get; private set; }

        public Equipment(){

        }

        public Equipment(string name)
        {
            if(string.IsNullOrEmpty(name)){
                throw new BusinessRuleValidationException("The Room Type cannot be null or empty.");
            }
            this.EquipmentName = name;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (Equipment)obj;
            return EquipmentName == other.EquipmentName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(EquipmentName);
        }
    }
}