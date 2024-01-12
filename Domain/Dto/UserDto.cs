namespace Domain.Dto
{
    public class UserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string? ProfilePicture { get; set; }
        public DateTimeOffset LastLogin { get; set; }
        public string? Biography { get; set; }
        public string TierName { get; set; }
    }
}
