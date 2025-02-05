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
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;


namespace MDBackofficeTests.integrationtests.usertests
{
    public class LoginIntegrationTests
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
        private readonly Mock<SignInManager<User>> _signinManagerMock;
        private readonly Mock<EmailService> _emailServiceMock;
        private readonly Mock<ILoginAdapter> _loginAdapterMock;
        private readonly Mock<IPatientMedicalRecordAdapter> _patientMRAMock;

        public LoginIntegrationTests()
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
        public async Task LoginCorrectCredentials_ReturnsTokenSuccessfully_IntegrationControllerService()
        {
            //Arrange
            var _controller = new UserController(_userService, _patientServiceMock.Object, _staffServiceMock.Object, _tokenServiceMock.Object);

            var email = "test@email.com";
            var id = "testid";
            var password = "NewPass00_d";

            var dtoMock = new LoginUserDto { Email = email, Password = password };

            var userMock = new Mock<User>();


            userMock.Setup(u => u.Id).Returns(id);
            userMock.Setup(u => u.UserName).Returns(email);
            userMock.Setup(u => u.Email).Returns(email);
            userMock.Setup(u => u.Status).Returns(true);
            userMock.Setup(u => u.PasswordHash).Returns(password);

            
            _userManagerMock.Setup(_userManagerMock => _userManagerMock.FindByEmailAsync(email)).ReturnsAsync(userMock.Object);
            _signinManagerMock.Setup(sm => sm.PasswordSignInAsync(userMock.Object, password, false, true)).ReturnsAsync(SignInResult.Success);
            _userManagerMock.Setup(um => um.GetRolesAsync(userMock.Object)).ReturnsAsync(["Doctor"]);
            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("AHSVFOSDYUDASJFNhvOUVu897GB876arbvn568n6CN865Vn5NFFn86f87Cb76cNvnVNYvhgvnu7676");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("testissuer");
            _configurationMock.Setup(c => c["Jwt:Audiend"]).Returns("testaudience");

            //Act
            var result = await _controller.Login(dtoMock);

            //Assert
            Assert.IsType<OkObjectResult>(result);
            _signinManagerMock.Verify(um => um.PasswordSignInAsync(userMock.Object, password, false, true), Times.Once);
        }


        [Fact]
        public async Task LoginInvalidCredentials_ReturnsTokenSuccessfully_IntegrationControllerService()
        {
            //Arrange
            var _controller = new UserController(_userService, _patientServiceMock.Object, _staffServiceMock.Object, _tokenServiceMock.Object);

            var email = "test@email.com";
            var id = "testid";
            var password = "NewPass00_d";

            var dtoMock = new LoginUserDto { Email = email, Password = "wrong-password" };

            var userMock = new Mock<User>();
            userMock.Setup(u => u.Id).Returns(id);
            userMock.Setup(u => u.UserName).Returns(email);
            userMock.Setup(u => u.Email).Returns(email);
            userMock.Setup(u => u.Status).Returns(true);
            userMock.Setup(u => u.PasswordHash).Returns(password);

            
            _userManagerMock.Setup(_userManagerMock => _userManagerMock.FindByEmailAsync(email)).ReturnsAsync(userMock.Object);
            _signinManagerMock.Setup(sm => sm.PasswordSignInAsync(userMock.Object, dtoMock.Password, false, true)).ReturnsAsync(SignInResult.Failed);
            _userManagerMock.Setup(um => um.GetRolesAsync(userMock.Object)).ReturnsAsync(["Doctor"]);
            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("AHSVFOSDYUDASJFNhvOUVu897GB876arbvn568n6CN865Vn5NFFn86f87Cb76cNvnVNYvhgvnu7676");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("testissuer");
            _configurationMock.Setup(c => c["Jwt:Audiend"]).Returns("testaudience");


            //Act
            var result = await _controller.Login(dtoMock);

            //Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }


        [Fact]
        public async Task LoginCorrectCredentials_ReturnsTokenSuccessfully_IntegrationServiceDomain()
        {
            //Arrange
            var _controller = new UserController(_userService, _patientServiceMock.Object, _staffServiceMock.Object, _tokenServiceMock.Object);

            var email = "test@email.com";
            var id = "testid";
            var password = "NewPass00_d";

            var dtoMock = new LoginUserDto { Email = email, Password = password };

            var user = new User();
            user.UserName = email;
            user.Email = email;
            user.Status = true;
            user.PasswordHash = password;

            _userManagerMock.Setup(_userManagerMock => _userManagerMock.FindByEmailAsync(email)).ReturnsAsync(user);
            _signinManagerMock.Setup(sm => sm.PasswordSignInAsync(user, password, false, true)).ReturnsAsync(SignInResult.Success);
            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(["Doctor"]);
            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("AHSVFOSDYUDASJFNhvOUVu897GB876arbvn568n6CN865Vn5NFFn86f87Cb76cNvnVNYvhgvnu7676");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("testissuer");
            _configurationMock.Setup(c => c["Jwt:Audiend"]).Returns("testaudience");

            //Act
            var result = await _userService.Login(dtoMock);

            //Assert
            Assert.NotNull(result);
            _signinManagerMock.Verify(um => um.PasswordSignInAsync(user, password, false, true), Times.Once);
        }


        [Fact]
        public async Task LoginInvalidCredentials_ReturnsTokenSuccessfully_IntegrationServiceDomain()
        {
            //Arrange
            var _controller = new UserController(_userService, _patientServiceMock.Object, _staffServiceMock.Object, _tokenServiceMock.Object);

            var email = "test@email.com";
            var id = "testid";
            var password = "NewPass00_d";

            var dtoMock = new LoginUserDto { Email = email, Password = "wrong-password" };

            var user = new User();
            user.UserName = email;
            user.Email = email;
            user.Status = true;
            user.PasswordHash = password;

            
            _userManagerMock.Setup(_userManagerMock => _userManagerMock.FindByEmailAsync(email)).ReturnsAsync(user);
            _signinManagerMock.Setup(sm => sm.PasswordSignInAsync(user, dtoMock.Password, false, true)).ReturnsAsync(SignInResult.Failed);
            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(["Doctor"]);
            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("AHSVFOSDYUDASJFNhvOUVu897GB876arbvn568n6CN865Vn5NFFn86f87Cb76cNvnVNYvhgvnu7676");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("testissuer");
            _configurationMock.Setup(c => c["Jwt:Audiend"]).Returns("testaudience");

             await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _userService.Login(dtoMock));
        }

