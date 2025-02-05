using MDBackoffice.Controllers;
using MDBackoffice.Domain.Emails;
using MDBackoffice.Domain.Logs;
using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.Shared;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MDBackofficeTests.integrationtests.patient
{
    public class FilteredPatientProfilesIntegrationTests
    {

        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IPatientRepository> _repoMock = new Mock<IPatientRepository>();
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private readonly Mock<LogService> _logServiceMock;
        private readonly Mock<UserService> _userServiceMock;
        private readonly Mock<EmailService> _emailServiceMock;
        private readonly Mock<ILoginAdapter> _loginAdapterMock;
        private readonly Mock<IPatientMedicalRecordAdapter> _patientMRAMock;


        public FilteredPatientProfilesIntegrationTests()
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

            _userServiceMock = new Mock<UserService>(userManagerMock.Object, roleManagerMock.Object, _logServiceMock.Object, signinManagerMock.Object, _emailServMock.Object, _configurationMock.Object, tokenServiceMock.Object, _loginAdapterMock.Object);
            _emailServiceMock = new Mock<EmailService>(tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);
            _patientMRAMock = new Mock<IPatientMedicalRecordAdapter>();
        }

        [Fact]
        public async Task GetFilteredPatientProfiles_ReturnsPatientDtoList_IntegrationControllerService()
        {
            // Arrange
            var _service = new PatientService(_unitOfWorkMock.Object, _logServiceMock.Object,
                                            _configurationMock.Object, _repoMock.Object,
                                            _userServiceMock.Object, _emailServiceMock.Object, _patientMRAMock.Object);

            var _controller = new PatientController(_service, _userServiceMock.Object);

            string firstName = "Duarte";
            string lastName = "Matos";
            string email = "exampleemail@gmail.com";
            string gender = "Male";
            string date = "2004-12-15";
            string medicalRecordNumber = "202410000001";

            PatientListingFilterParametersDto listingFilterParametersDto
                = new PatientListingFilterParametersDto(
                    firstName,
                    lastName,
                    email,
                    gender,
                    date,
                    medicalRecordNumber);

            List<PatientListingFilterParametersDto> listingFilterParametersDtosList = new List<PatientListingFilterParametersDto>
            {
                listingFilterParametersDto
            };

            PatientQueryParametersDto dto = new PatientQueryParametersDto(listingFilterParametersDtosList);
            var id = "202410000001";

            List<PatientDto> result = new List<PatientDto>();
            var dtoResult = new PatientDto("Rita Barbosa", "+351 910000000", "ritabarbosa@email.com", "Test, 1234-234, Test Test", "2004-12-15", id);

            result.Add(dtoResult);

            var patientMock = new Mock<Patient>("Duarte", "Matos", "Duarte Matos", "country, 12345, street test", "male", "+123", "12345678", "98765432", "exampleemail@gmail.com", "2004-12-15", "000001");

            List<Patient> resultList = new List<Patient> { patientMock.Object };
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService =>
                    _userService.CheckUserRole("valid-token", It.Is<string>(role => role == "Admin" || role == "Doctor")))
                    .Returns(true);
            _repoMock.Setup(_repo => _repo.FilterPatientProfiles(dto)).ReturnsAsync(resultList);

            //Act
            var resultController = await _controller.GetFilteredPatientProfiles(dto);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(resultController.Result);
            var returnedPatient = Assert.IsType<List<PatientDto>>(okResult.Value);
        }

        [Fact]
        public async Task FilterPatientProfiles_ReturnsPatientListDto_IntegrationServiceDomain()
        {
            //Arrange
            var _service = new PatientService(_unitOfWorkMock.Object, _logServiceMock.Object,
                                            _configurationMock.Object, _repoMock.Object,
                                            _userServiceMock.Object, _emailServiceMock.Object, _patientMRAMock.Object);

            string firstName = "Duarte";
            string lastName = "Matos";
            string email = "exampleemail@gmail.com";
            string gender = "male";
            string date = "2004-12-15";
            string medicalRecordNumber = "202410000001";

            PatientListingFilterParametersDto listingFilterParametersDto
                = new PatientListingFilterParametersDto(
                    firstName,
                    lastName,
                    email,
                    gender,
                    date,
                    medicalRecordNumber);

            List<PatientListingFilterParametersDto> listingFilterParametersDtosList = new List<PatientListingFilterParametersDto>
            {
                listingFilterParametersDto
            };

            PatientQueryParametersDto dto = new PatientQueryParametersDto(listingFilterParametersDtosList);
            var id = "202410000001";

            var patientMock = new Patient("Duarte", "Matos", "Duarte Matos", "country, 12345, street test", "male", "+123", "12345678", "98765432", "exampleemail@gmail.com", "2004-12-15", "000001");

            List<Patient> result = new List<Patient> { patientMock };

            _repoMock.Setup(_repo => _repo.FilterPatientProfiles(dto)).ReturnsAsync(result);

            // Act
            var patients = await _service.FilterPatientProfiles(dto);

            // Assert
            Assert.NotNull(patients);
            Assert.IsType<List<PatientDto>>(patients);
            Assert.Single(patients);
        }

    }
}
