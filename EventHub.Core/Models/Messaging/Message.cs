

using EventHub.Core.Models.Common;
using EventHub.Core.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventHub.Core.Models.Messaging
{
    public class Message : BaseModel
    {

        [Key]
        public Guid Id { get; set; }

        [MaxLength(2000)]
        [Required]
        public string Content { get; set; } = null!;
        public bool IsRead { get; set; }

        [ForeignKey(nameof(SenderId))]
        public UserProfile Sender { get; set; } = null!;
        public string SenderId { get; set; } = null!;

        [ForeignKey(nameof(ConversationId))]
        public Conversation Conversation { get; set; } = null!;
        public Guid ConversationId { get; set; }
    }
}
