namespace EventHub.Core.Exceptions.Oranizer.ForReject
{
    public class RejectApprovedRequest : Exception
    {
        public RejectApprovedRequest() 
            :base("Cannot reject user with approved status!")
        {
            
        }
    }
}
