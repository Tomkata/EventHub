using EventHub.Core.Exceptions.User;


namespace EventHub.Web.Hubs
{
    using EventHub.Core.DTOs.Messaging;
    using EventHub.Services.Interfaces.Messaging;
    using EventHub.Services.Interfaces.User;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using System.Security.Claims;
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly IConversationService _conversationService;
        private readonly IUserProfileService _userProfileService;

        public ChatHub(IMessageService messageService,
                       IConversationService conversationService,
                       IUserProfileService userProfileService)
        {
            this._messageService = messageService;
            this._conversationService = conversationService;
            this._userProfileService = userProfileService;

        }


        public async Task SendMessage(      
            Guid conversationId,
            string message)
        {
            var userId = Context.UserIdentifier;

            if (!await _userProfileService.ExistsAsync(userId,CancellationToken.None))
                throw new UserNotFoundException();


            var messageId = await _messageService.SendMessageAsync(conversationId, userId, message);
            
            await Clients.Group(conversationId.ToString())
                .SendAsync("ReceiveMessage", new MessageDto
                {
                    Id = messageId,
                    Content = message,
                    SenderId = userId,
                    CreatedAt = DateTime.Now
                });

        }


        public async Task JoinConversation(Guid conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
        }


    }
}
