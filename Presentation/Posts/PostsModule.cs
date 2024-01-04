//using Application.Interfaces;
//using Domain.Entities;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Routing;

//namespace Presentation.Posts
//{
//    public static class PostsModule
//    {
//        public static void AddPostsEndpoints(this IEndpointRouteBuilder app)
//        {
//            app.MapGet("/posts/all", async (IPostRepository post) =>
//            {
//                ICollection<Post> posts = await post.GetPosts();

//                return Results.Ok(posts);
//            });
//        }
//    }
//}
