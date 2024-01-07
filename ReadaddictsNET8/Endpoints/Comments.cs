using Application.Abstractions;
using Domain.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace ReadaddictsNET8.Endpoints
{
    public static class Comments
    {
        public static void AddCommentsEndpoints(this IEndpointRouteBuilder routes)
        {
            RouteGroupBuilder comments = routes.MapGroup("/api/v1/comments");

            comments.MapGet("/{id}", GetComment);
            comments.MapPost("/", CreateComment);
        }

        private static string GetUserId(ClaimsPrincipal user) => user.FindFirstValue(ClaimTypes.NameIdentifier);
        public static async Task<Results<Ok<CommentDto>, NotFound>> GetComment(ICommentRepository commentRepository, string id)
        {
            CommentDto? comment = await commentRepository.GetComment(id);

            if (comment is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(comment);
        }
        public static async Task<Results<Ok<CommentDto>, BadRequest>> CreateComment(ICommentRepository commentRepository, ClaimsPrincipal user, string comment, string postId, string? parentId)
        {
            CommentDto newComment = await commentRepository.NewComment(GetUserId(user), comment, postId, parentId);

            return TypedResults.Ok(newComment);
        }
    }
}
