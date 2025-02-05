using MDBackoffice.Controllers;
using MDBackoffice.Domain.Appointments;
using MDBackoffice.Domain.AppointmentStaffs;
using MDBackoffice.Domain.Emails;
using MDBackoffice.Domain.Logs;
using MDBackoffice.Domain.OperationRequests;
using MDBackoffice.Domain.OperationTypes;
using MDBackoffice.Domain.OperationTypes.ValueObjects.Phase;
using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;
using MDBackoffice.Domain.Rooms;
using MDBackoffice.Domain.RoomTypes;
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

namespace MDBackofficeTests.controllertests
{
    public class AppointmentControllerTests
    {
         private readonly Mock<AppointmentService> _serviceMock;
        private readonly Mock<UserService> _userServiceMock;
        private readonly Mock<IAppointmentRepository> _repoAppointMock = new Mock<IAppointmentRepository>();

        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IRoomRepository> _repoRoomMock = new Mock<IRoomRepository>();
        private readonly Mock<IOperationTypeRepository> _repoOpTypeMock = new Mock<IOperationTypeRepository>();
        private readonly Mock<IStaffRepository> _repoStaMock = new Mock<IStaffRepository>();
        private readonly Mock<IRequiredStaffRepository> _repoReqStaMock = new Mock<IRequiredStaffRepository>();
        private readonly Mock<IOperationRequestRepository> _repoOpReqMock = new Mock<IOperationRequestRepository>();
        private readonly Mock<IAppointmentStaffRepository> _appointmentStaffRepoMock = new Mock<IAppointmentStaffRepository>();
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<ILoginAdapter> _loginAdapterMock;

        public AppointmentControllerTests()
        {

            Mock<LogService> _logServiceMock = new Mock<LogService>(new Mock<IUnitOfWork>().Object, new Mock<ILogRepository>().Object);

            var identityOptionsMock = new Mock<IOptions<IdentityOptions>>();
            identityOptionsMock.Setup(o => o.Value).Returns(new IdentityOptions());
            var identityErrorDescriberMock = new Mock<IdentityErrorDescriber>();

            _userManagerMock = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, identityOptionsMock.Object,
                new Mock<IPasswordHasher<User>>().Object, new List<IUserValidator<User>> { new Mock<IUserValidator<User>>().Object },
                new List<IPasswordValidator<User>> { new Mock<IPasswordValidator<User>>().Object },
                new Mock<ILookupNormalizer>().Object, identityErrorDescriberMock.Object, new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<User>>>().Object);

            var signinManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object, new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<User>>().Object, identityOptionsMock.Object,
                new Mock<ILogger<SignInManager<User>>>().Object, new Mock<IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<User>>().Object);

            var roleManagerMock = new Mock<RoleManager<Role>>(new Mock<IRoleStore<Role>>().Object, new List<IRoleValidator<Role>>(),
                new Mock<ILookupNormalizer>().Object, identityErrorDescriberMock.Object, new Mock<ILogger<RoleManager<Role>>>().Object);

            var tokenServiceMock = new Mock<TokenService>(_unitOfWorkMock.Object, new Mock<ITokenRepository>().Object, _userManagerMock.Object);
            var _emailServiceMock = new Mock<EmailService>(tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);

            var _configurationMock = new Mock<IConfiguration>();
            _loginAdapterMock = new Mock<ILoginAdapter>();

            _userServiceMock = new Mock<UserService>(_userManagerMock.Object, roleManagerMock.Object, _logServiceMock.Object,
                signinManagerMock.Object, _emailServiceMock.Object, _configurationMock.Object, tokenServiceMock.Object, _loginAdapterMock.Object);
        
            _serviceMock = new Mock<AppointmentService>(_unitOfWorkMock.Object, _repoAppointMock.Object, _repoOpReqMock.Object,
                _repoRoomMock.Object, _repoOpTypeMock.Object, _repoStaMock.Object, _repoReqStaMock.Object, _appointmentStaffRepoMock.Object);
        }


        [Fact]
        public async Task Create_ReturnsOkResult_WithValidData()
        {
            // Arrange
            var _controller = new AppointmentController(_serviceMock.Object, _userServiceMock.Object);

            var createDto = new CreatingAppointmentDto(
                "test id",
                "R102",
                "14:00",
                "16:00",
                "2024-07-08",
                "2024-07-08",
                new List<string> { "D202500001" }
            );

            var appointmentId = Guid.NewGuid();
            var dto = new AppointmentDto(
                appointmentId,
                "Scheduled",
                "test id",
                "R102",
                "14:00:00",
                "16:00:00",
                "2024-07-08",
                "2024-07-08",
                new List<string> { "D202500001" }
            );

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };

            _userServiceMock
                .Setup(_userService => _userService.CheckUserRole("valid-token", "Doctor"))
                .Returns(false);

            _serviceMock.Setup(_service => _service.AddAsync(createDto)).ReturnsAsync(dto);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<AppointmentDto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedDto = Assert.IsType<AppointmentDto>(okResult.Value);

            Assert.Equal(dto.Id, returnedDto.Id);
            Assert.Equal(dto.StartDate, returnedDto.StartDate);

            _userServiceMock.Verify(_userService => _userService.CheckUserRole("valid-token", "Doctor"), Times.Once);
            _serviceMock.Verify(_service => _service.AddAsync(createDto), Times.Once);
        }

        [Fact]
        public async Task UpdateAppointment_ReturnsOkResult()
        {
            // Arrange
            var _controller = new AppointmentController(_serviceMock.Object, _userServiceMock.Object);


            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };

            _userServiceMock
                .Setup(_userService => _userService.CheckUserRole("valid-token", "Doctor"))
                .Returns(false);

            var id = "appointment-id";
            var updateDto = new UpdateAppointmentDto(
                id,
                "R201",
                 "14:00:00",
                 "16:00:00",
                 "2024-07-08",
                 "2024-07-08",
                 new List<string> { "D202500001" }
            );       
            var updatedAppointment = new AppointmentDto(
                Guid.NewGuid(),
                "Scheduled",
                id,  // Set OperationRequestId
                "R201",
                 "14:00:00",
                 "16:00:00",
                 "08/07/2024 00:00:00",
                 "08/07/2024 00:00:00",
                 new List<string> { "D202500001" }
            );         

            _serviceMock.Setup(x => x.UpdateAsync(updateDto)).ReturnsAsync(updatedAppointment);

           // Act
           var result = await _controller.UpdateAppointment(updateDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<AppointmentDto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(updatedAppointment, okResult.Value);
            _serviceMock.Verify(x => x.UpdateAsync(updateDto), Times.Once);
        }
    }

}