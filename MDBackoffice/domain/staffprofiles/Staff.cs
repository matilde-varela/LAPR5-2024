using System;
using System.Collections.Generic;
using MDBackoffice.Domain.Appointments;
using MDBackoffice.Domain.AppointmentStaffs;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.Specializations;
using MDBackoffice.Domain.Users;

namespace MDBackoffice.Domain.StaffProfiles
{
    public class Staff : Entity<StaffId>, IAggregateRoot
    {
        public LicenseNumber LicenseNumber { get; private set; }
        public Name Name { get; private set; }
        public Phone Phone { get; private set; }
        public Email Email { get; private set; }
        public List<Slot> Slots { get; set; }
        public Function Function { get; private set; }
        public ResidentialAddress Address { get; private set; }
        public SpecializationCode SpecializationId { get; private set; }
        public string? UserReference { get; set; }
        public bool Status { get; set; }
        public ICollection<AppointmentStaff> AppointmentStaffs {get; private set;} = new List<AppointmentStaff>();

        private Staff() { }
        public Staff(string seqNumber, string address, string licenseNumber, string firstName, string lastName, string email, string phoneNumber, Function function, SpecializationCode specializationId)
        {
            this.Id = new StaffId(function.GetCorrespondingChar(), seqNumber);
            Address = new ResidentialAddress(address);
            LicenseNumber = new LicenseNumber(licenseNumber);
            Name = new Name(firstName, lastName);
            Phone = new Phone(phoneNumber);
            Email = new Email(email);
            Slots = new List<Slot>();
            Function = function;
            Status = true;
            SpecializationId = specializationId ?? throw new BusinessRuleValidationException("Staff members must have a specialization.");
        }
        public Staff(string seqNumber, string address, string licenseNumber, string firstName, string lastName, string fullName, string email, string countryCode, string phoneNumber, string function, string specializationId)
        {
            Address = new ResidentialAddress(address);
            LicenseNumber = new LicenseNumber(licenseNumber);
            Name = new Name(firstName, lastName, fullName);
            Phone = new Phone(countryCode, phoneNumber);
            Email = new Email(email);
            Slots = [];
            Function = Function.GetFunctionByDescription(function);
            this.Id = new StaffId(Function.GetCorrespondingChar(), seqNumber);
            Status = true;
            SpecializationId = new SpecializationCode(specializationId) ?? throw new BusinessRuleValidationException("Staff members must have a specialization.");
        }

        public void AddSlot(string startTime, string endTime, string startDate, string endDate = null)
        {
            var slot = new Slot(startTime, endTime, startDate, endDate);
            Slots.Add(slot);
        }

        public virtual void AddUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            if (this.UserReference != null)
            {
                throw new InvalidOperationException("This staff member already has a user assigned.");
            }

            this.UserReference = user.Id;
        }

        public void ChangeEmail(string email){
            if(this.Email.Equals(new Email(email))) throw new BusinessRuleValidationException("The new email is identical to the existing one.");
            this.Email = new Email(email);
        }

        public void ChangePhone(string phone){
            if (this.Phone.Equals(new Phone(phone))) throw new BusinessRuleValidationException("The new phone number is identical to the existing one.");
            this.Phone = new Phone(phone);
        }

        public void ChangeAddress(string address){
             if (this.Address.Equals(new ResidentialAddress(address))) throw new BusinessRuleValidationException("The new address is identical to the existing one.");
            this.Address = new ResidentialAddress(address);
        }

        public void ChangeSpecialization(string specialization){
            if(SpecializationId.Equals(new SpecializationCode(specialization))) throw new BusinessRuleValidationException("The new specialization is identical to the existing one.");
            this.SpecializationId = new SpecializationCode(specialization);
        }

        public void ChangeSlots(List<SlotsDto> slots)
        {
            Slots = new List<Slot>();

            foreach (SlotsDto slotDto in slots)
            {
                this.Slots.Add(new Slot(slotDto.StartTime, slotDto.EndTime, slotDto.StartDate, slotDto.EndDate));

            }
        }

        public virtual void DeactivateProfile()
        {
            this.Status = false;
        }

        public virtual void ToggleProfile()
        {
            this.Status = !this.Status;
        }

        public void ActivateProfile()
        {
            this.Status = true;
        }

    }
}