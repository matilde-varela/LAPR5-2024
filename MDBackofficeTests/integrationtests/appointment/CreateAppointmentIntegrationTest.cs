using MDBackoffice.Controllers;
using MDBackoffice.Domain.Appointments;
using MDBackoffice.Domain.Emails;
using MDBackoffice.Domain.Logs;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.Tokens;
using MDBackoffice.Domain.Users;
using MDBackoffice.Infrastructure.Emails;
using MDBackoffice.Domain.AppointmentStaffs;
using MDBackoffice.Domain.OperationRequests;
using MDBackoffice.Domain.OperationTypes;
using MDBackoffice.Domain.OperationTypes.ValueObjects.Phase;
using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;
using MDBackoffice.Domain.Rooms;
using MDBackoffice.Domain.RoomTypes;
using MDBackoffice.Domain.StaffProfiles;
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
namespace MDBackofficeTests.integrationtests.appointment
{
    public class CreateAppointmentIntegrationTests
    {
        private readonly AppointmentService _service;
        private readonly Mock<UserService> _userServiceMock;
        private readonly AppointmentController _controller;
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

        public CreateAppointmentIntegrationTests()
        {
            _service = new AppointmentService(_unitOfWorkMock.Object, _repoAppointMock.Object, _repoOpReqMock.Object,
                _repoRoomMock.Object, _repoOpTypeMock.Object, _repoStaMock.Object, _repoReqStaMock.Object, _appointmentStaffRepoMock.Object);

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

            _controller = new AppointmentController(_service, _userServiceMock.Object);
        }

        [Fact]
        public async Task CreateAppointment_ReturnsOkResult_IntegrationControllerService()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };

            _userServiceMock
                .Setup(_userService => _userService.CheckUserRole("valid-token", "Doctor"))
                .Returns(false);



            var createDto = new CreatingAppointmentDto(
                "test id",                 
                "R102",                    
                "14:00:00",                
                "16:00:00",               
                "2024-07-08",              
                "2024-07-08",              
                new List<string> { "D202400001" } 
            );

            var id = "appointment-id";
            var dto = new AppointmentDto(
                Guid.NewGuid(),
                "Scheduled",
                id,
                "R201",
                 "14:00:00",
                 "16:00:00",
                 "08/07/2024 00:00:00",
                 "08/07/2024 00:00:00",
                 new List<string> { "D202400001" }
            );


            _repoOpReqMock
                .Setup(x => x.GetByIdAsync(It.IsAny<OperationRequestId>()))
                .ReturnsAsync(new OperationRequest(
                    "test id",
                    "2024-07-01",
                    "Elective",
                    "2024-06-01",
                    "O202400001",
                    "Test operation",
                    "202412000001",
                    "operation-type-id-1"
                ));

            _repoOpTypeMock
                .Setup(x => x.GetByIdAsync(It.IsAny<OperationTypeId>()))
                .ReturnsAsync(new OperationType(
                    "Type1",
                    120,
                    true,
                    new List<RequiredStaffDto>{
                        new RequiredStaffDto
                        {
                            StaffQuantity = 1,
                            Function = "Doctor",
                            Specialization = "10101010"
                        }
                    },
                    new List<PhaseDto>
                    {
                        new PhaseDto
                        {
                            Description = "Phase 1",
                            Duration = 60
                        },
                         new PhaseDto
                        {
                            Description = "Phase 2",
                            Duration = 60
                        },
                          new PhaseDto
                        {
                            Description = "Phase 3",
                            Duration = 60
                        }
                    }
                ));

            _repoOpTypeMock
                .Setup(x => x.GetRequiredStaffByOperationTypeIdAsync(It.IsAny<OperationTypeId>()))
                .ReturnsAsync(new List<RequiredStaff>
                {
                     new RequiredStaff(1, "Doctor", "10101010")
                });


            var staffMock = new Mock<Staff>("00001", "country, 12345, street test", "12345", "first", "last", "first last", "email@email.com", "+123", "12345678", "doctor", "10101010");

            _repoStaMock
                .Setup(x => x.GetByIdAsync(It.IsAny<StaffId>()))
                .ReturnsAsync(staffMock.Object);

            _appointmentStaffRepoMock
                .Setup(x => x.IsStaffAvailableAsync(It.IsAny<StaffId>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid?>()))
                .ReturnsAsync(true);

            var roomMock = new Mock<Room>(
                      new RoomNumber("R102"),
                      new RoomTypeCode("BLCOP-T1"),
                      new Mock<Capacity>(5).Object,
                       new List<Equipment>
                       {
                    new Mock<Equipment>("Surgical Light").Object,
                    new Mock<Equipment>("Surgical Table").Object
                       },
                       CurrentStatus.Available,
                       new List<Slot>
                       {
                    new Mock<Slot>("Routine surgery", "14:00", "16:00", "2024-07-08", "2024-07-08").Object
                       });

            _repoRoomMock.Setup(x => x.GetByIdAsync(It.IsAny<RoomNumber>())).ReturnsAsync(roomMock.Object);
            _repoRoomMock.Setup(x => x.IsRoomAvailableAsync(It.IsAny<RoomNumber>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid?>())).ReturnsAsync(true);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<AppointmentDto>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        }
    }
}
