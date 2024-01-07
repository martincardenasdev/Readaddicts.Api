using Domain.Dto;

namespace Application.Abstractions
{
    public interface ICommentRepository
    {
        Task<CommentDto> NewComment(string userId, string comment, string postId, string? parentId);
        Task<CommentDto?> GetComment(string commentId);
        Task<ICollection<CommentDto>> GetReplies(string parentId);
    }
}
