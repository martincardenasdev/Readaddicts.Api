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
            posts.MapPost("/create", CreatePost).RequireAuthorization();
            posts.MapDelete("/{id}", DeletePost).RequireAuthorization();
        }

        public static async Task<Results<Ok<ICollection<PostDto>>, NotFound>> GetAllPosts(IPostRepository postRepository)
        {
            ICollection<PostDto> posts = await postRepository.GetPosts();

            if (posts.Count is 0)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(posts);
        }
        public static async Task<Results<Ok<PostDto>, NotFound>> GetPost(IPostRepository postRepository, string id)
        {
            PostDto post = await postRepository.GetPost(id);

            if (post is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(post);
        }
        public static async Task<Results<Ok<string>, BadRequest>> CreatePost(IPostRepository postRepository, ClaimsPrincipal user, [FromForm] Post post, [FromForm] IFormFileCollection? images, [FromForm] string? groupId)
        {
            string userId = user.FindFirst(ClaimTypes.NameIdentifier).Value;

            string newPostId = await postRepository.CreatePost(userId, groupId, post, images);

            if (newPostId == string.Empty)
            {
                return TypedResults.BadRequest();
            }

            return TypedResults.Ok(newPostId);
        }
        public static async Task<Results<Ok, BadRequest>> DeletePost(IPostRepository postRepository, ClaimsPrincipal user, string id)
        {
            string userId = user.FindFirst(ClaimTypes.NameIdentifier).Value;

            bool deleted = await postRepository.DeletePost(userId, id);

            if (!deleted)
            {
                return TypedResults.BadRequest();
            }

            return TypedResults.Ok();
        }
    }
}
