using Application.Abstractions;
using Domain.Dto;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CommentRepository(ApplicationDbContext context) : ICommentRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<bool> DeleteComment(string userId, string commentId)
        {
            var comment = await _context.Comments
                .Where(x => x.Id == commentId && x.UserId == userId)
                .FirstOrDefaultAsync();

            if (comment is null)
                return false;

            // Get replies to delete them https://learn.microsoft.com/en-us/ef/core/saving/cascade-delete#database-cascade-limitations
            var replies = await GetReplies(commentId);

            _context.Comments.Remove(comment);

            int rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected > 0;
        }

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
                    PostId = x.PostId,
                    Content = x.Content,
                    Created = x.Created,
                    User = new UserDto
                    {
                        Id = x.User.Id,
                        UserName = x.User.UserName,
                        ProfilePicture = x.User.ProfilePicture
                    },
                    Children = replies,
                    ReplyCount = replies.Count
                }).FirstOrDefaultAsync();
        }

        public async Task<DataCountPagesDto<List<CommentDto>>> GetPostComments(string postId, int page, int limit)
        {
            List<CommentDto> comments = await _context.Comments
                .Where(x => x.PostId == postId && x.ParentId == null)
                .OrderByDescending(x => x.Created)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Include(x => x.User)
                .Select(x => new CommentDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    PostId = x.PostId,
                    Content = x.Content,
                    Created = x.Created,
                    User = new UserDto
                    {
                        Id = x.User.Id,
                        UserName = x.User.UserName,
                        ProfilePicture = x.User.ProfilePicture
                    },
                    ReplyCount = _context.Comments.Count(y => y.ParentId == x.Id)
                })
                .ToListAsync();

            int commentsCount = await _context.Comments.CountAsync(x => x.PostId == postId && x.ParentId == null);

            int pages = (int)Math.Ceiling((double)commentsCount / limit);

            return new DataCountPagesDto<List<CommentDto>>
            {
                Data = comments,
                Count = commentsCount,
                Pages = pages
            };
        }

        public async Task<List<CommentDto>> GetReplies(string parentId)
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
                    Children = await GetReplies(reply.Id),
                    ReplyCount = await _context.Comments.CountAsync(x => x.ParentId == reply.Id)
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

        public async Task<CommentDto> UpdateComment(string userId, string commentId, string content)
        {
            var comment = await _context.Comments
                .Where(x => x.Id == commentId && x.UserId == userId)
                .FirstOrDefaultAsync();

            if (comment is null)
                return null;

            comment.Content = content;
            comment.Modified = DateTimeOffset.UtcNow;

            _context.Comments.Update(comment);

            int rowsAffected = _context.SaveChanges();

            if (rowsAffected == 0)
                return null;

            return new CommentDto
            {
                Id = comment.Id,
                UserId = comment.UserId,
                Content = comment.Content,
                Created = comment.Created
            };
        }
    }
}
