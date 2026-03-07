

namespace EventHub.Services.Services.Messaging
{
    using EventHub.Core.DTOs.Messaging;
    using EventHub.Core.Exceptions.Messaging;
    using EventHub.Core.Models.Messaging;
    using EventHub.Repositories.Interfaces.Messaging;
    using EventHub.Services.Interfaces.Messaging;
    using Microsoft.EntityFrameworkCore;

    public class MessageService : IMessageService
    {
        private readonly IConversationRepository _conversationRepository;
        private readonly IMessageRepository _messageRepository;

        public MessageService(IConversationRepository conversationRepository,
                              IMessageRepository messageRepository)
        {
            this._messageRepository = messageRepository;
            this._conversationRepository = conversationRepository;
        }


        public async Task<Guid> SendMessageAsync(
            Guid conversationId,
            string senderId,
            string messageContent)
        {
            if (!await _conversationRepository.IsUserParticipantAsync(conversationId, senderId))
                throw new UserNotParticipantInConversationException();

            if (string.IsNullOrEmpty(messageContent))
                throw new MessageEmptyException();

            var message = new Message
            {
                ConversationId = conversationId,
                Content = messageContent.Trim(),
                SenderId = senderId
            };

            await _messageRepository.AddAsync(message);
            await _messageRepository.SaveChangesAsync();


            return message.Id;
        }

        public async Task<IEnumerable<MessageDto>> GetConversationMessagesAsync(
            Guid conversationId,
            Guid? beforeMessageId,
            int pageSize)
        {

            var query = _messageRepository.GetAllByConversationReadOnly(conversationId);

            if (beforeMessageId.HasValue)
            {
                var beforeMessage = await _messageRepository.GetAsync(beforeMessageId.Value);
                if (beforeMessage != null)
                    query = query.Where(x => x.CreatedAt < beforeMessage.CreatedAt);
            }

            query = query
                 .OrderByDescending(x => x.CreatedAt)
                 .ThenByDescending(x => x.Id)
                .Take(pageSize);


            var messages = await query.ToListAsync();

            messages.Reverse();

            var messagesDtos = messages
                .Select(x => new MessageDto
                {
                    Id = x.Id,
                     SenderId = x.SenderId,
                     Content = x.Content,
                     CreatedAt = x.CreatedAt,
                });

            return messagesDtos;
        }


    }
}
