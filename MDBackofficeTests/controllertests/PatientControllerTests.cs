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
using MDBackoffice.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Claims;
using MDBackoffice.Infrastructure.Users;


namespace MDBackofficeTests.controllertests
{
    public class PatientControllerTests
    {

        private readonly Mock<PatientService> _service;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IPatientRepository> _repoMock = new Mock<IPatientRepository>();
        private readonly PatientController _controller;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private readonly Mock<UserService> _userServiceMock;
        private readonly Mock<ILoginAdapter> _loginAdapterMock;
        private readonly Mock<IPatientMedicalRecordAdapter> _patientMRAMock;

        public PatientControllerTests()
        {
            Mock<LogService> _logServiceMock = new Mock<LogService>(new Mock<IUnitOfWork>().Object, new Mock<ILogRepository>().Object);

            var identityOptionsMock = new Mock<IOptions<IdentityOptions>>();
            identityOptionsMock.Setup(o => o.Value).Returns(new IdentityOptions());
            var identityErrorDescriberMock = new Mock<IdentityErrorDescriber>();


            _userManagerMock = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, identityOptionsMock.Object, new Mock<IPasswordHasher<User>>().Object, new List<IUserValidator<User>> { new Mock<IUserValidator<User>>().Object }, new List<IPasswordValidator<User>> { new Mock<IPasswordValidator<User>>().Object }, new Mock<ILookupNormalizer>().Object, identityErrorDescriberMock.Object, new Mock<IServiceProvider>().Object, new Mock<ILogger<UserManager<User>>>().Object);
            var roleManagerMock = new Mock<RoleManager<Role>>(new Mock<IRoleStore<Role>>().Object, new List<IRoleValidator<Role>>(), new Mock<ILookupNormalizer>().Object, identityErrorDescriberMock.Object, new Mock<ILogger<RoleManager<Role>>>().Object);

            var tokenServiceMock = new Mock<TokenService>(_unitOfWorkMock.Object, new Mock<ITokenRepository>().Object, _userManagerMock.Object);
            var _emailServiceMock = new Mock<EmailService>(tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);
            _loginAdapterMock = new Mock<ILoginAdapter>();

            var signinManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object,
                                                               new Mock<IHttpContextAccessor>().Object,
                                                               new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                                                               identityOptionsMock.Object,
                                                               new Mock<ILogger<SignInManager<User>>>().Object,
                                                               new Mock<IAuthenticationSchemeProvider>().Object,
                                                               new Mock<IUserConfirmation<User>>().Object);
            _userServiceMock = new Mock<UserService>(_userManagerMock.Object, roleManagerMock.Object, _logServiceMock.Object, signinManagerMock.Object, _emailServiceMock.Object, _configurationMock.Object, tokenServiceMock.Object, _loginAdapterMock.Object);

