using System.Security.Claims;
using MDBackoffice.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.StaffProfiles;
using MDBackoffice.Domain.Tokens;
using MDBackoffice.Domain.Shared;
using Microsoft.AspNetCore.Authentication.Google;


namespace MDBackoffice.Controllers
{

    [ApiController]
    [Route("api/")]
    public class UserController : ControllerBase
    {
        private readonly PatientService _patientService;
        private readonly StaffService _staffService;
        private readonly TokenService _tokenService;
        private readonly UserService _userService;
        public UserController(UserService userService, PatientService patientService, StaffService staffService, TokenService tokenService)
        {
            _userService = userService;
            _patientService = patientService;
            _staffService = staffService;
            _tokenService = tokenService;
        }

        [HttpPost("login-internal")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            try
            {
                var token = await _userService.Login(loginUserDto);
                return Ok(new { Token = token });
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { V = $"An unexpected error occurred: {ex.Message}" });
            }
        }

        [HttpGet("decode-token")]
        public IActionResult DecodeToken(string token)
        {
            try
            {
                var (email, roles) = _userService.DecodeJwtToken(token);
                return Ok(new { Email = email, Roles = roles });
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { V = $"An unexpected error occurred: {ex.Message}" });
            }
        }

        [HttpGet("login-external")]
        public async Task<IActionResult> LoginExternalStart()
        {
            try
            {
                var properties = await _userService.LoginExternalStart();
                return Challenge(properties, GoogleDefaults.AuthenticationScheme);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { V = $"An unexpected error occurred: {ex.Message}" });
            }
        }

        [HttpGet("login-externalEnd")]
        public async Task<IActionResult> LoginExternalEnd()
        {
            try
            {
                var token = await _userService.LoginExternalEnd();
                var redirectUrl = $"http://localhost:4200/login-callback?token={token}";
                return Redirect(redirectUrl);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { V = $"An unexpected error occurred: {ex.Message}" });
            }
        }

        [HttpPut("activate-staff")]
        public async Task<IActionResult> ConfirmEmailStaff([FromQuery] string userId, [FromQuery] string token, [FromBody] ConfirmEmailUserDto confirmEmailUserDto)
        {
            try
            {
                await _userService.ConfirmEmailStaff(userId, token, confirmEmailUserDto.NewPassword);
                return Ok(new { message = $"Email confirmed successfully and account activated." });
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { V = $"An unexpected error occurred: {ex.Message}" });
            }
        }

        [HttpPut("activate-patient")]
        public async Task<IActionResult> ConfirmEmailPatient([FromQuery] string userId, [FromQuery] string token)
        {
            try
            {
                await _userService.ConfirmEmailPatient(userId, token);
                return Ok(new { message = $"Email confirmed successfully and account activated." });
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { V = $"An unexpected error occurred: {ex.Message}" });
            }
        }

        [HttpPost("create-admin")]
        public async Task<IActionResult> RegisterAdminUser([FromBody] RegisterUserDto registerUserDto)
        {
            try
            {
                await _userService.CreateUserAsync(registerUserDto);
                return Ok("Email confirmed successfully and account activated.");
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { V = $"An unexpected error occurred: {ex.Message}" });
            }
        }

        [HttpPost("create-patient")]
        public async Task<IActionResult> RegisterPatientUser([FromBody] RegisterPatientUserDto registerPatientUserDto)
        {
            try
            {
                var user = await _userService.CreatePatientUserAsync(registerPatientUserDto);
                await _patientService.AddUser(user, registerPatientUserDto.Email, registerPatientUserDto.Phone);
                string email = await _patientService.GetProfileEmail(user.Email.ToString(), registerPatientUserDto.Phone);
                await _userService.SendConfirmationEmail(user, email);
                return Ok(new { message = "The user has been successfully created. Please verify your email to complete the registration." });
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"An unexpected error occurred: {ex.Message}" });
            }
        }

        [HttpPost("create-staff")]
        /* [Authorize(Policy = "Admin")] */
        public async Task<IActionResult> RegisterStaffUser([FromBody] RegisterUserDto registerUserDto)
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userService.CheckUserRole(token, "Admin")) 
                {
                    return BadRequest("Invalid authorization or user role.");
                }

                var user = await _userService.CreateStaffUserAsync(registerUserDto);
                await _staffService.AddUser(user, registerUserDto.Email, registerUserDto.Phone);
                string email = await _staffService.GetProfileEmail(user.Email.ToString(), registerUserDto.Phone);
                await _userService.SendConfirmationEmail(user, email);
                return Ok(new { message = "The user has been successfully created. Please verify your email to complete the registration." });
            }
            catch (InvalidOperationException ex1)
            {
                return BadRequest(ex1.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(new { V = $"An unexpected error occurred: {ex.Message}" });
            }
        }


        // Delete Patient Account Request: api/Delete-PatientAccountDeletionRequest
        [HttpDelete("Delete-PatientAccountDeletionRequest")]
        [Authorize(Policy = "Patient")]
        public async Task<ActionResult> DeletePatientAccountRequest()
        {
            IActionResult result = await GetUserInfo();

            // string email2 = "mangoboy.xv@gmail.com";
            // string id2 = "9edc6a2d-bb8b-4466-83e2-c96b4bd94eec";
            // string password2 = "Abcde12345!";

            // var userInfo2 = new { Id = id2, Email = email2, Password = password2 };
            // var result = new OkObjectResult(userInfo2);

            if (result is OkObjectResult okResult)
            {
                var userInfo = okResult.Value;

                string? email = userInfo?.GetType().GetProperty("Email")?.GetValue(userInfo, null)?.ToString();
                string? userId = userInfo?.GetType().GetProperty("Id")?.GetValue(userInfo, null)?.ToString();

                var token = await _tokenService.CreateAccountDeletionToken(email);

                var baseUrl = "http://localhost:4200";
                var link = $"{baseUrl}/confirm-patient-account-deletion?userId={userId}&token={token.TokenId}";

                _patientService.ConfirmPatientAccountDeletionEmail(link, email);

                return Ok(new { message = "An email asking for your account's deletion confirmation was sent." });
            }
            else
            {
                return BadRequest("User info could not be retrieved.");
            }
        }

        private async Task<IActionResult> GetUserInfo()
        {
            string? userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userEmail))
            {
                return NotFound("Email claim not found.");
            }
            User? user = await _userService.FindByEmailAsync(userEmail);

            if (user != null)
            {
                return Ok(new
                {
                    user.Id,
                    user.Email,
                });
            }
            else
            {
                return NotFound("User not found.");
            }
        }


        // Update Deletion of Patient Account Confirmation: api/Update-PatientAccountDeletionConfirmation
        [HttpPut("Update-PatientAccountDeletionConfirmation")]
        public async Task<ActionResult> ConfirmPatientAccountDeletionNotProfile([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest("User Email and Token are required.");
            }

            if (!await _userService.UserExistsById(userId))
            {
                return NotFound("User not found.");
            }

            if (await _tokenService.TokenExistsById(token) == null)
            {
                return NotFound("Token not found.");
            }

            if (await _tokenService.IsTokenExpired(token))
            {
                return Unauthorized("The provided token has expired.");
            }

            if (!await _tokenService.IsTokenActive(token))
            {
                return Unauthorized("The provided token was already used.");
            }

            var userEmail = await _userService.DeletePatientAccount(userId, token);
            // anonymize the patient's profile according to GDPR policies -> readme info for now
            await _patientService.AnonymizeProfile(userEmail);

            return Ok(new { message = "Patient account successfully deleted!\nSome of your non-identifiable data will be retained, as per our GDPR policies." });
        }


        [HttpPut("send-passwordEmail")]
        public async Task<IActionResult> ResetPassword([FromQuery] string email)
        {
            try
            {
                await _userService.ResetPassword(email);

                return Ok(new { message ="Password reset email was sent!"});
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { V = "An unexpected error occured." });
            }
        }

        [HttpPut("update-userPassword")]
        public async Task<IActionResult> UpdatePassword([FromQuery] string email, [FromQuery] string token, [FromBody] ConfirmEmailUserDto confirmEmailUserDto)
        {
            try
            {
                if (await _userService.UpdatePassword(email, token, confirmEmailUserDto.NewPassword))
                {
                    return Ok(new { message ="Password was changed successfully."});
                }
                else
                {
                    return BadRequest("Password update failed.");
                }
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { V = "An expected error occured" });
            }
        }

    }

    internal class GoogleTokenResponse
    {
    }
}
