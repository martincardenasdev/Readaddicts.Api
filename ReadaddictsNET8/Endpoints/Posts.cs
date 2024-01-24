using Application.Abstractions;
using Application.Interfaces;
using Domain.Dto;
using Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ReadaddictsNET8.Endpoints
{
    public static class Posts
    {
        public static void AddPostsEndpoints(this IEndpointRouteBuilder routes)
        {
            RouteGroupBuilder posts = routes.MapGroup("/api/v1/posts");

            posts.MapGet("/all", GetAllPosts);
            posts.MapGet("/{id}", GetPost);
            posts.MapPost("/create", CreatePost).RequireAuthorization().DisableAntiforgery();
            posts.MapDelete("/{id}", DeletePost).RequireAuthorization();
            posts.MapPatch("/{id}", UpdatePostContent).RequireAuthorization();
            posts.MapPatch("/{id}/images/add", AddImagesToPost).RequireAuthorization().DisableAntiforgery();
            posts.MapPatch("/{id}/images/delete", DeleteImageFromPost).RequireAuthorization();
            posts.MapGet("/{id}/comments", GetPostComments);
        }

        private static string GetUserId(ClaimsPrincipal user) => user.FindFirstValue(ClaimTypes.NameIdentifier);

        public static async Task<Ok<DataCountPagesDto<List<PostDto>>>> GetAllPosts(IPostRepository postRepository, int page, int limit)
        {
            var posts = await postRepository.GetPosts(page, limit);

            return TypedResults.Ok(posts);
        }
        public static async Task<Results<Ok<PostDto>, NotFound>> GetPost(IPostRepository postRepository, string id)
        {
            PostDto? post = await postRepository.GetPost(id);

            if (post is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(post);
        }
        public static async Task<Results<Ok<string>, BadRequest>> CreatePost(IPostRepository postRepository, ClaimsPrincipal user, [FromForm] Post post, [FromForm] IFormFileCollection? images, [FromForm] string? groupId)
        {
            string newPostId = await postRepository.CreatePost(GetUserId(user), groupId, post, images);

            if (newPostId == string.Empty)
            {
                return TypedResults.BadRequest();
            }

            return TypedResults.Ok(newPostId);
        }
        public static async Task<Results<Ok, BadRequest>> DeletePost(IPostRepository postRepository, ClaimsPrincipal user, string id)
        {
            bool deleted = await postRepository.DeletePost(GetUserId(user), id);

            if (!deleted)
            {
                return TypedResults.BadRequest();
            }

            return TypedResults.Ok();
        }
        public static async Task<Results<Ok<PostDto>, BadRequest>> UpdatePostContent(IPostRepository postRepository, ClaimsPrincipal user, string id, [FromQuery] string content)
        {
            (bool updated, PostDto postDto) = await postRepository.UpdatePostContent(id, GetUserId(user), content);

            if (!updated)
            {
                return TypedResults.BadRequest();
            }

            return TypedResults.Ok(postDto);
        }
        public static async Task<Results<Ok<IEnumerable<ImageDto>>, BadRequest>> AddImagesToPost(IPostRepository postRepository, ClaimsPrincipal user, string id, [FromForm] IFormFileCollection images)
        {
            var uploadedImages = await postRepository.AddImagesToPost(id, GetUserId(user), images);

            if (!uploadedImages.Any())
            {
                return TypedResults.BadRequest();
            }

            return TypedResults.Ok(uploadedImages);
        }
        public static async Task<Results<Ok<IEnumerable<string>>, BadRequest<IEnumerable<string>>>> DeleteImageFromPost(IPostRepository postRepository, ClaimsPrincipal user, string id, [FromBody] List<string> imageIds)
        {
            var (deleted, notDeleted) = await postRepository.DeleteImageFromPost(id, GetUserId(user), imageIds);

            if (notDeleted.Any() && deleted.Any())
            {
                return TypedResults.Ok(notDeleted);
            }

            if (notDeleted.Any() && !deleted.Any())
            {
                return TypedResults.BadRequest(notDeleted);
            }
             
            return TypedResults.Ok(deleted);
        }
        public static async Task<Results<Ok<DataCountPagesDto<List<CommentDto>>>, NotFound>> GetPostComments(ICommentRepository commentRepository, string id, int page, int limit)
        {
            var comments = await commentRepository.GetPostComments(id, page, limit);

            if (comments.Count == 0)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(comments);
        }
    }
}
