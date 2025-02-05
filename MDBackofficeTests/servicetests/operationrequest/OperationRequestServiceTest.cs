using MDBackoffice.Domain.Appointments;
using MDBackoffice.Domain.AppointmentStaffs;
using MDBackoffice.Domain.Emails;
using MDBackoffice.Domain.Logs;
using MDBackoffice.Domain.OperationRequests;
using MDBackoffice.Domain.OperationTypes;
using MDBackoffice.Domain.OperationTypes.ValueObjects.Phase;
using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;
using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.Rooms;
using MDBackoffice.Domain.RoomTypes;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.Specializations;
using MDBackoffice.Domain.StaffProfiles;
using MDBackoffice.Domain.Tokens;
using MDBackoffice.Domain.Users;
using MDBackoffice.Infrastructure;
using MDBackoffice.Infrastructure.Emails;
using MDBackoffice.Infrastructure.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace MDBackofficeTests.servicetests.operationrequest;
public class OperationRequestServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
    private readonly Mock<IOperationRequestRepository> _repoMock = new Mock<IOperationRequestRepository>();
    private readonly Mock<IStaffRepository> _repoStaMock = new Mock<IStaffRepository>();
    private readonly Mock<IPatientRepository> _repoPatMock = new Mock<IPatientRepository>();
    private readonly Mock<IOperationTypeRepository> _repoOpTypeMock = new Mock<IOperationTypeRepository>();
    private readonly Mock<IRoomRepository> _repoRoomMock = new Mock<IRoomRepository>();
    private readonly Mock<LogService> _logServiceMock = new Mock<LogService>(new Mock<IUnitOfWork>().Object, new Mock<ILogRepository>().Object);
    private readonly Mock<UserService> _userServiceMock;
    private readonly Mock<PatientService> _patientServiceMock;
    private readonly OperationRequestService _service;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<ILoginAdapter> _loginAdapterMock;
    private readonly Mock<IOperationSchedulerAdapter> _schedulerAdapterMock;
    private readonly Mock<RoomService> _roomServiceMock;
    private readonly Mock<SpecializationService> _specServiceMock = new Mock<SpecializationService>(new Mock<IUnitOfWork>().Object, new Mock<ISpecializationRepository>().Object);
    private readonly Mock<IPatientMedicalRecordAdapter> _patientMRAMock;

    public OperationRequestServiceTests()
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

        var roleManagerMock = new Mock<RoleManager<Role>>(
            new Mock<IRoleStore<Role>>().Object,
            new List<IRoleValidator<Role>>(),
            new Mock<ILookupNormalizer>().Object,
            identityErrorDescriberMock.Object,
             new Mock<ILogger<RoleManager<Role>>>().Object
        );

        var tokenServiceMock = new Mock<TokenService>(_unitOfWorkMock.Object, new Mock<ITokenRepository>().Object, _userManagerMock.Object);
        var _emailServiceMock = new Mock<EmailService>(tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);
        var _configurationMock = new Mock<IConfiguration>();
        _loginAdapterMock = new Mock<ILoginAdapter>();
        var signinManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object,
                                                               new Mock<IHttpContextAccessor>().Object,
                                                               new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                                                               identityOptionsMock.Object,
                                                               new Mock<ILogger<SignInManager<User>>>().Object,
                                                               new Mock<IAuthenticationSchemeProvider>().Object,
                                                               new Mock<IUserConfirmation<User>>().Object);
        _userServiceMock = new Mock<UserService>(
                _userManagerMock.Object,
                roleManagerMock.Object,
                _logServiceMock.Object,
                signinManagerMock.Object,
                _emailServiceMock.Object,
                _configurationMock.Object,
                tokenServiceMock.Object,
                _loginAdapterMock.Object
            );

        _patientMRAMock = new Mock<IPatientMedicalRecordAdapter>();

        _patientServiceMock = new Mock<PatientService>(_unitOfWorkMock.Object, _logServiceMock.Object, _configurationMock.Object, _repoPatMock.Object,
                    _userServiceMock.Object, _emailServiceMock.Object, _patientMRAMock.Object);
        _schedulerAdapterMock = new Mock<IOperationSchedulerAdapter>();
        
            var _repoRoomTypeMock = new Mock<IRoomTypeRepository>();

            _roomServiceMock = new Mock<RoomService>(_unitOfWorkMock.Object, _repoRoomMock.Object, _repoRoomTypeMock.Object);

            var _repoReqSta = new Mock<IRequiredStaffRepository>();
            var _repoAppointMock = new Mock<IAppointmentRepository>();
            var _repoAppointmentStaffMock = new Mock<IAppointmentStaffRepository>();

            var _appointmentServiceMock = new Mock<AppointmentService>( _unitOfWorkMock.Object, _repoAppointMock.Object, _repoMock.Object, _repoRoomMock.Object, _repoOpTypeMock.Object, _repoStaMock.Object, _repoReqSta.Object, _repoAppointmentStaffMock.Object);

            _service = new OperationRequestService(_unitOfWorkMock.Object, _repoMock.Object,
                                                    _repoStaMock.Object, _logServiceMock.Object, _patientServiceMock.Object,
                                                    _repoPatMock.Object, _repoOpTypeMock.Object, _userServiceMock.Object,  _schedulerAdapterMock.Object, _roomServiceMock.Object, _appointmentServiceMock.Object, _specServiceMock.Object);
    }

    [Fact]
    public async Task AddAsync_ReturnsOperationRequest()
    {
        // Arrange
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

        var staffMock = new Mock<Staff>("00001", "country, 12345, street test", "12345", "first", "last", "first last", "email@email.com", "+123", "12345678", "doctor", "25841209");
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
                Specialization = "25841209"
            }
        };

        var _repoRoomTypeMock = new Mock<IRoomTypeRepository>();

        var _repoReqSta = new Mock<IRequiredStaffRepository>();
        var _repoAppointMock = new Mock<IAppointmentRepository>();
        var _repoAppointmentStaffMock = new Mock<IAppointmentStaffRepository>();

        var _appointmentServiceMock = new Mock<AppointmentService>( _unitOfWorkMock.Object, _repoAppointMock.Object, _repoMock.Object, _repoRoomMock.Object, _repoOpTypeMock.Object, _repoStaMock.Object, _repoReqSta.Object, _repoAppointmentStaffMock.Object);

        var operationTypeMock = new Mock<OperationType>(opTyId, 100, true, reqStaff, phases);

        _repoStaMock.Setup(_repoMock => _repoMock.GetByIdAsync(It.IsAny<StaffId>())).ReturnsAsync(staffMock.Object);
        _repoPatMock.Setup(_repoPatMock => _repoPatMock.GetByIdAsync(It.IsAny<MedicalRecordNumber>()))
            .ReturnsAsync(patientMock.Object);
        _repoOpTypeMock.Setup(_repoOpTypeMock => _repoOpTypeMock.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(operationTypeMock.Object);

        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);


        var service = new OperationRequestService(_unitOfWorkMock.Object, _repoMock.Object,
                                                    _repoStaMock.Object, _logServiceMock.Object, _patientServiceMock.Object,
                                                    _repoPatMock.Object, _repoOpTypeMock.Object, _userServiceMock.Object, _schedulerAdapterMock.Object, _roomServiceMock.Object, _appointmentServiceMock.Object,_specServiceMock.Object);

        //Act
        var result = await service.AddAsync(dtoMock);

        //Assert
        Assert.NotNull(result);
        Assert.NotNull(result);
        Assert.Equal(staffId, result.StaffId);
        Assert.Equal(patientId, result.PatientId);
        Assert.Equal(opTyId, result.OperationTypeId);
        Assert.Equal(dtoMock.Description, result.Description);
        _repoMock.Verify(r => r.AddAsync(It.IsAny<OperationRequest>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }


    [Fact]
    public async Task GetOperationRequestByFiltersAsync_ReturnsCorrectOperationRequestDtos()
    {
        // Arrange
        var staffId = "D202500001";
            var opTyId = "tumor removal";
            var patientId = "202501000001";
        var email = "email@email.com";
        var priority = "Elective";
        var name = "John Doe";
        var operationType = "tumor removal";
        var status = "Requested";
        var dateOfRequest = "2024-10-25";
        var deadLineDate = "2024-12-31";


        var staffMock = new Staff("00001", "country, 12345, street test", "12345", "first", "last", "first last", "email@email.com", "+123", "12345678", "doctor", "251010300");
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
                Specialization = "251010300"
            }
        };

        var operationTypeMock = new Mock<OperationType>(opTyId, 100, true, reqStaff, phases);

        var operationRequests = new List<Mock<OperationRequest>>
        {
            new Mock<OperationRequest>("TestCode","2024-12-31","Elective","2024-10-25",
            staffId,"descript",patientId,opTyId),
        };

        var expectedDtos = new List<ListOperationRequestDto>
        {
            new ListOperationRequestDto("TestCode","first last", "tumor removal", "Requested"),
        };

        _repoStaMock.Setup(repo => repo.GetStaffWithEmail(email)).ReturnsAsync(staffMock);

        var operationRequestObjects = operationRequests.Select(mock => mock.Object).ToList();
        _repoMock.Setup(repo => repo.FindAllConditioned(new StaffId(staffId), name, priority, operationType, status, dateOfRequest, deadLineDate))
            .ReturnsAsync(operationRequestObjects);
        _repoPatMock.Setup(repo => repo.GetByIdAsync(new MedicalRecordNumber(patientId))).ReturnsAsync(patientMock.Object);


        // Act
        var result = await _service.GetOperationRequestByFiltersAsync(email, name, priority, operationType, status, dateOfRequest, deadLineDate);

        // Assert
        Assert.Equal(expectedDtos.Count, result.Count);
        for (int i = 0; i < expectedDtos.Count; i++)
        {
            Assert.Equal(expectedDtos[i].PatientName, result[i].PatientName);
            Assert.Equal(expectedDtos[i].OperationType, result[i].OperationType);
            Assert.Equal(expectedDtos[i].Status, result[i].Status);
        }

        _repoMock.Verify(repo => repo.FindAllConditioned(new StaffId(staffId), name, priority, operationType, status, dateOfRequest, deadLineDate), Times.Once);

    }

    [Fact]
    public async Task DeleteOperationRequest_ReturnsOkResponse()
    {
        // Arrange
        var staffId = "D202500001";
            var opTyId = "tumor removal";
            var patientId = "202501000001";
        var email = "email@email.com";

        var staffMock = new Mock<Staff>("00001", "country, 12345, street test", "12345", "first", "last", "first last", "email@email.com", "+123", "12345678", "doctor", "251010300");
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
                Specialization = "251010300"
            }
        };

        var operationTypeMock = new Mock<OperationType>(opTyId, 100, true, reqStaff, phases);

        var operationRequest = new Mock<OperationRequest>("TestCode", "2024-12-31", "Elective", "2024-10-25",
            staffId, "descript", patientId, opTyId);

        var password = "NewPass00_d";

        var userMock = new Mock<User>();
        userMock.Setup(u => u.Id).Returns(patientId);
        userMock.Setup(u => u.UserName).Returns(email);
        userMock.Setup(u => u.Email).Returns(email);
        userMock.Setup(u => u.Status).Returns(true);
        userMock.Setup(u => u.PasswordHash).Returns(password);

        _userManagerMock.Setup(r => r.FindByEmailAsync(email)).ReturnsAsync(userMock.Object);
        _repoStaMock.Setup(r => r.FindStaffWithUserId(patientId)).ReturnsAsync(staffMock.Object);
        _repoMock.Setup(repo => repo.GetByIdAsync(It.IsAny<OperationRequestId>())).ReturnsAsync(operationRequest.Object);

        _repoMock.Setup(r => r.Remove(operationRequest.Object));
        _unitOfWorkMock.Setup(r => r.CommitAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.DeleteOperationRequest(operationRequest.Object.Id.Value, email);

        //Assert
        Assert.True(result);
    }


    [Fact]
    public async Task DeleteOperationRequestFromPatient_ReturnsOkResponse()
    {
        // Arrange
        var staffId = "D202500001";
            var opTyId = "tumor removal";
            var patientId = "202501000001";
        var email = "email@email.com";

        var staffMock = new Mock<Staff>("00001", "country, 12345, street test", "12345", "first", "last", "first last", "email@email.com", "+123", "12345678", "doctor", "251010300");
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
                    Specialization = "251010300"
                }
            };

        var operationTypeMock = new Mock<OperationType>(opTyId, 100, true, reqStaff, phases);

        var operationRequest = new Mock<OperationRequest>("TestCode", "2024-12-31", "Elective", "2024-10-25",
            staffId, "descript", patientId, opTyId);

        var password = "NewPass00_d";

        var userMock = new Mock<User>();
        userMock.Setup(u => u.Id).Returns(patientId);
        userMock.Setup(u => u.UserName).Returns(email);
        userMock.Setup(u => u.Email).Returns(email);
        userMock.Setup(u => u.Status).Returns(true);
        userMock.Setup(u => u.PasswordHash).Returns(password);

        _userManagerMock.Setup(r => r.FindByEmailAsync(email)).ReturnsAsync(userMock.Object);
        _repoStaMock.Setup(r => r.FindStaffWithUserId(patientId)).ReturnsAsync(staffMock.Object);
        _repoPatMock.Setup(r => r.GetByIdWithAppointmentHistoryAsync(It.IsAny<MedicalRecordNumber>())).ReturnsAsync(patientMock.Object);
        _repoMock.Setup(repo => repo.GetByIdAsync(It.IsAny<OperationRequestId>())).ReturnsAsync(operationRequest.Object);
        patientMock.Setup(r => r.RemoveRequestFromHistory(operationRequest.Object.Id));
        _repoMock.Setup(r => r.Remove(operationRequest.Object));
        _unitOfWorkMock.Setup(r => r.CommitAsync()).ReturnsAsync(1);


        // Act
        var result = await _service.DeleteOperationRequestFromPatient(patientId, operationRequest.Object.Id.Value, email);

        //Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateOperationRequest_WhenFieldsAreChanged()
    {
        // Arrange
        var operationRequestId = new Mock<OperationRequestId>("6B29FC40-CA47-1067-B31D-00DD010662DA");
        var initialDeadLineDate = "2024-12-31";
        var initialPriority = "Elective";
        var initialDescription = "Initial Description";

        var updatedDeadLineDate = "2025-01-31";
        var updatedPriority = "Urgent";
        var updatedDescription = "Updated Description";

        UpdateOperationRequestDto dto = new UpdateOperationRequestDto("6B29FC40-CA47-1067-B31D-00DD010662DA", updatedDeadLineDate, updatedPriority, updatedDescription);

        var operationRequestMock = new Mock<OperationRequest>("6B29FC40-CA47-1067-B31D-00DD010662DA", initialDeadLineDate, initialPriority, "2024-10-25", "D202400001", initialDescription, "202410000001", "tumor removal");

        _repoMock.Setup(repo => repo.GetByIdAsync(It.IsAny<OperationRequestId>())).ReturnsAsync(operationRequestMock.Object);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.UpdateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(updatedDeadLineDate.ToString(), result.DeadLineDate.ToString().Split("/")[0]);
        Assert.Equal(updatedPriority, result.Priority);
        Assert.Equal(updatedDescription, result.Description);

        // Verify that CommitAsync is called to save changes
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(), Times.Once);
    }


}

