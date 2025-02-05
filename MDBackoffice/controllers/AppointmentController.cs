using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using MDBackoffice.Domain.Appointments;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.Users;
using MDBackoffice.Domain.Rooms;
using Newtonsoft.Json;

namespace MDBackoffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentService _service;
        private readonly UserService _userSvc;

        public AppointmentController(AppointmentService service, UserService svc)
        {
            _service = service;
            _userSvc = svc;
        }

        // POST: api/Appointment
        [HttpPost]
        public async Task<ActionResult<AppointmentDto>> Create(CreatingAppointmentDto dto)
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Doctor"))
                {
                    return BadRequest(new { message = "Invalid authorization or user role." });
                }

                var appointment = await _service.AddAsync(dto);
                return Ok(appointment);
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

        // POST: api/Appointment/get-all
        [HttpGet("get-all")]
        public async Task<List<AppointmentDto>> GetAll()
        {
            return await _service.GetAllAsync();
        }

        // POST: api/Appointment/get-all
        [HttpGet("get-by-id")]
        public async Task<ActionResult<AppointmentDto>> GetById(string id)
        {
            try
            {
                var dto = await _service.GetByIdAsync(id);
                return Ok(dto);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new
                {
                    ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { V = $"An unexpected error occurred: {ex.Message}" });
            }

        }

        // POST: api/Appointment/update-appointment
        [HttpPatch("update-appointment")]
        public async Task<ActionResult<AppointmentDto>> UpdateAppointment(UpdateAppointmentDto dto)
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Doctor"))
                {
                    return BadRequest(new { message = "Invalid authorization or user role." });
                }

                var appointment = await _service.UpdateAsync(dto);
                return Ok(appointment);
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

        // Get: api/Appointment/get-by-roomInfo
        [HttpPost("get-by-roomInfo")]
        public async Task<ActionResult<AppointmentWithoutStaffDto>> GetAppointmentForSimulation(AppointmentRoomInfoDto roomInfoDto)
        {
            try
            {
                var dto = await _service.GetAppointmentForSimulation(roomInfoDto);
                return Ok(dto.Value);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new
                {
                    ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { V = $"An unexpected error occurred: {ex.Message}" });
            }

        }

    }

}