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
using MDBackoffice.Domain.Specializations;


namespace MDBackofficeTests.controllertests
{
    public class SpecializationControllerTests
    {

        private readonly Mock<SpecializationService> _service;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<ISpecializationRepository> _repoMock = new Mock<ISpecializationRepository>();
        private readonly SpecializationController _controller;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private readonly Mock<UserService> _userServiceMock;
        private readonly Mock<ILoginAdapter> _loginAdapterMock;


        public SpecializationControllerTests()
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

            _service = new Mock<SpecializationService>(_unitOfWorkMock.Object, _repoMock.Object);
            _controller = new SpecializationController(_service.Object, _userServiceMock.Object);
        }

        [Fact]
        public async Task CreateSpecialization_Returns_CreatedResult()
        {
            //Arrage
            var dtoMock = new SpecializationDto{Code = "10101000", Denomination = "orthopaedics", Description = "The treatment or study of bones that have not grown correctly or that have been damaged."};
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
            var actionResult = Assert.IsType<ActionResult<SpecializationDto>>(result);
        }

        [Fact]
        public async Task GetSpecializationsCode_Returns_CreatedResult()
        {
            //Arrage
            var dtoMock = new SpecializationDto{Code = "10101000", Denomination = "orthopaedics", Description = "The treatment or study of bones that have not grown correctly or that have been damaged."};
            var listMock = new List<SpecializationDto>();
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);
            _service.Setup(u => u.GetSpecializationsByFiltersAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>())).ReturnsAsync(listMock);
            
            //Act
            var result = await _controller.GetSpecializationsByFilters(dtoMock.Code, null, null);

            //Assert
            var actionResult = Assert.IsType<ActionResult<List<SpecializationDto>>>(result);
        }

        [Fact]
        public async Task GetSpecializationsDenomination_Returns_CreatedResult()
        {
            //Arrage
            var dtoMock = new SpecializationDto{Code = "10101000", Denomination = "orthopaedics", Description = "The treatment or study of bones that have not grown correctly or that have been damaged."};
            var listMock = new List<SpecializationDto>();
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);
            _service.Setup(u => u.GetSpecializationsByFiltersAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>())).ReturnsAsync(listMock);
            
            //Act
            var result = await _controller.GetSpecializationsByFilters(null, dtoMock.Denomination, null);

            //Assert
            var actionResult = Assert.IsType<ActionResult<List<SpecializationDto>>>(result);
        }

        [Fact]
        public async Task GetSpecializationsDescription_Returns_CreatedResult()
        {
            //Arrage
            var dtoMock = new SpecializationDto{Code = "10101000", Denomination = "orthopaedics", Description = "The treatment or study of bones that have not grown correctly or that have been damaged."};
            var listMock = new List<SpecializationDto>();
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);
            _service.Setup(u => u.GetSpecializationsByFiltersAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>())).ReturnsAsync(listMock);
            
            //Act
            var result = await _controller.GetSpecializationsByFilters(null, null, dtoMock.Description);

            //Assert
            var actionResult = Assert.IsType<ActionResult<List<SpecializationDto>>>(result);
        }

        [Fact]
        public async Task EditSpecializationDenomination_Returns_CreatedResult()
        {
            //Arrage
            var dtoMock = new EditSpecializationDto("10101000","orthopaedics","");
            var dtoMock1 = new SpecializationDto{Code = "10101000", Denomination = "orthopaedics", Description = "smt"};
            var specMock = new Specialization("10101000", "smt", "smt");
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);
            _service.Setup(u => u.EditSpecialization(dtoMock)).ReturnsAsync(dtoMock1);
            
            //Act
            var result = await _controller.EditSpecialization(dtoMock);

            //Assert
            var actionResult = Assert.IsType<ActionResult<SpecializationDto>>(result);
        }

        [Fact]
        public async Task EditSpecializationDescription_Returns_CreatedResult()
        {
            //Arrage
            var dtoMock = new EditSpecializationDto("10101000","","aooga");
            var dtoMock1 = new SpecializationDto{Code = "10101000", Denomination = "smt", Description = "aooga"};
            var specMock = new Specialization("10101000", "smt", "smt");
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);
            _service.Setup(u => u.EditSpecialization(dtoMock)).ReturnsAsync(dtoMock1);
            
            //Act
            var result = await _controller.EditSpecialization(dtoMock);

            //Assert
            var actionResult = Assert.IsType<ActionResult<SpecializationDto>>(result);
        }

        [Fact]
        public async Task EditSpecialization_Returns_CreatedResult()
        {
            //Arrage
            var dtoMock = new EditSpecializationDto("10101000","orthopaedics","aooga");
            var dtoMock1 = new SpecializationDto{Code = "10101000", Denomination = "orthopaedics", Description = "aooga"};
            var specMock = new Specialization("10101000", "smt", "smt");
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);
            _service.Setup(u => u.EditSpecialization(dtoMock)).ReturnsAsync(dtoMock1);
            
            //Act
            var result = await _controller.EditSpecialization(dtoMock);

            //Assert
            var actionResult = Assert.IsType<ActionResult<SpecializationDto>>(result);
        }

    }
}