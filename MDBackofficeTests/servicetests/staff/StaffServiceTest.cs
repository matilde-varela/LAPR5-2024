using MDBackoffice.Domain.Logs;
using MDBackoffice.Domain.Emails;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.Specializations;
using MDBackoffice.Domain.Users;
using MDBackoffice.Domain.Tokens;
using MDBackoffice.Infrastructure.Emails;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using MDBackoffice.Domain.StaffProfiles;
using MDBackoffice.Domain.Patients;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using MDBackoffice.Infrastructure.Users;

namespace MDBackofficeTests.servicetests.staff;

public class StaffServiceTest
{
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IStaffRepository> _repoMock = new Mock<IStaffRepository>();
        private readonly Mock<LogService> _logServiceMock = new Mock<LogService>(new Mock<IUnitOfWork>().Object, new Mock<ILogRepository>().Object);
        private readonly Mock<ISpecializationRepository> _repoSpecMock = new Mock<ISpecializationRepository>();
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<UserService> _userServiceMock;
        private readonly Mock<EmailService> _emailServiceMock;
        private readonly StaffService _service;
        private readonly Mock<TokenService> _tokenServiceMock;
        private readonly Mock<ILoginAdapter> _loginAdapterMock;

        public StaffServiceTest()
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

                _loginAdapterMock = new Mock<ILoginAdapter>();
                _tokenServiceMock = new Mock<TokenService>(_unitOfWorkMock.Object, new Mock<ITokenRepository>().Object, _userManagerMock.Object);
                var _emailServMock = new Mock<EmailService>(_tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);
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
                    _emailServMock.Object,
                    _configurationMock.Object,
                    _tokenServiceMock.Object,
                    _loginAdapterMock.Object 
                );

                _emailServiceMock = new Mock<EmailService>(_tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);
                _service = new StaffService(_unitOfWorkMock.Object, _logServiceMock.Object,
                                             _repoMock.Object, _repoSpecMock.Object,
                                             _userManagerMock.Object, _configurationMock.Object, _emailServiceMock.Object,
                                             _userServiceMock.Object);
        }


        [Fact]
        public async Task CreateStaffProfile_ReturnsOkResult()
        {
            // Arrange
            var specializationId = "25841809";
            var dtoMock = new CreatingStaffDto
            ("12345",
                "Portugal, 4590-850, Rua da Sardinha",
                "Rita",
                "Barbosa",
                "+351 910000022",
                "something@email.com",
                "doctor",
                specializationId
                );

        var specializationMock = new Mock<Specialization>("25841809", "denom", "descrip");

        _repoSpecMock.Setup(_repoSpecMock => _repoSpecMock.FindByDenomination(It.IsAny<string>()))
                .ReturnsAsync(specializationMock.Object);

            // Act
            var result = await _service.CreateStaffProfile(dtoMock);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(specializationId, result.SpecializationId);
            Assert.Equal(dtoMock.Phone, result.Phone);
            Assert.Equal(dtoMock.Email, result.Email);
            Assert.Equal(dtoMock.Address, result.Address);
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Staff>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);

        }

        [Fact]
        public async Task UpdateAsync_ReturnsStaffDto()
        {
            //Arrange
            var dtoMock = new EditStaffDto
            ("+351 910000011",
              "test@email.com",
              "New, 1234-234, Updated",
              "25841201");

            var email = "ritabarbosa@email.com";
            var userid = "testid";
            var token = "test-token";
            var userMock = new Mock<User>();
            userMock.Setup(u => u.Id).Returns(userid);
            userMock.Setup(u => u.UserName).Returns(email);
            userMock.Setup(u => u.Email).Returns(email);
            userMock.Setup(u => u.Status).Returns(true);

            var staffMock = new Mock<Staff>("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa",email, "+351", "987654321", "Doctor", "251010300");
            var id = "D202400001";

            var dtoResult = new StaffDto("Rita Barbosa", "+351 910000011", "test@email.com", "New, 1234-234, Updated", id, "25841201");

            _repoMock.Setup(_repoPatMock => _repoPatMock.GetByIdAsync(It.IsAny<StaffId>()))
                .ReturnsAsync(staffMock.Object);
            _userManagerMock.Setup(_userManagerMock => _userManagerMock.FindByEmailAsync(email)).ReturnsAsync(userMock.Object);
            _userManagerMock.Setup(um => um.GetRolesAsync(userMock.Object)).ReturnsAsync(["Doctor"]);
            _userManagerMock.Setup(um => um.GenerateChangeEmailTokenAsync(userMock.Object, "test@email.com")).ReturnsAsync(token);
            _userManagerMock.Setup(um => um.GenerateEmailConfirmationTokenAsync(userMock.Object)).ReturnsAsync(token);
            _userManagerMock.Setup(um => um.ChangeEmailAsync(userMock.Object, "test@email.com", token)).ReturnsAsync(IdentityResult.Success);
            _configurationMock.Setup(c => c["App:Email"]).Returns("testemail@email.com");
            _configurationMock.Setup(c => c["App:BaseUrl"]).Returns("https://test/api");
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            //Act
            var result = await _service.UpdateAsync(id, dtoMock);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(dtoResult.Name, result.Name);
            Assert.Equal(dtoResult.Phone, result.Phone);
            Assert.Equal(dtoResult.Email, result.Email);
            Assert.Equal(dtoResult.Address, result.Address);
            Assert.Equal(dtoResult.SpecializationId, result.SpecializationId);
        }

    [Fact]
    public async Task GetAllAsync_ReturnsStaffListDto()
    {
        // Arrange
        var email = "ritabarbosa@email.com";
        var staffMock = new Mock<Staff>("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", email, "+351", "987654321", "Doctor", "251010300");

        List<Staff> result = new List<Staff> { staffMock.Object };

        _repoMock.Setup(_repo => _repo.GetAllAsync()).ReturnsAsync(result);

        // Act
        var staffs = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(staffs);
        Assert.IsType<List<StaffDto>>(staffs);
        Assert.Single(staffs);
    }

    [Fact]
    public async Task FilterPatientProfiles_ReturnsPatientListDto()
    {
        //Arrange
        string firstName = "Duarte";
        string lastName = "Matos";
        string email = "exampleemail@gmail.com";
        string specialization = "25841201";

        StaffListingFilterParametersDto listingFilterParametersDto
            = new StaffListingFilterParametersDto(
                firstName,
                lastName,
                email,
                specialization
                );

        List<StaffListingFilterParametersDto> listingFilterParametersDtosList = new List<StaffListingFilterParametersDto>
            {
                listingFilterParametersDto
            };

        StaffQueryParametersDto dto = new StaffQueryParametersDto(listingFilterParametersDtosList);
        var id = "202410000001";

        var staffMock = new Mock<Staff>("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", email, "+351", "987654321", "Doctor", "251010300");

        List<Staff> result = new List<Staff> { staffMock.Object };

        _repoMock.Setup(_repo => _repo.FilterStaffProfiles(dto)).ReturnsAsync(result);

        // Act
        var staffs = await _service.FilterStaffProfiles(dto);

        // Assert
        Assert.NotNull(staffs);
        Assert.IsType<List<StaffDto>>(staffs);
        Assert.Single(staffs);
    }

     [Fact]
    public async Task DeactivateStaffProfile_ReturnsOkResult()
    {
        // Arrange
        string email = "exampleemail@gmail.com";

        var staffMock = new Mock<Staff>("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", email, "+351", "987654321", "Doctor", "251010300");
        var id = "D202400001";
        

        var specializationMock = new Mock<Specialization>("Ortopethics");

        _repoMock.Setup(_repoPatMock => _repoPatMock.GetByIdAsync(It.IsAny<StaffId>()))
            .ReturnsAsync(staffMock.Object);
        staffMock.Setup(r => r.DeactivateProfile());
        _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        // Act
        var result = await _service.DeactivateStaffProfile(id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ConfirmEmailStaff_Sucessful()
    {
        // Arrange
        var email = "ritabarbosa@email.com";
        var userid = "testid";
        var userId = "testUserId";
        var token = "validToken";
        var userMock = new Mock<User>();
        userMock.Setup(u => u.Id).Returns(userid);
        userMock.Setup(u => u.UserName).Returns(email);
        userMock.Setup(u => u.Email).Returns(email);
        userMock.Setup(u => u.Status).Returns(true);

        var staffId = "D202400001";

        var staffMock = new Mock<Staff>("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", email, "+351", "987654321", "Doctor", "251010300");

        _userManagerMock.Setup(repo => repo.FindByIdAsync(userId)).ReturnsAsync(userMock.Object);
        _userManagerMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>())).Returns(Task.FromResult(IdentityResult.Success));
        _tokenServiceMock.Setup(t => t.ConfirmEmailToken(userId, token)).ReturnsAsync(true);
        _repoMock.Setup(_repoPatMock => _repoPatMock.GetByIdAsync(It.IsAny<StaffId>()))
                .ReturnsAsync(staffMock.Object);

        // Act
        await _service.ConfirmEmailStaff(userId, staffId, token);

        // Assert
        Assert.True(userMock.Object.Status);
        _userManagerMock.Verify(repo => repo.UpdateAsync(It.Is<User>(u => u == userMock.Object)), Times.Once);
    }
}