using Repository;
using AutoMapper;
using DTOs;
using Entities;
using System.Collections.Generic;
using System.Net.Security;
using Microsoft.Extensions.Logging;
namespace Services
{
    public class UsersService : IUsersService
    {
        public UsersService(IUsersRepository repository, IPasswordsService passwordsService, IMapper mapper, ILogger<UsersService> logger)
        {
            this._repository = repository;
            this.passwordsService = passwordsService;
            _mapper = mapper;
            _logger = logger;
        }
        IUsersRepository _repository;
        IPasswordsService passwordsService;
        IMapper _mapper;
        ILogger<UsersService> _logger;

        public async Task<IEnumerable<UserDTO>> GetUsers()
        {
            IEnumerable<User> users = await _repository.GetUsers();
            return  _mapper.Map<IEnumerable<User>,IEnumerable<UserDTO>>(users);
        }

        public async Task<UserDTO?> GetUserById(int id)
        {
            User? user = await _repository.GetUserById(id);
            return _mapper.Map<User, UserDTO>(user);
        }

        public async Task<UserDTO?> CreateUser(UserDTO user)
        {
            int Level = passwordsService.passwordValidation(user.Password);
            if (Level < 3)
                return null;
            User user1 = _mapper.Map<UserDTO, User>(user);
            user1 = await _repository.CreateUser(user1);
            return _mapper.Map<User, UserDTO>(user1);
        }
        public async Task<UserDTO?> Login(LoginUserDTO loggedUser)
        {
            User? user = _mapper.Map<LoginUserDTO, User>(loggedUser);
            user = await _repository.Login(user);
            _logger.LogInformation($"Login attempted with UserName {user?.UserName} and password {user?.Password}");
            return _mapper.Map<User,UserDTO>(user);
        }
        public async Task UpdateUser(int id, UserDTO user)
        {
            int Level = passwordsService.passwordValidation(user.Password);
            if (Level < 3)
                throw new("Password is too weak");
            User user1 = _mapper.Map<UserDTO,User>(user);
            await _repository.UpdateUser(id, user1);
        }
    }
}
