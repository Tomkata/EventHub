
namespace EventHub.Core.Exceptions.Oranizer.ForApprove
{
    public class OrganizerRequestNotFoundException : Exception
    {
        public OrganizerRequestNotFoundException()
            :base("Invalid user to approve!")
        {
            
        }
    }
}
