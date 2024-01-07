using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Group
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string CreatorId { get; set; }
        public string? Picture { get; set; }
        public DateTimeOffset Created { get; set; }
        public ICollection<User>? Users { get; set; }
        public User Creator { get; set; }
        public ICollection<Post>? Posts { get; set; }
    }
}
