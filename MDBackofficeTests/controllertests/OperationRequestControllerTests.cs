using MDBackoffice.Controllers;
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
using MDBackoffice.Infrastructure.Emails;
using MDBackoffice.Infrastructure.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Moq;
using System.Security.Claims;
using System.Xml.Linq;
using Xunit;


namespace MDBackofficeTests.controllertests
{


    public class OperationRequestControllerTests
    {

        private readonly Mock<OperationRequestService> _service;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IOperationRequestRepository> _repoMock = new Mock<IOperationRequestRepository>();
        private readonly Mock<IStaffRepository> _repoStaMock = new Mock<IStaffRepository>();
        private readonly Mock<IPatientRepository> _repoPatMock = new Mock<IPatientRepository>();
        private readonly Mock<IOperationTypeRepository> _repoOpTypeMock = new Mock<IOperationTypeRepository>();
        private readonly Mock<IRoomRepository> _repoRoomMock = new Mock<IRoomRepository>();
        private readonly OperationRequestController _controller;
        private readonly Mock<UserService> _userServiceMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<ILoginAdapter> _loginAdapterMock;
         private readonly Mock<IOperationSchedulerAdapter> _schedulerAdapterMock;
        private readonly Mock<RoomService> _roomServiceMock;
        private readonly Mock<SpecializationService> _specServiceMock = new Mock<SpecializationService>(new Mock<IUnitOfWork>().Object, new Mock<ISpecializationRepository>().Object);
        private readonly Mock<IPatientMedicalRecordAdapter> _patientMRAMock;

        public OperationRequestControllerTests()
        {
            Mock<LogService> _logServiceMock = new Mock<LogService>(new Mock<IUnitOfWork>().Object, new Mock<ILogRepository>().Object);

            var identityOptionsMock = new Mock<IOptions<IdentityOptions>>();
            identityOptionsMock.Setup(o => o.Value).Returns(new IdentityOptions());
            var identityErrorDescriberMock = new Mock<IdentityErrorDescriber>();

            _userManagerMock = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, identityOptionsMock.Object, new Mock<IPasswordHasher<User>>().Object, new List<IUserValidator<User>> { new Mock<IUserValidator<User>>().Object }, new List<IPasswordValidator<User>> { new Mock<IPasswordValidator<User>>().Object }, new Mock<ILookupNormalizer>().Object, identityErrorDescriberMock.Object, new Mock<IServiceProvider>().Object, new Mock<ILogger<UserManager<User>>>().Object);

            var signinManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object,
                                                                new Mock<IHttpContextAccessor>().Object,
                                                                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                                                                identityOptionsMock.Object,
                                                                new Mock<ILogger<SignInManager<User>>>().Object,
                                                                new Mock<IAuthenticationSchemeProvider>().Object,
                                                                new Mock<IUserConfirmation<User>>().Object);

            var roleManagerMock = new Mock<RoleManager<Role>>(new Mock<IRoleStore<Role>>().Object, new List<IRoleValidator<Role>>(), new Mock<ILookupNormalizer>().Object, identityErrorDescriberMock.Object, new Mock<ILogger<RoleManager<Role>>>().Object);

            var tokenServiceMock = new Mock<TokenService>(_unitOfWorkMock.Object, new Mock<ITokenRepository>().Object, _userManagerMock.Object);
            var _emailServiceMock = new Mock<EmailService>(tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);
            var _configurationMock = new Mock<IConfiguration>();
            _loginAdapterMock = new Mock<ILoginAdapter>();

            _userServiceMock = new Mock<UserService>(_userManagerMock.Object, roleManagerMock.Object, _logServiceMock.Object, signinManagerMock.Object, _emailServiceMock.Object, _configurationMock.Object, tokenServiceMock.Object, _loginAdapterMock.Object);
            _patientMRAMock = new Mock<IPatientMedicalRecordAdapter>();

            var _patientServiceMock = new Mock<PatientService>(_unitOfWorkMock.Object, _logServiceMock.Object, _configurationMock.Object, _repoPatMock.Object, _userServiceMock.Object, _emailServiceMock.Object, _patientMRAMock.Object);

            _schedulerAdapterMock = new Mock<IOperationSchedulerAdapter>();

            var _repoRoomTypeMock = new Mock<IRoomTypeRepository>();

            _roomServiceMock = new Mock<RoomService>(_unitOfWorkMock.Object, _repoRoomMock.Object, _repoRoomTypeMock.Object);

            var _repoReqSta = new Mock<IRequiredStaffRepository>();
            var _repoAppointMock = new Mock<IAppointmentRepository>();
            var _repoAppointmentStaffMock = new Mock<IAppointmentStaffRepository>();

            var _appointmentServiceMock = new Mock<AppointmentService>( _unitOfWorkMock.Object, _repoAppointMock.Object, _repoMock.Object, _repoRoomMock.Object, _repoOpTypeMock.Object, _repoStaMock.Object, _repoReqSta.Object, _repoAppointmentStaffMock.Object);

