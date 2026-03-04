

using EventHub.Core.Models.Common;
using EventHub.Core.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Channels;

namespace EventHub.Core.Models.Messaging
{
    public class Conversation : BaseModel
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey(nameof(User1))]
        public string User1Id { get; set; } = null!;
        public UserProfile User1 { get; set; } = null!;

        [ForeignKey(nameof(User2))]
        public string User2Id { get; set; } = null!;
        public UserProfile User2 { get; set; } = null!;

        public ICollection<Message> Messages { get; set; }
        = new List<Message>();

    }
}
