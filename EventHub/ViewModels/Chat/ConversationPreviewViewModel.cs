namespace EventHub.Web.ViewModels.Chat
{
    public class ConversationPreviewViewModel
    {

        public Guid ConversationId { get; set; }
        public string OtherUserId { get; set; } = null!;
        public string OtherUserName { get; set; } = null!;
        public string OtherUserProfileImagePath { get; set; } = null!;
        public string? LastMessage { get; set; } = null!;
        public DateTime? LastMessageAt { get; set; }

    }
}
    