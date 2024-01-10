using Domain.Dto;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Abstractions
{
    public interface IGroupRepository
    {
        Task<List<GroupDto>> GetGroups(int page, int limit);
        Task<GroupDto?> GetGroup(string groupId);
        Task<string> CreateGroup(string userId, Group group, IFormFile? picture);
        Task<bool> UpdateGroup(string groupId, string userId, Group group, IFormFile? picture);
        Task<bool> DeleteGroup(string groupId, string userId);
        Task<bool> JoinGroup(string userId, string groupId);
        Task<bool> LeaveGroup(string userId, string groupId);
    }
}
