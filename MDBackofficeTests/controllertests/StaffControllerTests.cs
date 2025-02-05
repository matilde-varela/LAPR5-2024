using MDBackoffice.Domain.Logs;
using MDBackoffice.Domain.Emails;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.Specializations;
using MDBackoffice.Domain.Users;
using MDBackoffice.Domain.Tokens;
using MDBackoffice.Infrastructure.Emails;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using MDBackoffice.Domain.StaffProfiles;
using MDBackoffice.Controllers;
using Microsoft.AspNetCore.Mvc;
using MDBackoffice.Domain.Patients;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using MDBackoffice.Infrastructure.Users;

namespace MDBackofficeTests.controllertests
{
    public class StaffControllerTests
    {
        private readonly Mock<StaffService> _service;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IStaffRepository> _repoMock = new Mock<IStaffRepository>();
        private readonly Mock<ISpecializationRepository> _repoSpecMock = new Mock<ISpecializationRepository>();
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<ILoginAdapter> _loginAdapterMock;
        private readonly Mock<UserService> _userServiceMock;
        private readonly StaffController _controller;

        public StaffControllerTests()
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

            _service = new Mock<StaffService>(_unitOfWorkMock.Object, _logServiceMock.Object,
                                            _repoMock.Object, _repoSpecMock.Object,
                                            _userManagerMock.Object, _configurationMock.Object, _emailServiceMock.Object,
                                            _userServiceMock.Object);
            _controller = new StaffController(_service.Object,_userServiceMock.Object);
        }

        [Fact]
        public async Task CreateStaffProfile_ReturnsOkResult()
        {
            // Arrange
            

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

            var specializationMock = new Mock<Specialization>("25841809","denom","descript");

            _repoSpecMock.Setup(_repoSpecMock => _repoSpecMock.FindByDenomination(It.IsAny<string>()))
                .ReturnsAsync(specializationMock.Object);
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);


            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _controller.CreateStaffProfile(dtoMock);

