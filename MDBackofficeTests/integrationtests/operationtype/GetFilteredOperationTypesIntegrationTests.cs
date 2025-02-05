using MDBackoffice.Controllers;
using MDBackoffice.Domain.Logs;
using MDBackoffice.Domain.OperationTypes.ValueObjects.Phase;
using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;
using MDBackoffice.Domain.OperationTypes;
using MDBackoffice.Domain.OperationTypesRecords;
using MDBackoffice.Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using MDBackoffice.Domain.Users;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using Microsoft.AspNetCore.Identity;
using MDBackoffice.Infrastructure.Users;
using Microsoft.Extensions.Options;
using MDBackoffice.Domain.Tokens;
using MDBackoffice.Domain.Emails;
using MDBackoffice.Infrastructure.Emails;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using MDBackoffice.Domain.Specializations;

namespace MDBackofficeTests.integrationtests.operationtype
{
    public class GetFilteredOperationTypesIntegrationTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IOperationTypeRepository> _repoMock = new Mock<IOperationTypeRepository>();
        private readonly Mock<OperationTypeRecordService> _opRecordService;
        private readonly OperationTypeService _service;
        private readonly Mock<UserService> _userService;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<ILoginAdapter> _loginAdapterMock;

        public GetFilteredOperationTypesIntegrationTests()
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

           var tokenServiceMock = new Mock<TokenService>(_unitOfWorkMock.Object, new Mock<ITokenRepository>().Object, _userManagerMock.Object);
            var _emailServiceMock = new Mock<EmailService>(tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);
            var _configurationMock = new Mock<IConfiguration>();
            _loginAdapterMock = new Mock<ILoginAdapter>();

            _userService = new Mock<UserService>(_userManagerMock.Object, roleManagerMock.Object, _logServiceMock.Object, signinManagerMock.Object, _emailServiceMock.Object, _configurationMock.Object, tokenServiceMock.Object, _loginAdapterMock.Object);

            _opRecordService = new Mock<OperationTypeRecordService>(_unitOfWorkMock.Object, _logServiceMock.Object, new Mock<IOperationTypeRecordRepository>().Object);

           var _specializationRepo = new Mock<ISpecializationRepository>();

            _service = new OperationTypeService(_unitOfWorkMock.Object, _repoMock.Object, _logServiceMock.Object, _opRecordService.Object, _specializationRepo.Object);
        
        }

        [Fact]
        public async Task GetFilteredOperationTypes_ReturnsOkOperationTypeDtos_IntegrationControllerService()
        {
            // Arrange
            var _controller = new OperationTypesController(_service, _userService.Object);

            var queryParameters = new OperationTypeQueryParametersDto
            {
                queryFilters = new List<OperationTypeListingFilterParametersDto>
                {
                    new OperationTypeListingFilterParametersDto
                    {
                        Name = "test type 1",
                        Specialization = "25001907",
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
                    Specialization = "25001907"
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
            _repoMock.Setup(repo => repo.FilterOperationTypes(queryParameters))
                    .ReturnsAsync(operationTypeObjects);

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userService.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);

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
        public async Task FilterOperationTypes_ReturnsOperationTypeDtos_IntegrationServiceDomain()
        {
            // Arrange
            var queryParameters = new OperationTypeQueryParametersDto
            {
                queryFilters = new List<OperationTypeListingFilterParametersDto>
                {
                    new OperationTypeListingFilterParametersDto
                    {
                        Name = "test type 1",
                        Specialization = "25001907",
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
                    Specialization = "25001907"
                }
            };
            var operationTypes = new List<OperationType>
            {
                new OperationType("test type 1", 100, true, reqStaffDto,phasesDto),
            };
            var expectedDtos = new List<OperationTypeDto>
            {
                new OperationTypeDto{Name ="test type 1",EstimatedDuration = 100, Status = true,RequiredStaff =reqStaffDto,Phases = phasesDto }
            };

            _repoMock.Setup(repo => repo.FilterOperationTypes(queryParameters))
                    .ReturnsAsync(operationTypes);

            // Act
            var result = await _service.FilterOperationTypes(queryParameters);

            // Assert
            Assert.Equal(expectedDtos.Count, result.Count);
            for (int i = 0; i < expectedDtos.Count; i++)
            {
                Assert.Equal(expectedDtos[i].Name, result[i].Name);
                Assert.Equal(expectedDtos[i].EstimatedDuration, result[i].EstimatedDuration);
                Assert.Equal(expectedDtos[i].Status, result[i].Status);

            }
            _repoMock.Verify(repo => repo.FilterOperationTypes(queryParameters), Times.Once);
        }
    }
}
