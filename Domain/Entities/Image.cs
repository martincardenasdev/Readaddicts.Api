namespace Domain.Entities
{
    public class Image
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Url { get; set; }
        public string CloudinaryPublicId { get; set; }
        public DateTimeOffset Created { get; set; }
        public User Creator { get; set; }
        public Post Post { get; set; }
    }
}
