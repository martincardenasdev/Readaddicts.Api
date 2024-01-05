namespace Domain.Dto
{
    public class PostDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Content { get; set; }
    }
}
