using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.Emails;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.Users;
using MDBackoffice.Domain.Logs;
using MDBackoffice.Domain.Tokens;
using MDBackoffice.Infrastructure.Emails;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using MDBackoffice.Infrastructure;
using MDBackoffice.Infrastructure.Users;

namespace MDBackofficeTests.servicetests.patient;

public class PatientServiceTests
{
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IPatientRepository> _repoMock = new Mock<IPatientRepository>();
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private readonly Mock<LogService> _logServiceMock = new Mock<LogService>(new Mock<IUnitOfWork>().Object, new Mock<ILogRepository>().Object);
        private readonly Mock<UserService> _userServiceMock;
        private readonly Mock<ILoginAdapter> _loginAdapterMock;
        private readonly Mock<EmailService> _emailServiceMock;
        private readonly PatientService _service;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IPatientMedicalRecordAdapter> _patientMRAMock;

        public PatientServiceTests()
        {
            var identityOptionsMock = new Mock<IOptions<IdentityOptions>>();
                identityOptionsMock.Setup(o => o.Value).Returns(new IdentityOptions());
            var identityErrorDescriberMock = new Mock<IdentityErrorDescriber>();

            _userManagerMock = new Mock<UserManager<User>>(
                new Mock<IUserStore<User>>().Object,
                identityOptionsMock.Object,
                new Mock<IPasswordHasher<User>>().Object,
                new List<IUserValidator<User>> { new Mock<IUserValidator<User>>().Object },
                new List<IPasswordValidator<User>> { new Mock<IPasswordValidator<User>>().Object },
                new Mock<ILookupNormalizer>().Object,
                identityErrorDescriberMock.Object, 
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object
            );

            var roleManagerMock = new Mock<RoleManager<Role>>(
                new Mock<IRoleStore<Role>>().Object,
                new List<IRoleValidator<Role>>(),
                new Mock<ILookupNormalizer>().Object,
                identityErrorDescriberMock.Object,
                new Mock<ILogger<RoleManager<Role>>>().Object
            );

                var tokenServiceMock = new Mock<TokenService>(_unitOfWorkMock.Object, new Mock<ITokenRepository>().Object, _userManagerMock.Object);
                var _emailServMock = new Mock<EmailService>(tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);
                _configurationMock = new Mock<IConfiguration>();
                _loginAdapterMock = new Mock<ILoginAdapter>();
                _patientMRAMock = new Mock<IPatientMedicalRecordAdapter>();

            var signinManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object,
                                                                      new Mock<IHttpContextAccessor>().Object,
                                                                      new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                                                                      identityOptionsMock.Object,
                                                                      new Mock<ILogger<SignInManager<User>>>().Object,
                                                                      new Mock<IAuthenticationSchemeProvider>().Object,
                                                                      new Mock<IUserConfirmation<User>>().Object);
            _userServiceMock = new Mock<UserService>(
                    _userManagerMock.Object,
                    roleManagerMock.Object,
                    _logServiceMock.Object,
                    signinManagerMock.Object,
                    _emailServMock.Object,
                    _configurationMock.Object,
                    tokenServiceMock.Object,
                    _loginAdapterMock.Object
                );

