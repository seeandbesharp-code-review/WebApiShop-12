using Entities;
namespace Repository
{
    public interface IUsersRepository
    {
        Task<IEnumerable<User>> GetUsers();
        Task<User> CreateUser(User user);
        Task<User?> GetUserById(int id);
        Task<User?> GetByUserName(string userName);
        Task UpdateUser(int id, User loggedUser);
        Task<bool> UserWithSameEmail(string email, int id);
    }
}