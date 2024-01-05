using Domain.Dto;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IPostRepository
    {
        Task<ICollection<PostDto>> GetPosts();
        Task<Post> GetPost(int id);
        Task<string> CreatePost(string userId, string? groupId, Post post, IFormFileCollection? images);
    }
}
