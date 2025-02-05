// using MDBackoffice.Controllers;
// using MDBackoffice.Domain.Emails;
// using MDBackoffice.Domain.Logs;
// using MDBackoffice.Domain.Patients;
// using MDBackoffice.Domain.Shared;
// using MDBackoffice.Domain.Specializations;
// using MDBackoffice.Domain.StaffProfiles;
// using MDBackoffice.Domain.Tokens;
// using MDBackoffice.Domain.Users;
// using MDBackoffice.Infrastructure.Emails;
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Options;
// using Moq;
// using Xunit;


// namespace MDBackofficeTests.integrationtests.usertests
// {
//     public class CreateStaffUserIntegrationTests
//     {

//         private readonly UserService _userService;
//         private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
//         private readonly Mock<LogService> _logServiceMock = new(new Mock<IUnitOfWork>().Object, new Mock<ILogRepository>().Object);
//         private readonly Mock<UserManager<User>> _userManagerMock;
//         private readonly Mock<IConfiguration> _configurationMock;
//         private readonly Mock<TokenService> _tokenServiceMock;
//         private readonly Mock<PatientService> _patientServiceMock;
//         private readonly Mock<StaffService> _staffServiceMock;
//         private readonly Mock<RoleManager<Role>> _roleManagerMock;
//         private readonly Mock<EmailService> _emailServiceMock;
//         private readonly Mock<IStaffRepository> _staffRepoMock;
//         private readonly Mock<IConfiguration> _configuration;
//         private readonly Mock<ISpecializationRepository> _specRepoMock;
//                 private readonly Mock<SignInManager<User>> _signinManagerMock;

//         public CreateStaffUserIntegrationTests()
//         {
//             var identityOptionsMock = new Mock<IOptions<IdentityOptions>>();
//             identityOptionsMock.Setup(o => o.Value).Returns(new IdentityOptions());
//             var identityErrorDescriberMock = new Mock<IdentityErrorDescriber>();

//             _userManagerMock = new Mock<UserManager<User>>(
//                 new Mock<IUserStore<User>>().Object,
//                 identityOptionsMock.Object,
//                 new Mock<IPasswordHasher<User>>().Object,
//                 new List<IUserValidator<User>> { new Mock<IUserValidator<User>>().Object },
//                 new List<IPasswordValidator<User>> { new Mock<IPasswordValidator<User>>().Object },
//                 new Mock<ILookupNormalizer>().Object,
//                 identityErrorDescriberMock.Object,
//                 new Mock<IServiceProvider>().Object,
//                 new Mock<ILogger<UserManager<User>>>().Object
//             );

//             _roleManagerMock = new Mock<RoleManager<Role>>(
//                 new Mock<IRoleStore<Role>>().Object,
//                 new List<IRoleValidator<Role>>(),
//                 new Mock<ILookupNormalizer>().Object,
//                 identityErrorDescriberMock.Object,
//                  new Mock<ILogger<RoleManager<Role>>>().Object
//             );

//  _signinManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object,
//                                                                   new Mock<IHttpContextAccessor>().Object,
//                                                                   new Mock<IUserClaimsPrincipalFactory<User>>().Object,
//                                                                   identityOptionsMock.Object,
//                                                                   new Mock<ILogger<SignInManager<User>>>().Object,
//                                                                   new Mock<IAuthenticationSchemeProvider>().Object,
//                                                                   new Mock<IUserConfirmation<User>>().Object);
          
//             _tokenServiceMock = new Mock<TokenService>(_unitOfWorkMock.Object, new Mock<ITokenRepository>().Object, _userManagerMock.Object);
//             _emailServiceMock = new Mock<EmailService>(_tokenServiceMock.Object, new Mock<IEmailAdapter>().Object);
//             _configurationMock = new Mock<IConfiguration>();
//             _userService = new UserService(
//                 _userManagerMock.Object,
//                 _roleManagerMock.Object,
//                 _logServiceMock.Object,
//                 _signinManagerMock.Object,
//                 _emailServiceMock.Object,
//                 _configurationMock.Object,
//                 _tokenServiceMock.Object
//             );

//             _staffRepoMock = new Mock<IStaffRepository>();
//             _specRepoMock = new Mock<ISpecializationRepository>();
//             _configuration = new Mock<IConfiguration>();

//             _patientServiceMock = new Mock<PatientService>(_unitOfWorkMock.Object, _logServiceMock.Object, _configurationMock.Object, new Mock<IPatientRepository>().Object,
//                     _userService, _emailServiceMock.Object);
//             _staffServiceMock = new Mock<StaffService>(_unitOfWorkMock.Object, _logServiceMock.Object, new Mock<IStaffRepository>().Object, new Mock<ISpecializationRepository>().Object,
//                    _userManagerMock.Object, _configurationMock.Object, _emailServiceMock.Object, _userService);
//         }

