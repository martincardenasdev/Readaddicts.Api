using Application.Abstractions;
using Domain.Dto;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CommentRepository(ApplicationDbContext context) : ICommentRepository
    {
        private readonly ApplicationDbContext _context = context;
        public async Task<CommentDto?> GetComment(string commentId)
        {
            var replies = await GetReplies(commentId);

            return await _context.Comments
                .Where(x => x.Id == commentId)
                .Include(x => x.User)
                .Include(x => x.Children)
                .Select(x => new CommentDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Content = x.Content,
                    Created = x.Created,
                    User = new UserDto
                    {
                        Id = x.User.Id,
                        UserName = x.User.UserName,
                        ProfilePicture = x.User.ProfilePicture
                    },
                    Children = replies
                }).FirstOrDefaultAsync();
        }

        public async Task<ICollection<CommentDto>> GetReplies(string parentId)
        {
            var replies = await _context.Comments
                .Where(x => x.ParentId == parentId)
                .Include(x => x.User)
                .ToListAsync();

            var comments = new List<CommentDto>();

            foreach (var reply in replies)
            {
                var replyDto = new CommentDto
                {
                    Id = reply.Id,
                    UserId = reply.UserId,
                    Content = reply.Content,
                    Created = reply.Created,
                    User = new UserDto
                    {
                        Id = reply.User.Id,
                        UserName = reply.User.UserName,
                        ProfilePicture = reply.User.ProfilePicture
                    },
                    Children = await GetReplies(reply.Id)
                };

                comments.Add(replyDto);
            }

            return comments;
        }

        public async Task<CommentDto> NewComment(string userId, string comment, string postId, string? parentId)
        {
            string? parent = string.IsNullOrWhiteSpace(parentId) ? null : parentId;

            bool postExists = await _context.Posts.AnyAsync(x => x.Id == postId);

            if (!postExists)
                return null;

            var newComment = new Comment
            {
                UserId = userId,
                PostId = postId,
                ParentId = parent,
                Content = comment,
                Created = DateTimeOffset.UtcNow,
            };

            _context.Comments.Add(newComment);

            int rowsAffected = await _context.SaveChangesAsync();

            if (rowsAffected == 0)
                return null;

            return new CommentDto
            {
                Id = newComment.Id,
                UserId = newComment.UserId,
                Content = newComment.Content,
                Created = newComment.Created
            };
        }
    }
}
