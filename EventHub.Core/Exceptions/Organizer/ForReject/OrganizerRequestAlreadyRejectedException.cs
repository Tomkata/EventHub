
namespace EventHub.Core.Exceptions.Oranizer.ForReject
{
using EventHub.Core.AppException;
    public class OrganizerRequestAlreadyRejectedException : ConflictException
    {
        public OrganizerRequestAlreadyRejectedException()
            :base("The user is already in rejected status!")
        {
        }
    }
}
