using Application.Interfaces;
using Domain.Entities;

namespace ReadaddictsNET8.Endpoints
{
    public static class Posts
    {
        public static void AddPostsEndpoints(this IEndpointRouteBuilder routes)
        {
            var posts = routes.MapGroup("/api/v1/posts");

            posts.MapGet("/all", async (IPostRepository postRepository) =>
            {
                ICollection<Post> posts = await postRepository.GetPosts();

                return Results.Ok(posts);
            });
        }
    }
}
