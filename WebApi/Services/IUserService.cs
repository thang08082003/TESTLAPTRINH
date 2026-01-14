using WebApi.DTOs;

namespace WebApi.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> GetUserByCodeAsync(string code);
        Task<UserDto> CreateUserAsync(CreateUserDto createDto);
        Task<UserDto> UpdateUserAsync(UpdateUserDto updateDto);
        Task<bool> DeleteUserAsync(int id);
        Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm);
        Task<IEnumerable<UserDto>> GetUsersByAgeRangeAsync(int minAge, int maxAge);
    }
}