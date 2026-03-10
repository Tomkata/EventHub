
namespace EventHub.Web.Controllers
{
    using AutoMapper;
    using EventHub.Services.Common;
    using EventHub.Services.Interfaces.Messaging;
    using EventHub.Web.ViewModels.Chat;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    public class ChatController : BaseController
    {

        private readonly IConversationService _conversationService;
        private readonly IMapper _mapper;
        private readonly IMessageService _messageService;


        public ChatController(IConversationService conversationService,
                              IMapper mapper,
                              IMessageService messageService)
        {
            this._conversationService = conversationService;
            this._mapper = mapper;
            this._messageService = messageService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            var userId = GetUserId();

            var conversations = await _conversationService
                .GetUserConversationsAsync(userId, pageNumber, 20, cancellationToken);

            var mapped = _mapper.Map<List<ConversationPreviewViewModel>>(conversations.Data);

            var model = new PagedResult<ConversationPreviewViewModel>
            {
                Data = mapped,
                CurrentPageNumber = conversations.CurrentPageNumber,
                PageSize = conversations.PageSize,
                TotalRecords = conversations.TotalRecords
            };

            return View(model);
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Conversation(Guid conversationId,
                                                      Guid beforeMessageId,
                                        CancellationToken cancellationToken)
        {
            var currUserId = GetUserId();

            if (!await _conversationService.IsUserParticipantAsync(conversationId, currUserId))
                return Forbid();


            var conversationInfo = await _conversationService
                .GetConversationInfoAsync(conversationId, currUserId, cancellationToken);

            var messages = await _messageService
                .GetConversationMessagesAsync(conversationId, null, 20);

            var model = new ConversationViewModel
            {
                ConversationId = conversationId,
                OtherUserName = conversationInfo.OtherUserName,
                CurrentUserId = currUserId,
                OtherUserProfileImagePath = conversationInfo.OtherUserProfileImagePath,
                Messages = _mapper.Map<IEnumerable<MessageViewModel>>(messages)
            };


            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartConversation(string targetUserId,
            CancellationToken cancellation)
        {

            var currUserId = GetUserId();

            var conversationId = await _conversationService
                .GetOrCreateConversationAsync(currUserId, targetUserId, cancellation);


            return RedirectToAction(nameof(Conversation), new { conversationId });
        }


    }
}
