

namespace EventHub.Core.Exceptions.Oranizer.ForDemote
{
using EventHub.Core.AppException;
    public class DemoteRejectedException : ConflictException
    {
        public DemoteRejectedException()
            :base("Cannot demote user with rejected status!")
        {
        }
    }
}
