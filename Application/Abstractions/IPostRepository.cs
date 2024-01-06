using Domain.Dto;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IPostRepository
    {
        Task<ICollection<PostDto>> GetPosts();
        Task<PostDto?> GetPost(string id);
        Task<string> CreatePost(string userId, string? groupId, Post post, IFormFileCollection? images);
        Task<bool> DeletePost(string userId, string postId);
        Task<(bool, PostDto)> UpdatePost(string userId, Post post);
    }
}