        [Fact]
        public async Task Login_FiveFailedAttempts_TriggersLockout_IntegrationControllerService()
        {
            // Arrange
            var email = "test@email.com";
            var dtoMock = new LoginUserDto { Email = email, Password = "wrong-password" };
            
            var userMock = new Mock<User>();
            userMock.Setup(u => u.UserName).Returns(email);
            userMock.Setup(u => u.Email).Returns(email);
            userMock.Setup(u => u.Status).Returns(true);

            _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(userMock.Object);
            _userManagerMock.Setup(um => um.IsLockedOutAsync(userMock.Object)).ReturnsAsync(false);
            _signinManagerMock.Setup(sm => sm.PasswordSignInAsync(userMock.Object, dtoMock.Password, false, true)).ReturnsAsync(SignInResult.Failed);

            // Act
            for (int i = 0; i < 5; i++)
            {
                await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _userService.Login(dtoMock));
            }

            _userManagerMock.Setup(um => um.IsLockedOutAsync(userMock.Object)).ReturnsAsync(true);
            _signinManagerMock.Setup(sm => sm.PasswordSignInAsync(userMock.Object, dtoMock.Password, false, true)).ReturnsAsync(SignInResult.LockedOut);

            //Assert
            var exception = await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _userService.Login(dtoMock));
            Assert.Equal("Account is locked out. Please try again later.", exception.Message);
        }

        [Fact]
        public async Task Login_FiveFailedAttempts_TriggersLockout_IntegrationServiceDomain()
        {
            // Arrange
            var email = "test@email.com";
            var password = "NewPass00_d";
            var dtoMock = new LoginUserDto { Email = email, Password = "wrong-password" };
            
            var user = new User();
            user.UserName = email;
            user.Email = email;
            user.Status = true;
            user.PasswordHash = password;

            _userManagerMock.Setup(_userManagerMock => _userManagerMock.FindByEmailAsync(email)).ReturnsAsync(user);
            _signinManagerMock.Setup(sm => sm.PasswordSignInAsync(user, dtoMock.Password, false, true)).ReturnsAsync(SignInResult.Failed);
          
            // Act
            for (int i = 0; i < 5; i++)
            {
                await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _userService.Login(dtoMock));
            }

            _userManagerMock.Setup(um => um.IsLockedOutAsync(user)).ReturnsAsync(true);
            _signinManagerMock.Setup(sm => sm.PasswordSignInAsync(user, dtoMock.Password, false, true)).ReturnsAsync(SignInResult.LockedOut);

            //Assert
            var exception = await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _userService.Login(dtoMock));
            Assert.Equal("Account is locked out. Please try again later.", exception.Message);
        }

    }
}



