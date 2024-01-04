using Domain.Entities;

namespace Application.Interfaces
{
    public interface IPostRepository
    {
        Task<ICollection<Post>> GetPosts();
        Task<Post> GetPost(int id);
    }
}
