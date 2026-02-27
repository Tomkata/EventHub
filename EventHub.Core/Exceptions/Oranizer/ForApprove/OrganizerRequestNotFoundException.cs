

namespace EventHub.Core.Exceptions.Oranizer.ForApprove
{
using EventHub.Core.AppException;
    public class OrganizerRequestNotFoundException : NotFoundException
    {
        public OrganizerRequestNotFoundException()
            :base("Invalid user to approve!")
        {
            
        }
    }
}
