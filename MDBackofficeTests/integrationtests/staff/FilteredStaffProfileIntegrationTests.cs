using MDBackoffice.Controllers;
using MDBackoffice.Domain.Emails;
using MDBackoffice.Domain.Logs;
using MDBackoffice.Domain.Patients;
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
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MDBackofficeTests.integrationtests.patient
{
    public class FilteredStaffProfilesIntegrationTests
    {

        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IStaffRepository> _repoMock = new Mock<IStaffRepository>();
        private readonly Mock<ISpecializationRepository> _repoSpecMock = new Mock<ISpecializationRepository>();
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private readonly Mock<LogService> _logServiceMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<UserService> _userServiceMock;
        private readonly Mock<EmailService> _emailServiceMock;
        private readonly StaffService _service;
        private readonly Mock<ILoginAdapter> _loginAdapterMock;

        public FilteredStaffProfilesIntegrationTests()
        {
            _logServiceMock = new Mock<LogService>(new Mock<IUnitOfWork>().Object, new Mock<ILogRepository>().Object);

            var identityOptionsMock = new Mock<IOptions<IdentityOptions>>();
            identityOptionsMock.Setup(o => o.Value).Returns(new IdentityOptions());
            var identityErrorDescriberMock = new Mock<IdentityErrorDescriber>();

            _userManagerMock = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, identityOptionsMock.Object, new Mock<IPasswordHasher<User>>().Object, new List<IUserValidator<User>> { new Mock<IUserValidator<User>>().Object }, new List<IPasswordValidator<User>> { new Mock<IPasswordValidator<User>>().Object }, new Mock<ILookupNormalizer>().Object, identityErrorDescriberMock.Object, new Mock<IServiceProvider>().Object, new Mock<ILogger<UserManager<User>>>().Object);
            var roleManagerMock = new Mock<RoleManager<Role>>(new Mock<IRoleStore<Role>>().Object, new List<IRoleValidator<Role>>(), new Mock<ILookupNormalizer>().Object, identityErrorDescriberMock.Object, new Mock<ILogger<RoleManager<Role>>>().Object);
            _loginAdapterMock = new Mock<ILoginAdapter>();
            var tokenServiceMock = new Mock<TokenService>(_unitOfWorkMock.Object, new Mock<ITokenRepository>().Object, _userManagerMock.Object);
            var _emailServMock = new Mock<EmailService>(tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);

            var signinManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object,
                                                               new Mock<IHttpContextAccessor>().Object,
                                                               new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                                                               identityOptionsMock.Object,
                                                               new Mock<ILogger<SignInManager<User>>>().Object,
                                                               new Mock<IAuthenticationSchemeProvider>().Object,
                                                               new Mock<IUserConfirmation<User>>().Object);

            _userServiceMock = new Mock<UserService>(_userManagerMock.Object, roleManagerMock.Object, _logServiceMock.Object, signinManagerMock.Object, _emailServMock.Object, _configurationMock.Object, tokenServiceMock.Object, _loginAdapterMock.Object);
            _emailServiceMock = new Mock<EmailService>(tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);

            _service = new StaffService(_unitOfWorkMock.Object, _logServiceMock.Object,
                                    _repoMock.Object, _repoSpecMock.Object,
                                    _userManagerMock.Object, _configurationMock.Object, _emailServiceMock.Object,
                                    _userServiceMock.Object);
        }

        [Fact]
        public async Task GetFilteredStaffProfiles_ReturnsStaffDtoList_IntegrationControllerService()
        {
            // Arrange
            var _service = new StaffService(_unitOfWorkMock.Object, _logServiceMock.Object,
                                            _repoMock.Object,
                                            _repoSpecMock.Object, _userManagerMock.Object, _configurationMock.Object, _emailServiceMock.Object,
                                            _userServiceMock.Object);

            var _controller = new StaffController(_service, _userServiceMock.Object);

            string firstName = "Duarte";
            string lastName = "Matos";
            string email = "exampleemail@gmail.com";
            string specialization = "Cardiology";

            StaffListingFilterParametersDto listingFilterParametersDto
                = new StaffListingFilterParametersDto(
                    firstName,
                    lastName,
                    email,
                    specialization);

            List<StaffListingFilterParametersDto> listingFilterParametersDtosList = new List<StaffListingFilterParametersDto>
            {
                listingFilterParametersDto
            };

            StaffQueryParametersDto dto = new StaffQueryParametersDto(listingFilterParametersDtosList);
            var id = "202410000001";

            List<StaffDto> result = new List<StaffDto>();
            var dtoResult = new StaffDto("Rita Barbosa", "+351 910000000", "ritabarbosa@email.com", "Test, 1234-234, Test Test", "2004-12-15", id);

            result.Add(dtoResult);

            var staffMock = new Mock<Staff>("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", email, "+351", "987654321", "Doctor", "251010299");

            List<Staff> resultList = new List<Staff> { staffMock.Object };

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);


            _repoMock.Setup(_repo => _repo.FilterStaffProfiles(dto)).ReturnsAsync(resultList);

            //Act
            var resultController = await _controller.GetFilteredStaffProfiles(dto);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(resultController.Result);
            var returnedPatient = Assert.IsType<List<StaffDto>>(okResult.Value);
        }

        [Fact]
        public async Task FilterStaffProfiles_ReturnsStaffListDto_IntegrationServiceDomain()
        {
            //Arrange
            var _service = new StaffService(_unitOfWorkMock.Object, _logServiceMock.Object,
                                            _repoMock.Object,
                                            _repoSpecMock.Object, _userManagerMock.Object, _configurationMock.Object, _emailServiceMock.Object,
                                            _userServiceMock.Object);

            string firstName = "Duarte";
            string lastName = "Matos";
            string email = "exampleemail@gmail.com";
            string specialization = "Cardiology";

            StaffListingFilterParametersDto listingFilterParametersDto
                = new StaffListingFilterParametersDto(
                    firstName,
                    lastName,
                    email,
                    specialization);

            List<StaffListingFilterParametersDto> listingFilterParametersDtosList = new List<StaffListingFilterParametersDto>
            {
                listingFilterParametersDto
            };

            StaffQueryParametersDto dto = new StaffQueryParametersDto(listingFilterParametersDtosList);
            var id = "202410000001";

            var staffMock = new Staff("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", email, "+351", "987654321", "Doctor", "251010299");

            List<Staff> result = new List<Staff> { staffMock };

            _repoMock.Setup(_repo => _repo.FilterStaffProfiles(dto)).ReturnsAsync(result);

            // Act
            var staffs = await _service.FilterStaffProfiles(dto);

            // Assert
            Assert.NotNull(staffs);
            Assert.IsType<List<StaffDto>>(staffs);
            Assert.Single(staffs);
        }

    }
}
