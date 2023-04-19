using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TO_DO.Data;
using TO_DO.DTOs;
using TO_DO.DTOs.Pagination;
using TO_DO.Models;
using TO_DO.Servises;

namespace ToDoUnitTest;

public class Calculator
{
    public int Add(int a, int b) => a + b;
}

class FakeEmailSender : IEmailSender
{
    public Task SendEmail(string to, string subject, string body)
    {
        return Task.CompletedTask;
    }
}

public class UnitTest1
{

    public static IEnumerable<object[]> AddData()
    {
        yield return new object[] { 1, 2, 3 };
        yield return new object[] { 2, 2, 4 };
        yield return new object[] { -3, -3, -6 };
        yield return new object[] { 10, 20, 30 };

    }

    [Theory]
    [MemberData(nameof(AddData))]
    //[InlineData(1, 2, 3)]
    //[InlineData(0, 0, 0)]
    //[InlineData(-3, 0, -3)]
    //[InlineData(-3, -3, -6)]

    public void Add_ReturnResult(int a, int b, int expectResult)
    {
        var calculator = new Calculator();

        var actualResult = calculator.Add(a, b);

        Assert.Equal(expectResult, actualResult);
    }


    [Fact]
    public async Task GetToDoItem_ReturnToDoItemWhichBelongToUser()
    {
        // Arrange
        var dbContext = new ToDoDbContext(new DbContextOptionsBuilder<ToDoDbContext>()
            .UseInMemoryDatabase("test").Options);
        var user = dbContext.Users.Add(new AppUser
        {
            UserName = "Test",
            Email = "test@gmail.com"
        }).Entity;

        var emailSender = new FakeEmailSender();

        var createdToDoItem = dbContext.ToDoItems.Add(new ToDoItem
        {
            Text = "Test",
            UserId = user.Id
        }).Entity;
        await dbContext.SaveChangesAsync();

        var service = new ToDoService(dbContext, emailSender);

        // Act

        var retrivedToDoItem = await service.GetToDoItem(user.Id, createdToDoItem.Id);

        // Assert
        Assert.NotNull(retrivedToDoItem);
    }

    [Fact]
    public async Task GetToDoItem_ReturnToDoItemWhichNotBelongToUser()
    {
        // Arrange
        var dbContext = new ToDoDbContext(new DbContextOptionsBuilder<ToDoDbContext>()
            .UseInMemoryDatabase("test").Options);
        var user = dbContext.Users.Add(new AppUser
        {
            UserName = "Test",
            Email = "test@gmail.com"
        }).Entity;
        var emailSender = new FakeEmailSender();
        var anotherUser = dbContext.Users.Add(new AppUser
        {
            UserName = "Test",
            Email = "test@gmail.com"
        }).Entity;

        var createdToDoItem = dbContext.ToDoItems.Add(new ToDoItem
        {
            Text = "Test",
            UserId = user.Id
        }).Entity;
        await dbContext.SaveChangesAsync();

        var service = new ToDoService(dbContext, emailSender);

        // Act

        var retrivedToDoItem = await service.GetToDoItem(anotherUser.Id, createdToDoItem.Id);

        // Assert
        Assert.Null(retrivedToDoItem);
    }


    [Fact]
    public async Task GetToDoItem_ReturnPaginatedToDoItemsWhichBelongToUser()
    {
        // Arrange
        var dbContext = new ToDoDbContext(new DbContextOptionsBuilder<ToDoDbContext>()
            .UseInMemoryDatabase("test").Options);
        var user = dbContext.Users.Add(new AppUser
        {
            UserName = "Test",
            Email = "test@gmail.com"
        }).Entity;
        var emailSender = new FakeEmailSender();
        Enumerable
             .Range(1, 5)
             .Select(i => new ToDoItem
             {
                 Text = $"Test {i}",
                 UserId = user.Id
             })
             .ToList()
             .ForEach(todo => dbContext.ToDoItems.Add(todo));

        await dbContext.SaveChangesAsync();

        var service = new ToDoService(dbContext, emailSender);

        // Act

        var retrivedToDoItems = await service.GetToDoItems(user.Id, 1, 3, null, null);

        // Assert
        retrivedToDoItems.Should().NotBeNull();
        retrivedToDoItems.Meta.Should().BeEquivalentTo(new PaginationMeta(1, 3, 5));

        retrivedToDoItems.Items.Should().HaveCount(3)
            .And.ContainSingle(item => item.Text == "Test 1")
            .And.ContainSingle(item => item.Text == "Test 2")
            .And.ContainSingle(item => item.Text == "Test 3");
    }

    [Fact]
    public async Task CreateToDoItem_CreateNewItem_And_SendEmailNotificaton()
    {
        var dbContext = new ToDoDbContext(new DbContextOptionsBuilder<ToDoDbContext>()
            .UseInMemoryDatabase("test").Options);
        var user = dbContext.Users.Add(new AppUser
        {
            UserName = "Test",
            Email = "test@gmail.com"
        }).Entity;

        var createdToDoItem = new CreateToDoItemRequest { Text = "Test" };  
            

        
        await dbContext.SaveChangesAsync();

        var emailSender = new Mock<IEmailSender>(MockBehavior.Strict);
        emailSender.Setup(e => e.SendEmail(user.Email, It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        
        var service = new ToDoService(dbContext, emailSender.Object);

        var retrivedToDoItem = await service.CreateTodoItem(user.Id, createdToDoItem);

        retrivedToDoItem.Should().NotBeNull();

        emailSender.VerifyAll();
        //emailSender.Verify(e=> e.SendEmail(user.Email, It.IsAny<string>(), It.IsAny<string>()));
        
    }
}