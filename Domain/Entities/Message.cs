namespace Domain.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public bool IsRead { get; set; }
        public User Sender { get; set; }
        public User Receiver { get; set; }
    }
}
