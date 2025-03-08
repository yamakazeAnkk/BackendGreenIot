using GreenIotApi.Models;
using GreenIotApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GreenIotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        // Inject UserService vào Controller
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // API POST để thêm người dùng mới
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            try
            {
                if (user == null || string.IsNullOrEmpty(user.Id))
                {
                    return BadRequest("User data or Id is null");
                }

                // Gọi service để thêm người dùng vào Firestore với documentId là user.Id
                var addedUser = await _userService.AddUserAsync(user);

                // Trả về thông tin người dùng mới với documentId (trong trường hợp này là user.Id)
                return Ok(new { message = "User added successfully", userId = addedUser.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        [HttpGet("{documentId}")]
        public async Task<IActionResult> GetUserById(string documentId)
        {
            var user = await _userService.GetUserByIdAsync(documentId);
            return Ok(user);
        }
    }
}
