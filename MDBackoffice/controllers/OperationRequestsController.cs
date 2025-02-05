using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using MDBackoffice.Domain.OperationRequests;
using MDBackoffice.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MDBackoffice.Domain.Users;
using Microsoft.IdentityModel.Tokens;
using MDBackoffice.Domain.StaffProfiles;

namespace MDBackoffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationRequestController : ControllerBase
    {
        private readonly OperationRequestService _service;
        private readonly UserService _userSvc;

        public OperationRequestController(OperationRequestService service, UserService svc)
        {
            _service = service;
            _userSvc = svc;
        }

        // GET: api/OperationRequest
        [HttpGet]
        [Route("Get-AllOpRequests")]
        public async Task<ActionResult<IEnumerable<OperationRequestDto>>> GetAll()
        {
            return await _service.GetAllAsysnc();
        }

        // GET: api/OperationRequest
        [HttpGet]
        [Route("Get-AllOpRequestsFromDoctor")]
        public async Task<ActionResult<IEnumerable<OperationRequestDto>>> GetAllFromDoctor()
        {
            var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("Invalid authorization or user role.");
            }
            var email = _userSvc.GetLoggedInEmail(token);

            return await _service.GetAllFromDoctorAsysnc(email);
        }

        // GET: api/OperationRequest/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OperationRequestDto>> GetGetById(string id)
        {
            var opReq = await _service.GetByIdAsync(new OperationRequestId(id));

            if (opReq == null)
            {
                return NotFound();
            }

            return opReq;
        }

        // POST: api/OperationRequest
        [HttpPost]
        // [Authorize(Policy = "Doctor")]
        public async Task<ActionResult<OperationRequestDto>> Create(CreatingOperationRequestDto dto)
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Doctor"))
                {
                    return BadRequest("Invalid authorization or user role.");
                }

                var opReq = await _service.AddAsync(dto);

                return CreatedAtAction(nameof(GetGetById), new { id = opReq.Id }, opReq);
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


        // PUT: api/OperationRequest/Update
        [HttpPut("Update")]
        // [Authorize(Policy = "Doctor")]
        public async Task<ActionResult<OperationRequestDto>> Update(UpdateOperationRequestDto dto)
        {

            var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Doctor"))
                {
                    return BadRequest("Invalid authorization or user role.");
                }

            string? userEmail = _userSvc.GetLoggedInEmail(token);

            if (!await _service.CheckDoctorIsRequestingDoctor(userEmail, dto.Id)){
                return BadRequest("You are not the requesting doctor for the choosen operation request.");
            }

            try
            {
                var opr = await _service.UpdateAsync(dto);

                if (opr == null)
                {
                    return NotFound("An operation request with the provided ID does not exist.");
                }
                return Ok(opr);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        [HttpGet("filtered")]
        // [Authorize(Policy = "Doctor")]
        public async Task<ActionResult<IEnumerable<OperationRequestDto>>> GetOperationRequestByFilters(
                                                                                [FromQuery] string? name = null,
                                                                                [FromQuery] string? priority = null,
                                                                                [FromQuery] string? operationType = null,
                                                                                [FromQuery] string? status = null,
                                                                                [FromQuery] string? dateofrequest = null,
                                                                                [FromQuery] string? deadlinedate = null)
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Doctor"))
                {
                    return BadRequest("Invalid authorization or user role.");
                }

                var email = _userSvc.DecodeJwtToken(token).Email;

                // var email = "ritabfbarbosa@gmail.com";
                if (email == null)
                {
                    return NotFound("Couldn't obtain the logged in user's email");
                }
                var operationRequests = await _service.GetOperationRequestByFiltersAsync(email, name, priority, operationType, status, dateofrequest, deadlinedate);

                if (operationRequests == null)
                {
                    return NotFound("No matching operation requests found.");
                }

                return Ok(operationRequests);
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

        // POST: api/OperationRequest/Delete-OperationRequest
        [HttpDelete("{id}")]
        //[Authorize(Policy = "Doctor")]
        public async Task<ActionResult> DeleteOperationRequest(string id)
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Doctor"))
                {
                    return BadRequest(new {message = "Invalid authorization or user role."});
                }

                var email = _userSvc.DecodeJwtToken(token).Email;

                bool result = await _service.DeleteOperationRequest(id, email);
                if (result)
                {
                    return Ok(new {message = "Operation request successfully removed."});
                }
                else
                {
                    return NotFound($"Operation request with ID {id} not found or wrong authorization.");
                }
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }

        // POST: api/OperationRequest/Delete-OperationRequest
        [HttpPut]
        [Route("Delete-OperationRequestFromPatient")]
       /*  [Authorize(Policy = "Doctor")] */
        public async Task<ActionResult> DeleteOperationRequestFromPatient([FromBody] AddOrRemoveFromPatientDto removingFromPatientDto) 
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Doctor"))
                {
                    return BadRequest(new {message = "Invalid authorization or user role."});
                }

                var email = _userSvc.DecodeJwtToken(token).Email;

                bool result = await _service.DeleteOperationRequestFromPatient(removingFromPatientDto.PatientId, removingFromPatientDto.OperationRequestId, email);
                if (result)
                {
                    return Ok("Operation request successfully removed from patient profile.");
                }
                else
                {
                    return NotFound($"Operation request with ID {removingFromPatientDto.PatientId} not found or wrong authorization.");
                }
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }

        // POST: api/OperationRequest/Add-OperationRequestToPatient
        [HttpPost]
        [Route("Add-OperationRequestToPatient")]
        /* [Authorize(Policy = "Doctor")] */
        public async Task<ActionResult> AddOperationRequestToPatient([FromBody] AddOrRemoveFromPatientDto addOrRemoveFromPatientDto) 
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Doctor"))
                {
                    return BadRequest(new {message = "Invalid authorization or user role."});
                }

                var email = _userSvc.GetLoggedInEmail(token);

                bool result = await _service.AddOperationRequestToPatient(addOrRemoveFromPatientDto.PatientId, addOrRemoveFromPatientDto.OperationRequestId, email);
                if (result)
                {
                    return Ok(new { message = "Operation request successfully add." });
                }
                else
                {
                    return NotFound($"Operation request with ID {addOrRemoveFromPatientDto.PatientId} not found or wrong authorization.");
                }
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }

        [HttpPost]
        [Route("Schedule")]

        public async Task<ActionResult> Schedule([FromBody] OperationRequestScheduleInfoDto operationRequestScheduleInfoDto) 
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Admin"))
                {
                    return BadRequest(new {message = "Invalid authorization or user role."});
                }

                var result = await _service.Schedule(operationRequestScheduleInfoDto);
                if (!result.IsNullOrEmpty())
                {
                    return Ok(new {message = result});
                }
                else
                {
                    return NotFound($"Operation request not found or wrong authorization.");
                }
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                    return BadRequest(new { V = $"An unexpected error occurred: {ex.Message}" });
            }
        }

    }
}