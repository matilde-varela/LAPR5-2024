using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.Emails;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.Users;
using MDBackoffice.Domain.Logs;
using MDBackoffice.Domain.Tokens;
using MDBackoffice.Infrastructure.Emails;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using MDBackoffice.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Dynamic;
using MDBackoffice.Infrastructure.Users;


namespace MDBackofficeTests.integrationtests.patient
{
    public class DeletePatientIntegrationTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IPatientRepository> _repoMock = new Mock<IPatientRepository>();
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private readonly Mock<LogService> _logServiceMock;
        private readonly Mock<UserService> _userServiceMock;
        private readonly Mock<EmailService> _emailServiceMock;
        private readonly Mock<ILoginAdapter> _loginAdapterMock;
        private readonly PatientService _service;
        private readonly Mock<IPatientMedicalRecordAdapter> _patientMRAMock;


        public DeletePatientIntegrationTests()
        {
            _logServiceMock = new Mock<LogService>(new Mock<IUnitOfWork>().Object, new Mock<ILogRepository>().Object);

            var identityOptionsMock = new Mock<IOptions<IdentityOptions>>();
            identityOptionsMock.Setup(o => o.Value).Returns(new IdentityOptions());
            var identityErrorDescriberMock = new Mock<IdentityErrorDescriber>();

            
            var userManagerMock = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, identityOptionsMock.Object, new Mock<IPasswordHasher<User>>().Object, new List<IUserValidator<User>> { new Mock<IUserValidator<User>>().Object }, new List<IPasswordValidator<User>> { new Mock<IPasswordValidator<User>>().Object }, new Mock<ILookupNormalizer>().Object, identityErrorDescriberMock.Object, new Mock<IServiceProvider>().Object, new Mock<ILogger<UserManager<User>>>().Object);
            var roleManagerMock = new Mock<RoleManager<Role>>(new Mock<IRoleStore<Role>>().Object, new List<IRoleValidator<Role>>(), new Mock<ILookupNormalizer>().Object, identityErrorDescriberMock.Object, new Mock<ILogger<RoleManager<Role>>>().Object);

            var tokenServiceMock = new Mock<TokenService>(_unitOfWorkMock.Object, new Mock<ITokenRepository>().Object, userManagerMock.Object);
            var _emailServMock = new Mock<EmailService>(tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);
            var _configurationMock = new Mock<IConfiguration>();
            _loginAdapterMock = new Mock<ILoginAdapter>();
            var signinManagerMock = new Mock<SignInManager<User>>(userManagerMock.Object,
                                                               new Mock<IHttpContextAccessor>().Object,
                                                               new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                                                               identityOptionsMock.Object,
                                                               new Mock<ILogger<SignInManager<User>>>().Object,
                                                               new Mock<IAuthenticationSchemeProvider>().Object,
                                                               new Mock<IUserConfirmation<User>>().Object);

            _userServiceMock = new Mock<UserService>(userManagerMock.Object, roleManagerMock.Object, _logServiceMock.Object, signinManagerMock.Object ,_emailServMock.Object, _configurationMock.Object, tokenServiceMock.Object, _loginAdapterMock.Object);
            _emailServiceMock = new Mock<EmailService>(tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);
            _patientMRAMock = new Mock<IPatientMedicalRecordAdapter>();

