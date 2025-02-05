using MDBackoffice.Domain.Logs;
using MDBackoffice.Domain.OperationTypes.ValueObjects.Phase;
using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;
using MDBackoffice.Domain.OperationTypes;
using MDBackoffice.Domain.OperationTypesRecords;
using MDBackoffice.Domain.Shared;
using Moq;
using Xunit;
using MDBackoffice.Controllers;
using Microsoft.AspNetCore.Mvc;
using MDBackoffice.Domain.Users;
using MDBackoffice.Infrastructure.Users;
using MDBackoffice.Domain.Tokens;
using MDBackoffice.Domain.Emails;
using MDBackoffice.Infrastructure.Emails;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using MDBackoffice.Domain.Specializations;

namespace MDBackofficeTests.controllertests
{
    public class OperationTypeControllerTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<LogService> _logServiceMock = new(new Mock<IUnitOfWork>().Object, new Mock<ILogRepository>().Object);
        private readonly Mock<IOperationTypeRepository> _repoMock = new Mock<IOperationTypeRepository>();
        private readonly Mock<OperationTypeRecordService> _opRecordService;
        private readonly Mock<OperationTypeService> _service;
        private readonly Mock<UserService> _userService;
        private readonly OperationTypesController _controller;
        private readonly Mock<ILoginAdapter> _loginAdapterMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<ISpecializationRepository> _specializationRepo = new Mock<ISpecializationRepository>();


        public OperationTypeControllerTests()
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


            _opRecordService = new Mock<OperationTypeRecordService>(_unitOfWorkMock.Object, _logServiceMock.Object, new Mock<IOperationTypeRecordRepository>().Object);

            _service = new Mock<OperationTypeService>(_unitOfWorkMock.Object, _repoMock.Object, _logServiceMock.Object, _opRecordService.Object, _specializationRepo.Object);

           var tokenServiceMock = new Mock<TokenService>(_unitOfWorkMock.Object, new Mock<ITokenRepository>().Object, _userManagerMock.Object);
            var _emailServiceMock = new Mock<EmailService>(tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);
            var _configurationMock = new Mock<IConfiguration>();
            _loginAdapterMock = new Mock<ILoginAdapter>();

            _userService = new Mock<UserService>(_userManagerMock.Object, roleManagerMock.Object, _logServiceMock.Object, signinManagerMock.Object, _emailServiceMock.Object, _configurationMock.Object, tokenServiceMock.Object, _loginAdapterMock.Object);

