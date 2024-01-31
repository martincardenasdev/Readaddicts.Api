using Domain.Common;
using Domain.Dto;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Abstractions
{
    public interface IGroupRepository
    {
        Task<DataCountPagesDto<IEnumerable<GroupDto>>> GetGroups(int page, int limit);
        Task<GroupDto?> GetGroup(string? userId, string groupId);
        Task<string> CreateGroup(string userId, Group group, IFormFile? picture);
        Task<bool> UpdateGroup(string groupId, string userId, Group group, IFormFile? picture);
        Task<bool> DeleteGroup(string groupId, string userId);
        Task<OperationResult<UserDto>> JoinGroup(string userId, string groupId);
        Task<OperationResult<UserDto>> LeaveGroup(string userId, string groupId);
        Task<DataCountPagesDto<IEnumerable<PostDto>>> GetPostsByGroup(string groupId, string userId, int page, int limit);
    }
}
