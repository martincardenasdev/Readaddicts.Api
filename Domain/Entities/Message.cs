namespace Domain.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public bool IsRead { get; set; }
        public User Sender { get; set; }
        public User Receiver { get; set; }
    }
}
