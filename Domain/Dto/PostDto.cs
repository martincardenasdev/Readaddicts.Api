using Domain.Entities;

namespace Domain.Dto
{
    public class PostDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Content { get; set; }
        public UserDto Creator { get; set; }
        public IEnumerable<ImageDto> Images { get; set; }
        public IEnumerable<CommentDto> Comments { get; set; }
        public int CommentCount { get; set; }
    }
}
