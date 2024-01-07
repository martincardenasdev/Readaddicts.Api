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
        Task<(bool, PostDto)> UpdatePostContent(string postId, string userId, string content);
        Task<IEnumerable<ImageDto>> AddImagesToPost(string postId, string userId, IFormFileCollection images);
        Task<(IEnumerable<string> deleted, IEnumerable<string> notDeleted)> DeleteImageFromPost(string postId, string userId, List<string> imageIds);
    }
}
