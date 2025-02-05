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
using MDBackoffice.Domain.RoomTypes;
using Microsoft.OpenApi.Any;


namespace MDBackofficeTests.controllertests
{
    public class RoomTypeControllerTests
    {

        private readonly Mock<RoomTypeService> _service;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IRoomTypeRepository> _repoMock = new Mock<IRoomTypeRepository>();
        private readonly RoomTypeController _controller;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private readonly Mock<UserService> _userServiceMock;
        private readonly Mock<ILoginAdapter> _loginAdapterMock;


        public RoomTypeControllerTests()
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

            _service = new Mock<RoomTypeService>(_unitOfWorkMock.Object, _repoMock.Object);
            _controller = new RoomTypeController(_service.Object, _userServiceMock.Object);
        }

        [Fact]
        public async Task CreateRoomType_Returns_CreatedResult()
        {
            //Arrage
            var dtoMock = new RoomTypeDto("BLCOP-T1", "Bloco Operatório", "Does the surgeries.");
            var roomtype = new Mock<RoomType>("BLCOP-T1", "Bloco Operatório", "Does the surgeries.");
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);
            _service.Setup(u => u.AddAsync(dtoMock)).ReturnsAsync(dtoMock);
            
            //Act
            var result = await _controller.Create(dtoMock);

            //Assert
            var actionResult = Assert.IsType<ActionResult<RoomTypeDto>>(result);
        }

    }
}