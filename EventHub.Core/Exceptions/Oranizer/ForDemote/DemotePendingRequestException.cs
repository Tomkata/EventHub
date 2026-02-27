using EventHub.Core.AppException;

namespace EventHub.Core.Exceptions.Oranizer.ForDemote
{
    public class DemotePendingRequestException : ConflictException
    {
        public DemotePendingRequestException()
            :base("Cannot demote user with pending request!")
        {
        }
    }
}
