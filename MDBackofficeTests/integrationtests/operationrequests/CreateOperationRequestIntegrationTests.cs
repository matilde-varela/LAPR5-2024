using MDBackoffice.Controllers;
using MDBackoffice.Domain.Emails;
using MDBackoffice.Domain.Logs;
using MDBackoffice.Domain.OperationRequests;
using MDBackoffice.Domain.OperationTypes.ValueObjects.Phase;
using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;
using MDBackoffice.Domain.OperationTypes;
using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.StaffProfiles;
using MDBackoffice.Domain.Tokens;
using MDBackoffice.Domain.Users;
using MDBackoffice.Infrastructure.Emails;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using MDBackoffice.Infrastructure.Users;
using MDBackoffice.Domain.Rooms;
using MDBackoffice.Domain.Appointments;
using MDBackoffice.Domain.AppointmentStaffs;
using MDBackoffice.Domain.RoomTypes;
using MDBackoffice.Domain.Specializations;


namespace MDBackofficeTests.integrationtests.operationrequests
{


    public class OperationRequestIntegrationTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IOperationRequestRepository> _repoMock = new Mock<IOperationRequestRepository>();
        private readonly Mock<IStaffRepository> _repoStaMock = new Mock<IStaffRepository>();
        private readonly Mock<IPatientRepository> _repoPatMock = new Mock<IPatientRepository>();
        private readonly Mock<IOperationTypeRepository> _repoOpTypeMock = new Mock<IOperationTypeRepository>();
        private readonly Mock<IRoomRepository> _repoRoomMock = new Mock<IRoomRepository>();
        private readonly Mock<LogService> _logServiceMock;
        private readonly Mock<PatientService> _patientServiceMock;
        private readonly Mock<UserService> _userServiceMock;
        private readonly Mock<ILoginAdapter> _loginAdapterMock;
        private readonly Mock<IOperationSchedulerAdapter> _schedulerAdapterMock;
        private readonly Mock<RoomService> _roomServiceMock;
        private readonly Mock<AppointmentService> _appointmentServiceMock;
        private readonly Mock<SpecializationService> _specServiceMock = new Mock<SpecializationService>(new Mock<IUnitOfWork>().Object, new Mock<ISpecializationRepository>().Object);
        private readonly Mock<IPatientMedicalRecordAdapter> _patientMRAMock;

        public OperationRequestIntegrationTests()
        {
            _logServiceMock = new Mock<LogService>(new Mock<IUnitOfWork>().Object, new Mock<ILogRepository>().Object);

            var identityOptionsMock = new Mock<IOptions<IdentityOptions>>();
            identityOptionsMock.Setup(o => o.Value).Returns(new IdentityOptions());
            var identityErrorDescriberMock = new Mock<IdentityErrorDescriber>();

            var userManagerMock = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, identityOptionsMock.Object, new Mock<IPasswordHasher<User>>().Object, new List<IUserValidator<User>> { new Mock<IUserValidator<User>>().Object }, new List<IPasswordValidator<User>> { new Mock<IPasswordValidator<User>>().Object }, new Mock<ILookupNormalizer>().Object, identityErrorDescriberMock.Object, new Mock<IServiceProvider>().Object, new Mock<ILogger<UserManager<User>>>().Object);
            var roleManagerMock = new Mock<RoleManager<Role>>(new Mock<IRoleStore<Role>>().Object, new List<IRoleValidator<Role>>(), new Mock<ILookupNormalizer>().Object, identityErrorDescriberMock.Object, new Mock<ILogger<RoleManager<Role>>>().Object);

            var tokenServiceMock = new Mock<TokenService>(_unitOfWorkMock.Object, new Mock<ITokenRepository>().Object, userManagerMock.Object);
            var _emailServiceMock = new Mock<EmailService>(tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);
            var _configurationMock = new Mock<IConfiguration>();
            _loginAdapterMock = new Mock<ILoginAdapter>();
            var signinManagerMock = new Mock<SignInManager<User>>(userManagerMock.Object,
                                                               new Mock<IHttpContextAccessor>().Object,
                                                               new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                                                               identityOptionsMock.Object,
                                                               new Mock<ILogger<SignInManager<User>>>().Object,
                                                               new Mock<IAuthenticationSchemeProvider>().Object,
                                                               new Mock<IUserConfirmation<User>>().Object);

            _userServiceMock = new Mock<UserService>(userManagerMock.Object, roleManagerMock.Object, _logServiceMock.Object, signinManagerMock.Object, _emailServiceMock.Object, _configurationMock.Object, tokenServiceMock.Object, _loginAdapterMock.Object);
            _patientMRAMock = new Mock<IPatientMedicalRecordAdapter>();
            _patientServiceMock = new Mock<PatientService>(_unitOfWorkMock.Object, _logServiceMock.Object, _configurationMock.Object, _repoPatMock.Object, _userServiceMock.Object, _emailServiceMock.Object, _patientMRAMock.Object);
            var _repoRoomTypeMock = new Mock<IRoomTypeRepository>();
            _schedulerAdapterMock = new Mock<IOperationSchedulerAdapter>();
            _roomServiceMock = new Mock<RoomService>(_unitOfWorkMock.Object, _repoRoomMock.Object, _repoRoomTypeMock.Object);

            var _repoReqSta = new Mock<IRequiredStaffRepository>();
            var _repoAppointMock = new Mock<IAppointmentRepository>();
            var _repoAppointmentStaffMock = new Mock<IAppointmentStaffRepository>();

