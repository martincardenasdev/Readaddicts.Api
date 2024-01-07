using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Post
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Content { get; set; }
        public DateTimeOffset? Modified { get; set; }
        public string? GroupId { get; set; }
        public User Creator { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public Group? Group { get; set; }
        public List<Image>? Images { get; set; }
    }
}
