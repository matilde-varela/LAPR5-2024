using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using MDBackoffice.Domain.Specializations;
using System.Collections.Generic;
using MDBackoffice.Domain.Users;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpecializationController : ControllerBase
    {
        private readonly SpecializationService _service;
        private readonly UserService _userSvc;
        public SpecializationController(SpecializationService service, UserService svc)
        {
            _service = service;
            _userSvc = svc;
        }

        // GET: api/Specializations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SpecializationDto>> GetGetById(string id)
        {
            var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

            if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Admin"))
            {
                return BadRequest("Invalid authorization or user role.");
            }

            var Specialization = await _service.GetByIdAsync(new SpecializationCode(id));

            if (Specialization == null)
            {
                return NotFound();
            }

            return Specialization;
        }

        [HttpGet("filtered")]
        public async Task<ActionResult<List<SpecializationDto>>> GetSpecializationsByFilters([FromQuery] string? code = null,
                                                                                             [FromQuery] string? denomination = null,
                                                                                             [FromQuery] string? description = null)
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Admin"))
                {
                    return BadRequest("Invalid authorization or user role.");
                }

                var Specialization = await _service.GetSpecializationsByFiltersAsync(code, denomination, description);
                return Ok(Specialization);
            }
            catch (Exception ex)
            {
                return BadRequest($"An unexpected error occurred ({ex.Message})");
            }
        }
        [HttpPost]
        public async Task<ActionResult<SpecializationDto>> Create(SpecializationDto dto)
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Admin"))
                {
                    return BadRequest("Invalid authorization or user role.");
                }
                var specialization = await _service.AddAsync(dto);

                return CreatedAtAction(nameof(GetGetById), new { id = new SpecializationCode(specialization.Code) }, specialization);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(ex.ToString());
            }
            catch (Exception ex)
            {
                return BadRequest($"An unexpected error occurred ({ex.Message})");
            }
        }

        [HttpPut]
        public async Task<ActionResult<SpecializationDto>> EditSpecialization(EditSpecializationDto dto)
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Admin"))
                {
                    return BadRequest("Invalid authorization or user role.");
                }
                var specialization = await _service.EditSpecialization(dto);

                return Accepted(specialization);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(ex.ToString());
            }
            catch (Exception ex)
            {
                return BadRequest($"An unexpected error occurred ({ex.Message})");
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<SpecializationDto>> DeleteSpecialization(string id)
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userSvc.CheckUserRole(token, "Admin"))
                {
                    return BadRequest("Invalid authorization or user role.");
                }

                await _service.DeleteAsync(id);
                return Ok(new { message = "Specialization was successfully deleted." });
            }
            catch (Exception ex)
            {
                return BadRequest($"An unexpected error occurred ({ex.Message})");
            }
        }
    }
}