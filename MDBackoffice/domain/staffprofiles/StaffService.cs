using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MDBackoffice.Domain.Logs;
using MDBackoffice.Domain.Emails;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.Specializations;
using MDBackoffice.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace MDBackoffice.Domain.StaffProfiles
{
    public class StaffService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStaffRepository _repo;
        private readonly LogService _logService;
        private readonly ISpecializationRepository _repoSpec;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly UserService _userService;


        public StaffService(IUnitOfWork unitOfWork, LogService logService, IStaffRepository repo, ISpecializationRepository repoSpec, UserManager<User> userManager, IConfiguration configuration, EmailService emailService, UserService userService)
        {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
            this._logService = logService;
            this._repoSpec = repoSpec;
            this._userManager = userManager;
            this._configuration = configuration;
            this._emailService = emailService;
            this._userService = userService;
        }
        public async Task<StaffDto> GetByIdAsync(StaffId id)
        {
            var staff = await this._repo.GetByIdAsync(id);

            if (staff == null)
                return null;

            return new StaffDto(staff.Name.ToString(), staff.Phone.ToString(), staff.Email.ToString(), staff.Address.ToString(), staff.Id.AsString(), staff.SpecializationId.AsString());
        }
        public async Task<StaffDto> CreateStaffProfile(CreatingStaffDto dto)
        {

            if (await _repo.ExistsStaffWithEmailOrPhone(dto.Email, dto.Phone.Split(' ')[0], dto.Phone.Split(' ')[1]))
            {
                throw new BusinessRuleValidationException("There already exists a staff member with that email or phone.");
            }

            Specialization spec = await _repoSpec.FindByDenomination(dto.SpecializationId) ??
                throw new BusinessRuleValidationException("Invalid specialization.");

            var seqNumber = await getSequentialNumber();

            Function? function = Function.GetFunctionByDescription(dto.Function) ??
                throw new BusinessRuleValidationException("Invalid function.");

            var staff = new Staff(seqNumber, dto.Address, dto.LicenseNumber, dto.FirstName, dto.LastName, dto.Email,
            dto.Phone, function, spec.Id);

            await this._repo.AddAsync(staff);

            await this._unitOfWork.CommitAsync();

            return new StaffDto(staff.Name.ToString(), staff.Phone.ToString(), staff.Email.ToString(), staff.Address.ToString(), staff.Id.AsString(), staff.SpecializationId.AsString());
        }

        public async Task AddUser(User user, string email, string phone)
        {
            bool result = await _repo.ExistsStaffWithEmailOrPhone(email, phone.Split(' ')[0], phone.Split(' ')[1]);

            if (!result)
            {
                throw new InvalidOperationException("There isn't a staff member associated with this email/phone.");
            }

            Staff staff = await _repo.GetStaffWithEmail(email);

            staff.AddUser(user);

            await this._unitOfWork.CommitAsync();
        }

        private async Task<string> getSequentialNumber()
        {
            try
            {
                StaffId staffId = await _repo.FindLastStaffIdAsync();
                string lastSequentialNumber = staffId.AsString().Substring(5);
                int newSequentialNumber = int.Parse(lastSequentialNumber) + 1;
                return newSequentialNumber.ToString("D5");
            }
            catch (NullReferenceException)
            {
                return "00001";
            }
        }

        public virtual async Task<List<StaffDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();

            List<StaffDto> listDto = list.ConvertAll(staff => new StaffDto(staff.Name.ToString(),
                staff.Phone.ToString(), staff.Email.ToString(), staff.Address.ToString(), staff.Id.AsString(), staff.SpecializationId.AsString()));

            return listDto;
        }

        public async Task<List<StaffWithFunctionDto>> GetAllActiveAsync()
        {
            var list = await _repo.GetAllActiveAsync();

            List<StaffWithFunctionDto> listDto = list.ConvertAll(staff => new StaffWithFunctionDto(staff.Name.ToString(),
                staff.Phone.ToString(), staff.Email.ToString(), staff.Address.ToString(), staff.Function.Description, staff.Id.AsString(), staff.SpecializationId.AsString()));

            return listDto;
        }

        public async Task<StaffDto> UpdateAsync(string id, EditStaffDto dto)
        {
            var logEntries = new List<string>();
            var staff = await this._repo.GetByIdAsync(new StaffId(id));

            if (staff == null)
                return null;

            bool phoneChange = false, emailChange = false, addressChange = false;
            string oldEmail = staff.Email.ToString();

            if (dto.Phone != null && dto.Phone != staff.Phone.ToString())
                phoneChange = true;

            if (dto.Email != null && dto.Email != staff.Email.ToString())
                emailChange = true;

            if (dto.Address != null && dto.Address != staff.Address.ToString())
                addressChange = true;

            string changedInformation = null;
            if(phoneChange || emailChange || addressChange)
                changedInformation = VerificationsToSendEmail(phoneChange, emailChange, addressChange, dto);
                await EditStaffProfile(oldEmail, dto.Email, emailChange, staff, changedInformation);

            if (phoneChange)
            {
                staff.ChangePhone(dto.Phone);
                logEntries.Add($"phone=[{dto.Phone}]");
            }
                
            if (emailChange)
            {
                staff.ChangeEmail(dto.Email);
                logEntries.Add($"email=[{dto.Email}]");
            }
                
            if (addressChange)
            {
                staff.ChangeAddress(dto.Address);
                logEntries.Add($"address=[{dto.Address}]");
            }      

            if (dto.SpecializationId != null){
                staff.ChangeSpecialization(dto.SpecializationId);
                logEntries.Add($"specialization=[{dto.SpecializationId}]");
            }  

            var log = $"Edited information of staff profile. The information edited was: {string.Join(", ", logEntries)}.";
            await _logService.CreateEditLog(staff.Id.AsString(), staff.GetType().ToString(), log);

            await this._unitOfWork.CommitAsync();

            
                return new StaffDto(staff.Name.ToString(), staff.Phone.ToString(), staff.Email.ToString(),
                staff.Address.ToString(), staff.Id.AsString(), staff.SpecializationId.AsString());
        }

        private async Task EditStaffProfile(string oldEmail, string newEmail, bool emailChange, Staff staff, string changedInformation)
        {
            staff.DeactivateProfile();
            
            await _userService.EditStaffUserProfile(oldEmail, newEmail, staff.Id.AsString(), emailChange, changedInformation);
        }

        private string VerificationsToSendEmail(bool phoneChange, bool emailChange, bool addressChange, EditStaffDto dto)
        {
            string changedInformation = "<p>The new information is as follows:</p><ul>";
    
            if (phoneChange)
                changedInformation += "<li>Phone Number: " + dto.Phone + "</li>";
            if (emailChange)
                changedInformation += "<li>Email: " + dto.Email + "</li>";
            if (addressChange)
                changedInformation += "<li>Address: " + dto.Address + "</li>";
            
            changedInformation += "</ul>";

            return changedInformation;
        }

        public virtual async Task ConfirmEmailStaff(string userId, string staffId, string token)
        {
            var staff = await this._repo.GetByIdAsync(new StaffId(staffId));

            await _userService.ConfirmEmailStaffWithoutPassword(userId, token);

            staff.ActivateProfile();
            await this._unitOfWork.CommitAsync();
        }

        public virtual async Task<List<StaffDto>> FilterStaffProfiles(StaffQueryParametersDto dto)
        {
            List<Staff> filteredStaffs = await _repo.FilterStaffProfiles(dto);
            List<StaffDto> StaffDtoListFiltered = [];

            foreach (Staff Staff in filteredStaffs)
            {
                if(Staff.Status){
                    //string name, string phone, string email, string address, string specializationDenomination
                    StaffDtoListFiltered.Add(new StaffDto(Staff.Name.ToString(), Staff.Phone.ToString(), Staff.Email.ToString(), Staff.Address.ToString(), Staff.Id.AsString(), Staff.SpecializationId.AsString(), "Active"));
                }
                else
                {
                    //string name, string phone, string email, string address, string specializationDenomination
                    StaffDtoListFiltered.Add(new StaffDto(Staff.Name.ToString(), Staff.Phone.ToString(), Staff.Email.ToString(), Staff.Address.ToString(), Staff.Id.AsString(), Staff.SpecializationId.AsString(), "Deactivated"));
                }
            
            }

            return StaffDtoListFiltered;
        }

        public async Task<bool> DeactivateStaffProfile(string id)
        {
            var staff = await _repo.GetByIdAsync(new StaffId(id));
            if (staff == null)
            {
                throw new BusinessRuleValidationException("No staff exists with that Id.");
            }

            staff.DeactivateProfile();

            await _logService.CreateEditLog(id, staff.GetType().ToString(), "Deactivation of staff's profile.");

            await this._unitOfWork.CommitAsync();

            return true;
        }

        public async Task<string> GetProfileEmail(string email, string phone)
        {
            Staff staff = await _repo.FindStaffWithEmailOrPhone(email, phone.Split(' ')[0], phone.Split(' ')[1]);

            return staff.Email.EmailAddress;
        }

        public async Task<bool> AddTimeSlots(AddTimeSlotsDto addTimeSlotsDto, string email)
        {
            var staff = await this._repo.GetStaffWithEmail(email);
            if (staff == null)
            {
                throw new BusinessRuleValidationException("No staff exists with that Id.");
            }

            string[] slots = addTimeSlotsDto.Slot.Split("-");

            string startTimeSlot = slots[0].Replace("[","").Replace("]","");

            string endTimeSlot = slots[1].Replace("[","").Replace("]","");

            staff.AddSlot(startTimeSlot, endTimeSlot, addTimeSlotsDto.Date);

            await this._unitOfWork.CommitAsync();

            return true;
        }
    }
}
