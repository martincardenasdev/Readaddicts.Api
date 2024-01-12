using Domain.Dto;

namespace Application.Abstractions
{
    public interface ICommentRepository
    {
        Task<CommentDto> NewComment(string userId, string comment, string postId, string? parentId);
        Task<CommentDto> UpdateComment(string userId, string commentId, string content);
        Task<bool> DeleteComment(string userId, string commentId);
        Task<CommentDto?> GetComment(string commentId);
        Task<List<CommentDto>> GetReplies(string parentId);
        Task<DataCountPagesDto<List<CommentDto>>> GetPostComments(string postId, int page, int limit);
    }
}