            _patientMRAMock = new Mock<IPatientMedicalRecordAdapter>();
            _service = new Mock<PatientService>(_unitOfWorkMock.Object, _logServiceMock.Object,
                                            _configurationMock.Object, _repoMock.Object,
                                            _userServiceMock.Object, _emailServiceMock.Object, _patientMRAMock.Object);
            _controller = new PatientController(_service.Object, _userServiceMock.Object);
        }

        [Fact]
        public async Task CreatePatientProfile_Returns_CreatedResult()
        {
            //Arrage
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
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _patientMRAMock.Setup(m => m.CreateMedicalRecord(It.IsAny<MedicalRecordNumber>(), It.IsAny<List<string>>(), It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            //Act
            var result = await _controller.CreatePatientProfile(dtoMock);

            //Assert
            var actionResult = Assert.IsType<ActionResult<PatientDto>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetPatientById", createdAtActionResult.ActionName);
        }

        [Fact]
        public async Task DeletePatientProfile_ReturnsOkResult()
        {
            //Arrange
            var id = "202501000001";
            var dtoMock = new EditPatientDto
            (
                "Rita Barbosa",
                "+351 910000000",
                "ritabarbosa@email.com",
                "Test, 1234-234, Test Test",
                "2004-12-15"
            );
            var dtoId = new IdPassDto(id);

            var patientMock = new Mock<Patient>("first", "last", "first last", "country, 12345, street test", "female", "+123", "12345678", "98765432", "email@email.com", "2000-10-10", "000001");
            var dtoResult = new PatientDto("Rita Barbosa", "+351 910000000", "ritabarbosa@email.com", "Test, 1234-234, Test Test", "2004-12-15", id);
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);
            _repoMock.Setup(r => r.ExistsPatientWithId(id)).ReturnsAsync(true);
            _repoMock.Setup(_repoPatMock => _repoPatMock.GetByIdAsync(It.IsAny<MedicalRecordNumber>()))
            .ReturnsAsync(patientMock.Object);

            patientMock.Setup(p => p.Anonymize()).Returns(true);
            _userServiceMock.Setup(u => u.DeleteByIdAsync(patientMock.Object.UserReference)).ReturnsAsync(IdentityResult.Success);

            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);


            //Act
            var result = await _controller.DeletePatientProfile(dtoId);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            _repoMock.Verify(um => um.ExistsPatientWithId(id), Times.Once);
            _repoMock.Verify(um => um.GetByIdAsync(new MedicalRecordNumber(id)), Times.Once);
            _userServiceMock.Verify(um => um.DeleteByIdAsync(patientMock.Object.UserReference), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOkPatientDto()
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

            var dtoResult = new PatientDto("Rita Barbosa", "+351 910000000", "ritabarbosa@email.com", "Test, 1234-234, Test Test", "2004-12-15", id);
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);
            _repoMock.Setup(_repoPatMock => _repoPatMock.GetByIdAsync(It.IsAny<MedicalRecordNumber>()))
                .ReturnsAsync(patientMock.Object);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            //Act
            var result = await _controller.EditPatientProfile(id, dtoMock);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPatient = Assert.IsType<PatientDto>(okResult.Value);
            Assert.Equal(dtoResult.Name, returnedPatient.Name);
            Assert.Equal(dtoResult.Phone, returnedPatient.Phone);
            Assert.Equal(dtoResult.Email, returnedPatient.Email);
            Assert.Equal(dtoResult.Address, returnedPatient.Address);
            Assert.Equal(dtoResult.DateBirth, returnedPatient.DateBirth);
            Assert.Equal(dtoResult.PatientId, returnedPatient.PatientId);
        }

        [Fact]
        public async Task GetPatientProfiles_ReturnsPatientDtoList()
        {
            //Arrange
            List<PatientDto> result = new List<PatientDto>();
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);
            _service.Setup(p => p.GetAllAsysnc()).ReturnsAsync(result);

            //Act
            var resultList = _controller.GetPatientProfiles();

            //Assert
            var okResult = Assert.IsType<ActionResult<IEnumerable<PatientDto>>>(resultList.Result);
            var returnedPatient = Assert.IsType<List<PatientDto>>(okResult.Value);
        }

        [Fact]
        public async Task GetFilteredPatientProfiles_ReturnsPatientDtoList()
        {
            // Arrange
            string firstName = "Duarte";
            string lastName = "Matos";
            string email = "exampleemail@gmail.com";
            string gender = "Male";
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

            List<PatientDto> result = new List<PatientDto>();
            var dtoResult = new PatientDto("Rita Barbosa", "+351 910000000", "ritabarbosa@email.com", "Test, 1234-234, Test Test", "2004-12-15", id);

            result.Add(dtoResult);
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService =>
                    _userService.CheckUserRole("valid-token", It.Is<string>(role => role == "Admin" || role == "Doctor")))
                    .Returns(true);
            _service.Setup(p => p.FilterPatientProfiles(dto)).ReturnsAsync(result);

            //Act
            var resultController = await _controller.GetFilteredPatientProfiles(dto);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(resultController.Result);
            var returnedPatient = Assert.IsType<List<PatientDto>>(okResult.Value);
        }

        [Fact]
        public async Task EditProfile_ReturnsAcceptedPatientDto()
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

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);
            _userServiceMock.Setup(_userService => _userService.GetLoggedInEmail("valid-token")).Returns(oldEmail);

            _repoMock.Setup(_repoPatMock => _repoPatMock.FindPatientWithUserEmail(oldEmail))
                        .ReturnsAsync(patientMock.Object);
            _userManagerMock.Setup(_userManagerMock => _userManagerMock.FindByEmailAsync(oldEmail)).ReturnsAsync(userMock.Object);
            _userManagerMock.Setup(_userManagerMock => _userManagerMock.GenerateChangeEmailTokenAsync(userMock.Object, newEmail)).ReturnsAsync(token);
            _userManagerMock.Setup(_userManagerMock => _userManagerMock.ChangeEmailAsync(userMock.Object, newEmail, token)).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(_userManagerMock => _userManagerMock.UpdateAsync(userMock.Object));
            _userManagerMock.Setup(_userManagerMock => _userManagerMock.GetRolesAsync(userMock.Object)).ReturnsAsync(["Patient"]);
            _configurationMock.Setup(c => c["App:Email"]).Returns("testemail@email.com");
            _configurationMock.Setup(c => c["App:BaseUrl"]).Returns("https://test/api");
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            //Act
            var result = await _controller.EditPatientProfile(dtoMock);

            //Assert
            var okResult = Assert.IsType<AcceptedResult>(result.Result);
            var returnedPatient = Assert.IsType<PatientDto>(okResult.Value);
            Assert.Equal(dtoResult.Name, returnedPatient.Name);
            Assert.Equal(dtoResult.Phone, returnedPatient.Phone);
            Assert.Equal(dtoResult.Email, returnedPatient.Email);
            Assert.Equal(dtoResult.Address, returnedPatient.Address);
            Assert.Equal(dtoResult.DateBirth, returnedPatient.DateBirth);
            Assert.Equal(dtoResult.PatientId, returnedPatient.PatientId);
        }

        [Fact]
        public async Task DownloadMedicalRecord_ReturnsOkPatient()
        {
            // Arrange
            var dtoMock = new DownloadMedicalRecordDto(
                "202411000001",
                "Abcde12345!");
                
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Patient")).Returns(false);
            _userServiceMock.Setup(_userService => _userService.GetLoggedInEmail("valid-token")).Returns("ritabarbosa@email.com");
            _userServiceMock.Setup(_userService => _userService.ConfirmUserPasswordAsync("ritabarbosa@email.com", dtoMock.Password)).ReturnsAsync(true);
            _repoMock.Setup(_repoPatMock => _repoPatMock.GetMedicalRecordNumberOfPatientWithEmail("ritabarbosa@email.com")).ReturnsAsync(It.IsAny<MedicalRecordNumber>());
            _patientMRAMock.Setup(m => m.ExportMedicalRecordData(It.IsAny<MedicalRecordNumber>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("expected/file.pdf");

            // Act
            var result = await _controller.DownloadMedicalRecord(dtoMock);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var resturnedString = Assert.IsType<string>(okResult.Value);
            Assert.Equal("expected/file.pdf", resturnedString);
        }
    }
}