            // Assert
            var actionResult = Assert.IsType<ActionResult<StaffDto>>(result); // Ensure it's the correct ActionResult type
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.NotNull(okObjectResult.Value);
            
        }
        [Fact]
        public async Task EditStaffProfile_ReturnsAcceptedStaffDto()
        {
            //Arrange
            var dtoMock = new EditStaffDto
            ("+351 910000011",
              "test@email.com",
              "New, 1234-234, Updated",
              "25841209");

            var email = "ritabarbosa@email.com";
            var userid = "testid";
            var token = "test-token";
            var userMock = new Mock<User>();
            userMock.Setup(u => u.Id).Returns(userid);
            userMock.Setup(u => u.UserName).Returns(email);
            userMock.Setup(u => u.Email).Returns(email);
            userMock.Setup(u => u.Status).Returns(true);

            var staffMock = new Mock<Staff>("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", email, "+351", "987654321", "Doctor", "25841809");
            var id = "D202400001";

            var dtoResult = new StaffDto("Rita Barbosa", "+351 910000011", "test@email.com", "New, 1234-234, Updated", id, "25841209");

            _repoMock.Setup(_repoPatMock => _repoPatMock.GetByIdAsync(It.IsAny<StaffId>()))
                .ReturnsAsync(staffMock.Object);
            _userManagerMock.Setup(_userManagerMock => _userManagerMock.FindByEmailAsync(email)).ReturnsAsync(userMock.Object);
            _userManagerMock.Setup(um => um.GetRolesAsync(userMock.Object)).ReturnsAsync(["Doctor"]);
            _userManagerMock.Setup(um => um.GenerateChangeEmailTokenAsync(userMock.Object, "test@email.com")).ReturnsAsync(token);
            _userManagerMock.Setup(um => um.GenerateEmailConfirmationTokenAsync(userMock.Object)).ReturnsAsync(token);
            _userManagerMock.Setup(um => um.ChangeEmailAsync(userMock.Object, "test@email.com", token)).ReturnsAsync(IdentityResult.Success);
            _configurationMock.Setup(c => c["App:Email"]).Returns("testemail@email.com");
            _configurationMock.Setup(c => c["App:BaseUrl"]).Returns("https://test/api");
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            //Act
            var result = await _controller.EditStaffProfile(id, dtoMock);

            //Assert
            var acceptedResult = Assert.IsType<AcceptedResult>(result.Result);
            var returnedStaff = Assert.IsType<StaffDto>(acceptedResult.Value);
            Assert.Equal(dtoResult.Name, returnedStaff.Name);
            Assert.Equal(dtoResult.Phone, returnedStaff.Phone);
            Assert.Equal(dtoResult.Email, returnedStaff.Email);
            Assert.Equal(dtoResult.Address, returnedStaff.Address);
            Assert.Equal(dtoResult.SpecializationId, returnedStaff.SpecializationId);
        }




        [Fact]
        public async Task GetStaffProfiles_ReturnsStaffDtoList()
        {
            //Arrange
            List<StaffDto> result = new List<StaffDto>();
            _service.Setup(p => p.GetAllAsync()).ReturnsAsync(result);

            //Act
            var resultList = _controller.GetStaffProfiles();

            //Assert
            var okResult = Assert.IsType<ActionResult<IEnumerable<StaffDto>>>(resultList.Result);
            var returnedStaff = Assert.IsType<List<StaffDto>>(okResult.Value);
        }

        [Fact]
        public async Task GetFilteredStaffProfiles_ReturnsStaffDtoList()
        {
            // Arrange
            string firstName = "Duarte";
            string lastName = "Matos";
            string email = "exampleemail@gmail.com";
            string specialization = "Cardiology";

            StaffListingFilterParametersDto listingFilterParametersDto
                = new StaffListingFilterParametersDto(
                    firstName,
                    lastName,
                    email,
                    specialization
                    );

            List<StaffListingFilterParametersDto> listingFilterParametersDtosList = new List<StaffListingFilterParametersDto>
            {
                listingFilterParametersDto
            };

            StaffQueryParametersDto dto = new StaffQueryParametersDto(listingFilterParametersDtosList);
            var id = "202410000001";

            List<StaffDto> result = new List<StaffDto>();
            var dtoResult = new StaffDto("Rita Barbosa", "+351 910000000", "ritabarbosa@email.com", "Test, 1234-234, Test Test", "2004-12-15", id);

            result.Add(dtoResult);
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);
            _service.Setup(p => p.FilterStaffProfiles(dto)).ReturnsAsync(result);

            //Act
            var resultController = await _controller.GetFilteredStaffProfiles(dto);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(resultController.Result);
            var returnedPatient = Assert.IsType<List<StaffDto>>(okResult.Value);
        }


        [Fact]
        public async Task DeactivateStaffProfile_ReturnsOkResult()
        {
            // Arrange
            string email = "exampleemail@gmail.com";

            var staffMock = new Mock<Staff>("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", email, "+351", "987654321", "Doctor", "25841809");
            var id = "D202400001";
            

            var specializationMock = new Mock<Specialization>("25841809");
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);
            _repoMock.Setup(_repoPatMock => _repoPatMock.GetByIdAsync(It.IsAny<StaffId>()))
                .ReturnsAsync(staffMock.Object);
            staffMock.Setup(r => r.DeactivateProfile());
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _controller.DeactivateStaffProfile(new IdPassDto(id));

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }
        
        [Fact]
        public async Task ConfirmEmailPatient_ReturnsOkResult()
        {
            // Arrange
            var userId = "valid_user_id";
            var token = "valid_token";
            var staffId = "D202400001";
            var email = "ritabarbosa@email.com";

            var staffMock = new Mock<Staff>("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", email, "+351", "987654321", "Doctor", "25841809");

            _service.Setup(service => service.ConfirmEmailStaff(userId,staffId, token))
                            .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ConfirmEmailStaff(userId, staffId, token);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

    }
}