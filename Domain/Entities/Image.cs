namespace Domain.Entities
{
    public class Image
    {
        public string Id { get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
        public string Url { get; set; }
        public string CloudinaryPublicId { get; set; }
        public DateTimeOffset Created { get; set; }
        public User Creator { get; set; }
        public Post Post { get; set; }
    }
}
