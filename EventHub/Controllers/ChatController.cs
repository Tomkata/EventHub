
namespace EventHub.Web.Controllers
{
    using AutoMapper;
    using EventHub.Services.Common;
    using EventHub.Services.Interfaces.Messaging;
    using EventHub.Web.ViewModels.Chat;
    using EventHub.Web.ViewModels.Events;
    using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
    public class ChatController : BaseController
    {

        private readonly IConversationService _conversationService;
        private readonly IMapper _mapper;

        public ChatController(IConversationService conversationService,
                              IMapper mapper)
        {
            this._conversationService = conversationService;
            this._mapper = mapper;
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
        public IActionResult Conversation(Guid conversationId)
        {
            return View(conversationId);
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
