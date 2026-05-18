using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCity.Application;
using SmartCity.Application.DTOs;

namespace SmartCity_WebServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RetailPropertyController : ControllerBase
    {
        private readonly IRetailPropertyService _retailPropertyService;

        public RetailPropertyController(IRetailPropertyService retailPropertyService)
        {
            _retailPropertyService = retailPropertyService;
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Broker")]
        public async Task<IActionResult> Create([FromBody] RetailPropertyRequestDto retailProperty)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _retailPropertyService.Create(retailProperty);
            return Ok(result);
        }

        [HttpPut("Update/{id:int}")]
        [Authorize(Roles = "Broker")]
        public async Task<IActionResult> Update(int id, [FromBody] RetailPropertyRequestDto retailProperty)
        {
            if (id <= 0)
                return BadRequest("Invalid property ID.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            retailProperty.Id = id;
            var result = await _retailPropertyService.Update(retailProperty);

            if (result == null)
                return NotFound($"Property with ID {id} not found.");

            return Ok(result);
        }

        [HttpGet("Properties")]
        [Authorize(Roles = "Broker,HouseSeeker")]
        public async Task<IActionResult> GetProperties()
        {
            try
            {
                var result = await _retailPropertyService.GetRetailProperties();

                if (result == null || !result.Any())
                    return NotFound("No properties found.");

                return Ok(result);
            }
            catch(Exception ex)
            {
                return StatusCode(500, "Internal error occured.");
            }
        }

        [HttpDelete("Delete/{id:int}")]
        [Authorize(Roles = "Broker")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid property ID.");

            await _retailPropertyService.Delete(id);

            return Ok();
        }
    }
}