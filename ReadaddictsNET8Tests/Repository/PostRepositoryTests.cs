using Application.Abstractions;
using Domain.Dto;
using Domain.Entities;
using FluentAssertions;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Text;

namespace ReadaddictsNET8Tests.Repository
{
    public class PostRepositoryTests
    {
        private readonly Mock<ICloudinaryRepository> _cloudinaryMock;
        public PostRepositoryTests()
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

            if (!await dbContext.Posts.AnyAsync())
            {
                var posts = new List<Post>
                {
                    new() {
                        Id = "1",
                        UserId = "1",
                        GroupId = null,
                        Created = DateTimeOffset.UtcNow,
                        Content = "Post 1"
                    },
                    new() {
                        Id = "2",
                        UserId = "1",
                        GroupId = null,
                        Created = DateTimeOffset.UtcNow,
                        Content = "Post 2"
                    },
                    new() {
                        Id = "3",
                        UserId = "1",
                        GroupId = null,
                        Created = DateTimeOffset.UtcNow,
                        Content = "Post 3"
                    }
                };

                dbContext.Posts.AddRange(posts);
            }

            if (!await dbContext.Users.AnyAsync())
            {
                var users = new List<User>
                {
                    new()
                    {
                        Id = "1",
                        UserName = "User 1",
                        Email = "email@email.com"
                    },
                    new()
                    {
                        Id = "2",
                        UserName = "User 2",
                        Email = "email@email.com"
                    },
                    new()
                    {
                        Id = "3",
                        UserName = "User 3",
                        Email = "email@email.com"
                    }
                };

                dbContext.Users.AddRange(users);
            }

            if (!await dbContext.Comments.AnyAsync())
            {
                var comments = new List<Comment>
                {
                    new()
                    {
                        Id = "1",
                        UserId = "1",
                        PostId = "1",
                        Content = "Comment 1",
                        Created = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Id = "2",
                        UserId = "1",
                        PostId = "1",
                        Content = "Comment 2",
                        Created = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Id = "3",
                        UserId = "1",
                        PostId = "1",
                        Content = "Comment 3",
                        Created = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Id = "4",
                        UserId = "1",
                        PostId = "2",
                        Content = "Comment 4",
                        Created = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Id = "5",
                        UserId = "1",
                        PostId = "2",
                        Content = "Comment 5",
                        Created = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Id = "6",
                        UserId = "1",
                        PostId = "2",
                        Content = "Comment 6",
                        Created = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Id = "7",
                        UserId = "1",
                        PostId = "3",
                        Content = "Comment 7",
                        Created = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Id = "8",
                        UserId = "1",
                        PostId = "3",
                        Content = "Comment 8",
                        Created = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Id = "9",
                        UserId = "1",
                        PostId = "3",
                        Content = "Comment 9",
                        Created = DateTimeOffset.UtcNow
                    }
                };

                dbContext.Comments.AddRange(comments);
            }

            if (!await dbContext.Images.AnyAsync())
            {
                var images = new List<Image>
                {
                    new()
                    {
                        Id = "1",
                        PostId = "1",
                        UserId = "1",
                        Url = "FakeURL",
                        CloudinaryPublicId = "FakePublicId",
                        Created = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Id = "2",
                        PostId = "1",
                        UserId = "1",
                        Url = "FakeURL",
                        CloudinaryPublicId = "FakePublicId",
                        Created = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Id = "3",
                        PostId = "2",
                        UserId = "2",
                        Url = "FakeURL",
                        CloudinaryPublicId = "FakePublicId",
                        Created = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Id = "4",
                        PostId = "2",
                        UserId = "2",
                        Url = "FakeURL",
                        CloudinaryPublicId = "FakePublicId",
                        Created = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Id = "5",
                        PostId = "3",
                        UserId = "3",
                        Url = "FakeURL",
                        CloudinaryPublicId = "FakePublicId",
                        Created = DateTimeOffset.UtcNow
                    },
                    new()
                    {
                        Id = "6",
                        PostId = "3",
                        UserId = "3",
                        Url = "FakeURL",
                        CloudinaryPublicId = "FakePublicId",
                        Created = DateTimeOffset.UtcNow
                    }
                };

                dbContext.Images.AddRange(images);
            }

