
namespace EventHub.Core.Exceptions.Oranizer.ForReject
{
using EventHub.Core.AppException;
    public class OrganizerRequestAlreadyApprovedException : ConflictException
    {
        public OrganizerRequestAlreadyApprovedException() 
            :base("Cannot reject user with approved status!")
        {
            
        }
    }
}
