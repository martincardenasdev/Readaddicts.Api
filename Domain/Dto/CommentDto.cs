namespace Domain.Dto
{
    public class CommentDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Content { get; set; }
        public DateTimeOffset Created { get; set; }
        public UserDto User { get; set; }
        public int ReplyCount { get; set; }
        public ICollection<CommentDto> Children { get; set; }
    }
}
