using Application.Abstractions;
using Domain.Dto;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class GroupRepository(ApplicationDbContext context, ICloudinaryRepository cloudinary) : IGroupRepository
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ICloudinaryRepository _cloudinary = cloudinary;

        public async Task<string> CreateGroup(string userId, Group group, IFormFile? picture)
        {
            if (picture is not null)
            {
                var (imageUrl, _, _) = await _cloudinary.Upload(picture, 300, 300);
                group.Picture = imageUrl;
            }

            if (string.IsNullOrWhiteSpace(group.Name))
            {
                return null;
            }

            var newGroup = new Group
            {
                Name = group.Name,
                Description = group.Description,
                CreatorId = userId,
                Picture = group.Picture,
                Created = DateTimeOffset.UtcNow
            };

            _context.Add(newGroup);
            int rowsAffected = await _context.SaveChangesAsync();

            if (rowsAffected is 0)
            {
                return null;
            }

            var relation = new UserGroup
            {
                UserId = userId,
                GroupId = newGroup.Id
            };

            _context.Add(relation);
            int rowsAffected2 = await _context.SaveChangesAsync();

            if (rowsAffected2 is 0)
            {
                return null;
            }

            return newGroup.Id;
        }

        public async Task<bool> DeleteGroup(string groupId, string userId)
        {
            var group = await _context.Groups.FindAsync(groupId);

            if (group is null || userId != group.CreatorId)
            {
                return false;
            }

            _context.Remove(group);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<GroupDto?> GetGroup(string groupId)
        {
            return await _context.Groups
                .Where(x => x.Id == groupId)
                .Select(x => new GroupDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    CreatorId = x.CreatorId,
                    Picture = x.Picture,
                    Created = x.Created
                }).FirstOrDefaultAsync();
        }

        public async Task<List<GroupDto>> GetGroups(int page, int limit)
        {
            return await _context.Groups
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(x => new GroupDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    CreatorId = x.CreatorId,
                    Picture = x.Picture,
                    Created = x.Created
                }).ToListAsync();
        }

        public async Task<bool> JoinGroup(string userId, string groupId)
        {
            bool exists = _context.Groups.Any(x => x.Id == groupId);
            bool userInGroup = _context.UsersGroups.Any(x => x.UserId == userId && x.GroupId == groupId);

            if (!exists || userInGroup)
            {
                return false;
            }

            var relation = new UserGroup
            {
                UserId = userId,
                GroupId = groupId
            };

            _context.Add(relation);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> LeaveGroup(string userId, string groupId)
        {
            bool exists = _context.Groups.Any(x => x.Id == groupId);
            bool userInGroup = _context.UsersGroups.Any(x => x.UserId == userId && x.GroupId == groupId);

            if (!exists || !userInGroup)
            {
                return false;
            }

            var relation = await _context.UsersGroups
                .Where(x => x.UserId == userId && x.GroupId == groupId)
                .FirstOrDefaultAsync();

            _context.Remove(relation);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateGroup(string groupId, string userId, Group group, IFormFile? picture)
        {
            var groupToUpdate = await _context.Groups.FindAsync(groupId);

            if (groupToUpdate is null || userId != groupToUpdate.CreatorId)
            {
                return false;
            }

            bool anyChanges = false;

            if (picture is not null)
            {
                var (imageUrl, _, _) = await _cloudinary.Upload(picture, 300, 300);
                groupToUpdate.Picture = imageUrl;
                anyChanges = true;
            }

            if (group is not null && !string.IsNullOrWhiteSpace(group.Name))
            {
                groupToUpdate.Name = group.Name;
                anyChanges = true;
            }

            if (group is not null && !string.IsNullOrWhiteSpace(group.Description))
            {
                groupToUpdate.Description = group.Description;
                anyChanges = true;
            }

            if (!anyChanges)
            {
                return false;
            }

            _context.Update(groupToUpdate);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
