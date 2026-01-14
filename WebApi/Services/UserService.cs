using Microsoft.EntityFrameworkCore;
using SharedLib.Models;
using WebApi.Data;
using WebApi.DTOs;
using WebApi.Exceptions;

namespace WebApi.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
         
            return await _context.Users
                .AsNoTracking()
                .OrderBy(u => u.Code)
                .Select(u => MapToDto(u))
                .ToListAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserDto?> GetUserByCodeAsync(string code)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Code == code);

            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createDto)
        {
          
            await ValidateUserBusinessRules(createDto.Code, createDto.Email);

            var user = new User
            {
                Code = createDto.Code,
                FullName = createDto.FullName,
                DateOfBirth = createDto.DateOfBirth,
                Email = createDto.Email,
                Phone = createDto.Phone,
                Address = createDto.Address
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return MapToDto(user);
        }

        public async Task<UserDto> UpdateUserAsync(UpdateUserDto updateDto)
        {
            var user = await _context.Users.FindAsync(updateDto.Id)
                ?? throw new NotFoundException($"Người dùng với ID {updateDto.Id} không tồn tại");

          
            await ValidateUserBusinessRules(updateDto.Code, updateDto.Email, updateDto.Id);

        
            user.Code = updateDto.Code;
            user.FullName = updateDto.FullName;
            user.DateOfBirth = updateDto.DateOfBirth;
            user.Email = updateDto.Email;
            user.Phone = updateDto.Phone;
            user.Address = updateDto.Address;

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return MapToDto(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

  
        public async Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u =>
                    u.Code.Contains(searchTerm) ||
                    u.FullName.Contains(searchTerm) ||
                    u.Email.Contains(searchTerm) ||
                    u.Phone.Contains(searchTerm))
                .OrderBy(u => u.FullName)
                .Select(u => MapToDto(u))
                .ToListAsync();
        }

     
        public async Task<IEnumerable<UserDto>> GetUsersByAgeRangeAsync(int minAge, int maxAge)
        {
            var today = DateTime.Today;
            var minBirthDate = today.AddYears(-maxAge);
            var maxBirthDate = today.AddYears(-minAge);

            return await _context.Users
                .AsNoTracking()
                .Where(u => u.DateOfBirth >= minBirthDate && u.DateOfBirth <= maxBirthDate)
                .OrderBy(u => u.DateOfBirth)
                .Select(u => MapToDto(u))
                .ToListAsync();
        }

       
        private async Task ValidateUserBusinessRules(string code, string email, int? excludeId = null)
        {
            var codeExists = await _context.Users
                .AnyAsync(u => u.Code == code && (excludeId == null || u.Id != excludeId));

            if (codeExists)
                throw new BusinessRuleException("Mã người dùng đã tồn tại");

            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == email && (excludeId == null || u.Id != excludeId));

            if (emailExists)
                throw new BusinessRuleException("Email đã tồn tại");
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Code = user.Code,
                FullName = user.FullName,
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address
            };
        }
    }
}