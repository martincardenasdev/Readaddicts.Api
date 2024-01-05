using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class PostRepository(ApplicationDbContext context) : IPostRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<int> CreatePost(string userId, int? groupId, Post post, IFormFileCollection? images)
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
                return 0;
            }

            return newPost.Id;
        }

        public async Task<Post> GetPost(int id)
        {
            return await _context.Posts.FindAsync(id);
        }

        public async Task<ICollection<Post>> GetPosts()
        {
            return await _context.Posts.ToListAsync();
        }
    }
}
