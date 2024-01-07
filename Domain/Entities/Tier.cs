using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Tier
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<User>? Users { get; set; }
    }
}
