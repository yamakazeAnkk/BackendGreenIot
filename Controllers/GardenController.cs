using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using GreenIotApi.Services;
using GreenIotApi.DTOs;
using GreenIotApi.Models;
using GreenIotApi.DTOs.ModelEdits;

namespace GreenIotApi.Controllers
{
    [ApiController]
    [Route("api/gardens")]
    public class GardenController : ControllerBase
    {
        private readonly GardenService _gardenService;
        private readonly IMapper _mapper;

        public GardenController(GardenService gardenService, IMapper mapper)
        {
            _gardenService = gardenService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGardens()
        {
            var gardens = await _gardenService.GetAllGardensAsync();
            return Ok(gardens);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGardenById(string id)
        {
            var garden = await _gardenService.GetGardenAsync(id);
            return Ok(garden);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddGarden([FromForm] GardenModelEdit gardenModelEdit, IFormFile image)
        {
            if (gardenModelEdit == null)
            {
                return BadRequest("Garden data is required.");
            }

            Stream imageStream = null;
            string fileName = null;
            string contentType = null;

            if (image != null)
            {
                imageStream = image.OpenReadStream();
                fileName = image.FileName;
                contentType = image.ContentType; 
            }   

            var garden = _mapper.Map<Garden>(gardenModelEdit);
            var addedGarden = await _gardenService.AddGardenAsync(garden, imageStream, fileName, contentType);

            return Ok(addedGarden);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGarden(string id, GardenDto gardenDto)
        {
            var garden = _mapper.Map<Garden>(gardenDto);
            await _gardenService.UpdateGardenAsync(id, garden);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGarden(string id)
        {
            await _gardenService.DeleteGardenAsync(id);
            return NoContent();
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetGardensByUserId(string userId)
        {
            var gardens = await _gardenService.GetGardensByUserIdAsync(userId);
            return Ok(gardens);
        }

        [HttpGet("user/{userId}/garden")]
        public async Task<IActionResult> FilterGardensByUserIdAndGardenId(string userId = "mk97H9R96VSwZL3MKZK7vy0wAeA3", string name = "Cần Thơ")
        {
            var gardens = await _gardenService.FilterGardensByUserIdAndGardenIdAsync(userId, name);
            return Ok(gardens);
        }

    }
}