         _service = new PatientService(_unitOfWorkMock.Object, _logServiceMock.Object,
                                         _configurationMock.Object, _repoMock.Object,
                                         _userServiceMock.Object, _emailServiceMock.Object, _patientMRAMock.Object);
        
        }
        

    [Fact]
    public async Task DeletePatientProfile_ReturnsOkResponse_IntegrationControllerService()
    {
        // Arrange
       var _controller = new PatientController(_service,_userServiceMock.Object);

        var dtoMock = new EditPatientDto
        ("Rita Barbosa",
            "+351 910000000",
            "ritabarbosa@email.com",
            "Test, 1234-234, Test Test",
            "2004-12-15");

        var patientMock = new Mock<Patient>("first", "last", "first last", "country, 12345, street test", "female", "+123", "12345678", "98765432", "email@email.com", "2000-10-10", "000001");
        var id = "202410000001";
        var idDto = new IdPassDto(id);

        var dtoResult = new PatientDto("Rita Barbosa", "+351 910000000", "ritabarbosa@email.com", "Test, 1234-234, Test Test", "2004-12-15", id);
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);
            _repoMock.Setup(r => r.ExistsPatientWithId(id)).ReturnsAsync(true);
        _repoMock.Setup(_repoPatMock => _repoPatMock.GetByIdAsync(It.IsAny<MedicalRecordNumber>()))
            .ReturnsAsync(patientMock.Object);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);
        patientMock.Setup(p => p.Anonymize()).Returns(true);
        _userServiceMock.Setup(s => s.DeleteByIdAsync(patientMock.Object.UserReference)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.DeletePatientProfile(idDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("{ message = Patient profile and account succefully deleted. }", okResult.Value.ToString());
    }

    [Fact]
    public async Task DeletePatientProfile_ReturnsBadRequest_IntegrationControllerService()
    {
        // Arrange
        var _controller = new PatientController(_service, _userServiceMock.Object);

        var idPassDto = new IdPassDto( "202410000001" );
        _repoMock.Setup(s => s.ExistsPatientWithId(idPassDto.Id))
            .ThrowsAsync(new BusinessRuleValidationException("There is no patient profile with the given Id."));

        // Act
        var result = await _controller.DeletePatientProfile(idPassDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        //Assert.Equal("There is no patient profile with the given Id.", badRequestResult);
    }
    

    [Fact]
    public async Task DeletePatientProfile_ReturnsOkResponse_IntegrationServiceDomain()
    {
        // Arrange
        var dtoMock = new EditPatientDto
        ("Rita Barbosa",
            "+351 910000000",
            "ritabarbosa@email.com",
            "Test, 1234-234, Test Test",
            "2004-12-15");

        var patientMock = new Mock<Patient>("first", "last", "first last", "country, 12345, street test", "female", "+123", "12345678", "98765432", "email@email.com", "2000-10-10", "000001");
        var id = "202410000001";
        var idDto = new IdPassDto(id);

        var dtoResult = new PatientDto("Rita Barbosa", "+351 910000000", "ritabarbosa@email.com", "Test, 1234-234, Test Test", "2004-12-15", id);

        _repoMock.Setup(r => r.ExistsPatientWithId(id)).ReturnsAsync(true);
        _repoMock.Setup(_repoPatMock => _repoPatMock.GetByIdAsync(It.IsAny<MedicalRecordNumber>()))
            .ReturnsAsync(patientMock.Object);
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);
        patientMock.Setup(p => p.Anonymize()).Returns(true);

        _userServiceMock.Setup(s => s.DeleteByIdAsync(patientMock.Object.UserReference)).ReturnsAsync(IdentityResult.Success);

        // Act
        await _service.DeletePatientProfile(id);

        // Assert
        _repoMock.Verify(um => um.ExistsPatientWithId(id), Times.Once);
        _repoMock.Verify(um => um.GetByIdAsync(new MedicalRecordNumber(id)), Times.Once);
        _userServiceMock.Verify(um => um.DeleteByIdAsync(patientMock.Object.UserReference), Times.Once);
        _logServiceMock.Verify(um => um.CreateDeletionLog(patientMock.Object.UserReference, "MDBackoffice.Domain.Users", "Deletion of patient's account."), Times.Once);
    }

    [Fact]
    public async Task DeletePatientProfile_ReturnsBadRequest_IntegrationServiceDomain()
    {
        // Arrange
                // Arrange
        var dtoMock = new EditPatientDto
        ("Rita Barbosa",
            "+351 910000000",
            "ritabarbosa@email.com",
            "Test, 1234-234, Test Test",
            "2004-12-15");

        var patientMock = new Mock<Patient>("first", "last", "first last", "country, 12345, street test", "female", "+123", "12345678", "98765432", "email@email.com", "2000-10-10", "000001");
        var id = "202410000001";
        var idPassDto = new IdPassDto(id);

        var dtoResult = new PatientDto("Rita Barbosa", "+351 910000000", "ritabarbosa@email.com", "Test, 1234-234, Test Test", "2004-12-15", id);

        _repoMock.Setup(r => r.ExistsPatientWithId(id)).ReturnsAsync(false);

        // Act
        var exception = await Assert.ThrowsAsync<BusinessRuleValidationException>(async () => await _service.DeletePatientProfile(id));

        // Assert
        Assert.Equal("There is no patient profile with the given Id.", exception.Message);
    
        _repoMock.Verify(um => um.ExistsPatientWithId(id), Times.Once);
        _repoMock.Verify(um => um.GetByIdAsync(It.IsAny<MedicalRecordNumber>()), Times.Never);
        _userServiceMock.Verify(um => um.DeleteByIdAsync(It.IsAny<string>()), Times.Never);
    }


    }
}
