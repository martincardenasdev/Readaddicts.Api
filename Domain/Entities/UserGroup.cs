using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class UserGroup
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string GroupId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public User User { get; set; } // Added
        public Group Group { get; set; } // Added
    }
}
