using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User : IdentityUser
    {
        public DateTimeOffset LastLogin { get; set; }
        public int TierId { get; set; }
        public string? Biography { get; set; }
        public string? ProfilePicture { get; set; }
        public Tier? Tier { get; set; }
        public ICollection<Message>? MessagesSent { get; set; }
        public ICollection<Message>? MessagesReceived { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Group>? Groups { get; set; }
        public ICollection<Group>? GroupsCreated { get; set; }
        public ICollection<Post>? Posts { get; set; }
        public ICollection<Image>? UploadedImages { get; set; }
    }
}
