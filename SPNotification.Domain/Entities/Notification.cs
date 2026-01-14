namespace SPNotifications.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; private set; }
        public string User { get; private set; } = null!;
        public string Message { get; private set; } = null!;
        public string Type { get; private set; } = null!;
        public bool Read { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // Construtor para EF
        protected Notification() { }

        public Notification(string user, string message, string type)
        {
            Id = Guid.NewGuid();
            User = user;
            Message = message;
            Type = type;
            Read = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsRead()
        {
            Read = true;
        }
    }
}
