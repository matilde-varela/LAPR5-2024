using MDBackoffice.Controllers;
using MDBackoffice.Domain.Emails;
using MDBackoffice.Domain.Logs;
using MDBackoffice.Domain.OperationTypes;
using MDBackoffice.Domain.OperationTypesRecords;
using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.Specializations;
using MDBackoffice.Domain.StaffProfiles;
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
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MDBackofficeTests.integrationtests.usertests
{
    public class DeletePatientAccountIntegrationTests
    {
        private readonly UserService _userService;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<LogService> _logServiceMock = new(new Mock<IUnitOfWork>().Object, new Mock<ILogRepository>().Object);
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<TokenService> _tokenServiceMock;
        private readonly Mock<PatientService> _patientServiceMock;
        private readonly Mock<StaffService> _staffServiceMock;
        private readonly Mock<RoleManager<Role>> _roleManagerMock;
        private readonly Mock<EmailService> _emailServiceMock;
        private readonly Mock<ILoginAdapter> _loginAdapterMock;
        private readonly Mock<IPatientMedicalRecordAdapter> _patientMRAMock;

        public DeletePatientAccountIntegrationTests()
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

            var _signinManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object,
                                                               new Mock<IHttpContextAccessor>().Object,
                                                               new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                                                               identityOptionsMock.Object,
                                                               new Mock<ILogger<SignInManager<User>>>().Object,
                                                               new Mock<IAuthenticationSchemeProvider>().Object,
                                                               new Mock<IUserConfirmation<User>>().Object);

            _userService = new UserService(
                _userManagerMock.Object,
                _roleManagerMock.Object,
                _logServiceMock.Object,
                _signinManagerMock.Object,
                _emailServiceMock.Object,
                _configurationMock.Object,
                _tokenServiceMock.Object,
                _loginAdapterMock.Object
            );

            _patientMRAMock = new Mock<IPatientMedicalRecordAdapter>();
            _patientServiceMock = new Mock<PatientService>(_unitOfWorkMock.Object, _logServiceMock.Object, _configurationMock.Object, new Mock<IPatientRepository>().Object,
                    _userService, _emailServiceMock.Object, _patientMRAMock.Object);
            _staffServiceMock = new Mock<StaffService>(_unitOfWorkMock.Object, _logServiceMock.Object, new Mock<IStaffRepository>().Object, new Mock<ISpecializationRepository>().Object,
                   _userManagerMock.Object, _configurationMock.Object, _emailServiceMock.Object, _userService);
        }

        [Fact]
        public async Task ConfirmPatientAccountDeletionNotProfile_SuccessReturnsOk_IntegrationControllerService()
        {
            // Arrange
            var _controller = new UserController(_userService, _patientServiceMock.Object, _staffServiceMock.Object, _tokenServiceMock.Object);

            var userId = "testUserId";
            var token = Guid.NewGuid().ToString();
            var userEmail = "user@example.com";

            var user = new Mock<User>();
            user.Setup(u => u.Id).Returns(userId); 
            user.Setup(u => u.Email).Returns(userEmail); 

            _userManagerMock.Setup(u => u.FindByIdAsync(userId)).ReturnsAsync(user.Object); 
            _userManagerMock.Setup(u => u.DeleteAsync(user.Object)).ReturnsAsync(IdentityResult.Success); 
            _tokenServiceMock.Setup(t => t.TokenExistsById(token)).ReturnsAsync(true);
            _tokenServiceMock.Setup(t => t.IsTokenExpired(token)).ReturnsAsync(false);
            _tokenServiceMock.Setup(t => t.IsTokenActive(token)).ReturnsAsync(true);

            // Act
            var result = await _controller.ConfirmPatientAccountDeletionNotProfile(userId, token);

            // Assert
            _patientServiceMock.Verify(p => p.AnonymizeProfile(userEmail), Times.Once);
            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((OkObjectResult)result).Value);
            var okResult = result as OkObjectResult;
            Assert.Equal("{ message = Patient account successfully deleted!\nSome of your non-identifiable data will be retained, as per our GDPR policies. }", okResult.Value.ToString());
      }

        [Fact]
        public async Task DeletePatientAccount_Success_IntegrationServiceDomain()
        {
            //Arrange
            var email = "test@email.com";
            var id = "2966dfhf93739hd3-397993du93d3d3";
            var token = Guid.NewGuid().ToString();

            var user = new User { Id = id, Email = email };

            _userManagerMock.Setup(_userManagerMock => _userManagerMock.FindByIdAsync(id)).ReturnsAsync(user);
            _tokenServiceMock.Setup(t => t.InactivateAsync(token)).ReturnsAsync(new TokenDto());

            //Act
            var result = await _userService.DeleteAsync(id);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<string>(result);
            Assert.Equal(email, result);

        }

    }
}
