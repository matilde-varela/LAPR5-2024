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
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using MDBackoffice.Domain.Tokens;
using MDBackoffice.Domain.Emails;
using MDBackoffice.Infrastructure.Emails;
using Microsoft.Extensions.Configuration;
using MDBackoffice.Infrastructure.Users;
using MDBackoffice.Domain.Specializations;

namespace MDBackofficeTests.integrationtests.operationtype
{
    public class DeletOperationTypesIntegrationTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<LogService> _logServiceMock = new(new Mock<IUnitOfWork>().Object, new Mock<ILogRepository>().Object);
        private readonly Mock<IOperationTypeRepository> _repoMock = new Mock<IOperationTypeRepository>();
        private readonly Mock<OperationTypeRecordService> _opRecordService;
        private readonly Mock<UserService> _userService;
        private readonly OperationTypeService _service;
        private readonly Mock<UserService> _userServiceMock;
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<ILoginAdapter> _loginAdapterMock;
      

        public DeletOperationTypesIntegrationTests()
        {
            Mock<LogService> _logServiceMock = new Mock<LogService>(new Mock<IUnitOfWork>().Object, new Mock<ILogRepository>().Object);

            var identityOptionsMock = new Mock<IOptions<IdentityOptions>>();
            identityOptionsMock.Setup(o => o.Value).Returns(new IdentityOptions());
            var identityErrorDescriberMock = new Mock<IdentityErrorDescriber>();

            var _userManagerMock = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, identityOptionsMock.Object, new Mock<IPasswordHasher<User>>().Object, new List<IUserValidator<User>> { new Mock<IUserValidator<User>>().Object }, new List<IPasswordValidator<User>> { new Mock<IPasswordValidator<User>>().Object }, new Mock<ILookupNormalizer>().Object, identityErrorDescriberMock.Object, new Mock<IServiceProvider>().Object, new Mock<ILogger<UserManager<User>>>().Object);

            var signinManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object,
                                                                new Mock<IHttpContextAccessor>().Object,
                                                                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                                                                identityOptionsMock.Object,
                                                                new Mock<ILogger<SignInManager<User>>>().Object,
                                                                new Mock<IAuthenticationSchemeProvider>().Object,
                                                                new Mock<IUserConfirmation<User>>().Object);

            var roleManagerMock = new Mock<RoleManager<Role>>(new Mock<IRoleStore<Role>>().Object, new List<IRoleValidator<Role>>(), new Mock<ILookupNormalizer>().Object, identityErrorDescriberMock.Object, new Mock<ILogger<RoleManager<Role>>>().Object);


            _opRecordService = new Mock<OperationTypeRecordService>(_unitOfWorkMock.Object, _logServiceMock.Object, new Mock<IOperationTypeRecordRepository>().Object);

            var _specializationRepo = new Mock<ISpecializationRepository>();

            _service = new Mock<OperationTypeService>(_unitOfWorkMock.Object, _repoMock.Object, _logServiceMock.Object, _opRecordService.Object, _specializationRepo.Object).Object;

           var tokenServiceMock = new Mock<TokenService>(_unitOfWorkMock.Object, new Mock<ITokenRepository>().Object, _userManagerMock.Object);
            var _emailServiceMock = new Mock<EmailService>(tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);
            var _configurationMock = new Mock<IConfiguration>();
            var _loginAdapterMock = new Mock<ILoginAdapter>();

            _userService = new Mock<UserService>(_userManagerMock.Object, roleManagerMock.Object, _logServiceMock.Object, signinManagerMock.Object, _emailServiceMock.Object, _configurationMock.Object, tokenServiceMock.Object, _loginAdapterMock.Object);


            _opRecordService = new Mock<OperationTypeRecordService>(_unitOfWorkMock.Object, _logServiceMock.Object, new Mock<IOperationTypeRecordRepository>().Object);

            _service = new OperationTypeService(_unitOfWorkMock.Object, _repoMock.Object, _logServiceMock.Object, _opRecordService.Object, _specializationRepo.Object);
        
        }

        [Fact]
        public async Task RemoveOperationType_ValidId_ReturnsOkResult_IntegrationControllerService()
        {
            var _controller = new OperationTypesController(_service, _userService.Object);

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
                    Specialization = "25001907"
                }
            };

            var operationType = new OperationType("test type 1", 100, true, reqStaffDto,phasesDto);

            var expectedDto = new OperationTypeDto {Name ="test type 1",EstimatedDuration = 100, Status = true,RequiredStaff = reqStaffDto,Phases = phasesDto };

            _repoMock.Setup(repo => repo.GetByNameAsync(operationTypeId)).ReturnsAsync(operationType);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userService.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);


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
        public async Task InactivateAsync_ReturnsOperationTypeDTO_IntegrationServiceDomain()
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
                    Specialization = "25001907"
                }
            };

            var operationTypeMock = new Mock<OperationType>("test type 1", 100, true, reqStaffDto,phasesDto);

            var expectedDto = new OperationTypeDto {Name ="test type 1",EstimatedDuration = 100, Status = true,RequiredStaff = reqStaffDto,Phases = phasesDto };

            _repoMock.Setup(repo => repo.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(operationTypeMock.Object);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.InactivateAsync(operationTypeId);

            // Assert 
            Assert.NotNull(result);
            Assert.Equal(expectedDto.Name, result.Name);
            Assert.Equal(expectedDto.EstimatedDuration, result.EstimatedDuration);
            Assert.NotEqual(expectedDto.Status, result.Status);
        }
    }
}