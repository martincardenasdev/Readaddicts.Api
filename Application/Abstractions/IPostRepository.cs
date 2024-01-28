using Domain.Dto;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IPostRepository
    {
        Task<DataCountPagesDto<List<PostDto>>> GetPosts(int page, int limit);
        Task<PostDto?> GetPost(string id);
        Task<string> CreatePost(string userId, string? groupId, Post post, IFormFileCollection? images);
        Task<bool> DeletePost(string userId, string postId);
        Task<(bool, PostDto)> UpdatePostContent(string postId, string userId, string content);
        Task<IEnumerable<ImageDto>> AddImagesToPost(string postId, string userId, IFormFileCollection images);
        Task<(IEnumerable<string> deleted, IEnumerable<string> notDeleted)> DeleteImageFromPost(string postId, string userId, List<string> imageIds);
        Task<DataCountPagesDto<IEnumerable<PostDto>>> GetPostsByUser(string username, int page, int limit);
    }
}
