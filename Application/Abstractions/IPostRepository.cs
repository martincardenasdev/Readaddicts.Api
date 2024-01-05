using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IPostRepository
    {
        Task<ICollection<Post>> GetPosts();
        Task<Post> GetPost(int id);
        Task<int> CreatePost(string userId, int? groupId, Post post, IFormFileCollection? images);
    }
}
