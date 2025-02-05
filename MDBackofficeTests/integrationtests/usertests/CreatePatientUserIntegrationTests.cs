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
using MDBackoffice.Domain.StaffProfiles;
using MDBackoffice.Domain.Specializations;
using MDBackoffice.Infrastructure.Users;


namespace MDBackofficeTests.integrationtests.patient
{
    public class CreatePatientUserIntegrationTests
    {
        private readonly Mock<UserService> _userServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<LogService> _logServiceMock = new(new Mock<IUnitOfWork>().Object, new Mock<ILogRepository>().Object);
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<RoleManager<Role>> _roleManagerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<TokenService> _tokenServiceMock;
        private readonly Mock<PatientService> _patientServiceMock;
        private readonly Mock<EmailService> _emailServiceMock;
        private readonly Mock<SignInManager<User>> _signinManagerMock;
        private readonly Mock<StaffService> _staffServiceMock;
        private readonly Mock<IStaffRepository> _staffRepoMock;
        private readonly Mock<ISpecializationRepository> _specRepoMock;
        private readonly Mock<IPatientRepository> _patientRepoMock;
        private readonly Mock<ILoginAdapter> _loginAdapterMock;
                private readonly Mock<IPatientMedicalRecordAdapter> _patientMRAMock;


        public CreatePatientUserIntegrationTests()
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

            _roleManagerMock = new Mock<RoleManager<Role>>(
                new Mock<IRoleStore<Role>>().Object,
                new List<IRoleValidator<Role>>(),
                new Mock<ILookupNormalizer>().Object,
                identityErrorDescriberMock.Object,
                 new Mock<ILogger<RoleManager<Role>>>().Object
            );

            _tokenServiceMock = new Mock<TokenService>(_unitOfWorkMock.Object, new Mock<ITokenRepository>().Object, _userManagerMock.Object);
            _emailServiceMock = new Mock<EmailService>(_tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);
            _configurationMock = new Mock<IConfiguration>();
            _loginAdapterMock = new Mock<ILoginAdapter>();

            _signinManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object,
                                                              new Mock<IHttpContextAccessor>().Object,
                                                              new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                                                              identityOptionsMock.Object,
                                                              new Mock<ILogger<SignInManager<User>>>().Object,
                                                              new Mock<IAuthenticationSchemeProvider>().Object,
                                                              new Mock<IUserConfirmation<User>>().Object);

            _userServiceMock = new Mock<UserService>(_userManagerMock.Object, _roleManagerMock.Object, _logServiceMock.Object, _signinManagerMock.Object, _emailServiceMock.Object, _configurationMock.Object, _tokenServiceMock.Object, _loginAdapterMock.Object);

