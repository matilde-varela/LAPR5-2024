using MDBackoffice.Domain.Emails;
using MDBackoffice.Domain.Logs;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.Tokens;
using MDBackoffice.Domain.Users;
using MDBackoffice.Infrastructure.Emails;
using MDBackoffice.Infrastructure.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace MDBackofficeTests.servicetests.user
{


    public class UserServiceTests
    {
        private readonly UserService _userService;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<LogService> _logServiceMock = new Mock<LogService>(new Mock<IUnitOfWork>().Object, new Mock<ILogRepository>().Object);
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<RoleManager<Role>> _roleManagerMock;
        private readonly Mock<SignInManager<User>> _signinManagerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<TokenService> _tokenServiceMock;
        private readonly Mock<ILoginAdapter> _loginAdapterMock;

        public UserServiceTests() {
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

            _roleManagerMock = new Mock<RoleManager<Role>>(
                new Mock<IRoleStore<Role>>().Object,
                new List<IRoleValidator<Role>>(),
                new Mock<ILookupNormalizer>().Object,
                identityErrorDescriberMock.Object,
                 new Mock<ILogger<RoleManager<Role>>>().Object
            );

            _signinManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object,
                                                                      new Mock<IHttpContextAccessor>().Object,
                                                                      new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                                                                      identityOptionsMock.Object,
                                                                      new Mock<ILogger<SignInManager<User>>>().Object,
                                                                      new Mock<IAuthenticationSchemeProvider>().Object,
                                                                      new Mock<IUserConfirmation<User>>().Object);

            _tokenServiceMock = new Mock<TokenService>(_unitOfWorkMock.Object, new Mock<ITokenRepository>().Object, _userManagerMock.Object);
            var _emailServiceMock = new Mock<EmailService>(_tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);
            _configurationMock = new Mock<IConfiguration>();
            _loginAdapterMock = new Mock<ILoginAdapter>();

            _userService = new UserService(
                    _userManagerMock.Object,
                    _roleManagerMock.Object,
                    _logServiceMock.Object,
                    _signinManagerMock.Object,
                    _emailServiceMock.Object,
                    _configurationMock.Object,
                    _tokenServiceMock.Object,
                    _loginAdapterMock.Object
                );
        }

        [Fact]
        public async Task ResetPassword_Successfull()
        {
            // Arrange
            var email = "test@email.com";
            var userMock = new Mock<User>();
            userMock.Setup(u => u.UserName).Returns(email);
            userMock.Setup(u => u.Email).Returns(email);
            userMock.Setup(u => u.Status).Returns(true);

            var resultMock = IdentityResult.Success;
            _userManagerMock.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(userMock.Object);
            _userManagerMock.Setup(um => um.UpdateAsync(userMock.Object)).ReturnsAsync(resultMock);
            _userManagerMock.Setup(um => um.GenerateEmailConfirmationTokenAsync(userMock.Object)).ReturnsAsync("test token");
            _userManagerMock.Setup(um => um.GetRolesAsync(userMock.Object)).ReturnsAsync(["Doctor"]);
            _configurationMock.Setup(c => c["App:Email"]).Returns("testemail@email.com");
            _configurationMock.Setup(c => c["App:BaseUrl"]).Returns("https://test/api");
            // Act
            await _userService.ResetPassword(email);

            // Assert
            userMock.Verify(u => u.changeStatus(false), Times.Once); // Verify that changeStatus was called
            _userManagerMock.Verify(um => um.UpdateAsync(userMock.Object), Times.Once); // Verify UpdateAsync was called
            _userManagerMock.Verify(um => um.GenerateEmailConfirmationTokenAsync(userMock.Object), Times.Once); // Verify token generation
        }

        [Fact]
        public async Task UpdatePassword_ReturnsSuccess()
        {
            // Arrange
            var email = "test@email.com";
            var id = "testid";
            var token = "test-token";
            var password = "NewPass00_d";
            var userMock = new Mock<User>();
            userMock.Setup(u => u.Id).Returns(id);
            userMock.Setup(u => u.UserName).Returns(email);
            userMock.Setup(u => u.Email).Returns(email);
            userMock.Setup(u => u.Status).Returns(true);

            var resultMock = IdentityResult.Success;
            _userManagerMock.Setup(_userManagerMock => _userManagerMock.FindByEmailAsync(email)).ReturnsAsync(userMock.Object);//
            _userManagerMock.Setup(um => um.FindByIdAsync(id)).ReturnsAsync(userMock.Object);
            _userManagerMock.Setup(um => um.ConfirmEmailAsync(userMock.Object, token)).ReturnsAsync(resultMock);
            _tokenServiceMock.Setup(_tokenServiceMock => _tokenServiceMock.ConfirmEmailToken(id,token)).ReturnsAsync(true);
            _tokenServiceMock.Setup(_tokenServiceMock => _tokenServiceMock.GeneratePasswordResetTokenAsync(userMock.Object)).ReturnsAsync(token);
            _userManagerMock.Setup(um => um.ResetPasswordAsync(userMock.Object, token,password)).ReturnsAsync(resultMock);
            _userManagerMock.Setup(um => um.UpdateAsync(userMock.Object)).ReturnsAsync(resultMock);

            // Act
            var result =  await _userService.UpdatePassword(email,token, password);

            // Assert
            Assert.True(result);
            _userManagerMock.Verify(um => um.UpdateAsync(userMock.Object), Times.Once);
            _userManagerMock.Verify(um => um.ResetPasswordAsync(userMock.Object,token,password), Times.Once);
        }


        [Fact]
        public async Task LoginCorrectCredentials_ReturnsTokenSuccessfully()
        {
            //Arrange
            var email = "test@email.com";
            var id = "testid";
            var password = "NewPass00_d";
          
            var dtoMock = new LoginUserDto { Email = email, Password = password };

            var userMock = new Mock<User>();


            userMock.Setup(u => u.Id).Returns(id);
            userMock.Setup(u => u.UserName).Returns(email);
            userMock.Setup(u => u.Email).Returns(email);
            userMock.Setup(u => u.Status).Returns(true);
            userMock.Setup(u => u.PasswordHash).Returns(password);

            
            _userManagerMock.Setup(_userManagerMock => _userManagerMock.FindByEmailAsync(email)).ReturnsAsync(userMock.Object);
            _signinManagerMock.Setup(sm => sm.PasswordSignInAsync(userMock.Object, password,false,true)).ReturnsAsync(SignInResult.Success);
            _userManagerMock.Setup(um => um.GetRolesAsync(userMock.Object)).ReturnsAsync(["Doctor"]);
            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("AHSVFOSDYUDASJFNhvOUVu897GB876arbvn568n6CN865Vn5NFFn86f87Cb76cNvnVNYvhgvnu7676");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("testissuer");
            _configurationMock.Setup(c => c["Jwt:Audiend"]).Returns("testaudience");

            //Act
            var result = await _userService.Login(dtoMock);

            //Assert
            Assert.NotNull(result);
            _signinManagerMock.Verify(um => um.PasswordSignInAsync(userMock.Object, password, false, true), Times.Once);
        }
        [Fact]
        public async Task LoginInvalidCredentials_ThrowsBusinessRuleValidationException()
        {
            //Arrange
            var email = "test@email.com";
            var id = "testid";
            var password = "NewPass00_d";

            var dtoMock = new LoginUserDto { Email = email, Password = "wrong-password" };

            var userMock = new Mock<User>();
            userMock.Setup(u => u.Id).Returns(id);
            userMock.Setup(u => u.UserName).Returns(email);
            userMock.Setup(u => u.Email).Returns(email);
            userMock.Setup(u => u.Status).Returns(true);
            userMock.Setup(u => u.PasswordHash).Returns(password);


            _userManagerMock.Setup(_userManagerMock => _userManagerMock.FindByEmailAsync(email)).ReturnsAsync(userMock.Object);
            _signinManagerMock.Setup(sm => sm.PasswordSignInAsync(userMock.Object, dtoMock.Password, false, true)).ReturnsAsync(SignInResult.Failed);
            _userManagerMock.Setup(um => um.GetRolesAsync(userMock.Object)).ReturnsAsync(["Doctor"]);
            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("AHSVFOSDYUDASJFNhvOUVu897GB876arbvn568n6CN865Vn5NFFn86f87Cb76cNvnVNYvhgvnu7676");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("testissuer");
            _configurationMock.Setup(c => c["Jwt:Audiend"]).Returns("testaudience");


            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _userService.Login(dtoMock));        
        }


        [Fact]
        public async Task Login_FiveFailedAttempts_ThrowsBusinessRuleValidationException()
        {
            //Arrange
            var email = "test@email.com";
            var id = "testid";
            var password = "NewPass00_d";

            var dtoMock = new LoginUserDto { Email = email, Password = "wrong-password" };

            var userMock = new Mock<User>();
            userMock.Setup(u => u.Id).Returns(id);
            userMock.Setup(u => u.UserName).Returns(email);
            userMock.Setup(u => u.Email).Returns(email);
            userMock.Setup(u => u.Status).Returns(true);
            userMock.Setup(u => u.PasswordHash).Returns(password);

            _userManagerMock.Setup(_userManagerMock => _userManagerMock.FindByEmailAsync(email)).ReturnsAsync(userMock.Object);
            _signinManagerMock.Setup(sm => sm.PasswordSignInAsync(userMock.Object, dtoMock.Password, false, true)).ReturnsAsync(SignInResult.Failed);

            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _userService.Login(dtoMock));        
        }

        [Fact]
        public async Task CreateStaffUserAsync_ReturnsUser()
        {
            // Arrange
            var email = "test@gmail.com";
            var password = "#Test12345";
            var role = "doctor";
            var phone = "+351 960444772";

            var registerUserDto = new RegisterUserDto { Email = email, Password = password, Role = role, Phone = phone };

            _roleManagerMock.Setup(rm => rm.RoleExistsAsync(role)).ReturnsAsync(true);

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), password))
                            .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), role))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.CreateStaffUserAsync(registerUserDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
            _userManagerMock.Verify(r => r.CreateAsync(It.IsAny<User>(), password), Times.Once);
            _userManagerMock.Verify(r => r.AddToRoleAsync(It.IsAny<User>(), role), Times.Once);
            _roleManagerMock.Verify(r => r.RoleExistsAsync(role), Times.Once);
        }

        [Fact]
        public async Task DeletePatientAccount_Success()
        {
            //Arrange
            var email = "test@email.com";
            var id = "testid";
            var password = "NewPass00_d";
            var token = Guid.NewGuid().ToString();

            var userMock = new Mock<User>();

            userMock.Setup(u => u.Id).Returns(id);
            userMock.Setup(u => u.UserName).Returns(email);
            userMock.Setup(u => u.Email).Returns(email);

            _userManagerMock.Setup(_userManagerMock => _userManagerMock.FindByIdAsync(id)).ReturnsAsync(userMock.Object);
            _tokenServiceMock.Setup(t => t.InactivateAsync(token)).ReturnsAsync(new TokenDto());

            //Act
            var result = await _userService.DeleteAsync(id);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<string>(result);
            Assert.Equal(email, result);

        }

        [Fact]
        public async Task CreatePatientUserAsync_ReturnsUser()
        {
            // Arrange
            var email = "test@gmail.com";
            var password = "#Test12345";
            var phone = "+351 960444772";
            var role = "Patient";

            var registerPatientUserDto = new RegisterPatientUserDto { Email = email, Password = password, Phone = phone };

            _roleManagerMock.Setup(rm => rm.RoleExistsAsync(role)).ReturnsAsync(true);

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), password))
                            .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), role))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _userService.CreatePatientUserAsync(registerPatientUserDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
            _userManagerMock.Verify(r => r.CreateAsync(It.IsAny<User>(), password), Times.Once);
            _userManagerMock.Verify(r => r.AddToRoleAsync(It.IsAny<User>(), role), Times.Once);
            _roleManagerMock.Verify(r => r.RoleExistsAsync(role), Times.Once);
        }

        [Fact]
        public async Task ConfirmEmailPatient_ReturnsSucess()
        {
            // Arrange
            var email = "ritabarbosa@email.com";
            var userid = "testid";
            var userId = "testUserId";
            var token = "validToken";
            var userMock = new Mock<User> ();
            userMock.Setup(u => u.Id).Returns(userid);
            userMock.Setup(u => u.UserName).Returns(email);
            userMock.Setup(u => u.Email).Returns(email);
            userMock.Setup(u => u.Status).Returns(true);
            
            _userManagerMock.Setup(repo => repo.FindByIdAsync(userId)).ReturnsAsync(userMock.Object);
            _userManagerMock.Setup(repo => repo.UpdateAsync(It.IsAny<User>())).Returns(Task.FromResult(IdentityResult.Success));
            _tokenServiceMock.Setup(t => t.ConfirmEmailToken(userId, token)).ReturnsAsync(true);

            // Act
            await _userService.ConfirmEmailPatient(userId, token);

            // Assert
            Assert.True(userMock.Object.Status);
            _userManagerMock.Verify(repo => repo.UpdateAsync(It.Is<User>(u => u == userMock.Object)), Times.Once);
        }
    }
}