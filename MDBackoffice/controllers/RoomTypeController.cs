using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using MDBackoffice.Domain.Rooms;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.RoomTypes;
using MDBackoffice.Domain.Users;

namespace MDBackoffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypeController : ControllerBase
    {
        private readonly RoomTypeService _service;
        private readonly UserService _userService;

        public RoomTypeController(RoomTypeService service, UserService userService)
        {
            _service = service;
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<RoomTypeDto>> Create(RoomTypeDto dto)
        {
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString()?.Split(' ')[1];

                if (string.IsNullOrWhiteSpace(token) || _userService.CheckUserRole(token, "Admin"))
                {
                    return BadRequest("Invalid authorization or user role.");
                }
                var roomType = await _service.AddAsync(dto);
                return Ok(roomType);
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

        [HttpGet("get-all")]
        public async Task<List<RoomTypeDto>> GetAll()
        {
            return await _service.GetAllAsync();
        }

    }

}