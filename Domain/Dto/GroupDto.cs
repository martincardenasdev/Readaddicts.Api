namespace Domain.Dto
{
    public class GroupDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string CreatorId { get; set; }
        public string? Picture { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}
