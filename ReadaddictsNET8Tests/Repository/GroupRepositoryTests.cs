using Application.Abstractions;
using Domain.Dto;
using Domain.Entities;
using FluentAssertions;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace ReadaddictsNET8Tests.Repository
{
    public class GroupRepositoryTests
    {
        private readonly Mock<ICloudinaryRepository> _cloudinaryMock;
        public GroupRepositoryTests()
        {
            _cloudinaryMock = new Mock<ICloudinaryRepository>();
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

            if (!await dbContext.Groups.AnyAsync())
            {
                var groups = new List<Group>
                {
                    new() { Id = "1", Name = "Group 1", CreatorId = "1", Created = DateTimeOffset.UtcNow },
                    new() { Id = "2", Name = "Group 2", CreatorId = "1", Created = DateTimeOffset.UtcNow },
                    new() { Id = "3", Name = "Group 3", CreatorId = "2", Created = DateTimeOffset.UtcNow },
                    new() { Id = "4", Name = "Group 4", CreatorId = "2", Created = DateTimeOffset.UtcNow },
                    new() { Id = "5", Name = "Group 5", CreatorId = "3", Created = DateTimeOffset.UtcNow },
                };

                await dbContext.Groups.AddRangeAsync(groups);
            }

            if (!await dbContext.Users.AnyAsync())
            {
                var users = new List<User>
                {
                    new() { Id = "1", UserName = "User 1", Email = "Email 1"},
                    new() { Id = "2", UserName = "User 2", Email = "Email 2"},
                    new() { Id = "3", UserName = "User 3", Email = "Email 3"},
                    new() { Id = "4", UserName = "User 4", Email = "Email 4"},
                    new() { Id = "5", UserName = "User 5", Email = "Email 5"},
                };

                await dbContext.Users.AddRangeAsync(users);
            }

            await dbContext.SaveChangesAsync();

            return dbContext;
        }

        [Fact]
        public async Task GetGroups_ReturnsGroupList()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var groupRepository = new GroupRepository(dbContext, _cloudinaryMock.Object);

            // Act
            var result = await groupRepository.GetGroups(1,5);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(5);
            result.Should().BeOfType<List<GroupDto>>();
        }

        [Fact]
        public async Task GetGroup_ReturnsGroup()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var groupRepository = new GroupRepository(dbContext, _cloudinaryMock.Object);

            // Act
            var result = await groupRepository.GetGroup("1");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<GroupDto>();
        }

        [Fact]
        public async Task GetGroup_ReturnsNull()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var groupRepository = new GroupRepository(dbContext, _cloudinaryMock.Object);

            // Act
            var result = await groupRepository.GetGroup("6");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateGroup_ReturnsGroupId()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var groupRepository = new GroupRepository(dbContext, _cloudinaryMock.Object);
            var group = new Group
            {
                Name = "Group 6",
                CreatorId = "1",
                Created = DateTimeOffset.UtcNow
            };

            // Act
            var result = await groupRepository.CreateGroup("1", group, null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<string>();
        }

        [Fact]
        public async Task CreateGroup_ReturnsNull()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var groupRepository = new GroupRepository(dbContext, _cloudinaryMock.Object);
            var group = new Group
            {
                Name = "",
                CreatorId = "1",
                Created = DateTimeOffset.UtcNow
            };

            // Act
            var result = await groupRepository.CreateGroup("6", group, null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateGroup_ReturnsTrue()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var groupRepository = new GroupRepository(dbContext, _cloudinaryMock.Object);
            var group = new Group
            {
                Name = "New group name",
                Description = "New group description"
            };

            // Act
            var result = await groupRepository.UpdateGroup("1", "1", group, null);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateGroup_ReturnsFalse()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var groupRepository = new GroupRepository(dbContext, _cloudinaryMock.Object);
            var group = new Group
            {
                Name = "",
                Description = ""
            };

            // Act
            var result = await groupRepository.UpdateGroup("1", "1", group, null);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteGroup_ReturnsTrue()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var groupRepository = new GroupRepository(dbContext, _cloudinaryMock.Object);

            // Act
            var result = await groupRepository.DeleteGroup("1", "1");

            // Assert
            result.Should().BeTrue();
        }
    }
}
