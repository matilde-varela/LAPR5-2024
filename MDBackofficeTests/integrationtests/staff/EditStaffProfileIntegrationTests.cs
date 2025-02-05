using MDBackoffice.Controllers;
using MDBackoffice.Domain.Emails;
using MDBackoffice.Domain.Logs;
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

namespace MDBackofficeTests.integrationtests.staff
{
    public class EditStaffProfileIntegrationTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IStaffRepository> _repoMock = new Mock<IStaffRepository>();
        private readonly Mock<ISpecializationRepository> _repoSpecMock = new Mock<ISpecializationRepository>();
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private readonly Mock<LogService> _logServiceMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<UserService> _userServiceMock;
        private readonly Mock<EmailService> _emailServiceMock;
        private readonly Mock<ILoginAdapter> _loginAdapterMock;
        private readonly StaffService _service;
        private readonly Mock<TokenService> tokenServiceMock;

        public EditStaffProfileIntegrationTests()
        {
            _logServiceMock = new Mock<LogService>(new Mock<IUnitOfWork>().Object, new Mock<ILogRepository>().Object);

            var identityOptionsMock = new Mock<IOptions<IdentityOptions>>();
            identityOptionsMock.Setup(o => o.Value).Returns(new IdentityOptions());
            var identityErrorDescriberMock = new Mock<IdentityErrorDescriber>();

            _userManagerMock = new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, identityOptionsMock.Object, new Mock<IPasswordHasher<User>>().Object, new List<IUserValidator<User>> { new Mock<IUserValidator<User>>().Object }, new List<IPasswordValidator<User>> { new Mock<IPasswordValidator<User>>().Object }, new Mock<ILookupNormalizer>().Object, identityErrorDescriberMock.Object, new Mock<IServiceProvider>().Object, new Mock<ILogger<UserManager<User>>>().Object);
            var roleManagerMock = new Mock<RoleManager<Role>>(new Mock<IRoleStore<Role>>().Object, new List<IRoleValidator<Role>>(), new Mock<ILookupNormalizer>().Object, identityErrorDescriberMock.Object, new Mock<ILogger<RoleManager<Role>>>().Object);

