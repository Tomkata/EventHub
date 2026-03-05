using System;
using System.Collections.Generic;
using System.Text;

namespace EventHub.Core.DTOs.Messaging
{
    public class ConversationPreviewDto
    {
        public string UserNameA { get; set; }= null!;
        public string UserAId { get; set; } = null!;
        public string UserNameB { get; set; } = null!;
        public string UserBId { get; set; } = null!;

    }
}
