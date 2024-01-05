using Application.Interfaces;
using Domain.Dto;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PostRepository(ApplicationDbContext context) : IPostRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<string> CreatePost(string userId, string? groupId, Post post, IFormFileCollection? images)
        {
            if (groupId is null)
            {
                post.GroupId = null;
            }
            else
            {
                post.GroupId = groupId;
            }

            var newPost = new Post
            {
                UserId = userId,
                GroupId = post.GroupId,
                Created = DateTimeOffset.UtcNow,
                Content = post.Content,
                Images = post.Images
            };

            _context.Add(newPost);

            int rowsAffected = await _context.SaveChangesAsync();

            if (rowsAffected is 0)
            {
                return string.Empty;
            }

            return newPost.Id;
        }

        public async Task<Post> GetPost(int id)
        {
            return await _context.Posts.FindAsync(id);
        }

        public async Task<ICollection<PostDto>> GetPosts()
        {
            return await _context.Posts
                .Select(post => new PostDto
                {
                    Id = post.Id,
                    UserId = post.UserId,
                    Created = post.Created,
                    Content = post.Content
                })
                .ToListAsync();
        }
    }
}