//         [Fact]
//         public async Task CreateStaffUser_ReturnsOkResult_IntegrationControllerService()
//         {
//             //Arrange
//             var _controller = new UserController(_userService, _patientServiceMock.Object, _staffServiceMock.Object, _tokenServiceMock.Object);

//             var email = "test@email.com";
//             var password = "NewPass00_d";
//             var role = "doctor";
//             var phone = "+351 960444772";

//             var dtoMock = new RegisterUserDto 
//             { 
//                 Email = email, 
//                 Password = password, 
//                 Phone = phone, 
//                 Role = role 
//             };

//             var resultMock = IdentityResult.Success;
//             var address = "Portugal, 4570-860, Rua das Oliveiras";
//             var specialization = "Orthopedics";
//             var id = "testid";  
//             var seqNumber = "00001";
//             var specMock = new Mock<Specialization>(specialization);
//             var phoneParts = phone.Split(' ');
            
//             var staffMock = new Mock<Staff>(seqNumber, address, "12345", "Rita", "Barbosa", "Rita Barbosa", email, "+351", "987654321", "Doctor", specialization);
//             var staffDto = new StaffDto(staffMock.Object.Id.Value, staffMock.Object.Name.ToString(), staffMock.Object.Phone.ToString(), staffMock.Object.Email.ToString(), staffMock.Object.Address.ToString(), staffMock.Object.SpecializationId.Value, [] );
            
//             var createStaffProfile = new CreatingStaffDto(
//                 staffMock.Object.LicenseNumber.Number,
//                 address,
//                 staffMock.Object.Name.FirstName,
//                 staffMock.Object.Name.LastName,
//                 phone,
//                 email,
//                 role,
//                 specialization
//             );
          
//             var userMock = new Mock<User>();
//             userMock.Setup(u => u.Id).Returns(id);
//             userMock.Setup(u => u.UserName).Returns(email);
//             userMock.Setup(u => u.Email).Returns(email);
//             userMock.Setup(u => u.Status).Returns(true);
//             userMock.Setup(u => u.PasswordHash).Returns(password);

//             //idk what to put here
//             string token = "";

//             //_userService.CreateStaffUserAsync
//             _roleManagerMock.Setup(rm => rm.RoleExistsAsync(role)).ReturnsAsync(true);
//             _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), password)).ReturnsAsync(resultMock);
//             _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), role)).ReturnsAsync(resultMock);

//             //_staffService.AddUser
//             _staffRepoMock.Setup(_staffRepoMock => _staffRepoMock.ExistsStaffWithEmailOrPhone(createStaffProfile.Email, phoneParts[0], phoneParts[1])).ReturnsAsync(true);
//             _staffRepoMock.Setup(_staffRepoMock => _staffRepoMock.GetStaffWithEmail(createStaffProfile.Email)).ReturnsAsync(staffMock.Object);
//             staffMock.Setup(staffMock => staffMock.AddUser(userMock.Object));
//             _unitOfWorkMock.Setup(_unitOfWorkMock => _unitOfWorkMock.CommitAsync()).ReturnsAsync(1);

//             //_staffService.GetProfileEmail
//             _staffRepoMock.Setup(_staffRepoMock => _staffRepoMock.FindStaffWithEmailOrPhone(createStaffProfile.Email, phoneParts[0], phoneParts[1])).ReturnsAsync(staffMock.Object);

//             // _userService.SendConfirmationEmail
//             _userManagerMock.Setup(um => um.GenerateEmailConfirmationTokenAsync(userMock.Object)).ReturnsAsync(token);
            
//             //do email DTO and check setup
//             var emailMessage = new EmailMessageDto(_configuration.Object["App:Email"], userMock.Object.Email, "Test  #1",  "Just checking.");
//             _emailServiceMock.Setup(um => um.SendConfirmationEmail(emailMessage));


//             //Act
//             var result = await _controller.RegisterStaffUser(dtoMock);

//             // Verify interactions
//             _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<User>(), password), Times.Once);
//             _userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<User>(), role), Times.Once);
//             _staffServiceMock.Verify(staffService => staffService.AddUser(It.IsAny<User>(), email, phone), Times.Once);
//             _staffServiceMock.Verify(staffService => staffService.GetProfileEmail(email, phone), Times.Once);
//         }


//         [Fact]
//         public async Task CreateStaffUserWithNoStaffProfile_ReturnsBadRequest_IntegrationControllerService()
//         {
//             //Arrange
//             var _controller = new UserController(_userService, _patientServiceMock.Object, _staffServiceMock.Object, _tokenServiceMock.Object);

//             var email = "test@email.com";
//             var password = "NewPass00_d";
//             var role = "doctor";
//             var phone = "+351 960444772";

