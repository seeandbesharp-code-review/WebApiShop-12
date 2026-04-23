using Entities;
namespace Repository
{
    public interface IUsersRepository
    {
        Task<IEnumerable<User>> GetUsers();
        Task<User> CreateUser(User user);
        Task<User?> GetUserById(int id);
        Task<User?> Login(User loggedUser);
        Task UpdateUser(int id, User loggedUser);
        Task<bool> UserWithSameEmail(string email, int id);
    }
}