
namespace EventHub.Core.Exceptions.Oranizer.ForApply
{
using EventHub.Core.AppException;
    public class OrganizerCooldownNotExpiredException : ConflictException
    {
        public OrganizerCooldownNotExpiredException()
            :base("Organizer cooldown is not expired!")
        {
        }
    }
}
