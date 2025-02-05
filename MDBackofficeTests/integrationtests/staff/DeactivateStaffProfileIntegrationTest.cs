using MDBackoffice.Domain.Logs;
using MDBackoffice.Domain.Emails;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.Specializations;
using MDBackoffice.Domain.Users;
using MDBackoffice.Domain.Tokens;
using MDBackoffice.Infrastructure.Emails;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using MDBackoffice.Domain.StaffProfiles;
using MDBackoffice.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using MDBackoffice.Infrastructure.Users;

namespace MDBackofficeTests.integrationtests.staff
{
    public class DeactivateStaffProfileIntegrationTest
    {

        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IStaffRepository> _repoMock = new Mock<IStaffRepository>();
        private readonly Mock<ISpecializationRepository> _repoSpecMock = new Mock<ISpecializationRepository>();
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private readonly Mock<LogService> _logServiceMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<UserService> _userServiceMock;
        private readonly Mock<ILoginAdapter> _loginAdapterMock;
        private readonly Mock<EmailService> _emailServiceMock;

        public DeactivateStaffProfileIntegrationTest()
        {
            _logServiceMock = new Mock<LogService>(new Mock<IUnitOfWork>().Object, new Mock<ILogRepository>().Object);

            var identityOptionsMock = new Mock<IOptions<IdentityOptions>>();
            identityOptionsMock.Setup(o => o.Value).Returns(new IdentityOptions());
            var identityErrorDescriberMock = new Mock<IdentityErrorDescriber>();
            
            _userManagerMock = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, identityOptionsMock.Object, new Mock<IPasswordHasher<User>>().Object, new List<IUserValidator<User>> { new Mock<IUserValidator<User>>().Object }, new List<IPasswordValidator<User>> { new Mock<IPasswordValidator<User>>().Object }, new Mock<ILookupNormalizer>().Object, identityErrorDescriberMock.Object, new Mock<IServiceProvider>().Object, new Mock<ILogger<UserManager<User>>>().Object);
            var roleManagerMock = new Mock<RoleManager<Role>>(new Mock<IRoleStore<Role>>().Object, new List<IRoleValidator<Role>>(), new Mock<ILookupNormalizer>().Object, identityErrorDescriberMock.Object, new Mock<ILogger<RoleManager<Role>>>().Object);

            var tokenServiceMock = new Mock<TokenService>(_unitOfWorkMock.Object, new Mock<ITokenRepository>().Object, _userManagerMock.Object);
            var _emailServMock = new Mock<EmailService>(tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);
            var _configurationMock = new Mock<IConfiguration>();
            _loginAdapterMock = new Mock<ILoginAdapter>();
            var signinManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object,
                                                               new Mock<IHttpContextAccessor>().Object,
                                                               new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                                                               identityOptionsMock.Object,
                                                               new Mock<ILogger<SignInManager<User>>>().Object,
                                                               new Mock<IAuthenticationSchemeProvider>().Object,
                                                               new Mock<IUserConfirmation<User>>().Object);

            _userServiceMock = new Mock<UserService>(_userManagerMock.Object, roleManagerMock.Object, _logServiceMock.Object, signinManagerMock.Object, _emailServMock.Object, _configurationMock.Object, tokenServiceMock.Object, _loginAdapterMock.Object);
            _emailServiceMock = new Mock<EmailService>(tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);
        }

        [Fact]
        public async Task DeactivateStaffProfile_ReturnsOkResult_IntegrationControllerService()
        {
            var _service = new StaffService(_unitOfWorkMock.Object, _logServiceMock.Object,
                                            _repoMock.Object, _repoSpecMock.Object,
                                            _userManagerMock.Object, _configurationMock.Object, _emailServiceMock.Object,
                                            _userServiceMock.Object);

            var controller = new StaffController(_service,_userServiceMock.Object);
             // Arrange
            string email = "exampleemail@gmail.com";

            var staffMock = new Mock<Staff>("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", email, "+351", "987654321", "Doctor", "25841209");
            var id = "D202400001";

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);


            _repoMock.Setup(_repoPatMock => _repoPatMock.GetByIdAsync(It.IsAny<StaffId>()))
                .ReturnsAsync(staffMock.Object);
            staffMock.Setup(r => r.DeactivateProfile());
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await controller.DeactivateStaffProfile(new IdPassDto(id));

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public async Task DeactivateStaffProfile_ReturnsOkResult_IntegrationServiceDomain()
        {
             // Arrange
            var _service = new StaffService(_unitOfWorkMock.Object, _logServiceMock.Object,
                                            _repoMock.Object, _repoSpecMock.Object,
                                            _userManagerMock.Object, _configurationMock.Object, _emailServiceMock.Object,
                                            _userServiceMock.Object);
            string email = "exampleemail@gmail.com";

            var staff = new Staff("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", email, "+351", "987654321", "Doctor", "25841209");
            var id = "D202400001";
            

            var specializationMock = new Mock<Specialization>("251010300");

            _repoMock.Setup(_repoPatMock => _repoPatMock.GetByIdAsync(It.IsAny<StaffId>()))
                .ReturnsAsync(staff);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.DeactivateStaffProfile(id);

            // Assert
            Assert.True(result);
        }
    }
}