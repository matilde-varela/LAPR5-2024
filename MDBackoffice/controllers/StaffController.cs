using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    // [Authorize(Policy = "Staff")]
    public class StaffController : ControllerBase
    {
        private readonly StaffService _service;
        private readonly UserService _userSvc;
        public StaffController(StaffService service, UserService svc)
        {
            _service = service;
            _userSvc = svc;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StaffDto>> GetStaffById(string id)
        {

            var staff = await _service.GetByIdAsync(new StaffId(id));
            if (staff == null)
            {
                return NotFound();
            }
            return Ok(staff);
        }

        // POST: api/Staff/Create-StaffProfile
        [HttpPost]
        [Route("Create-StaffProfile")]
        //    [Authorize(Policy = "Admin")]
        public async Task<ActionResult<StaffDto>> CreateStaffProfile(CreatingStaffDto dto)
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Admin")) 
                {
                    return BadRequest("Invalid authorization or user role.");
                }

                var staff = await _service.CreateStaffProfile(dto);

                return Ok(staff);
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

        [HttpGet]
        [Route("Get-StaffProfiles")]
        public async Task<ActionResult<IEnumerable<StaffDto>>> GetStaffProfiles()
        {
            return await _service.GetAllAsync();
        }

        [HttpGet]
        [Route("Get-ActiveStaffProfiles")]
        public async Task<ActionResult<IEnumerable<StaffWithFunctionDto>>> GetActiveStaffProfiles()
        {
            return await _service.GetAllActiveAsync();
        }


        // PUT: api/Staff/5
        [HttpPut("{id}")]
        //  [Authorize(Policy = "Admin")]
        public async Task<ActionResult<StaffDto>> EditStaffProfile(string id, EditStaffDto dto)
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Admin"))
                {
                    return BadRequest("Invalid authorization or user role.");
                }

                var staff = await _service.UpdateAsync(id, dto);
                if (staff == null)
                {
                    return NotFound();
                }

                return Accepted(staff);
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

        [HttpPut("activate-staffProfile")]
        public async Task<IActionResult> ConfirmEmailStaff([FromQuery] string userId, [FromQuery] string staffId, [FromQuery] string token)
        {
            try
            {
                await _service.ConfirmEmailStaff(userId, staffId, token);
                return Ok(new { message ="Email confirmed successfully and contact information changed."});
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

        // POST: api/Staff/Filtered-List
        [HttpPost("Filtered-List")]
        // [Authorize(Policy = "Admin")]
        public async Task<ActionResult<List<StaffDto>>> GetFilteredStaffProfiles(StaffQueryParametersDto dto)
        {
            var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

            if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Admin"))
            {
                return BadRequest("Invalid authorization or user role.");
            }

            var staff = await _service.FilterStaffProfiles(dto);
            if (staff.IsNullOrEmpty())
            {
                return NotFound(new { Message = "No staff matching the filtering criteria." });
            }

            return Ok(staff);
        }

        // POST: api/Staff/Deactivate-StaffProfile
        [HttpPut]
        [Route("Deactivate-StaffProfile")]
        // [Authorize(Policy = "Admin")]
        public async Task<ActionResult> DeactivateStaffProfile([FromBody] IdPassDto idPassDto)
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Admin"))
                {
                    return BadRequest("Invalid authorization or user role.");
                }

                bool result = await _service.DeactivateStaffProfile(idPassDto.Id);
                if (result)
                {
                    return Ok(new { message = "Staff deactivated successfully." });
                }
                else
                {
                    return NotFound($"Staff with ID {idPassDto.Id} not found.");
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
        [Route("Add-TimeSlots")]
        public async Task<ActionResult> AddTimeSlots([FromBody] AddTimeSlotsDto addTimeSlotsDto) 
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Doctor"))
                {
                    return BadRequest(new {message = "Invalid authorization or user role."});
                }

                var email = _userSvc.DecodeJwtToken(token).Email;

                bool result = await _service.AddTimeSlots(addTimeSlotsDto, email);
                if (result)
                {
                    return Ok(result);
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
                return StatusCode(500, new { ex.Message });
            }
        }

    }
}