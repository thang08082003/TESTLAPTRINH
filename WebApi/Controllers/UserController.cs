using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using WebApi.Exceptions;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound(new { message = "Không tìm thấy người dùng" });

            return Ok(user);
        }

        // GET: api/User/search?term=nguyen
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<UserDto>>> SearchUsers([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return BadRequest(new { message = "Từ khóa tìm kiếm không được rỗng" });

            var users = await _userService.SearchUsersAsync(term);
            return Ok(users);
        }

        // GET: api/User/age-range?minAge=20&maxAge=40
        [HttpGet("age-range")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByAgeRange(
            [FromQuery] int minAge = 0,
            [FromQuery] int maxAge = 100)
        {
            var users = await _userService.GetUsersByAgeRangeAsync(minAge, maxAge);
            return Ok(users);
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createDto)
        {
            try
            {
                var user = await _userService.CreateUserAsync(createDto);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (BusinessRuleException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest(new { message = "ID không khớp" });

            try
            {
                await _userService.UpdateUserAsync(updateDto);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BusinessRuleException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (!result)
                return NotFound(new { message = "Không tìm thấy người dùng" });

            return NoContent();
        }
    }
}