using Entities;
using DTOs;

namespace Services
{
    public interface IUsersService
    {
        Task<IEnumerable<UserDTO>> GetUsers();
        Task<UserDTO?> CreateUser(UserDTO user);
        Task<UserDTO?> GetUserById(int id);
        Task<UserDTO?> Login(LoginUserDTO loggedUser);
        Task UpdateUser(int id, UserDTO user);
    }
}