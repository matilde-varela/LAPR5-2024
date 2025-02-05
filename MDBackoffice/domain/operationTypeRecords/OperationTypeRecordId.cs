using System;
using MDBackoffice.Domain.Shared; 

namespace MDBackoffice.Domain.OperationTypesRecords
{
    public class OperationTypeRecordId : EntityId
    {

        public OperationTypeRecordId(string value):base(value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Operation name cannot be null or empty.");
            }
        }

        override
        protected  object createFromString(string text){
            return text;
        }
        override
        public string AsString(){
            return (string) Value;
        }
    }
}