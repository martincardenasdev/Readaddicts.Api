namespace Domain.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Content { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public int? GroupId { get; set; }
        public User Creator { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public Group? Group { get; set; }
        public ICollection<Image>? Images { get; set; }
    }
}
