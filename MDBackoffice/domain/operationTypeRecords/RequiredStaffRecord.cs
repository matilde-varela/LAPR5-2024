using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.OperationTypesRecords
    {
        public class RequiredStaffRecord : IValueObject
        {
            public string RequiredStaffRecordId {get; set;}
            public OperationTypeRecordId OperationTypeRecordId { get; set; }
            public NumberStaff StaffQuantity { get;  private set; }
            public Function Function { get;  private set; }
            public StaffSpecialization SpecializationId { get;  private set; }


            public RequiredStaffRecord(){
                // for ORM
            }

            public RequiredStaffRecord(int staffneeded, string function,  string specialization)
            {
                RequiredStaffRecordId = RandomSequenceGenerator.GenerateUniqueRandomSequence(5);
                StaffQuantity = new NumberStaff(staffneeded);
                SpecializationId = new StaffSpecialization(specialization);
                Function = Function.GetFunctionByDescription(function);
            }

        }
    }