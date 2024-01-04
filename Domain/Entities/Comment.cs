namespace Domain.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int PostId { get; set; }
        public int? ParentId { get; set; }
        public string Content { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public User User { get; set; }
        public Comment? Parent { get; set; }
        public Post Post { get; set; }
        public ICollection<Comment>? Children { get; set; }
    }
}