            await dbContext.SaveChangesAsync();

            return dbContext;
        }

        [Fact]
        public async Task GetPosts_ReturnsAllPostsCountAndPages()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var postRepository = new PostRepository(dbContext, _cloudinaryMock.Object);

            // Act
            var result = await postRepository.GetPosts(1, 3);

            // Assert
            result.Data.Should().NotBeNullOrEmpty();
            result.Data.Should().HaveCount(3);
            result.Data.Should().BeOfType<List<PostDto>>();
            result.Count.Should().Be(3);
            result.Pages.Should().Be(1);
            result.Should().BeOfType<DataCountPagesDto<List<PostDto>>>();
        }

        [Fact]
        public async Task GetPost_ReturnsPost()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var postRepository = new PostRepository(dbContext, _cloudinaryMock.Object);

            // Act
            var result = await postRepository.GetPost("1");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<PostDto>();
            result.Id.Should().Be("1");
            result.UserId.Should().Be("1");
            result.Creator.Should().NotBeNull();
            result.Creator.Id.Should().Be("1");
            result.Creator.UserName.Should().Be("User 1");
            result.Creator.ProfilePicture.Should().BeNull();
            result.Images.Should().NotBeNullOrEmpty();
            result.Images.Should().HaveCount(2);
            result.Images.Should().BeOfType<List<ImageDto>>();
        }

        [Fact]
        public async Task GetPost_ReturnsNull()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var postRepository = new PostRepository(dbContext, _cloudinaryMock.Object);

            // Act
            var result = await postRepository.GetPost("4");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreatePost_ReturnsPostId()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var postRepository = new PostRepository(dbContext, _cloudinaryMock.Object);
            var post = new Post
            {
                UserId = "1",
                GroupId = null,
                Created = DateTimeOffset.UtcNow,
                Content = "Post 4"
            };

            // Act
            var result = await postRepository.CreatePost("1", null, post, null);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().BeOfType<string>();
        }

        [Fact]
        public async Task CreatePost_ReturnsEmptyString()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var postRepository = new PostRepository(dbContext, _cloudinaryMock.Object);
            var post = new Post
            {
                UserId = "1",
                GroupId = null,
                Created = DateTimeOffset.UtcNow,
                Content = ""
            };

            // Act
            var result = await postRepository.CreatePost("2", null, post, null);

            // Assert
            result.Should().BeNullOrEmpty();
            result.Should().BeOfType<string>();
        }

        [Fact]
        public async Task DeletePost_ReturnsTrue()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var postRepository = new PostRepository(dbContext, _cloudinaryMock.Object);

            // Act
            var result = await postRepository.DeletePost("1", "1");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeletePost_ReturnsFalse()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var postRepository = new PostRepository(dbContext, _cloudinaryMock.Object);

            // Act
            var result = await postRepository.DeletePost("1", "999");

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteImageFromPost_ReturnsDeleted()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var postRepository = new PostRepository(dbContext, _cloudinaryMock.Object);
            var imageIds = new List<string> { "1", "2" };

            _cloudinaryMock
                .Setup(x => x.Destroy(It.IsAny<List<Image>>()))
                .ReturnsAsync((imageIds, Enumerable.Empty<string>()));

            // Act
            var (deleted, notDeleted) = await postRepository.DeleteImageFromPost("1", "1", imageIds);

            // Assert
            deleted.Should().NotBeNullOrEmpty();
            deleted.Should().HaveCount(2);
            deleted.Should().BeOfType<List<string>>();
            notDeleted.Should().BeEmpty();
            notDeleted.Should().HaveCount(0);
        }

        [Fact]
        public async Task DeleteImageFromPost_ReturnsNotDeleted()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var postRepository = new PostRepository(dbContext, _cloudinaryMock.Object);
            var imageIds = new List<string> { "1", "2" };

            _cloudinaryMock
                .Setup(x => x.Destroy(It.IsAny<List<Image>>()))
                .ReturnsAsync((Enumerable.Empty<string>(), imageIds));

            // Act
            var (deleted, notDeleted) = await postRepository.DeleteImageFromPost("1", "1", imageIds);

            // Assert
            deleted.Should().BeEmpty();
            deleted.Should().HaveCount(0);
            notDeleted.Should().NotBeNullOrEmpty();
            notDeleted.Should().HaveCount(2);
            notDeleted.Should().BeOfType<List<string>>();
        }

        [Fact]
        public async Task UpdatePostContent_ReturnsTrue()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var postRepository = new PostRepository(dbContext, _cloudinaryMock.Object);

            string content = "content update";

            // Act
            var (result, postDto) = await postRepository.UpdatePostContent("1", "1", content);

            // Assert
            result.Should().BeTrue();
            postDto.Should().NotBeNull();
            postDto.Should().BeOfType<PostDto>();
            postDto.Content.Should().Be(content);
        }

        [Fact]
        public async Task UpdatePostContent_ReturnsFalse()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var postRepository = new PostRepository(dbContext, _cloudinaryMock.Object);

            string content = "content update";

            // Act
            var (result, postDto) = await postRepository.UpdatePostContent("1", "999", content);

            // Assert
            result.Should().BeFalse();
            postDto.Should().BeOfType<PostDto>();
            postDto.Content.Should().NotBe(content);
        }

        [Fact]
        public async Task AddImagesToPost_ReturnsImages()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var postRepository = new PostRepository(dbContext, _cloudinaryMock.Object);
            var images = new FormFileCollection
            {
                new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy img")), 0, 0, "Data", "dummy.jpg"),
                new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy img")), 0, 0, "Data", "dummy.jpg")
            };

            var tupleResultList = new List<(string imageUrl, string publicId, string result)>
            {
                ("FakeURL", "FakePublicId", "FakeResult"),
                ("FakeURL2", "FakePublicId2", "FakeResult2")
            };

            _cloudinaryMock
                .Setup(x => x.UploadMany(It.IsAny<IFormFileCollection>()))
                .ReturnsAsync(tupleResultList);

            // Act
            var result = await postRepository.AddImagesToPost("1", "1", images);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(2);
            result.Should().BeOfType<List<ImageDto>>();
            foreach (var item in result)
            {
                item.Should().NotBeNull();
                item.Url.Should().NotBeNullOrEmpty();
                item.Url.Should().BeOfType<string>();
                item.Id.Should().NotBeNullOrEmpty();
                item.Id.Should().BeOfType<string>();
            }
        }

        [Fact]
        public async Task AddImagesToPost_ReturnsEmptyList()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var postRepository = new PostRepository(dbContext, _cloudinaryMock.Object);
            var images = new FormFileCollection
            {
                new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy img")), 0, 0, "Data", "dummy.jpg"),
                new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy img")), 0, 0, "Data", "dummy.jpg")
            };

            var tupleResultList = new List<(string imageUrl, string publicId, string result)>();

            _cloudinaryMock
                .Setup(x => x.UploadMany(It.IsAny<IFormFileCollection>()))
                .ReturnsAsync(tupleResultList);

            // Act
            var result = await postRepository.AddImagesToPost("1", "1", images);

            // Assert
            result.Should().BeEmpty();
            result.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetPostsByUser_ReturnsPosts()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var postRepository = new PostRepository(dbContext, _cloudinaryMock.Object);

            // Act
            var result = await postRepository.GetPostsByUser("User 1", 1, 3);

            // Assert
            result.Data.Should().NotBeNullOrEmpty();
            result.Data.Should().HaveCount(3);
            result.Data.Should().BeOfType<List<PostDto>>();
            result.Count.Should().Be(3);
            result.Pages.Should().Be(1);
            result.Should().BeOfType<DataCountPagesDto<IEnumerable<PostDto>>>();
            result.Data.Should().NotContainNulls();
            result.Data.Should().BeOfType<List<PostDto>>();
        }

        [Fact]
        public async Task GetPostsByUser_ReturnsNoPosts()
        {
            // Arrange
            var dbContext = await GetApplicationDbContext();
            var postRepository = new PostRepository(dbContext, _cloudinaryMock.Object);

            // Act
            var result = await postRepository.GetPostsByUser("User 4", 1, 3);

            // Assert
            result.Data.Should().BeEmpty();
            result.Data.Should().HaveCount(0);
            result.Count.Should().Be(0);
            result.Pages.Should().Be(0);
        }
    }
}
