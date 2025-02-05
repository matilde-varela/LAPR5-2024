using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.StaffProfiles;
using MDBackoffice.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MDBackoffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly PatientService _service;
        private readonly UserService _userSvc;

        public PatientController(PatientService service, UserService svc)
        {
            _service = service;
            _userSvc = svc;
        }

        [HttpGet("SimpleId/{id}")]
        public async Task<ActionResult<PatientDto>> GetPatientById(string id)
        {
            var medicalRecordNumber = new MedicalRecordNumber(id);
            var patient = await _service.GetByIdAsync(medicalRecordNumber);
            if (patient == null)
            {
                return NotFound();
            }
            return Ok(patient);
        }


        [HttpGet("{id}")]
        [Route("Get-PatientWithId")]
        //  [Authorize(Policy = "Admin")]
        public async Task<ActionResult<PatientDto>> GetPatientById(MedicalRecordNumber id)
        {
            var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

            if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Admin"))
            {
                return BadRequest("Invalid authorization or user role.");
            }

            var patient = await _service.GetByIdAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return Ok(patient);
        }

        [HttpPost]
        [Route("Create-PatientProfile")]
        //[Authorize(Policy = "Admin")]
        public async Task<ActionResult<PatientDto>> CreatePatientProfile(CreatingPatientDto dto)
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Admin"))
                {
                    return BadRequest("Invalid authorization or user role.");
                }
                var patient = await _service.CreatePatientProfile(dto, token);

                return CreatedAtAction(nameof(GetPatientById), new { id = patient.PatientId }, patient);
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

        [HttpPost]
        [Route("Download-Medical-Record")]
        //[Authorize(Policy = "Admin")]
        public async Task<ActionResult<PatientDto>> DownloadMedicalRecord(DownloadMedicalRecordDto dto)
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Patient"))
                {
                    return BadRequest("Invalid authorization or user role.");
                }
                var patient = await _service.DownloadMedicalRecord(dto, token);

                return Ok(patient);
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

        [HttpPut]
        [Route("Delete-PatientProfile")]
        //[Authorize(Policy = "Admin")]
        public async Task<IActionResult> DeletePatientProfile([FromBody] IdPassDto idPassDto)
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Admin"))
                {
                    return BadRequest("Invalid authorization or user role.");
                }
                await _service.DeletePatientProfile(idPassDto.Id);

                return Ok(new { message = "Patient profile and account succefully deleted." });
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex1)
            {
                return BadRequest(new { V = ex1.Message });
            }
        }


        [HttpPut]
        //[Authorize(Policy = "Patient")]
        public async Task<ActionResult<PatientDto>> EditPatientProfile(EditPatientProfileDto dto)
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Patient"))
                {
                    return BadRequest("Invalid authorization or user role.");
                }

                var email = _userSvc.GetLoggedInEmail(token);
                var patientDto = await _service.EditProfile(email, dto);

                return Accepted(patientDto);
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


        //GET: api/Patient
        [HttpGet]
        [Route("Get-PatientProfiles")]
        //[Authorize(Policy = "Admin")]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetPatientProfiles()
        {
            var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

            if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Admin") || _userSvc.CheckUserRole(token, "Doctor"))
            {
                return BadRequest("Invalid authorization or user role.");
            }
            return await _service.GetAllAsysnc();
        }


        //PUT: api/Patient/5
        [HttpPut("{id}")]
        //[Authorize(Policy = "Admin")]
        public async Task<ActionResult<PatientDto>> EditPatientProfile(string id, EditPatientDto dto)
        {

            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Admin"))
                {
                    return BadRequest("Invalid authorization or user role.");
                }

                var patient = await _service.UpdateAsync(id, dto);

                if (patient == null)
                {
                    return NotFound();
                }

                return Ok(patient);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // GET: api/Patient/Filtered-List
        [HttpPost("Filtered-List")]
        public async Task<ActionResult<List<PatientDto>>> GetFilteredPatientProfiles(PatientQueryParametersDto dto)
        {
            var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

            if (string.IsNullOrWhiteSpace(token) || (!_userSvc.CheckUserRole(token, "Doctor") && !_userSvc.CheckUserRole(token, "Admin")))
            {
                return BadRequest("Invalid authorization or user role.");
            }

            var patients = await _service.FilterPatientProfiles(dto);

            if (patients.IsNullOrEmpty())
            {
                return NotFound(new { Message = "No patients matching the filtering criteria." });
            }

            return Ok(patients);
        }

         // GET: api/Patient/Filtered-List
        [HttpGet("get-privacy-policy")]
        public async Task<ActionResult<string>> GetPrivacyPolicy()
        {

            var policyText = _service.ObtainPrivacyPolicyText();

            if (policyText.IsNullOrEmpty())
            {
                return Ok("No privacy policy available yet.");
            }

            return Ok(policyText);
        }
    }

    

}