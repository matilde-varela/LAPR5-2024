using MDBackoffice.Controllers;
using MDBackoffice.Domain.Emails;
using MDBackoffice.Domain.Logs;
using MDBackoffice.Domain.OperationTypes;
using MDBackoffice.Domain.OperationTypes.ValueObjects.Phase;
using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;
using MDBackoffice.Domain.OperationTypesRecords;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using Moq;

using Xunit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using MDBackoffice.Domain.Tokens;
using MDBackoffice.Infrastructure.Emails;
using Microsoft.Extensions.Configuration;
using MDBackoffice.Infrastructure.Users;
using MDBackoffice.Domain.Specializations;
using Microsoft.OpenApi.Any;

namespace MDBackofficeTests.integrationtests.operationtype
{
    public class CreateOperationTypeIntegrationTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<LogService> _logServiceMock = new(new Mock<IUnitOfWork>().Object, new Mock<ILogRepository>().Object);
        private readonly Mock<IOperationTypeRepository> _repoMock = new Mock<IOperationTypeRepository>();
        private readonly Mock<OperationTypeRecordService> _opRecordService;
        private readonly OperationTypeService _service;
        private readonly Mock<UserService> _userService;
        private readonly Mock<ISpecializationRepository> _specializationRepo = new Mock<ISpecializationRepository>();
        public CreateOperationTypeIntegrationTests()
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

           var tokenServiceMock = new Mock<TokenService>(_unitOfWorkMock.Object, new Mock<ITokenRepository>().Object, _userManagerMock.Object);
            var _emailServiceMock = new Mock<EmailService>(tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);
            var _configurationMock = new Mock<IConfiguration>();
            var _loginAdapterMock = new Mock<ILoginAdapter>();

            _userService = new Mock<UserService>(_userManagerMock.Object, roleManagerMock.Object, _logServiceMock.Object, signinManagerMock.Object, _emailServiceMock.Object, _configurationMock.Object, tokenServiceMock.Object, _loginAdapterMock.Object);


            _opRecordService = new Mock<OperationTypeRecordService>(_unitOfWorkMock.Object, _logServiceMock.Object, new Mock<IOperationTypeRecordRepository>().Object);

            _service = new OperationTypeService(_unitOfWorkMock.Object, _repoMock.Object, _logServiceMock.Object, _opRecordService.Object, _specializationRepo.Object);
        
        }

        [Fact]
        public async Task Create_ReturnsOperationTypeDto_ControllerServiceIntegrationTest()
        {

            var _controller = new OperationTypesController(_service, _userService.Object);
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
                    Specialization = "25001907"
                }
            };

            var Specialization1 = new Specialization("25001907","Something","Something");
         

            var dto = new OperationTypeDto { Name = "test type 1", EstimatedDuration = 100, Status = true, RequiredStaff = reqStaffDto, Phases = phasesDto };
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userService.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);
            _specializationRepo.Setup(u => u.FindByDenomination("25001907")).ReturnsAsync(Specialization1);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            //Act
            var result = await _controller.Create(dto);

            //Assert
            Assert.IsType<ActionResult<OperationTypeDto>>(result);

        }

        [Fact]
        public async Task AddAsync_ReturnsOperationDto_ServiceDomainIntegrationTest()
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
                    var dto = new OperationTypeDto { Name = "test type 1", EstimatedDuration = 100, Status = true, RequiredStaff = reqStaffDto, Phases = phasesDto };
                    _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

                    var Specialization1 = new Specialization("25001907","Something","Something");
                     _specializationRepo.Setup(u => u.FindByDenomination("25001907")).ReturnsAsync(Specialization1);
           
                    // Act
                    var result = await _service.AddAsync(dto); // Await the result

                    // Assert
                    Assert.IsType<OperationTypeDto>(result); // Now this checks the correct type
                }


        }

}
