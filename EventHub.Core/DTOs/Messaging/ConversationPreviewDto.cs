
namespace EventHub.Core.DTOs.Messaging
{
    public class ConversationPreviewDto
    {
        public Guid ConversationId { get; set; }
        public string OtherUserId { get; set; } = null!;
        public string OtherUserName { get; set; } = null!;
        public string OtherUserProfileImagePath { get; set; } = null!;

    }
    }
}