            _emailServiceMock = new Mock<EmailService>(tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);
            _service = new PatientService(_unitOfWorkMock.Object, _logServiceMock.Object,
                                            _configurationMock.Object, _repoMock.Object,
                                            _userServiceMock.Object, _emailServiceMock.Object, _patientMRAMock.Object);
    }


       [Fact]
       public async Task CreatePatientProfile_ReturnsPatient()
       {

        //Arrange
        var dtoMock = new CreatingPatientDto
            ("Rita",
                "Barbosa",
                "Portugal, 4590-850, Rua da Sardinha",
                "+351 910000000",
                "ritabarbosa@email.com",
                "+351 912345678",
                "Female",
                "2004-12-15",
                new List<string> { "6A80", "3A01.1" },
                new List<string> { "BZ05.3", "BZ02.2" },
                "description");

        _patientMRAMock.Setup(m => m.CreateMedicalRecord(It.IsAny<MedicalRecordNumber>(),It.IsAny<List<string>>(), It.IsAny<List<string>>(),It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        //Act
        var result = await _service.CreatePatientProfile(dtoMock, "test-token");

        //Assert
        Assert.NotNull(result);
        Assert.Equal(dtoMock.Phone, result.Phone);
        Assert.Equal(dtoMock.Email, result.Email);
        Assert.Equal(dtoMock.Address, result.Address);
        Assert.Equal(dtoMock.DateBirth, result.DateBirth);        
        _repoMock.Verify(r => r.AddAsync(It.IsAny<Patient>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

       } 



        [Fact]
        public async Task UpdateAsync_ReturnsPatientDto()
        {
            //Arrange
                var dtoMock = new EditPatientDto
                ("Rita Barbosa",
                  "+351 910000000",
                  "ritabarbosa@email.com",
                  "Test, 1234-234, Test Test",
                  "2004-12-15");

            var patientMock = new Mock<Patient>("first", "last", "first last", "country, 12345, street test", "female", "+123", "12345678", "98765432", "email@email.com", "2000-10-10", "000001");
            var id = "202501000001";
            
            var dtoResult = new PatientDto("Rita Barbosa", "+351 910000000", "ritabarbosa@email.com", "Test, 1234-234, Test Test", "2004-12-15",id);
  
            _repoMock.Setup(_repoPatMock => _repoPatMock.GetByIdAsync(It.IsAny<MedicalRecordNumber>()))
                .ReturnsAsync(patientMock.Object);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            //Act
            var result =  await _service.UpdateAsync(id, dtoMock);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(dtoResult.Name, result.Name);
            Assert.Equal(dtoResult.Phone, result.Phone);
            Assert.Equal(dtoResult.Email, result.Email);
            Assert.Equal(dtoResult.Address, result.Address);
            Assert.Equal(dtoResult.DateBirth, result.DateBirth);
            Assert.Equal(dtoResult.PatientId, result.PatientId);
        }


    [Fact]
    public async Task DeletePatientProfile_ReturnsTask()
    {
        // Arrange
        var id = "202501000001";
        var dtoMock = new EditPatientDto
        (
            "Rita Barbosa",
            "+351 910000000",
            "ritabarbosa@email.com",
            "Test, 1234-234, Test Test",
            "2004-12-15"
        );

        var patientMock = new Mock<Patient>("first", "last", "first last", "country, 12345, street test", "female", "+123", "12345678", "98765432", "email@email.com", "2000-10-10", "000001");
        var dtoResult = new PatientDto("Rita Barbosa", "+351 910000000", "ritabarbosa@email.com", "Test, 1234-234, Test Test", "2004-12-15", id);

        _repoMock.Setup(r => r.ExistsPatientWithId(id)).ReturnsAsync(true);
        _repoMock.Setup(_repoPatMock => _repoPatMock.GetByIdAsync(It.IsAny<MedicalRecordNumber>()))
            .ReturnsAsync(patientMock.Object);

        patientMock.Setup(p => p.Anonymize()).Returns(true);

        _logServiceMock.Setup(l => l.CreateDeletionLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        _userServiceMock.Setup(u => u.DeleteByIdAsync(patientMock.Object.UserReference)).ReturnsAsync(IdentityResult.Success);

        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        // Act
        await _service.DeletePatientProfile(id);

        // Assert
        _repoMock.Verify(um => um.ExistsPatientWithId(id), Times.Once);
        _repoMock.Verify(um => um.GetByIdAsync(new MedicalRecordNumber(id)), Times.Once);
        _userServiceMock.Verify(um => um.DeleteByIdAsync(patientMock.Object.UserReference), Times.Once);
        _logServiceMock.Verify(um => um.CreateDeletionLog(patientMock.Object.UserReference, "MDBackoffice.Domain.Users", "Deletion of patient's account."), Times.Once);
    }


    [Fact]
    public async Task DeletePatientProfile_ReturnsBusinessRuleValidationException() 
    {
        // Arrange
        var id = "202501000001";
        var dtoMock = new EditPatientDto
        (
            "Rita Barbosa",
            "+351 910000000",
            "ritabarbosa@email.com",
            "Test, 1234-234, Test Test",
            "2004-12-15"
        );

        var patientMock = new Mock<Patient>("first", "last", "first last", "country, 12345, street test", "female", "+123", "12345678", "98765432", "email@email.com", "2000-10-10", "000001");

        var dtoResult = new PatientDto("Rita Barbosa", "+351 910000000", "ritabarbosa@email.com", "Test, 1234-234, Test Test", "2004-12-15", id);

        _repoMock.Setup(r => r.ExistsPatientWithId(id)).ReturnsAsync(true);
        _repoMock.Setup(_repoPatMock => _repoPatMock.GetByIdAsync(It.IsAny<MedicalRecordNumber>()))
            .ReturnsAsync(patientMock.Object);
        
        patientMock.Setup(p => p.Anonymize()).Returns(false);

        // Act
        var exception = await Assert.ThrowsAsync<BusinessRuleValidationException>(async () => await _service.DeletePatientProfile(id));

        // Assert
        Assert.Equal("It was not possible to anonymize the profile.", exception.Message);

        _repoMock.Verify(um => um.ExistsPatientWithId(id), Times.Once);
        _repoMock.Verify(um => um.GetByIdAsync(new MedicalRecordNumber(id)), Times.Once);
        _userServiceMock.Verify(um => um.DeleteByIdAsync(It.IsAny<string>()), Times.Never);
    }


        [Fact]
        public async Task EditProfile_ReturnsPatientDtoSucessfully()
        {
            //Arrange
            var oldEmail = "tes@email.com";
            var newEmail = "tesNew@email.com";
            var email = "test@email.com";
            var id = "testid";
            var password = "NewPass00_d";

            var dtoMock = new EditPatientProfileDto
            ("Rita Barbosa",
              "+351 910000000",
              "+351 910000010",
              newEmail,
              "Test, 1234-234, Test Test");


            var patientMock = new Mock<Patient>("first", "last", "first last", "country, 12345, street test", "female", "+123", "12345678", "98765432", oldEmail, "2000-10-10", "000001");
            var userMock = new Mock<User>();
            userMock.Setup(u => u.Id).Returns(id);
            userMock.Setup(u => u.UserName).Returns(email);
            userMock.Setup(u => u.Email).Returns(email);
            userMock.Setup(u => u.Status).Returns(true);
            userMock.Setup(u => u.PasswordHash).Returns(password);

            var token = "test-token";
            var idPatient = "202501000001";

            var dtoResult = new PatientDto("Rita Barbosa", "+351 910000000", newEmail, "Test, 1234-234, Test Test", "2000-10-10", idPatient);

            _repoMock.Setup(_repoPatMock => _repoPatMock.FindPatientWithUserEmail(oldEmail))
                .ReturnsAsync(patientMock.Object);
             _userManagerMock.Setup(_userManagerMock => _userManagerMock.FindByEmailAsync(oldEmail)).ReturnsAsync(userMock.Object);
             _userManagerMock.Setup(_userManagerMock => _userManagerMock.GenerateChangeEmailTokenAsync(userMock.Object,newEmail)).ReturnsAsync(token);
             _userManagerMock.Setup(_userManagerMock => _userManagerMock.ChangeEmailAsync(userMock.Object, newEmail,token)).ReturnsAsync(IdentityResult.Success);
             _userManagerMock.Setup(_userManagerMock => _userManagerMock.UpdateAsync(userMock.Object));
             _userManagerMock.Setup(_userManagerMock => _userManagerMock.GetRolesAsync(userMock.Object)).ReturnsAsync(["Patient"]);
            _configurationMock.Setup(c => c["App:Email"]).Returns("testemail@email.com");
            _configurationMock.Setup(c => c["App:BaseUrl"]).Returns("https://test/api");

              _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            //Act
            var result = await _service.EditProfile(oldEmail, dtoMock);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(dtoResult.Name, result.Name);
            Assert.Equal(dtoResult.Phone, result.Phone);
            Assert.Equal(dtoResult.Email, result.Email);
            Assert.Equal(dtoResult.Address, result.Address);
            Assert.Equal(dtoResult.DateBirth, result.DateBirth);
            Assert.Equal(dtoResult.PatientId, result.PatientId);
        }



    [Fact]
    public async Task AnonymizeProfile_ReturnsBool()
    {
        //Arrange
        var dtoMock = new CreatingPatientDto
            ("Rita",
                "Barbosa",
                "Portugal, 4590-850, Rua da Sardinha",
                "+351 910000000",
                "ritabarbosa@email.com",
                "+351 912345678",
                "Female",
                "2004-12-15",
                new List<string> { "6A80", "3A01.1" },
                new List<string> { "BZ05.3", "BZ02.2" },
                "description");

        _patientMRAMock.Setup(m => m.CreateMedicalRecord(It.IsAny<MedicalRecordNumber>(),It.IsAny<List<string>>(), It.IsAny<List<string>>(),It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        var service = new PatientService(_unitOfWorkMock.Object, _logServiceMock.Object,
                                            _configurationMock.Object, _repoMock.Object,
                                            _userServiceMock.Object, _emailServiceMock.Object, _patientMRAMock.Object);

        //Act
        var profile = await service.CreatePatientProfile(dtoMock, "test-token");
        var result = await service.AnonymizeProfile(profile.Email);

        //Assert
        Assert.NotNull(result);
        Assert.Equal(false, result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsPatientListDto()
    {
        // Arrange
        var patientMock = new Mock<Patient>("first", "last", "first last", "country, 12345, street test", "female", "+123", "12345678", "98765432", "email@email.com", "2000-10-10", "000001");

        List<Patient> result = new List<Patient> { patientMock.Object };

        _repoMock.Setup(_repo => _repo.GetAllAsync()).ReturnsAsync(result);

        // Act
        var patients = await _service.GetAllAsysnc();

        // Assert
        Assert.NotNull(patients);
        Assert.IsType<List<PatientDto>>(patients);
        Assert.Single(patients);
    }

    [Fact]
    public async Task FilterPatientProfiles_ReturnsPatientListDto()
    {
        //Arrange
        string firstName = "Duarte";
        string lastName = "Matos";
        string email = "exampleemail@gmail.com";
        string gender = "male";
        string date = "2004-12-15";
        string medicalRecordNumber = "202501000001";

        PatientListingFilterParametersDto listingFilterParametersDto
            = new PatientListingFilterParametersDto(
                firstName,
                lastName,
                email,
                gender,
                date,
                medicalRecordNumber);

        List<PatientListingFilterParametersDto> listingFilterParametersDtosList = new List<PatientListingFilterParametersDto>
            {
                listingFilterParametersDto
            };

        PatientQueryParametersDto dto = new PatientQueryParametersDto(listingFilterParametersDtosList);
        var id = "202501000001";

        var patientMock = new Mock<Patient>("Duarte", "Matos", "Duarte Matos", "country, 12345, street test", "male", "+123", "12345678", "98765432", "exampleemail@gmail.com", "2004-12-15", "000001");

        List<Patient> result = new List<Patient> { patientMock.Object };

        _repoMock.Setup(_repo => _repo.FilterPatientProfiles(dto)).ReturnsAsync(result);

        // Act
        var patients = await _service.FilterPatientProfiles(dto);

        // Assert
        Assert.NotNull(patients);
        Assert.IsType<List<PatientDto>>(patients);
        Assert.Single(patients);
    }

     [Fact]
        public async Task DownloadMedicalRecord_ReturnsString()
        {
            // Arrange
            var dtoMock = new DownloadMedicalRecordDto(
                "202411000001",
                "Abcde12345!");
                
 
            _userServiceMock.Setup(_userService => _userService.GetLoggedInEmail("valid-token")).Returns("ritabarbosa@email.com");
            _userServiceMock.Setup(_userService => _userService.ConfirmUserPasswordAsync("ritabarbosa@email.com", dtoMock.Password)).ReturnsAsync(true);
            _repoMock.Setup(_repoPatMock => _repoPatMock.GetMedicalRecordNumberOfPatientWithEmail("ritabarbosa@email.com")).ReturnsAsync(It.IsAny<MedicalRecordNumber>());
            _patientMRAMock.Setup(m => m.ExportMedicalRecordData(It.IsAny<MedicalRecordNumber>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("expected/file.pdf");

            // Act
            var result = await _service.DownloadMedicalRecord(dtoMock, "valid-token");

            // Assert
            Assert.NotNull(result);
            Assert.IsType<string>(result);
            Assert.Equal("expected/file.pdf", result);
        }
        
}