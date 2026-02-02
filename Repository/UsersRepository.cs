using Entities;
using Microsoft.EntityFrameworkCore;


namespace Repository
{
    public class UsersRepository : IUsersRepository
    {
        WebApiShopContext _webApiShopContext;

        public UsersRepository(WebApiShopContext webApiShopContext)
        {
            _webApiShopContext = webApiShopContext;
        }
        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _webApiShopContext.Users.ToListAsync();
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _webApiShopContext.Users.FindAsync(id);
        }

        public async Task<User> CreateUser(User user)
        {
            await _webApiShopContext.Users.AddAsync(user);
            await _webApiShopContext.SaveChangesAsync();
            return user;
        }

        public async Task<User?> Login(User loggedUser)
        {
            return await _webApiShopContext.Users.Where(user => (user.UserName == loggedUser.UserName && user.Password == loggedUser.Password)).FirstOrDefaultAsync();
        }

        public async Task UpdateUser(int id, User loggedUser)
        {
            _webApiShopContext.Users.Update(loggedUser);
            await _webApiShopContext.SaveChangesAsync();
        }

    }
}
