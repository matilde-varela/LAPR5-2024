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
    public class ListSpecializationIntegrationTest
    {

        private readonly SpecializationService _service;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<ISpecializationRepository> _repoMock = new Mock<ISpecializationRepository>();
        private readonly SpecializationController _controller;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private readonly Mock<UserService> _userServiceMock;
        private readonly Mock<ILoginAdapter> _loginAdapterMock;


        public ListSpecializationIntegrationTest()
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

            _service = new SpecializationService(_unitOfWorkMock.Object, _repoMock.Object);
            _controller = new SpecializationController(_service, _userServiceMock.Object);
        }

        [Fact]
        public async Task GetSpecializationsCode_Returns_CreatedResult_ControllerService()
        {
            //Arrage
            var dtoMock1 = new SpecializationDto{Code = "10101000", Denomination = "orthopaedics", Description = "The treatment or study of bones that have not grown correctly or that have been damaged."};
            var dtoMock2 = new SpecializationDto{Code = "10101010", Denomination = "cardiology", Description = "The treatment or study of the heart."};
            var listMock = new List<SpecializationDto>
            {
                dtoMock1,
                dtoMock2
            };
            var specMock1 = new Mock<Specialization>("10101000", "orthopaedics", "The treatment or study of bones that have not grown correctly or that have been damaged.");
            var specMock2 = new Mock<Specialization>("10101010", "cardiology", "The treatment or study of the heart.");
            var listspecMock = new List<Specialization>
            {
                specMock1.Object,
                specMock2.Object
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);
            _repoMock.Setup(u => u.FindAllConditioned(null, dtoMock1.Denomination, null)).ReturnsAsync(listspecMock);
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };

            //Act
            var result = await _controller.GetSpecializationsByFilters(dtoMock1.Code, null, null);

            //Assert
            var actionResult = Assert.IsType<ActionResult<List<SpecializationDto>>>(result);
        }

        [Fact]
        public async Task GetSpecializationsDenomination_Returns_CreatedResult_ControllerService()
        {
            //Arrage
            var dtoMock1 = new SpecializationDto{Code = "10101000", Denomination = "orthopaedics", Description = "The treatment or study of bones that have not grown correctly or that have been damaged."};
            var dtoMock2 = new SpecializationDto{Code = "10101010", Denomination = "cardiology", Description = "The treatment or study of the heart."};
            var listMock = new List<SpecializationDto>
            {
                dtoMock1,
                dtoMock2
            };
            var specMock1 = new Mock<Specialization>("10101000", "orthopaedics", "The treatment or study of bones that have not grown correctly or that have been damaged.");
            var specMock2 = new Mock<Specialization>("10101010", "cardiology", "The treatment or study of the heart.");
            var listspecMock = new List<Specialization>
            {
                specMock1.Object,
                specMock2.Object
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);
            _repoMock.Setup(u => u.FindAllConditioned(null, dtoMock1.Denomination, null)).ReturnsAsync(listspecMock);
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
              
            //Act
            var result = await _controller.GetSpecializationsByFilters(null, dtoMock1.Denomination, null);

            //Assert
            var actionResult = Assert.IsType<ActionResult<List<SpecializationDto>>>(result);
        }

        [Fact]
        public async Task GetSpecializationsDescription_Returns_CreatedResult_ControllerService()
        {
            //Arrage
            var dtoMock1 = new SpecializationDto{Code = "10101000", Denomination = "orthopaedics", Description = "The treatment or study of bones that have not grown correctly or that have been damaged."};
            var dtoMock2 = new SpecializationDto{Code = "10101010", Denomination = "cardiology", Description = "The treatment or study of the heart."};
            var listMock = new List<SpecializationDto>
            {
                dtoMock1,
                dtoMock2
            };
            var specMock1 = new Mock<Specialization>("10101000", "orthopaedics", "The treatment or study of bones that have not grown correctly or that have been damaged.");
            var specMock2 = new Mock<Specialization>("10101010", "cardiology", "The treatment or study of the heart.");
            var listspecMock = new List<Specialization>
            {
                specMock1.Object,
                specMock2.Object
            };
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);
            _repoMock.Setup(u => u.FindAllConditioned(null, dtoMock1.Denomination, null)).ReturnsAsync(listspecMock);
             
            //Act
            var result = await _controller.GetSpecializationsByFilters(null, null, dtoMock1.Description);

            //Assert
            var actionResult = Assert.IsType<ActionResult<List<SpecializationDto>>>(result);
        }

        [Fact]
        public async Task GetSpecializationsCode_Returns_CreatedResult_ServiceDomain()
        {
            //Arrage
            var dtoMock1 = new SpecializationDto{Code = "10101000", Denomination = "orthopaedics", Description = "The treatment or study of bones that have not grown correctly or that have been damaged."};
            var dtoMock2 = new SpecializationDto{Code = "10101010", Denomination = "cardiology", Description = "The treatment or study of the heart."};
            var listMock = new List<SpecializationDto>
            {
                dtoMock1,
                dtoMock2
            };
            var specMock1 = new Mock<Specialization>("10101000", "orthopaedics", "The treatment or study of bones that have not grown correctly or that have been damaged.");
            var specMock2 = new Mock<Specialization>("10101010", "cardiology", "The treatment or study of the heart.");
            var listspecMock = new List<Specialization>
            {
                specMock1.Object,
                specMock2.Object
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);
            _repoMock.Setup(u => u.FindAllConditioned(dtoMock1.Code, null, null)).ReturnsAsync(listspecMock);
           
            //Act
            var result = await _service.GetSpecializationsByFiltersAsync(dtoMock1.Code, null, null);

            //Assert
            var actionResult = Assert.IsType<List<SpecializationDto>>(result);
            Assert.Equal(result.ElementAt(0).Code, dtoMock1.Code);
            Assert.Equal(result.ElementAt(0).Denomination, dtoMock1.Denomination);
            Assert.Equal(result.ElementAt(0).Description, dtoMock1.Description);
            Assert.Equal(result.ElementAt(1).Code, dtoMock2.Code);
            Assert.Equal(result.ElementAt(1).Denomination, dtoMock2.Denomination);
            Assert.Equal(result.ElementAt(1).Description, dtoMock2.Description);
        }

        [Fact]
        public async Task GetSpecializationsDenomination_Returns_CreatedResult_ServiceDomain()
        {
            //Arrage
            var dtoMock1 = new SpecializationDto{Code = "10101000", Denomination = "orthopaedics", Description = "The treatment or study of bones that have not grown correctly or that have been damaged."};
            var dtoMock2 = new SpecializationDto{Code = "10101010", Denomination = "cardiology", Description = "The treatment or study of the heart."};
            var listMock = new List<SpecializationDto>
            {
                dtoMock1,
                dtoMock2
            };
            var specMock1 = new Mock<Specialization>("10101000", "orthopaedics", "The treatment or study of bones that have not grown correctly or that have been damaged.");
            var specMock2 = new Mock<Specialization>("10101010", "cardiology", "The treatment or study of the heart.");
            var listspecMock = new List<Specialization>
            {
                specMock1.Object,
                specMock2.Object
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);
            _repoMock.Setup(u => u.FindAllConditioned(null, dtoMock1.Denomination, null)).ReturnsAsync(listspecMock);
           
            //Act
            var result = await _service.GetSpecializationsByFiltersAsync(null, dtoMock1.Denomination, null);

            //Assert
            Assert.Equal(result.ElementAt(0).Code, dtoMock1.Code);
            Assert.Equal(result.ElementAt(0).Denomination, dtoMock1.Denomination);
            Assert.Equal(result.ElementAt(0).Description, dtoMock1.Description);
            Assert.Equal(result.ElementAt(1).Code, dtoMock2.Code);
            Assert.Equal(result.ElementAt(1).Denomination, dtoMock2.Denomination);
            Assert.Equal(result.ElementAt(1).Description, dtoMock2.Description);
        }

        [Fact]
        public async Task GetSpecializationsDescription_Returns_CreatedResult_ServiceDomain()
        {
            //Arrage
            var dtoMock1 = new SpecializationDto{Code = "10101000", Denomination = "orthopaedics", Description = "The treatment or study of bones that have not grown correctly or that have been damaged."};
            var dtoMock2 = new SpecializationDto{Code = "10101010", Denomination = "cardiology", Description = "The treatment or study of the heart."};
            var listMock = new List<SpecializationDto>
            {
                dtoMock1,
                dtoMock2
            };
            var specMock1 = new Mock<Specialization>("10101000", "orthopaedics", "The treatment or study of bones that have not grown correctly or that have been damaged.");
            var specMock2 = new Mock<Specialization>("10101010", "cardiology", "The treatment or study of the heart.");
            var listspecMock = new List<Specialization>
            {
                specMock1.Object,
                specMock2.Object
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);
            _repoMock.Setup(u => u.FindAllConditioned(null, null, dtoMock1.Description)).ReturnsAsync(listspecMock);
           
            //Act
            var result = await _service.GetSpecializationsByFiltersAsync(null, null, dtoMock1.Description);

            //Assert
            Assert.Equal(result.ElementAt(0).Code, dtoMock1.Code);
            Assert.Equal(result.ElementAt(0).Denomination, dtoMock1.Denomination);
            Assert.Equal(result.ElementAt(0).Description, dtoMock1.Description);
            Assert.Equal(result.ElementAt(1).Code, dtoMock2.Code);
            Assert.Equal(result.ElementAt(1).Denomination, dtoMock2.Denomination);
            Assert.Equal(result.ElementAt(1).Description, dtoMock2.Description);
        }

    }
}