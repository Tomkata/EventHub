using EventHub.Services.Interfaces.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Web.Controllers
{
    public class ChatController : Controller
    {
        [Authorize]
        public IActionResult Conversation(Guid conversationId)
        {
            return View(conversationId);
        }

        [Authorize]
        public async Task<IActionResult> TestConversation(
    [FromServices] IConversationService conversationService,
    CancellationToken cancellationToken)
        {
            // Замени с реални user IDs от базата
            var user1Id = "1114f597-8c53-415c-90c5-f540f479631d";
            var user2Id = "962008b3-f9b1-499f-89cc-757b61bf1020";

            var conversationId = await conversationService
                .GetOrCreateConversationAsync(user1Id, user2Id, cancellationToken);

            return RedirectToAction("Conversation", new { conversationId });
        }
    }
}
