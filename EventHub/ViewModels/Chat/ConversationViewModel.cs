namespace EventHub.Web.ViewModels.Chat
{
    public class ConversationViewModel
    {
        public Guid ConversationId { get; set; }
        public string CurrentUserId { get; set; } = null!;
        public string OtherUserName { get; set; } = null!;
        public string OtherUserProfileImagePath { get; set; } = null!;
        public IEnumerable<MessageViewModel> Messages { get; set; } = [];

    }
}