            _controller = new OperationTypesController(_service.Object, _userService.Object);
        }

        [Fact]
        public async Task GetFilteredOperationTypes_ReturnsOkOperationTypeDtos()
        {
            // Arrange
            var queryParameters = new OperationTypeQueryParametersDto
            {
                queryFilters = new List<OperationTypeListingFilterParametersDto>
                {
                    new OperationTypeListingFilterParametersDto
                    {
                        Name = "test type 1",
                        Specialization = "25841809",
                        Status = "Active"
                    }
                }
            };

            var phasesDto = new List<PhaseDto>
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

            var reqStaffDto = new List<RequiredStaffDto>
            {
                new RequiredStaffDto{
                    StaffQuantity = 1,
                    Function = "doctor",
                    Specialization = "25841809"
                }
            };

            var operationTypes = new List<Mock<OperationType>>
            {
                new Mock<OperationType>("test type 1", 100, true, reqStaffDto,phasesDto),
            };


            var expectedDtos = new List<OperationTypeDto>
            {
                new OperationTypeDto{Name ="test type 1",EstimatedDuration = 100, Status = true,RequiredStaff =reqStaffDto,Phases = phasesDto }
            };

            var operationTypeObjects = operationTypes.Select(mock => mock.Object).ToList();
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userService.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);


            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            _repoMock.Setup(repo => repo.FilterOperationTypes(queryParameters))
                    .ReturnsAsync(operationTypeObjects);

            // Act
            var result = await _controller.GetFilteredOperationTypes(queryParameters);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualDtos = Assert.IsType<List<OperationTypeDto>>(okResult.Value);
            Assert.Equal(expectedDtos.Count, actualDtos.Count);

            for (int i = 0; i < expectedDtos.Count; i++)
            {
                Assert.Equal(expectedDtos[i].Name, actualDtos[i].Name);
                Assert.Equal(expectedDtos[i].EstimatedDuration, actualDtos[i].EstimatedDuration);
                Assert.Equal(expectedDtos[i].Status, actualDtos[i].Status);
            }

            _repoMock.Verify(repo => repo.FilterOperationTypes(queryParameters), Times.Once);
        }


        [Fact]
        public async Task RemoveOperationType_ValidId_ReturnsOkResult()
        {
            // Arrange
            var operationTypeId = "test type 1";
             var phasesDto = new List<PhaseDto>
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

            var reqStaffDto = new List<RequiredStaffDto>
            {
                new RequiredStaffDto{
                    StaffQuantity = 1,
                    Function = "doctor",
                    Specialization = "25841809"
                }
            };

            var operationType = new OperationType("test type 1", 100, true, reqStaffDto,phasesDto);

            var expectedDto = new OperationTypeDto {Name ="test type 1",EstimatedDuration = 100, Status = true,RequiredStaff = reqStaffDto,Phases = phasesDto };

            _repoMock.Setup(repo => repo.GetByNameAsync(operationTypeId)).ReturnsAsync(operationType);
           
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userService.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);

            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _controller.RemoveOperationType(operationTypeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualDto = Assert.IsType<OperationTypeDto>(okResult.Value);
            Assert.Equal(expectedDto.Name, actualDto.Name);
            Assert.Equal(expectedDto.EstimatedDuration, actualDto.EstimatedDuration);
            Assert.NotEqual(expectedDto.Status, actualDto.Status);
        }


        [Fact]
        public async Task EditOperationType_ReturnsOkResult()
        {
            // Arrange
             var phasesDto = new List<PhaseDto>
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

            var reqStaffDto = new List<RequiredStaffDto>
            {
                new RequiredStaffDto{
                    StaffQuantity = 1,
                    Function = "doctor",
                    Specialization = "25841809"
                }
            };

            var recordID = "record 1";

            var operationType = new Mock<OperationType>("test type 1", 100, true, reqStaffDto,phasesDto);
            var expectedDto = new OperationTypeDto {Name ="test type 1",EstimatedDuration = 100, Status = true,RequiredStaff = reqStaffDto,Phases = phasesDto };
            var recordDto = new OperationTypeRecordDto(recordID, 1, "2024-10-27", operationType.Object.Id.Value,
             operationType.Object.Name.OperationName, operationType.Object.EstimatedDuration.TotalDurationMinutes,
              operationType.Object.Status.Active, reqStaffDto, phasesDto);

            var editDto = new EditOpTypeDto
            {
                Id = operationType.Object.Id.Value.ToString(),
                Name = "NEW NAME",
                EstimatedDuration = 300,
                Status = true
            };


            _repoMock.Setup(r => r.GetByIdWithStaffAsync(operationType.Object.Id)).ReturnsAsync(operationType.Object);
            _opRecordService.Setup(r =>r.AddAsync(operationType.Object)).ReturnsAsync(recordDto);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userService.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);

            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);


            // Act
            var result = await _controller.EditOperationType(editDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsOperationTypeDto()
        {
            //Arrange
            var operationTypeId = "test type 1";
            var phasesDto = new List<PhaseDto>
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

            var reqStaffDto = new List<RequiredStaffDto>
            {
                new RequiredStaffDto{
                    StaffQuantity = 1,
                    Function = "doctor",
                    Specialization = "25841809"
                }
            };

            var dto = new OperationTypeDto { Name = "test type 1", EstimatedDuration = 100, Status = true, RequiredStaff = reqStaffDto, Phases = phasesDto };

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userService.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);

            _service.Setup(s => s.AddAsync(dto)).ReturnsAsync(dto);

            //Act
            var result = await _controller.Create(dto);

            //Assert
            Assert.IsType<ActionResult<OperationTypeDto>>(result);  

        }

    }
}
