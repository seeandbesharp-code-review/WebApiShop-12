using Entities;
using DTOs;

namespace Services
{
    public interface IUsersService
    {
        Task<IEnumerable<UserDTO>> GetUsers();
        Task<AuthResultDTO?> CreateUser(UserDTO user);
        Task<UserDTO?> GetUserById(int id);
        Task<AuthResultDTO?> Login(LoginUserDTO loggedUser);
        Task UpdateUser(int id, UserDTO user);
        Task<bool> UserWithSameEmail(string email, int id = -1);
        bool IsPasswordStrong(string password);
    }
}