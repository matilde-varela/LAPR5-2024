using MDBackoffice.Controllers;
using MDBackoffice.Domain.Emails;
using MDBackoffice.Domain.Logs;
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
using Xunit;

namespace MDBackofficeTests.integrationtests.usertests
{
    public class ResetPasswordIntegrationTests
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

        public ResetPasswordIntegrationTests()
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

            var signinManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object,
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
                    signinManagerMock.Object,
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
        public async Task ResetPassword_ReturnsOkResult_IntegrationControllerService()
        {
            // Arrange
            var _controller = new UserController(_userService, _patientServiceMock.Object, _staffServiceMock.Object, _tokenServiceMock.Object);

            var email = "test@email.com";
            var userMock = new Mock<User>();
            userMock.Setup(u => u.UserName).Returns(email);
            userMock.Setup(u => u.Email).Returns(email);
            userMock.Setup(u => u.Status).Returns(true);
            var resultMock = IdentityResult.Success;

            _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(userMock.Object);
            _userManagerMock.Setup(um => um.UpdateAsync(userMock.Object)).ReturnsAsync(resultMock);
            _userManagerMock.Setup(um => um.GenerateEmailConfirmationTokenAsync(userMock.Object)).ReturnsAsync("test token");
            _userManagerMock.Setup(um => um.GetRolesAsync(userMock.Object)).ReturnsAsync(["Doctor"]);
            _configurationMock.Setup(c => c["App:Email"]).Returns("testemail@email.com");
            _configurationMock.Setup(c => c["App:BaseUrl"]).Returns("https://test/api");
            // Act
            var result = await _controller.ResetPassword(email);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            userMock.Verify(u => u.changeStatus(false), Times.Once); // Verify that changeStatus was called
            _userManagerMock.Verify(um => um.UpdateAsync(userMock.Object), Times.Once); // Verify UpdateAsync was called
            _userManagerMock.Verify(um => um.GenerateEmailConfirmationTokenAsync(userMock.Object), Times.Once); // Verify token generation
        }

        [Fact]
        public async Task UpdatePassword_ReturnsOkResult_IntegrationControllerService()
        {
            // Arrange
            var _controller = new UserController(_userService, _patientServiceMock.Object, _staffServiceMock.Object, _tokenServiceMock.Object);

            var email = "test@email.com";
            var id = "testid";
            var token = "test-token";
            var password = "NewPass00_d";
            var dtoMock = new ConfirmEmailUserDto { NewPassword = password };

            var userMock = new Mock<User>();
                userMock.Setup(u => u.Id).Returns(id);
                userMock.Setup(u => u.UserName).Returns(email);
                userMock.Setup(u => u.Email).Returns(email);
                userMock.Setup(u => u.Status).Returns(true);
            var resultMock = IdentityResult.Success;

            _userManagerMock.Setup(_userManagerMock => _userManagerMock.FindByEmailAsync(email)).ReturnsAsync(userMock.Object);//
            _userManagerMock.Setup(um => um.FindByIdAsync(id)).ReturnsAsync(userMock.Object);
            _userManagerMock.Setup(um => um.ConfirmEmailAsync(userMock.Object, token)).ReturnsAsync(resultMock);
            _tokenServiceMock.Setup(_tokenServiceMock => _tokenServiceMock.ConfirmEmailToken(id, token)).ReturnsAsync(true);
            _tokenServiceMock.Setup(_tokenServiceMock => _tokenServiceMock.GeneratePasswordResetTokenAsync(userMock.Object)).ReturnsAsync(token);
            _userManagerMock.Setup(um => um.ResetPasswordAsync(userMock.Object, token, password)).ReturnsAsync(resultMock);
            _userManagerMock.Setup(um => um.UpdateAsync(userMock.Object)).ReturnsAsync(resultMock);
            // Act
            var result = await _controller.UpdatePassword(email, token, dtoMock);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _userManagerMock.Verify(um => um.UpdateAsync(userMock.Object), Times.Once);
            _userManagerMock.Verify(um => um.ResetPasswordAsync(userMock.Object, token, password), Times.Once);
        }

        [Fact]
        public async Task ResetPassword_ReturnsOkResult_IntegrationServiceDomain()
        {
            // Arrange
            var _controller = new UserController(_userService, _patientServiceMock.Object, _staffServiceMock.Object, _tokenServiceMock.Object);

            var email = "test@email.com";
            var user = new User();
            user.UserName = email;
            user.Email = email;
            user.Status = true;
            var resultMock = IdentityResult.Success;

            _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.UpdateAsync(user)).ReturnsAsync(resultMock);
            _userManagerMock.Setup(um => um.GenerateEmailConfirmationTokenAsync(user)).ReturnsAsync("test token");
            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(["Doctor"]);
            _configurationMock.Setup(c => c["App:Email"]).Returns("testemail@email.com");
            _configurationMock.Setup(c => c["App:BaseUrl"]).Returns("https://test/api");
            // Act
            var result = await _controller.ResetPassword(email);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.False(user.Status);
            _userManagerMock.Verify(um => um.UpdateAsync(user), Times.Once); // Verify UpdateAsync was called
            _userManagerMock.Verify(um => um.GenerateEmailConfirmationTokenAsync(user), Times.Once); // Verify token generation
        }

        [Fact]
        public async Task UpdatePassword_ReturnsOkResult_IntegrationServiceDomain()
        {
            var email = "test@email.com";
            var id = "testid";
            var token = "test-token";
            var password = "NewPass00_d";
            var user = new User();
            user.UserName = email;
            user.Id = id;
            user.Email = email;
            user.Status = true;
            var resultMock = IdentityResult.Success;

            _userManagerMock.Setup(_userManagerMock => _userManagerMock.FindByEmailAsync(email)).ReturnsAsync(user);//
            _userManagerMock.Setup(um => um.FindByIdAsync(id)).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.ConfirmEmailAsync(user, token)).ReturnsAsync(resultMock);
            _tokenServiceMock.Setup(_tokenServiceMock => _tokenServiceMock.ConfirmEmailToken(id, token)).ReturnsAsync(true);
            _tokenServiceMock.Setup(_tokenServiceMock => _tokenServiceMock.GeneratePasswordResetTokenAsync(user)).ReturnsAsync(token);
            _userManagerMock.Setup(um => um.ResetPasswordAsync(user, token, password)).ReturnsAsync(resultMock);
            _userManagerMock.Setup(um => um.UpdateAsync(user)).ReturnsAsync(resultMock);

            // Act
            var result = await _userService.UpdatePassword(email, token, password);

            // Assert
            Assert.True(result);
            _userManagerMock.Verify(um => um.UpdateAsync(user), Times.Once);
            _userManagerMock.Verify(um => um.ResetPasswordAsync(user, token, password), Times.Once);
        }
    }


   
}
