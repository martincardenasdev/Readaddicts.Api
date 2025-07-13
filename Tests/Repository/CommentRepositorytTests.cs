using Domain.Common;
using Domain.Dto;
using Domain.Entities;
using FluentAssertions;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Tests.Repository
{
    public class CommentRepositorytTests
    {
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

            if (!await dbContext.Comments.AnyAsync())
            {
                var comments = new List<Comment>
                {
                    new() { Id = "1", UserId = "1", PostId = "1", Content = "Comment 1", Created = DateTimeOffset.UtcNow },
                    new() { Id = "2", UserId = "1", PostId = "1", Content = "Comment 2", Created = DateTimeOffset.UtcNow },
                    new() { Id = "3", UserId = "1", PostId = "1", Content = "Comment 3", Created = DateTimeOffset.UtcNow },
                    new() { Id = "4", UserId = "1", PostId = "1", Content = "Comment 4", Created = DateTimeOffset.UtcNow },
                    new() { Id = "5", UserId = "1", PostId = "1", Content = "Comment 5", Created = DateTimeOffset.UtcNow },
                };

                var children = new List<Comment>
                {
                    new() { Id = "6", UserId = "1", PostId = "1", Content = "Comment 6", Created = DateTimeOffset.UtcNow, ParentId = "1" },
                    new() { Id = "7", UserId = "1", PostId = "1", Content = "Comment 7", Created = DateTimeOffset.UtcNow, ParentId = "1" },
                    new() { Id = "8", UserId = "1", PostId = "1", Content = "Comment 8", Created = DateTimeOffset.UtcNow, ParentId = "1" },
                };

                await dbContext.Comments.AddRangeAsync(comments);
                await dbContext.Comments.AddRangeAsync(children);
            }

            if (!await dbContext.Posts.AnyAsync())
            {
                var posts = new List<Post>
                {
                    new() { Id = "1", UserId = "1", Created = DateTimeOffset.UtcNow, Content = "Content 1" },
                    new() { Id = "2", UserId = "1", Created = DateTimeOffset.UtcNow, Content = "Content 2" },
                    new() { Id = "3", UserId = "1", Created = DateTimeOffset.UtcNow, Content = "Content 3" },
                    new() { Id = "4", UserId = "1", Created = DateTimeOffset.UtcNow, Content = "Content 4" },
                    new() { Id = "5", UserId = "1", Created = DateTimeOffset.UtcNow, Content = "Content 5" },
                };

                await dbContext.Posts.AddRangeAsync(posts);
            }

            if (!await dbContext.Users.AnyAsync())
            {
                var users = new List<User>
                {
                    new() { Id = "1", UserName = "User 1", Email = "Email 1" },
                    new() { Id = "2", UserName = "User 2", Email = "Email 2" },
                    new() { Id = "3", UserName = "User 3", Email = "Email 3" },
                    new() { Id = "4", UserName = "User 4", Email = "Email 4" },
                    new() { Id = "5", UserName = "User 5", Email = "Email 5" },
                };

                await dbContext.Users.AddRangeAsync(users);
            }

            await dbContext.SaveChangesAsync();

            return dbContext;
        }

        [Fact]
        public async Task NewComment_ReturnsCommentDto()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var commentRepository = new CommentRepository(dbContext);

            // Act
            var result = await commentRepository.NewComment("1", "Comment 6", "1", null);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be("1");
            result.Content.Should().Be("Comment 6");
        }

        [Fact]
        public async Task NewComment_ReturnsNull()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var commentRepository = new CommentRepository(dbContext);

            // Act
            var result = await commentRepository.NewComment("1", "Comment 6", "999", null);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateComment_ReturnsCommentDto()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var commentRepository = new CommentRepository(dbContext);

            // Act
            var result = await commentRepository.UpdateComment("1", "1", "Comment 1 Updated");

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be("1");
            result.Content.Should().Be("Comment 1 Updated");
        }

        [Fact]
        public async Task UpdateComment_ReturnsNull()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var commentRepository = new CommentRepository(dbContext);

            // Act
            var result = await commentRepository.UpdateComment("1", "999", "Comment 1 Updated");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteComment_ReturnsTrue()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var commentRepository = new CommentRepository(dbContext);

            // Act
            var result = await commentRepository.DeleteComment("1", "1");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteComment_ReturnsFalse()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var commentRepository = new CommentRepository(dbContext);

            // Act
            var result = await commentRepository.DeleteComment("1", "999");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetComment_ReturnsCommentDtoWithChildren()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var commentRepository = new CommentRepository(dbContext);

            // Act
            var result = await commentRepository.GetComment("1");

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be("1");
            result.UserId.Should().Be("1");
            result.Content.Should().Be("Comment 1");
            result.Children.Count.Should().BeGreaterThan(1);
        }

        [Fact]
        public async Task GetComment_ReturnsCommentDtoWithoutChildren()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var commentRepository = new CommentRepository(dbContext);

            // Act
            var result = await commentRepository.GetComment("6");

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be("6");
            result.UserId.Should().Be("1");
            result.Content.Should().Be("Comment 6");
            result.Children.Count.Should().Be(0);
        }

        [Fact]
        public async Task GetComment_ReturnsNull()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var commentRepository = new CommentRepository(dbContext);

            // Act
            var result = await commentRepository.GetComment("999");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetReplies_ReturnsReplies()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var commentRepository = new CommentRepository(dbContext);

            // Act
            var result = await commentRepository.GetReplies("1");

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().BeGreaterThan(1);
        }

        [Fact]
        public async Task GetReplies_ReturnsEmptyList()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var commentRepository = new CommentRepository(dbContext);

            // Act
            var result = await commentRepository.GetReplies("999");

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(0);
        }

        [Fact]
        public async Task GetPostComments_ReturnsComments()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var commentRepository = new CommentRepository(dbContext);

            // Act
            var result = await commentRepository.GetPostComments("1", 1, 3);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(5);
            result.Should().BeOfType<DataCountPagesDto<List<CommentDto>>>();
            result.Data.Should().BeOfType<List<CommentDto>>();
        }

        [Fact]
        public async Task GetPostComments_ReturnsEmptyList()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var commentRepository = new CommentRepository(dbContext);

            // Act
            var result = await commentRepository.GetPostComments("999", 1, 3);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(0);
        }

        [Fact]
        public async Task GetCommentsByUser_ReturnsComments()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var commentRepository = new CommentRepository(dbContext);

            // Act
            var result = await commentRepository.GetCommentsByUser("User 1", 1, 3);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(8);
            result.Should().BeOfType<DataCountPagesDto<IEnumerable<CommentDto>>>();
            result.Data.Should().BeOfType<List<CommentDto>>();
        }

        [Fact]
        public async Task GetCommentsByUser_ReturnsEmptyList()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var commentRepository = new CommentRepository(dbContext);

            // Act
            var result = await commentRepository.GetCommentsByUser("User 999", 1, 3);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(0);
        }
    }
}
