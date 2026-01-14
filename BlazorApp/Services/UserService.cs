using SharedLib.Models;
using System.Net.Http.Json;

namespace BlazorApp.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                var users = await _httpClient.GetFromJsonAsync<List<User>>("api/User");
                return users ?? new List<User>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting users: {ex.Message}");
                return new List<User>();
            }
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<User>($"api/User/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<User?> CreateUserAsync(User user)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/User", user);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<User>();
                }

                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error creating user: {error}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateUserAsync(int id, User user)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/User/{id}", user);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error updating user: {error}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/User/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error deleting user: {error}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
                return false;
            }
        }
    }
}
