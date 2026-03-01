using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using Xunit;

public class UsersRepositoryTests
{
    private async Task<(WebApiShopContext context, UsersRepository repository)> TearUpAsync()
    {
        var options = new DbContextOptionsBuilder<WebApiShopContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var context = new WebApiShopContext(options);

        // Seed initial data
        context.Users.AddRange(
            new User { UserId = 1, UserName = "user1", Password = "pass1", FirstName = "John", LastName = "Doe" },
            new User { UserId = 2, UserName = "user2", Password = "pass2", FirstName = "Jane", LastName = "Smith" }
        );
        await context.SaveChangesAsync();

        var repository = new UsersRepository(context);
        return (context, repository);
    }

    private void TearDown(WebApiShopContext context)
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }

    [Fact]
    public async Task GetUsers_ReturnsAllUsers()
    {
        var (context, repository) = await TearUpAsync();
        try
        {
            var users = await repository.GetUsers();
            Assert.Equal(2, users.Count());
            Assert.Contains(users, u => u.UserName == "user1");
            Assert.Contains(users, u => u.UserName == "user2");
        }
        finally
        {
            TearDown(context);
        }
    }

    [Fact]
    public async Task GetUserById_ReturnsCorrectUser()
    {
        var (context, repository) = await TearUpAsync();
        try
        {
            var user = await repository.GetUserById(1);
            Assert.NotNull(user);
            Assert.Equal("user1", user.UserName);
        }
        finally
        {
            TearDown(context);
        }
    }

    [Fact]
    public async Task GetUserById_ReturnsNull_WhenNotFound()
    {
        var (context, repository) = await TearUpAsync();
        try
        {
            var user = await repository.GetUserById(99);
            Assert.Null(user);
        }
        finally
        {
            TearDown(context);
        }
    }

    [Fact]
    public async Task CreateUser_AddsUserAndSaves()
    {
        var (context, repository) = await TearUpAsync();
        try
        {
            var newUser = new User { UserName = "newuser", Password = "newpass", FirstName = "Alice", LastName = "Wonder" };
            var result = await repository.CreateUser(newUser);

            Assert.NotNull(result);
            Assert.Equal("newuser", result.UserName);

            var users = await repository.GetUsers();
            Assert.Equal(3, users.Count());
            Assert.Contains(users, u => u.UserName == "newuser");
        }
        finally
        {
            TearDown(context);
        }
    }

    [Fact]
    public async Task Login_ReturnsUser_WhenCredentialsMatch()
    {
        var (context, repository) = await TearUpAsync();
        try
        {
            var loginUser = new User { UserName = "user1", Password = "pass1" };
            var result = await repository.Login(loginUser);

            Assert.NotNull(result);
            Assert.Equal("user1", result.UserName);
        }
        finally
        {
            TearDown(context);
        }
    }

    [Fact]
    public async Task Login_ReturnsNull_WhenCredentialsDoNotMatch()
    {
        var (context, repository) = await TearUpAsync();
        try
        {
            var loginUser = new User { UserName = "user1", Password = "wrongpass" };
            var result = await repository.Login(loginUser);

            Assert.Null(result);
        }
        finally
        {
            TearDown(context);
        }
    }

    [Fact]
    public async Task UpdateUser_UpdatesUserAndSaves()
    {
        var (context, repository) = await TearUpAsync();
        try
        {
            var user = await repository.GetUserById(1);
            user.Password = "updatedpass";
            await repository.UpdateUser(1, user);

            var updatedUser = await repository.GetUserById(1);
            Assert.Equal("updatedpass", updatedUser.Password);
        }
        finally
        {
            TearDown(context);
        }
    }
}