//             var dtoMock = new RegisterUserDto 
//             { 
//                 Email = email, 
//                 Password = password, 
//                 Phone = phone, 
//                 Role = role 
//             };

//             var resultMock = IdentityResult.Success;
//             var address = "Portugal, 4570-860, Rua das Oliveiras";
//             var specialization = "Orthopedics";
//             var seqNumber = "00001";
//             var specMock = new Mock<Specialization>(specialization);
//             var phoneParts = phone.Split(' ');
            
//             var staffMock = new Mock<Staff>(seqNumber, address, "12345", "Rita", "Barbosa", "Rita Barbosa", email, "+351", "987654321", "Doctor", specialization);
            
//             var createStaffProfile = new CreatingStaffDto(
//                 staffMock.Object.LicenseNumber.Number,
//                 address,
//                 staffMock.Object.Name.FirstName,
//                 staffMock.Object.Name.LastName,
//                 phone,
//                 email,
//                 role,
//                 specialization
//             );
          
//             var userMock = new Mock<User>();

//             //_userService.CreateStaffUserAsync
//             _roleManagerMock.Setup(rm => rm.RoleExistsAsync(role)).ReturnsAsync(true);
//             _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), password)).ReturnsAsync(resultMock);
//             _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), role)).ReturnsAsync(resultMock);

//             //_staffService.AddUser
//             _staffRepoMock.Setup(_staffRepoMock => _staffRepoMock.ExistsStaffWithEmailOrPhone(createStaffProfile.Email, phoneParts[0], phoneParts[1])).ReturnsAsync(true);

//             //Act
//             var result = await _controller.RegisterStaffUser(dtoMock);

//             // Verify interactions
//             Assert.IsType<BadRequestObjectResult>(result);
//         }



//      //   [Fact]
//      //   public async Task CreateStaffUser_ReturnsOkResult_IntegrationServiceDomain()
//     //    {
//             // //Arrange
//             // var _controller = new UserController(_userService, _patientServiceMock.Object, _staffServiceMock.Object, _tokenServiceMock.Object);

//             // //Act
//             // var result = await _userService.CreateStaffUserAsync(dtoMock);

//             // //Assert
//      //   }


//         [Fact]
//         public async Task CreateStaffUser_ReturnsBadRequest_IntegrationServiceDomain()
//         {
//             //Arrange



//             var email = "test@email.com";
//             var password = "NewPass00_d";
//             var role = "doctor";
//             var phone = "+351 960444772";


//             var user = new User();
//             user.UserName = email;
//             user.Email = email;
//             user.Status = true;
//             user.PasswordHash = password;

//             _userManagerMock.Setup(_userManagerMock => _userManagerMock.FindByEmailAsync(email)).ReturnsAsync(user);
//             _userManagerMock.Setup(_userManagerMock => _userManagerMock.CheckPasswordAsync(user, password)).ReturnsAsync(true);










//             var _controller = new UserController(_userService, _patientServiceMock.Object, _staffServiceMock.Object, _tokenServiceMock.Object);


//             var dtoMock = new RegisterUserDto 
//             { 
//                 Email = email, 
//                 Password = password, 
//                 Phone = phone, 
//                 Role = role 
//             };

//             var resultMock = IdentityResult.Success;
//             var address = "Portugal, 4570-860, Rua das Oliveiras";
//             var specialization = "Orthopedics";
//             var seqNumber = "00001";
//             var specMock = new Mock<Specialization>(specialization);
//             var phoneParts = phone.Split(' ');
            
//             var staffMock = new Mock<Staff>(seqNumber, address, "12345", "Rita", "Barbosa", "Rita Barbosa", email, "+351", "987654321", "Doctor", specialization);
            
//             var createStaffProfile = new CreatingStaffDto(
//                 staffMock.Object.LicenseNumber.Number,
//                 address,
//                 staffMock.Object.Name.FirstName,
//                 staffMock.Object.Name.LastName,
//                 phone,
//                 email,
//                 role,
//                 specialization
//             );





          
//             var userMock = new Mock<User>();

//             //_userService.CreateStaffUserAsync
//             _roleManagerMock.Setup(rm => rm.RoleExistsAsync(role)).ReturnsAsync(true);
//             _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), password)).ReturnsAsync(resultMock);
//             _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), role)).ReturnsAsync(resultMock);

//             //_staffService.AddUser
//             _staffRepoMock.Setup(_staffRepoMock => _staffRepoMock.ExistsStaffWithEmailOrPhone(createStaffProfile.Email, phoneParts[0], phoneParts[1])).ReturnsAsync(true);

//             //Act
//             var result = await _controller.RegisterStaffUser(dtoMock);

//             // Verify interactions
//             Assert.IsType<BadRequestObjectResult>(result);
//         }

//     }
// }