            _patientRepoMock = new Mock<IPatientRepository>();
            _patientMRAMock = new Mock<IPatientMedicalRecordAdapter>();
            _patientServiceMock = new Mock<PatientService>(_unitOfWorkMock.Object, _logServiceMock.Object, _configurationMock.Object, _patientRepoMock.Object,
                    _userServiceMock.Object, _emailServiceMock.Object, _patientMRAMock.Object);
            _staffRepoMock = new Mock<IStaffRepository>();
            _specRepoMock = new Mock<ISpecializationRepository>();
            _staffServiceMock = new Mock<StaffService>(_unitOfWorkMock.Object, _logServiceMock.Object, _staffRepoMock.Object, _specRepoMock.Object,
                    _userManagerMock.Object, _configurationMock.Object, _emailServiceMock.Object, _userServiceMock.Object);
        }

        [Fact]
        public async Task RegisterPatientUser_ReturnsOkResult_IntegrationControllerService()
        {
             // Arrange
            var _controller = new UserController(_userServiceMock.Object, _patientServiceMock.Object, _staffServiceMock.Object, _tokenServiceMock.Object);

            var email = "test@email.com";
            var password = "NewPass00_d";
            var id = "testid";
            var phone = "+351 960444772";

            var dtoMock = new RegisterPatientUserDto
            {
                Email = email,
                Password = password,
                Phone = phone,
            };


            var userMock = new Mock<User>();
            userMock.Setup(u => u.Id).Returns(id);
            userMock.Setup(u => u.UserName).Returns(email);
            userMock.Setup(u => u.Email).Returns(email);
            userMock.Setup(u => u.Status).Returns(true);
            userMock.Setup(u => u.PasswordHash).Returns(password);

            var resultMock = IdentityResult.Success;

            var patientMock = new Mock<Patient>("first", "last", "first last", "country, 12345, street test", "female", "+123", "960444772", "98765432", email, "2000-10-10", "000001");
            var patientDto = new PatientDto(patientMock.Object.Name.ToString(), patientMock.Object.PhoneNumber.ToString(), patientMock.Object.Email.ToString(),
                patientMock.Object.Id.AsString());

            var creatingPatientProfile = new CreatingPatientDto(
                "first",
                "last",
                "country, 12345, street test",
                phone,
                email,
                "+351 98765432",
                "female",
                "2000-10-10",
                new List<string> { "6A80", "3A01.1" },
                new List<string> { "BZ05.3", "BZ02.2" },
                "description"
            );

            var token = "test-token";
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), password)).ReturnsAsync(resultMock);
            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), "Patient")).ReturnsAsync(resultMock);
            _roleManagerMock.Setup(rm => rm.RoleExistsAsync("Patient")).ReturnsAsync(true);
            _userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(["Patient"]);
            _patientRepoMock.Setup(repo => repo.ExistsPatientWithEmailOrPhone(email, "+351", "960444772")).ReturnsAsync(true);
            _patientRepoMock.Setup(repo => repo.GetPatientWithEmail(email)).ReturnsAsync(patientMock.Object);
            _patientRepoMock.Setup(s => s.FindPatientWithEmailOrPhone(email, "+351", "960444772")).ReturnsAsync(patientMock.Object);
            _userManagerMock.Setup(u => u.GenerateEmailConfirmationTokenAsync(It.IsAny<User>())).ReturnsAsync(token);
            _configurationMock.Setup(c => c["App:Email"]).Returns("testemail@email.com");
            _configurationMock.Setup(c => c["App:BaseUrl"]).Returns("https://test/api");
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _controller.RegisterPatientUser(dtoMock);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal("{ message = The user has been successfully created. Please verify your email to complete the registration. }", okResult.Value.ToString());

            // Verify interactions
            _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<User>(), password), Times.Once);
            _userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<User>(), "Patient"), Times.Once);
        }


        [Fact]
        public async Task CreatePatientUserAsync_ReturnsUser_IntegrationServiceDomain()
        {
            // Arrange
            var email = "test@gmail.com";
            var password = "#Test12345";
            var phone = "+351 960444772";
            var role = "Patient";

            var registerPatientUserDto = new RegisterPatientUserDto { Email = email, Password = password, Phone = phone };

            _roleManagerMock.Setup(rm => rm.RoleExistsAsync(role)).ReturnsAsync(true);

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), password))
                            .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), role))
                            .ReturnsAsync(IdentityResult.Success);

            var userService = new UserService(
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _logServiceMock.Object,
                _signinManagerMock.Object,
                _emailServiceMock.Object,
               _configurationMock.Object,
                _tokenServiceMock.Object,
                _loginAdapterMock.Object
            );

            // Act
            var result = await userService.CreatePatientUserAsync(registerPatientUserDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
            _userManagerMock.Verify(r => r.CreateAsync(It.IsAny<User>(), password), Times.Once);
            _userManagerMock.Verify(r => r.AddToRoleAsync(It.IsAny<User>(), role), Times.Once);
            _roleManagerMock.Verify(r => r.RoleExistsAsync(role), Times.Once);
        }



        [Fact]
        public async Task ConfirmEmailPatient_ReturnsOkResult_IntegrationControllerService()
        {
            // Arrange
            var _controller = new UserController(_userServiceMock.Object, _patientServiceMock.Object, _staffServiceMock.Object, _tokenServiceMock.Object);

            var userId = "valid_user_id";
            var token = "valid_token";

            _userServiceMock.Setup(service => service.ConfirmEmailPatient(userId, token))
                            .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ConfirmEmailPatient(userId, token);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("{ message = Email confirmed successfully and account activated. }", okResult.Value.ToString());
        }


         [Fact]
        public async Task ConfirmEmailPatient_ReturnsSucess_IntegrationServiceDomain()
        {
            // Arrange
            var userId = "testUserId";
            var token = "validToken";
            var user = new User { Status = false };
            
            _userManagerMock.Setup(repo => repo.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>())).Returns(Task.FromResult(IdentityResult.Success));
            _tokenServiceMock.Setup(t => t.ConfirmEmailToken(userId, token)).ReturnsAsync(true);

            var userService = new UserService(
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _logServiceMock.Object,
                _signinManagerMock.Object,
                _emailServiceMock.Object,
               _configurationMock.Object,
                _tokenServiceMock.Object,
                _loginAdapterMock.Object
            );

            // Act
            await userService.ConfirmEmailPatient(userId, token);

            // Assert
            Assert.True(user.Status);
            _userManagerMock.Verify(repo => repo.UpdateAsync(It.Is<User>(u => u == user)), Times.Once);
        }


    }
}