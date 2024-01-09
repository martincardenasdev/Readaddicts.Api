namespace Domain.Dto
{
    public class MessageDto
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public UserDto Sender { get; set; }
        public UserDto Receiver { get; set; }
    }
}
