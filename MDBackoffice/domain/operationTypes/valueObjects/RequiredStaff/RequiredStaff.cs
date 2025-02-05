using System;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.Specializations;

namespace MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff
{
    public class RequiredStaff : Entity<RequiredStaffId>
    {

        public OperationTypeId OperationTypeId { get; set; }
        public NumberStaff StaffQuantity { get;  private set; }
        public Function Function { get;  private set; }
        public SpecializationCode SpecializationId { get;  private set; }


        public RequiredStaff(){
            // for ORM
        } 

        public RequiredStaff(int staffneeded, string function,  string specialization)
        {
            this.Id = new RequiredStaffId(RandomSequenceGenerator.GenerateUniqueRandomSequence(5));
            this.StaffQuantity = new NumberStaff(staffneeded);
            this.SpecializationId = new SpecializationCode(specialization);
            this.Function = Function.GetFunctionByDescription(function);
        }

        private static Function MapFunction(string functionDescription)
        {
            return functionDescription.ToLower() switch
            {
                "intern" => Function.Intern,
                "doctor" => Function.Doctor,
                "nurse" => Function.Nurse,
                "assistant" => Function.Assistant,
                _ => throw new ArgumentException("Invalid function description")
            };
        }

        public void ChangeStaffQuantity(int quantity)
        {
            this.StaffQuantity = new NumberStaff(quantity);
        }


        public void ChangeFunction(string function)
        {
            this.Function = MapFunction(function);
        }

        
        public void ChangeSpecialization(string specialization)
        {
            this.SpecializationId = new SpecializationCode(specialization);
        }

    
    }
}