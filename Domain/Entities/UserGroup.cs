using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class UserGroup
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string GroupId { get; set; }
    }
}
