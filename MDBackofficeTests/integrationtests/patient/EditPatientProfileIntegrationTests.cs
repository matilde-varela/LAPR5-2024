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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MDBackofficeTests.integrationtests.patient
{
    public class EditPatientProfileIntegrationTests
    {

        private readonly PatientService _service;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IPatientRepository> _repoMock = new Mock<IPatientRepository>();
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private readonly Mock<ILoginAdapter> _loginAdapterMock;
        private readonly Mock<UserService> _userServiceMock;
        private readonly Mock<IPatientMedicalRecordAdapter> _patientMRAMock;

        public EditPatientProfileIntegrationTests()
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
        public async Task EditProfile_ReturnsAcceptedPatientDto_IntegrationControllerService()
        {
            //Arrange
            var _controller = new PatientController(_service, _userServiceMock.Object);

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
        public async Task EditProfile_ReturnsPatientDtoSucessfully_IntegrationServiceDomain()
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


            var patient = new Patient("first", "last", "first last", "country, 12345, street test", "female", "+123", "12345678", "98765432", oldEmail, "2000-10-10", "000001");
            var user = new User();
            user.UserName = email;
            user.Email = email;
            user.Status = true;
            user.PasswordHash = password;

            var token = "test-token";
            var idPatient = "202501000001";

            var dtoResult = new PatientDto("Rita Barbosa", "+351 910000000", newEmail, "Test, 1234-234, Test Test", "2000-10-10", idPatient);

            _repoMock.Setup(_repoPatMock => _repoPatMock.FindPatientWithUserEmail(oldEmail))
                .ReturnsAsync(patient);
            _userManagerMock.Setup(_userManagerMock => _userManagerMock.FindByEmailAsync(oldEmail)).ReturnsAsync(user);
            _userManagerMock.Setup(_userManagerMock => _userManagerMock.GenerateChangeEmailTokenAsync(user, newEmail)).ReturnsAsync(token);
            _userManagerMock.Setup(_userManagerMock => _userManagerMock.ChangeEmailAsync(user, newEmail, token)).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(_userManagerMock => _userManagerMock.UpdateAsync(user));
            _userManagerMock.Setup(_userManagerMock => _userManagerMock.GetRolesAsync(user)).ReturnsAsync(["Patient"]);
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
    }
}