            _appointmentServiceMock = new Mock<AppointmentService>( _unitOfWorkMock.Object, _repoAppointMock.Object, _repoMock.Object, _repoRoomMock.Object, _repoOpTypeMock.Object, _repoStaMock.Object, _repoReqSta.Object, _repoAppointmentStaffMock.Object);
        }

        [Fact]
        public async Task CreateWithValidData_ReturnsCreatedResult_IntegrationControllerService()
        {
            // Pass mocked dependencies to OperationRequestService
            var _service = new OperationRequestService(_unitOfWorkMock.Object, _repoMock.Object,
                                                    _repoStaMock.Object, _logServiceMock.Object, _patientServiceMock.Object,
                                                    _repoPatMock.Object, _repoOpTypeMock.Object, _userServiceMock.Object, _schedulerAdapterMock.Object, _roomServiceMock.Object, _appointmentServiceMock.Object, _specServiceMock.Object);

            var _controller = new OperationRequestController(_service, _userServiceMock.Object);
            var staffId = "D202500001";
            var opTyId = "tumor removal";
            var patientId = "202501000001";
            var dtoMock = new CreatingOperationRequestDto
            ("2024-12-31",
                "Elective",
                "2024-10-25",
                "Pending",
                staffId,
                "descript",
                patientId,
                opTyId
            );
            var staffMock = new Mock<Staff>("00001", "country, 12345, street test", "12345", "first", "last", "first last", "email@email.com", "+123", "12345678", "doctor", "25001907");
            var patientMock = new Mock<Patient>("first", "last", "first last", "country, 12345, street test", "female", "+123", "12345678", "98765432", "email@email.com", "2000-10-10", "000001");
            var phases = new List<PhaseDto>
            {
                new PhaseDto {
                            Description = "descrip",
                            Duration = 25
                            },
                new PhaseDto {
                            Description = "descrip2",
                            Duration = 50
                            },
                new PhaseDto {
                            Description = "descrip3",
                            Duration = 25
                            }
            };
            var reqStaff = new List<RequiredStaffDto>
            {
                new RequiredStaffDto{
                    StaffQuantity = 1,
                    Function = "doctor",
                    Specialization = "25001907"
                }
            };

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Doctor")).Returns(false);

            var operationTypeMock = new Mock<OperationType>(opTyId, 100, true, reqStaff, phases);
            _repoStaMock.Setup(_repoMock => _repoMock.GetByIdAsync(It.IsAny<StaffId>())).ReturnsAsync(staffMock.Object);
            _repoPatMock.Setup(_repoPatMock => _repoPatMock.GetByIdAsync(It.IsAny<MedicalRecordNumber>()))
                .ReturnsAsync(patientMock.Object);
            _repoOpTypeMock.Setup(_repoOpTypeMock => _repoOpTypeMock.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(operationTypeMock.Object);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _controller.Create(dtoMock);

            // Assert
            var actionResult = Assert.IsType<ActionResult<OperationRequestDto>>(result); // Ensure it's the correct ActionResult type
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetGetById", createdAtActionResult.ActionName);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ReturnsOperationRequest_IntegrationServiceDomain()
        {
            // Pass mocked dependencies to OperationRequestService
            var staffId = "D202500001";
            var opTyId = "tumor removal";
            var patientId = "202501000001";
            var dto = new CreatingOperationRequestDto
            ("2024-12-31",
                "Elective",
                "2024-10-25",
                "Pending",
                staffId,
                "descript",
                patientId,
                opTyId
            );
            var staff = new Staff("00001", "country, 12345, street test", "12345", "first", "last", "first last", "email@email.com", "+123", "12345678", "doctor", "25001907");
            var patient = new Patient("first", "last", "first last", "country, 12345, street test", "female", "+123", "12345678", "98765432", "email@email.com", "2000-10-10", "000001");
            var phases = new List<PhaseDto>
            {
                new PhaseDto {
                            Description = "descrip",
                            Duration = 25
                            },
                new PhaseDto {
                            Description = "descrip2",
                            Duration = 50
                            },
                new PhaseDto {
                            Description = "descrip3",
                            Duration = 25
                            }
            };
            var reqStaff = new List<RequiredStaffDto>
            {
                new RequiredStaffDto{
                    StaffQuantity = 1,
                    Function = "doctor",
                    Specialization = "25001907"
                }
            };
            var operationType = new OperationType(opTyId, 100, true, reqStaff, phases);
            _repoStaMock.Setup(_repoMock => _repoMock.GetByIdAsync(It.IsAny<StaffId>())).ReturnsAsync(staff);
            _repoPatMock.Setup(_repoPatMock => _repoPatMock.GetByIdAsync(It.IsAny<MedicalRecordNumber>()))
                .ReturnsAsync(patient);
            _repoOpTypeMock.Setup(_repoOpTypeMock => _repoOpTypeMock.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(operationType);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            var _service = new OperationRequestService(_unitOfWorkMock.Object, _repoMock.Object,
                                                 _repoStaMock.Object, _logServiceMock.Object, _patientServiceMock.Object,
                                                 _repoPatMock.Object, _repoOpTypeMock.Object, _userServiceMock.Object, _schedulerAdapterMock.Object, _roomServiceMock.Object, _appointmentServiceMock.Object, _specServiceMock.Object);

            // Act
            var result = await _service.AddAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result);
            Assert.Equal(staffId, result.StaffId);
            Assert.Equal(patientId, result.PatientId);
            Assert.Equal(opTyId, result.OperationTypeId);
            Assert.Equal(dto.Description, result.Description);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<OperationRequest>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }
    }
}
