using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;
using Xunit;
using Moq;
using Moq.EntityFrameworkCore;

public class UsersRepositoryTests
{
    [Fact]
    public async Task GetUsers_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { UserId = 1, UserName = "user1", Password = "pass1", FirstName = "John", LastName = "Doe" },
            new User { UserId = 2, UserName = "user2", Password = "pass2", FirstName = "Jane", LastName = "Smith" }
        };

        var mockContext = new Mock<WebApiShopContext>();
        mockContext.Setup(x => x.Users).ReturnsDbSet(users);

        var repository = new UsersRepository(mockContext.Object);

        // Act
        var result = await repository.GetUsers();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, u => u.UserName == "user1");
    }

    [Fact]
    public async Task GetUserById_ReturnsCorrectUser()
    {
        // Arrange
        var users = new List<User>
        {
            new User { UserId = 1, UserName = "user1" },
            new User { UserId = 2, UserName = "user2" }
        };

        var mockContext = new Mock<WebApiShopContext>();
        mockContext.Setup(x => x.Users).ReturnsDbSet(users);

        var repository = new UsersRepository(mockContext.Object);

        // Act
        var user = await repository.GetUserById(1);

        // Assert
        Assert.NotNull(user);
        Assert.Equal("user1", user.UserName);
    }

    [Fact]
    public async Task Login_ReturnsUser_WhenCredentialsMatch()
    {
        // Arrange
        var users = new List<User>
        {
            new User { UserId = 1, UserName = "user1", Password = "pass1" }
        };

        var mockContext = new Mock<WebApiShopContext>();
        mockContext.Setup(x => x.Users).ReturnsDbSet(users);

        var repository = new UsersRepository(mockContext.Object);
        var loginUser = new User { UserName = "user1", Password = "pass1" };

        // Act
        var result = await repository.Login(loginUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("user1", result.UserName);
    }

    [Fact]
    public async Task CreateUser_AddsUserAndSaves()
    {
        // Arrange
        var users = new List<User>(); 
        var mockContext = new Mock<WebApiShopContext>();
        mockContext.Setup(x => x.Users).ReturnsDbSet(users);

        var repository = new UsersRepository(mockContext.Object);
        var newUser = new User { UserName = "newuser", Password = "newpass" };

        // Act
        var result = await repository.CreateUser(newUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("newuser", result.UserName);
        mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once());
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsNull()
    {
        // Arrange
        var users = new List<User>
    {
        new User { UserId = 1, UserName = "testUser", Password = "correctPassword" }
    };

        var mockContext = new Mock<WebApiShopContext>();
        mockContext.Setup(x => x.Users).ReturnsDbSet(users);

        var repository = new UsersRepository(mockContext.Object);
        var loginAttempt = new User { UserName = "testUser", Password = "wrongPassword" };

        // Act
        var result = await repository.Login(loginAttempt);

        // Assert
        Assert.Null(result); 
    }
}