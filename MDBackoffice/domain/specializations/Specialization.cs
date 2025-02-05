using System;
using System.Collections.Generic;
using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Specializations
{
    public class Specialization : Entity<SpecializationCode>, IAggregateRoot
    {
        public SpecializationDenomination Denomination { get; set; }
        public SpecializationDescription Description { get; set; }

        public Specialization()
        {
            //for EF Core
        }

        public Specialization(string code, string denomination, string description)
        {
            this.Id = new SpecializationCode(code);
            this.Denomination = new SpecializationDenomination(denomination);
            this.Description = new SpecializationDescription(description);
        }

        public void ChangeDenomination(string denom)
        {
            Denomination = new SpecializationDenomination(denom);
        }
        public void ChangeDescription(string descrip)
        {
            Description = new SpecializationDescription(descrip);
        }
    }
}