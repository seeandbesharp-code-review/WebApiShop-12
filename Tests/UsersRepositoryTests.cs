using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using Xunit;

public class UsersRepositoryTests : IDisposable
{
    private readonly WebApiShopContext _context;
    private readonly UsersRepository _repository;

    // tearUp: create DB and seed data
    public UsersRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<WebApiShopContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new WebApiShopContext(options);

        // Seed initial data
        _context.Users.AddRange(
            new User { UserId = 1, UserName = "user1", Password = "pass1", FirstName = "John", LastName = "Doe" },
            new User { UserId = 2, UserName = "user2", Password = "pass2", FirstName = "Jane", LastName = "Smith" }
        );
        _context.SaveChanges();

        _repository = new UsersRepository(_context);
    }

    // tearDown: delete DB
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetUsers_ReturnsAllUsers()
    {
        var users = await _repository.GetUsers();
        Assert.Equal(2, users.Count());
        Assert.Contains(users, u => u.UserName == "user1");
        Assert.Contains(users, u => u.UserName == "user2");
    }

    [Fact]
    public async Task GetUserById_ReturnsCorrectUser()
    {
        var user = await _repository.GetUserById(1);
        Assert.NotNull(user);
        Assert.Equal("user1", user.UserName);
    }

    [Fact]
    public async Task GetUserById_ReturnsNull_WhenNotFound()
    {
        var user = await _repository.GetUserById(99);
        Assert.Null(user);
    }

    [Fact]
    public async Task CreateUser_AddsUserAndSaves()
    {
        var newUser = new User { UserName = "newuser", Password = "newpass", FirstName = "Alice", LastName = "Wonder" };
        var result = await _repository.CreateUser(newUser);

        Assert.NotNull(result);
        Assert.Equal("newuser", result.UserName);

        var users = await _repository.GetUsers();
        Assert.Equal(3, users.Count());
        Assert.Contains(users, u => u.UserName == "newuser");
    }

    [Fact]
    public async Task Login_ReturnsUser_WhenCredentialsMatch()
    {
        var loginUser = new User { UserName = "user1", Password = "pass1" };
        var result = await _repository.Login(loginUser);

        Assert.NotNull(result);
        Assert.Equal("user1", result.UserName);
    }

    [Fact]
    public async Task Login_ReturnsNull_WhenCredentialsDoNotMatch()
    {
        var loginUser = new User { UserName = "user1", Password = "wrongpass" };
        var result = await _repository.Login(loginUser);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateUser_UpdatesUserAndSaves()
    {
        var user = await _repository.GetUserById(1);
        user.Password = "updatedpass";
        await _repository.UpdateUser(1, user);

        var updatedUser = await _repository.GetUserById(1);
        Assert.Equal("updatedpass", updatedUser.Password);
    }
}
