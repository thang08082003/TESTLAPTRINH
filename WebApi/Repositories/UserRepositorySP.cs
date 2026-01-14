using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SharedLib.Models;
using WebApi.Data;

namespace WebApi.Repositories
{
    public class UserRepositorySP : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepositorySP(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .FromSqlRaw("EXEC sp_GetAllUsers")
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            var parameter = new SqlParameter("@UserId", id);

            var users = await _context.Users
                .FromSqlRaw("EXEC sp_GetUserById @UserId", parameter)
                .ToListAsync();

            return users.FirstOrDefault();
        }

        public async Task<User> CreateAsync(User user)
        {
            var parameters = new[]
            {
                new SqlParameter("@Code", user.Code),
                new SqlParameter("@FullName", user.FullName),
                new SqlParameter("@DateOfBirth", user.DateOfBirth),
                new SqlParameter("@Email", user.Email),
                new SqlParameter("@Phone", user.Phone),
                new SqlParameter("@Address", user.Address),
                new SqlParameter("@NewUserId", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output }
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_CreateUser @Code, @FullName, @DateOfBirth, @Email, @Phone, @Address, @NewUserId OUTPUT",
                parameters);

            user.Id = (int)parameters[6].Value;
            return user;
        }

        // Implement other methods...

        public Task<User?> GetByCodeAsync(string code)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CodeExistsAsync(string code, int? excludeId = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            throw new NotImplementedException();
        }
    }
}