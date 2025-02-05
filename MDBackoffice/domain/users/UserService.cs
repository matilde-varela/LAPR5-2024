using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MDBackoffice.Domain.Emails;
using MDBackoffice.Domain.Logs;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.StaffProfiles;
using MDBackoffice.Domain.Tokens;
using MDBackoffice.Infrastructure.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.ObjectPool;
using Microsoft.IdentityModel.Tokens;

namespace MDBackoffice.Domain.Users
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signinManager;
        private readonly EmailService _emailService;
        private readonly TokenService _tokenService;
        private readonly LogService _logService;
        private readonly IConfiguration _configuration;
        private readonly ILoginAdapter _loginAdapter;
        public UserService(UserManager<User> userManager, RoleManager<Role> roleManager,
                                LogService logService, SignInManager<User> signInManager,

                                EmailService emailService, IConfiguration configuration,
                                TokenService tokenService, ILoginAdapter loginAdapter)
        {

            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _logService = logService;
            _signinManager = signInManager;
            _tokenService = tokenService;
            _configuration = configuration;
            _loginAdapter = loginAdapter;
        }

        public virtual async Task ConfirmEmailPatient(string userId, string token)
        {
            await ConfirmEmailVerifications(userId, token);

            User user = await FindByIdAsync(userId);
            user.changeStatus(true);
            await UpdateAsync(user);
        }

        public async Task ConfirmEmailStaff(string userId, string token, string newPassword)
        {
            await ConfirmEmailVerifications(userId, token);

            User user = await FindByIdAsync(userId);
            var resetToken = await _tokenService.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
            user.changeStatus(true);
            await UpdateAsync(user);
        }

        public async Task ConfirmEmailStaffWithoutPassword(string userId, string token)
        {
            await ConfirmEmailVerifications(userId, token);

            User user = await FindByIdAsync(userId);
            user.changeStatus(true);
            await UpdateAsync(user);
        }

        private async Task ConfirmEmailVerifications(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
                throw new BusinessRuleValidationException("User Email and Token are required.");

            var decodedToken = Uri.UnescapeDataString(token);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new BusinessRuleValidationException("Unable to find the specified user.");

            var result = await _tokenService.ConfirmEmailToken(userId, decodedToken);
            if (!result)
                throw new BusinessRuleValidationException("Unable to confirm the email.");

        }

        public async Task<string> Login(LoginUserDto loginUserDto)
        {

            var user = await _userManager.FindByEmailAsync(loginUserDto.Email)
              ?? throw new BusinessRuleValidationException("Invalid email.");

            if (!user.Status)
                throw new BusinessRuleValidationException("Account not yet activated.");

            if (await _userManager.IsLockedOutAsync(user))
                throw new BusinessRuleValidationException("Account is locked out. Please try again later.");

            // Sign in the user and check for lockout on failure
            var signInResult = await _signinManager.PasswordSignInAsync(
                user,
                loginUserDto.Password,
                isPersistent: false,
                lockoutOnFailure: true // Enable lockout on failure
            );

            if (signInResult.IsLockedOut)
                throw new BusinessRuleValidationException("Account is locked out due to too many failed login attempts.");

            if (!signInResult.Succeeded)
                throw new BusinessRuleValidationException("Invalid password.");

            return await GenerateJwtToken(user);
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30), // THE TIME THE TOKEN IS ATIVE
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public virtual (string? Email, IList<string> Roles) DecodeJwtToken(string token)
        {

            var handler = new JwtSecurityTokenHandler();

            if (handler.CanReadToken(token))
            {
                // Read the token without validating it
                var jwtToken = handler.ReadJwtToken(token);

                var email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                var roles = jwtToken.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();

                return (email, roles);
            }
            else
            {
                throw new ArgumentException("Invalid JWT token.");
            }
        }

        public virtual async Task<bool> UserExistsById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId) ?? throw new BusinessRuleValidationException("Unable to find the specified user.");
            return true;
        }

        public async Task<bool> UserExistsByEmail(string email)
        {
            User user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return false;
            }
            return true;
        }

        public async Task<User> FindByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<User> FindByEmailAsync(string userEmail)
        {
            return await _userManager.FindByEmailAsync(userEmail);
        }

        private async Task<IdentityResult> UpdateAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<User> CreateUserAsync(RegisterUserDto dto)
        {
            return await CreateAsync(dto.Email, dto.Password, dto.Role, true);
        }
        public async Task<User> CreateStaffUserAsync(RegisterUserDto dto)
        {
            return await CreateAsync(dto.Email, dto.Password, dto.Role, false);
        }
        public async Task<User> CreatePatientUserAsync(RegisterPatientUserDto registerPatientUserDto)
        {
            return await CreateAsync(registerPatientUserDto.Email, registerPatientUserDto.Password, "Patient", false);
        }
        private async Task<User> CreateAsync(string email, string password, string role, bool activation)
        {
            bool roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
                throw new BusinessRuleValidationException("The specified role is not available.");

            var user = new User { UserName = email, Email = email, Status = activation };

            var createResult = await _userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
                throw new BusinessRuleValidationException("Unable to create the user.");

            var addToRoleResult = await _userManager.AddToRoleAsync(user, role);
            if (!addToRoleResult.Succeeded)
                throw new BusinessRuleValidationException("Unable to configure the user's role.");

            return user;
        }
        public virtual async Task<IdentityResult> DeleteByIdAsync(string userReference)
        {
            return await _userManager.DeleteAsync(await _userManager.FindByIdAsync(userReference));
        }

        public async Task EditUserProfile(string oldEmail, string newEmail)
        {
            User user = await _userManager.FindByEmailAsync(oldEmail) ?? throw new BusinessRuleValidationException("Can't find the currently logged in user.");
            string token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
            var result = await _userManager.ChangeEmailAsync(user, newEmail, token);

            if (!result.Succeeded)
            {
                throw new BusinessRuleValidationException("Unable to update the user's email.");
            }

            user.changeStatus(false);
            await _userManager.UpdateAsync(user);
            await SendConfirmationChangeEmail(user, oldEmail, await _userManager.GenerateEmailConfirmationTokenAsync(user));
        }

        public async Task EditStaffUserProfile(string oldEmail, string newEmail, string staffId, bool emailChange, string changedInformation)
        {
            User user = await _userManager.FindByEmailAsync(oldEmail) ?? throw new BusinessRuleValidationException("Can't find the user with that email.");
            if (emailChange)
            {
                string token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
                var result = await _userManager.ChangeEmailAsync(user, newEmail, token);

                if (!result.Succeeded)
                {
                    throw new BusinessRuleValidationException("Unable to update the user's email.");
                }
            }
            user.changeStatus(false);
            await _userManager.UpdateAsync(user);

            await SendContactInformationConfirmationChange(user, oldEmail, await _userManager.GenerateEmailConfirmationTokenAsync(user), staffId, changedInformation);
        }

        private async Task SendConfirmationChangeEmail(User user, string email, string token)
        {
            string confirmationLink = await ConfigureUrlConfirmation(token, user);
            var body = "<p>Hello,</p><p>This email was sent to notify you that your email address has been updated in the HealthCare Clinic System. </p><p><a href='" + confirmationLink + "'>Click in the link to confirm your email change.</a></p><p>Thank you for choosing us,<br>HealthCare Clinic</p></body></html>";

            await SendEmail(email, "Update Email Confirmation", body);
        }

        private async Task SendContactInformationConfirmationChange(User user, string email, string token, string staffId, string changedInformation)
        {
            string confirmationLink = await ConfigureUrlConfirmationStaffProfile(token, user, staffId);


            string body = "<p>Hello,</p>" +
                        "<p>This email was sent to notify you that your contact information has been updated in the HealthCare Clinic System.</p>" +
                        changedInformation +
                        "<p><a href='" + confirmationLink + "'>Click here to confirm the change of your contact information.</a></p>" +
                        "<p>Thank you for choosing us,<br>HealthCare Clinic</p></body></html>";

            await SendEmail(email, "Update Contact Information Confirmation", body);
        }

        public virtual async Task SendConfirmationEmail(User user, string email)
        {
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string confirmationLink = await ConfigureUrlConfirmation(token, user);
            var body = "<p>Hello,</p><p>This email was sent to inform you that your user account in the HealthCare Clinic System has been created. </p><p><a href='" + confirmationLink + "'>Click in the link to confirm the activation of the account.</a></p><p>Thank you for choosing us,<br>HealthCare Clinic</p></body></html>";

            await SendEmail(email, "Account Activation", body);
        }

        private async Task SendPasswordEmail(User user, string token)
        {
            string confirmationLink = await ConfigureUrlPasswordConfirmation(token, user);
            var body = "<p>Hello,</p><p>This email was sent to allow you to change your password in your HealthCare Clinic System account. </p><p><a href='" + confirmationLink + "'>Click in the link to change your password.</a></p><p>Thank you for choosing us,<br>HealthCare Clinic</p></body></html>";

            await SendEmail(user.Email, "Password Reset", body);
        }

        private async Task<string> ConfigureUrlConfirmation(string token, User user)
        {
            string? role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            if (role == null) throw new NullReferenceException("Can't obtain the user role.");

            var encodedToken = Uri.EscapeDataString(token);
            var baseUrl = "http://localhost:4200";
            return role.Equals("Patient")
            ? $"{baseUrl}/patient/verify?userId={user.Id}&token={encodedToken}"
            : $"{baseUrl}/staff/verify-staff?userId={user.Id}&token={encodedToken}";
        }

        private async Task<string> ConfigureUrlConfirmationStaffProfile(string token, User user, string staffId)
        {
            string? role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            if (role == null) throw new NullReferenceException("Can't obtain the user role.");

            var encodedToken = Uri.EscapeDataString(token);
            var baseUrl = "http://localhost:4200";
            return  $"{baseUrl}/verify-profile-edit?userId={user.Id}&staffId={staffId}&token={encodedToken}";
        }

        private async Task<string> ConfigureUrlPasswordConfirmation(string token, User user)
        {
            string? role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            if (role == null) throw new NullReferenceException("Can't obtain the user role.");

            var encodedToken = Uri.EscapeDataString(token);
            // var baseUrl = _configuration["App:BaseUrl"];
            var baseUrl = "http://localhost:4200";
            if (role.Equals("Admin") || role.Equals("Technician") || role.Equals("Doctor") || role.Equals("Nurse"))
                return $"{baseUrl}/Update-UserPassword?email={user.Email}&token={encodedToken}";


            throw new BusinessRuleValidationException("Invalid user role. Only Admin, Doctor, Technician, or Nurse are allowed.");
        }

        private async Task SendEmail(string receiver, string subject, string body)
        {
            if (receiver == null) throw new NullReferenceException("The recipient's email cannot be null.");

            EmailMessageDto emailDto = new(
                           _configuration["App:Email"] ?? throw new NullReferenceException("Hospital email not configured."),
                           receiver,
                           subject,
                           body
                       );
            await _emailService.SendConfirmationEmail(emailDto);
        }

        public async Task ResetPassword(string email)
        {
            User user = await _userManager.FindByEmailAsync(email) ?? throw new BusinessRuleValidationException("Can't find the currently logged in user.");
            user.changeStatus(false);
            await _userManager.UpdateAsync(user);
            await SendPasswordEmail(user, await _userManager.GenerateEmailConfirmationTokenAsync(user));
        }

        public async Task<bool> UpdatePassword(string email, string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
            {
                throw new Exception("User Email and Token are required.");
            }

            if (!await UserExistsByEmail(email))
            {
                throw new Exception("User not found.");
            }

            string userId = (await FindByEmailAsync(email)).Id;
            var result = await _tokenService.ConfirmEmailToken(userId, token);
            if (result)
            {
                User user = await FindByIdAsync(userId);
                var resetPasswordToken = await _tokenService.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, resetPasswordToken, newPassword);
                user.changeStatus(true);
                await UpdateAsync(user);
                return true;
            }
            return false;
        }


        public async Task<string> DeleteAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return null;

            await _userManager.DeleteAsync(user);
            await _logService.CreateDeletionLog(user.Id.ToString(), user.GetType().Name, "Deletion of user account.");

            return user.Email;
        }

        public virtual async Task<string> DeletePatientAccount(string userId, string token)
        {
            //token was used so make it inactive
            await _tokenService.InactivateAsync(token);
            //delete account
            return await DeleteAsync(userId);
        }

        public async Task<AuthenticationProperties> LoginExternalStart()
        {
            return await _loginAdapter.GetRedirectionInfo();
        }

        public async Task<string> LoginExternalEnd()
        {
            // Retrieve the external login information after the user has been redirected back from the external provider (Google)
            var externalLoginInfo = await _signinManager.GetExternalLoginInfoAsync();

            if (externalLoginInfo == null)
                throw new BusinessRuleValidationException("External login information is missing. Please try again.");

            // Get the user's email from the external login info
            var email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
                throw new BusinessRuleValidationException("Google authentication was unsuccessful. Please try again.");

            // Check if the user exists in your system by email
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                throw new BusinessRuleValidationException("Invalid email.");

            if (!user.Status)
                throw new BusinessRuleValidationException("Account not yet activated.");

            // Check if the account is locked out
            if (await _userManager.IsLockedOutAsync(user))
                throw new BusinessRuleValidationException("Account is locked out. Please try again later.");

            // Check if the user is already logged in with Google
            var existingLogin = await _userManager.FindByLoginAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey);
            if (existingLogin != null)
            {
                // If the login exists, directly return the JWT token
                return await GenerateJwtToken(existingLogin);
            }

            // If the user hasn't logged in with Google before, add the external login
            var result = await _userManager.AddLoginAsync(user, externalLoginInfo);
            if (!result.Succeeded)
            {
                return "Error happened during external login association.";
            }

            // Attempt to sign the user in using their external login
            var signInResult = await _signinManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, isPersistent: false);
            if (signInResult.Succeeded)
            {
                // Sign-in successful, generate JWT token
                return await GenerateJwtToken(user);
            }

            // Handle any issues with sign-in
            return "Error happened during sign-in.";
        }

        public virtual bool CheckUserRole(string token, string role)
        {
            var userInfo = DecodeJwtToken(token);
            return !userInfo.Roles[0].Equals(role);
        }

        public virtual string GetLoggedInEmail(string token)
        {
           var userInfo = DecodeJwtToken(token);

           return userInfo.Email ?? throw new BusinessRuleValidationException("No logged in user.");
        }

        public virtual async Task<bool> ConfirmUserPasswordAsync(string email, string password)
        {
            User user = await _userManager.FindByEmailAsync(email) ?? throw new BusinessRuleValidationException("Can't find the currently logged in user.");

            return await this._userManager.CheckPasswordAsync(user, password);
        }
    }
}
