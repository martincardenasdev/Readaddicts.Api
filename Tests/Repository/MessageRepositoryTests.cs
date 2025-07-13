using Domain.Dto;
using Domain.Entities;
using FluentAssertions;
using Infrastructure;
using Infrastructure.Hubs;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Tests.Repository
{
    public class MessageRepositoryTests
    {
        private readonly IHubContext<ChatHub> _hubContext;
        public MessageRepositoryTests()
        {
            var mockHubContext = new Mock<IHubContext<ChatHub>>();
            _hubContext = mockHubContext.Object;
        }
        private static DbContextOptions<ApplicationDbContext> GetInMemoryDatabaseOptions
        {
            get
            {
                return new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;
            }
        }

        private static readonly IConfiguration config;
        private static async Task<ApplicationDbContext> GetApplicationDbContext()
        {
            var dbContext = new ApplicationDbContext(GetInMemoryDatabaseOptions, config);
            await dbContext.Database.EnsureCreatedAsync();

            if (!await dbContext.Messages.AnyAsync())
            {
                var messages = new List<Message>
                {
                    new() { Id = "1", SenderId = "1", ReceiverId = "2", Content = "Message 1", Timestamp = DateTimeOffset.UtcNow, IsRead = false },
                    new() { Id = "2", SenderId = "1", ReceiverId = "2", Content = "Message 2", Timestamp = DateTimeOffset.UtcNow, IsRead = false },
                    new() { Id = "3", SenderId = "1", ReceiverId = "2", Content = "Message 3", Timestamp = DateTimeOffset.UtcNow, IsRead = false },
                    new() { Id = "4", SenderId = "1", ReceiverId = "2", Content = "Message 4", Timestamp = DateTimeOffset.UtcNow, IsRead = false },
                    new() { Id = "5", SenderId = "1", ReceiverId = "2", Content = "Message 5", Timestamp = DateTimeOffset.UtcNow, IsRead = false },
                    new() { Id = "6", SenderId = "2", ReceiverId = "1", Content = "Message 6", Timestamp = DateTimeOffset.UtcNow, IsRead = false },
                    new() { Id = "7", SenderId = "2", ReceiverId = "1", Content = "Message 7", Timestamp = DateTimeOffset.UtcNow, IsRead = false },
                    new() { Id = "8", SenderId = "2", ReceiverId = "1", Content = "Message 8", Timestamp = DateTimeOffset.UtcNow, IsRead = false },
                    new() { Id = "9", SenderId = "2", ReceiverId = "1", Content = "Message 9", Timestamp = DateTimeOffset.UtcNow, IsRead = false },
                    new() { Id = "10", SenderId = "2", ReceiverId = "1", Content = "Message 10", Timestamp = DateTimeOffset.UtcNow, IsRead = false }
                };

                await dbContext.Messages.AddRangeAsync(messages);
            }

            if (!await dbContext.Users.AnyAsync())
            {
                var users = new List<User>
                {
                    new() { Id = "1", UserName = "User 1", Email = "Email 1"},
                    new() { Id = "2", UserName = "User 2", Email = "Email 2"}
                };

                await dbContext.Users.AddRangeAsync(users);
            }

            await dbContext.SaveChangesAsync();

            return dbContext;
        }

        [Fact]
        public async Task GetUserMessages_ReturnsMessageList()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var messageRepository = new MessageRepository(dbContext, _hubContext);

            // Act
            var result = await messageRepository.GetUserMessages("2");

            // Assert
            result.Should().BeOfType<List<Message>>();
            result.Should().HaveCount(5);

            foreach (var message in result)
            {
                message.Should().BeOfType<Message>();
                message.Sender.Should().BeOfType<User>();
                message.Receiver.Should().BeOfType<User>();
                message.Sender.Id.Should().Be("1");
                message.Receiver.Id.Should().Be("2");
            }
        }

        [Fact]
        public async Task GetUserMessages_ReturnsEmptyList()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var messageRepository = new MessageRepository(dbContext, _hubContext);

            // Act
            var result = await messageRepository.GetUserMessages("999");

            // Assert
            result.Should().BeOfType<List<Message>>();
            result.Should().HaveCount(0);
        }

        [Fact]
        public async Task Send_ReturnsMessage()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();

            // Dont ask how this works copilot did it
            var mockClientProxy = new Mock<IClientProxy>();
            mockClientProxy.Setup(clientProxy => clientProxy.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var mockClients = new Mock<IHubClients>();
            mockClients.Setup(clients => clients.User(It.IsAny<string>())).Returns(mockClientProxy.Object);

            var mockHubContext = new Mock<IHubContext<ChatHub>>();
            mockHubContext.Setup(hubContext => hubContext.Clients).Returns(mockClients.Object);
            var messageRepository = new MessageRepository(dbContext, mockHubContext.Object);

            // Act
            var result = await messageRepository.Send("2", "1", "Message 6");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<MessageDto>();
            result.Content.Should().Be("Message 6");
        }

        [Fact]
        public async Task Send_ReturnsNull()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var messageRepository = new MessageRepository(dbContext, _hubContext);

            // Act
            var result = await messageRepository.Send("2", "999", "Message 6");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetConversation_ReturnsMessageDtoList()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var messageRepository = new MessageRepository(dbContext, _hubContext);

            // Act
            var result = await messageRepository.GetConversation(1, 5, "1", "2");

            // Assert
            result.Should().BeOfType<List<MessageDto>>();
            result.Should().HaveCount(5);
            foreach (var message in result)
            {
                message.Should().BeOfType<MessageDto>();
                message.SenderId.Should().Be("2");
                message.ReceiverId.Should().Be("1");
            }
        }

        [Fact]
        public async Task GetConversation_ReturnsEmptyList()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var messageRepository = new MessageRepository(dbContext, _hubContext);

            // Act
            var result = await messageRepository.GetConversation(1, 5, "999", "2");

            // Assert
            result.Should().BeOfType<List<MessageDto>>();
            result.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetRecentChats_ReturnsUsersList()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var messageRepository = new MessageRepository(dbContext, _hubContext);

            // Act
            var result = await messageRepository.GetRecentChats("1");

            // Assert
            result.Should().BeOfType<List<UserDto>>();
            result.Should().HaveCount(1);
            foreach (var user in result)
            {
                user.Should().BeOfType<UserDto>();
                user.Id.Should().Be("2");
            }
        }

        [Fact]
        public async Task GetRecentChats_ReturnsEmptyList()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var messageRepository = new MessageRepository(dbContext, _hubContext);

            // Act
            var result = await messageRepository.GetRecentChats("999");

            // Assert
            result.Should().BeOfType<List<UserDto>>();
            result.Should().HaveCount(0);
        }

        [Fact]
        public async Task ReadMessages_ReturnsDeletedMessagesCount()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var messageRepository = new MessageRepository(dbContext, _hubContext);

            // Act
            var result = await messageRepository.ReadMessages("1", "2");

            // Assert
            result.Should().Be(5);
        }

        [Fact]
        public async Task GetMessageNotificationCount_ReturnsCount()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            // Dont ask how this works copilot did it
            var mockClientProxy = new Mock<IClientProxy>();
            mockClientProxy.Setup(clientProxy => clientProxy.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var mockClients = new Mock<IHubClients>();
            mockClients.Setup(clients => clients.User(It.IsAny<string>())).Returns(mockClientProxy.Object);

            var mockHubContext = new Mock<IHubContext<ChatHub>>();
            mockHubContext.Setup(hubContext => hubContext.Clients).Returns(mockClients.Object);
            var messageRepository = new MessageRepository(dbContext, mockHubContext.Object);

            // Act
            var result = await messageRepository.GetMessageNotificationCount("1");

            // Assert
            result.Should().Be(5);
        }

        [Fact]
        public async Task UpdateUserLastLogin_ReturnsTrue()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var messageRepository = new MessageRepository(dbContext, _hubContext);

            // Act
            var result = await messageRepository.UpdateUserLastLogin("1");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateUserLastLogin_ReturnsFalse()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var messageRepository = new MessageRepository(dbContext, _hubContext);

            // Act
            var result = await messageRepository.UpdateUserLastLogin("999");

            // Assert
            result.Should().BeFalse();
        }
    }
}
