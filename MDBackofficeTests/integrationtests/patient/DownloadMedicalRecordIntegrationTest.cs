using MDBackoffice.Controllers;
using MDBackoffice.Domain.Emails;
using MDBackoffice.Domain.Logs;
using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.Tokens;
using MDBackoffice.Domain.Users;
using MDBackoffice.Infrastructure.Emails;
using MDBackoffice.Infrastructure.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace MDBackofficeTests.integrationtests.patient
{
    public class DownloadMedicalRecordIntegrationTests
    {

        private readonly PatientService _service;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IPatientRepository> _repoMock = new Mock<IPatientRepository>();
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private readonly Mock<ILoginAdapter> _loginAdapterMock;
        private readonly Mock<UserService> _userServiceMock;
        private readonly Mock<IPatientMedicalRecordAdapter> _patientMRAMock;

        public DownloadMedicalRecordIntegrationTests()
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

            _service = new PatientService(_unitOfWorkMock.Object, _logServiceMock.Object,
                                            _configurationMock.Object, _repoMock.Object,
                                            _userServiceMock.Object, _emailServiceMock.Object, _patientMRAMock.Object);

        }

        [Fact]
        public async Task DownloadMedicalRecord_ReturnsOkPatient_IntegrationControllerService()
        {
            // Arrange
            var _controller = new PatientController(_service, _userServiceMock.Object);

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


        [Fact]
        public async Task DownloadMedicalRecord_ReturnsString_IntegrationServiceDomain()
        {
            // Arrange
            var dtoMock = new DownloadMedicalRecordDto(
                "202412000001",
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
}