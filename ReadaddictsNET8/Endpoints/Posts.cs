using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Antiforgery;
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
        }

        public static async Task<Results<Ok<ICollection<Post>>, NotFound>> GetAllPosts(IPostRepository postRepository)
        {
            ICollection<Post> posts = await postRepository.GetPosts();

            if (posts.Count is 0)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(posts);
        }
        public static async Task<Results<Ok<Post>, NotFound>> GetPost(IPostRepository postRepository, int id)
        {
            Post post = await postRepository.GetPost(id);

            if (post is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(post);
        }
        public static async Task<Results<Ok<int>, BadRequest>> CreatePost(IPostRepository postRepository, ClaimsPrincipal user, [FromForm] Post post, [FromForm] IFormFileCollection? images, [FromForm] int? groupId)
        {
            string userId = user.FindFirst(ClaimTypes.NameIdentifier).Value;

            int newPostId = await postRepository.CreatePost(userId, groupId, post, images);

            if (newPostId is 0)
            {
                return TypedResults.BadRequest();
            }

            return TypedResults.Ok(newPostId);
        }
    }
}