            _service = new Mock<OperationRequestService>(_unitOfWorkMock.Object, _repoMock.Object,
                                                    _repoStaMock.Object, _logServiceMock.Object, _patientServiceMock.Object,
                                                    _repoPatMock.Object, _repoOpTypeMock.Object, _userServiceMock.Object,  _schedulerAdapterMock.Object, _roomServiceMock.Object, _appointmentServiceMock.Object,_specServiceMock.Object);

            _controller = new OperationRequestController(_service.Object, _userServiceMock.Object);
        }

        [Fact]
        public async Task Create_ReturnsCreatedResult()
        {
            // Arrange
            var controller = new OperationRequestController(_service.Object, _userServiceMock.Object);

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Doctor")).Returns(false);

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

            var staffMock = new Mock<Staff>("00001", "country, 12345, street test", "12345", "first", "last", "first last", "email@email.com", "+123", "12345678", "doctor", "25841809");
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
                    Specialization = "25841809"
                }
            };

            var operationTypeMock = new Mock<OperationType>(opTyId, 100, true, reqStaff, phases);

            _repoStaMock.Setup(_repoMock => _repoMock.GetByIdAsync(It.IsAny<StaffId>())).ReturnsAsync(staffMock.Object);
            _repoPatMock.Setup(_repoPatMock => _repoPatMock.GetByIdAsync(It.IsAny<MedicalRecordNumber>()))
                .ReturnsAsync(patientMock.Object);
            _repoOpTypeMock.Setup(_repoOpTypeMock => _repoOpTypeMock.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(operationTypeMock.Object);

            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await controller.Create(dtoMock);

            // Assert
            var actionResult = Assert.IsType<ActionResult<OperationRequestDto>>(result); // Ensure it's the correct ActionResult type
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.Equal("GetGetById", createdAtActionResult.ActionName);
        }



        [Fact]
        public async Task GetOperationRequestByFilters_ReturnsOkOperationTypeDtos()
        {
            // Arrange
            // Set up the User claims
            var emailClaim = "email@email.com";

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

            var staffMock = new Staff("00001", "country, 12345, street test", "12345", "first", "last", "first last", "email@email.com", "+123", "12345678", "doctor", "25841809");
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
                    Specialization = "25841809"
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


            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Doctor")).Returns(false);

            _userServiceMock.Setup(_userService => _userService.DecodeJwtToken("valid-token")).Returns((emailClaim, new List<string> { "Doctor" }));


            // Act
            var result = await _controller.GetOperationRequestByFilters(name, priority, operationType, status, dateOfRequest, deadLineDate);


            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualDtos = Assert.IsType<List<ListOperationRequestDto>>(okResult.Value);
            Assert.Equal(expectedDtos.Count, actualDtos.Count);
            for (int i = 0; i < expectedDtos.Count; i++)
            {
                Assert.Equal(expectedDtos[i].PatientName, actualDtos[i].PatientName);
                Assert.Equal(expectedDtos[i].OperationType, actualDtos[i].OperationType);
                Assert.Equal(expectedDtos[i].Status, actualDtos[i].Status);
            }

            _repoMock.Verify(repo => repo.FindAllConditioned(new StaffId(staffId), name, priority, operationType, status, dateOfRequest, deadLineDate), Times.Once);
        }

        [Fact]
        public async Task GetAllFromDoctor_ReturnsOkOperationRequestDto()
        {
            //Arrange
            var emailClaim = "email@email.com";
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.GetLoggedInEmail("valid-token")).Returns(emailClaim);

            var id = Guid.NewGuid();
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

            var staffMock = new Staff("00001", "country, 12345, street test", "12345", "first", "last", "first last", "email@email.com", "+123", "12345678", "doctor", "25841809");
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
                    Specialization = "25841809"
                }
            };

            List<OperationRequestDto> operations = new List<OperationRequestDto>();

            OperationRequestDto operationRequestDto = new OperationRequestDto(id, deadLineDate, priority, dateOfRequest, status, staffId, name, patientId, opTyId);

            operations.Add(operationRequestDto);

            _service.Setup(s => s.GetAllFromDoctorAsysnc(emailClaim)).ReturnsAsync(operations);

            //Act
            var result = _controller.GetAllFromDoctor();

            //Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<ActionResult<IEnumerable<OperationRequestDto>>>(result.Result);
            var returnedPatient = Assert.IsType<List<OperationRequestDto>>(okResult.Value);
        }

        [Fact]
        public async Task Update_ReturnsUpdateOperationRequestDto()
        {
            //Arrange
            var emailClaim = "email@email.com";

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Doctor")).Returns(false);
            _userServiceMock.Setup(_userService => _userService.GetLoggedInEmail("valid-token")).Returns(emailClaim);

            var id = Guid.NewGuid();
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

            var staffMock = new Staff("00001", "country, 12345, street test", "12345", "first", "last", "first last", "email@email.com", "+123", "12345678", "doctor", "25841809");
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
                    Specialization = "25841809"
                }
            };

            UpdateOperationRequestDto updateOperationRequestDto = new UpdateOperationRequestDto(id.ToString(), deadLineDate, priority, operationType);
            OperationRequestDto operationRequestDto = new OperationRequestDto(id, deadLineDate, priority, dateOfRequest, status, staffId, name, patientId, opTyId);

            _service.Setup(s => s.CheckDoctorIsRequestingDoctor(emailClaim, updateOperationRequestDto.Id)).ReturnsAsync(true);
            _service.Setup(s => s.UpdateAsync(updateOperationRequestDto)).ReturnsAsync(operationRequestDto);

            //Act
            var result = await _controller.Update(updateOperationRequestDto);

            //Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedInfo = Assert.IsType<OkObjectResult>(okResult);

        }



        [Fact]
        public async Task DeleteOperationRequest_ReturnsOkResponse()
        {
            // Arrange

            // Set up the User claims
            var emailClaim = "email@email.com";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, emailClaim)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var staffId = "D202500001";
            var opTyId = "tumor removal";
            var patientId = "202501000001";
            var email = "email@email.com";
            var password = "NewPass00_d";


            var staffMock = new Mock<Staff>("00001", "country, 12345, street test", "12345", "first", "last", "first last", "email@email.com", "+123", "12345678", "doctor", "25841809");
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
                    Specialization = "25841809"
                }
            };

            var operationTypeMock = new Mock<OperationType>(opTyId, 100, true, reqStaff, phases);

            var operationRequest = new Mock<OperationRequest>("TestCode", "2024-12-31", "Elective", "2024-10-25",
                staffId, "descript", patientId, opTyId);


            var userMock = new Mock<User>();

            userMock.Setup(u => u.Id).Returns(patientId);
            userMock.Setup(u => u.UserName).Returns(email);
            userMock.Setup(u => u.Email).Returns(email);
            userMock.Setup(u => u.Status).Returns(true);
            userMock.Setup(u => u.PasswordHash).Returns(password);

            _userManagerMock.Setup(r => r.FindByEmailAsync(email)).ReturnsAsync(userMock.Object);
            _repoStaMock.Setup(r => r.FindStaffWithUserId(patientId)).ReturnsAsync(staffMock.Object);
            _repoMock.Setup(repo => repo.GetByIdAsync(operationRequest.Object.Id)).ReturnsAsync(operationRequest.Object);

            _repoMock.Setup(r => r.Remove(operationRequest.Object));
            _unitOfWorkMock.Setup(r => r.CommitAsync()).ReturnsAsync(1);

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Doctor")).Returns(false);

            _userServiceMock.Setup(_userService => _userService.DecodeJwtToken("valid-token")).Returns((emailClaim, new List<string> { "Doctor" }));


            // Act
            var result = await _controller.DeleteOperationRequest(operationRequest.Object.Id.Value);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }


        [Fact]
        public async Task DeleteOperationRequestFromPatient_ReturnsOkResponse()
        {
            // Arrange

            // Set up the User claims
            var emailClaim = "email@email.com";

            var staffId = "D202500001";
            var opTyId = "tumor removal";
            var patientId = "202501000001";
            var email = "email@email.com";
            var password = "NewPass00_d";


            var staffMock = new Mock<Staff>("00001", "country, 12345, street test", "12345", "first", "last", "first last", "email@email.com", "+123", "12345678", "doctor", "25841809");
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
                    Specialization = "25841809"
                }
            };

            var operationTypeMock = new Mock<OperationType>(opTyId, 100, true, reqStaff, phases);

            var operationRequest = new Mock<OperationRequest>("TestCode", "2024-12-31", "Elective", "2024-10-25",
                staffId, "descript", patientId, opTyId);

            var removeDto = new AddOrRemoveFromPatientDto(patientId, operationRequest.Object.Id.Value);

            var userMock = new Mock<User>();
            userMock.Setup(u => u.Id).Returns(patientId);
            userMock.Setup(u => u.UserName).Returns(email);
            userMock.Setup(u => u.Email).Returns(email);
            userMock.Setup(u => u.Status).Returns(true);
            userMock.Setup(u => u.PasswordHash).Returns(password);

            _userManagerMock.Setup(r => r.FindByEmailAsync(email)).ReturnsAsync(userMock.Object);
            _repoStaMock.Setup(r => r.FindStaffWithUserId(patientId)).ReturnsAsync(staffMock.Object);
            _repoPatMock.Setup(repo => repo.GetByIdAsync(It.IsAny<MedicalRecordNumber>())).ReturnsAsync(patientMock.Object);
            _unitOfWorkMock.Setup(r => r.CommitAsync()).ReturnsAsync(1);

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Doctor")).Returns(false);

             _userServiceMock.Setup(_userService => _userService.DecodeJwtToken("valid-token")).Returns((emailClaim, new List<string> { "Doctor" }));

            _service.Setup( _service => _service.DeleteOperationRequestFromPatient(removeDto.PatientId, removeDto.OperationRequestId, email)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteOperationRequestFromPatient(removeDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

    }
}
