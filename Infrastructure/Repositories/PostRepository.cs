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

        public async Task<bool> DeletePost(string userId, string postId)
        {
            Post post = await _context.FindAsync<Post>(postId);

            if (post is null)
            {
                return false;
            }

            if (post.UserId != userId)
            {
                return false;
            }

            _context.Remove(post);
            int rowsAffected = await _context.SaveChangesAsync();

            if (rowsAffected is 0)
            {
                return false;
            }

            return true;
        }

        public async Task<PostDto?> GetPost(string id)
        {
            return await _context.Posts
                .Where(post => post.Id == id)
                .Include(post => post.Creator)
                .Include(post => post.Comments)
                .Select(post => new PostDto
                {
                    Id = post.Id,
                    UserId = post.UserId,
                    Created = post.Created,
                    Content = post.Content,
                    Creator = new UserDto
                    {
                        UserId = post.Creator.Id,
                        UserName = post.Creator.UserName,
                        ProfilePicture = post.Creator.ProfilePicture
                    },
                    Images = post.Images.Select(image => new ImageDto
                    {
                        Id = image.Id,
                        Url = image.Url
                    }).ToList(),
                    CommentCount = post.Comments.Count,
                    Comments = post.Comments.Select(comment => new CommentDto
                    {
                        Id = comment.Id,
                        UserId = comment.UserId,
                        Content = comment.Content,
                        Created = comment.Created,
                        ReplyCount = _context.Comments.Count(reply => reply.ParentId == comment.Id)
                    }).ToList()
                }).FirstOrDefaultAsync();
        }

        public async Task<ICollection<PostDto>> GetPosts()
        {
            return await _context.Posts
                .Include(post => post.Creator)
                .Select(post => new PostDto
                {
                    Id = post.Id,
                    UserId = post.UserId,
                    Created = post.Created,
                    Content = post.Content,
                    Creator = new UserDto
                    {
                        UserId = post.Creator.Id,
                        UserName = post.Creator.UserName,
                        ProfilePicture = post.Creator.ProfilePicture
                    },
                    Images = post.Images.Select(image => new ImageDto
                    {
                        Id = image.Id,
                        Url = image.Url
                    }).Take(5).ToList(),
                    CommentCount = _context.Comments.Count(comment => comment.PostId == post.Id)
                })
                .ToListAsync();
        }

        public async Task<(bool, PostDto)> UpdatePost(string userId, Post post)
        {
            var postToUpdate = await _context.FindAsync<Post>(post.Id);

            if (postToUpdate is null)
            {
                return (false, new PostDto());
            }

            if (postToUpdate.UserId != userId)
            {
                return (false, new PostDto());
            }

            // Here is what we want to update
            postToUpdate.Content = post.Content;

            _context.Update(postToUpdate);
            int rowsAffected = await _context.SaveChangesAsync();

            if (rowsAffected is 0)
            {
                return (false, new PostDto());
            }

            return (true, new PostDto
            {
                Id = postToUpdate.Id,
                UserId = postToUpdate.UserId,
                Created = postToUpdate.Created,
                Content = postToUpdate.Content,
                Creator = new UserDto
                {
                    UserId = postToUpdate.Creator.Id,
                    UserName = postToUpdate.Creator.UserName,
                    ProfilePicture = postToUpdate.Creator.ProfilePicture
                },
                Images = postToUpdate.Images.Select(image => new ImageDto
                {
                    Id = image.Id,
                    Url = image.Url
                }).ToList(),
                CommentCount = _context.Comments.Count(comment => comment.PostId == postToUpdate.Id)
            });
        }
    }
}
