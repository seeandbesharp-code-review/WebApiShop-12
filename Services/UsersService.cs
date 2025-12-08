using Repository;
using System.Net.Security;
using Entities;
namespace Services
{
    public class UsersService : IUsersService
    {
        IUsersRepository repository;
        IPasswordsService passwordsService;

        public UsersService(IUsersRepository repository, IPasswordsService passwordsService)
        {
            this.repository = repository;
            this.passwordsService = passwordsService;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await repository.GetUsers();
        }

        public async Task<User?> GetUserById(int id)
        {
            return await repository.GetUserById(id);
        }

        public async Task<User?> CreateUser(User user)
        {
            int Level = passwordsService.passwordValidation(user.Password);
            if (Level < 3)
                return null;
            return await repository.CreateUser(user);
        }
        public async Task<User?> Login(User loggedUser)
        {
            return await repository.Login(loggedUser);
        }
        public async Task UpdateUser(int id, User user)
        {
            int Level = passwordsService.passwordValidation(user.Password);
            if (Level < 3)
                throw new InvalidOperationException("Password is too weak");
            await repository.UpdateUser(id, user);
        }
    }
}
