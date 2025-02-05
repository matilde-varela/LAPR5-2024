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
    public class CreateStaffIntegrationTest
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

        public CreateStaffIntegrationTest()
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
        public async Task CreateCreateStaffProfile_ReturnsOkResult_IntegrationControllerService()
        {
            // Pass mocked dependencies to StaffService
            var _service = new StaffService(_unitOfWorkMock.Object, _logServiceMock.Object,
                                            _repoMock.Object, _repoSpecMock.Object,
                                            _userManagerMock.Object, _configurationMock.Object, _emailServiceMock.Object,
                                            _userServiceMock.Object);

            var controller = new StaffController(_service,_userServiceMock.Object);
            var specializationId = "25841809";

            var dtoMock = new CreatingStaffDto
            ("12345",
                "Portugal, 4590-850, Rua da Sardinha",
                "Rita",
                "Barbosa",
                "+351 910000022",
                "something@email.com",
                "doctor",
                specializationId
                );

            var specializationMock = new Mock<Specialization>("25841809","denom","descrip");
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);


            _repoSpecMock.Setup(_repoSpecMock => _repoSpecMock.FindByDenomination(It.IsAny<string>()))
                .ReturnsAsync(specializationMock.Object);

            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await controller.CreateStaffProfile(dtoMock);

            // Assert
            var actionResult = Assert.IsType<ActionResult<StaffDto>>(result); // Ensure it's the correct ActionResult type
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.NotNull(okObjectResult.Value);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }


        [Fact]
        public async Task CreateStaffProfile_ReturnsOkResult_IntegrationServiceDomain()
        {
            // Pass mocked dependencies to StaffService
            var specializationId = "25841809";

            var dtoMock = new CreatingStaffDto
            ("12345",
                "Portugal, 4590-850, Rua da Sardinha",
                "Rita",
                "Barbosa",
                "+351 910000022",
                "something@email.com",
                "doctor",
                specializationId
                );

            var specializationMock = new Mock<Specialization>("25841809", "denom", "descrip");

            _repoSpecMock.Setup(_repoSpecMock => _repoSpecMock.FindByDenomination(It.IsAny<string>()))
                .ReturnsAsync(specializationMock.Object);

            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            var _service = new StaffService(_unitOfWorkMock.Object, _logServiceMock.Object,
                                            _repoMock.Object, _repoSpecMock.Object,
                                            _userManagerMock.Object, _configurationMock.Object, _emailServiceMock.Object,
                                            _userServiceMock.Object);

            // Act
            var result = await _service.CreateStaffProfile(dtoMock);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(specializationId, result.SpecializationId);
            Assert.Equal(dtoMock.Phone, result.Phone);
            Assert.Equal(dtoMock.Email, result.Email);
            Assert.Equal(dtoMock.Address, result.Address);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Staff>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);


        }
    }
}