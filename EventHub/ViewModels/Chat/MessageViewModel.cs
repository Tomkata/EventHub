namespace EventHub.Web.ViewModels.Chat
{
    public class MessageViewModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = null!;
        public string SenderId { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
