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
        public IEnumerable<UserDto> Users { get; set; }
        public UserDto Creator { get; set; }
        public IEnumerable<PostDto> Posts { get; set; }
        public int MembersCount { get; set; }
        public bool IsMember { get; set; }
    }
}