            tokenServiceMock = new Mock<TokenService>(_unitOfWorkMock.Object, new Mock<ITokenRepository>().Object, _userManagerMock.Object);
            var _emailServMock = new Mock<EmailService>(tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);
            _loginAdapterMock = new Mock<ILoginAdapter>();
            var signinManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object,
                                                               new Mock<IHttpContextAccessor>().Object,
                                                               new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                                                               identityOptionsMock.Object,
                                                               new Mock<ILogger<SignInManager<User>>>().Object,
                                                               new Mock<IAuthenticationSchemeProvider>().Object,
                                                               new Mock<IUserConfirmation<User>>().Object);

            _userServiceMock = new Mock<UserService>(_userManagerMock.Object, roleManagerMock.Object, _logServiceMock.Object,signinManagerMock.Object, _emailServMock.Object, _configurationMock.Object, tokenServiceMock.Object, _loginAdapterMock.Object);
            _emailServiceMock = new Mock<EmailService>(tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);

            _service = new StaffService(_unitOfWorkMock.Object, _logServiceMock.Object,
                                    _repoMock.Object, _repoSpecMock.Object,
                                    _userManagerMock.Object, _configurationMock.Object, _emailServiceMock.Object,
                                    _userServiceMock.Object);  
        }

        [Fact]
        public async Task EditStaffProfile_ReturnsAcceptedStaffDto_IntegrationControllerService()
        {
            //Arrange
            var _controller = new StaffController(_service,_userServiceMock.Object);

            var dtoMock = new EditStaffDto
            ("+351 910000011",
              "test@email.com",
              "New, 1234-234, Updated",
              "25841300");

            var email = "ritabarbosa@email.com";
            var userid = "testid";
            var token = "test-token";
            var userMock = new Mock<User>();
            userMock.Setup(u => u.Id).Returns(userid);
            userMock.Setup(u => u.UserName).Returns(email);
            userMock.Setup(u => u.Email).Returns(email);
            userMock.Setup(u => u.Status).Returns(true);

            var staffMock = new Mock<Staff>("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", email, "+351", "987654321", "Doctor", "25841209");
            var id = "D202400001";

            var dtoResult = new StaffDto("Rita Barbosa", "+351 910000011", "test@email.com", "New, 1234-234, Updated", id, "25841300");

            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer valid-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = context
            };
            _userServiceMock.Setup(_userService => _userService.CheckUserRole("valid-token", "Admin")).Returns(false);


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
            var result = await _controller.EditStaffProfile(id, dtoMock);

            //Assert
            var acceptedResult = Assert.IsType<AcceptedResult>(result.Result);
            var returnedStaff = Assert.IsType<StaffDto>(acceptedResult.Value);
            Assert.Equal(dtoResult.Name, returnedStaff.Name);
            Assert.Equal(dtoResult.Phone, returnedStaff.Phone);
            Assert.Equal(dtoResult.Email, returnedStaff.Email);
            Assert.Equal(dtoResult.Address, returnedStaff.Address);
            Assert.Equal(dtoResult.SpecializationId, returnedStaff.SpecializationId);
        }


        [Fact]
        public async Task UpdateAsync_ReturnsStaffDto_IntegrationServiceDomain()
        {
            //Arrange
            var dtoMock = new EditStaffDto
            ("+351 910000011",
              "test@email.com",
              "New, 1234-234, Updated",
              "25841300");

            var email = "ritabarbosa@email.com";
  
            var token = "test-token";
            var user = new User();
            user.UserName = email;
            user.Email = email;
            user.Status = true;

            var staff= new Staff("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", email, "+351", "987654321", "Doctor", "25841209");
            var id = "D202400001";

            var dtoResult = new StaffDto("Rita Barbosa", "+351 910000011", "test@email.com", "New, 1234-234, Updated", id, "25841300");

            _repoMock.Setup(_repoPatMock => _repoPatMock.GetByIdAsync(It.IsAny<StaffId>()))
                .ReturnsAsync(staff);
            _userManagerMock.Setup(_userManagerMock => _userManagerMock.FindByEmailAsync(email)).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(["Doctor"]);
            _userManagerMock.Setup(um => um.GenerateChangeEmailTokenAsync(user, "test@email.com")).ReturnsAsync(token);
            _userManagerMock.Setup(um => um.GenerateEmailConfirmationTokenAsync(user)).ReturnsAsync(token);
            _userManagerMock.Setup(um => um.ChangeEmailAsync(user, "test@email.com", token)).ReturnsAsync(IdentityResult.Success);
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
        public async Task ConfirmEmailStaff_ReturnsOkResult_IntegrationControllerService()
        {
            // Arrange
            var _controller = new StaffController(_service,_userServiceMock.Object);
            var email = "ritabarbosa@email.com";
            var userId = "valid_user_id";
            var token = "valid_token";
            var staffId = "D202400001";
            var userMock = new Mock<User>();
            userMock.Setup(u => u.Id).Returns(userId);
            userMock.Setup(u => u.UserName).Returns(email);
            userMock.Setup(u => u.Email).Returns(email);
            userMock.Setup(u => u.Status).Returns(true);

            var staffMock = new Mock<Staff>("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", email, "+351", "987654321", "Doctor", "25841209");
            _userManagerMock.Setup(repo => repo.FindByIdAsync(userId)).ReturnsAsync(userMock.Object);
            _userManagerMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>())).Returns(Task.FromResult(IdentityResult.Success));
            tokenServiceMock.Setup(t => t.ConfirmEmailToken(userId, token)).ReturnsAsync(true);
            _repoMock.Setup(_repoPatMock => _repoPatMock.GetByIdAsync(It.IsAny<StaffId>()))
                    .ReturnsAsync(staffMock.Object);

            // Act
            var result = await _controller.ConfirmEmailStaff(userId,staffId, token);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ConfirmEmailStaff_Sucessful()
        {
            // Arrange
            var email = "ritabarbosa@email.com";
            var userId = "testUserId";
            var token = "validToken";
            var user = new User();
            user.UserName = email;
            user.Email = email;
            user.Status = true;

            var staffId = "D202400001";

            var staff = new Staff("00001", "Portugal, 4570-860, Rua das Oliveiras", "12345", "Rita", "Barbosa", "Rita Barbosa", email, "+351", "987654321", "Doctor", "25841209");

            _userManagerMock.Setup(repo => repo.FindByIdAsync(userId)).ReturnsAsync(user);
            _userManagerMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>())).Returns(Task.FromResult(IdentityResult.Success));
            tokenServiceMock.Setup(t => t.ConfirmEmailToken(userId, token)).ReturnsAsync(true);
            _repoMock.Setup(_repoPatMock => _repoPatMock.GetByIdAsync(It.IsAny<StaffId>()))
                    .ReturnsAsync(staff);

            // Act
            await _service.ConfirmEmailStaff(userId, staffId, token);

            // Assert
            Assert.True(user.Status);
        }
    }
}
