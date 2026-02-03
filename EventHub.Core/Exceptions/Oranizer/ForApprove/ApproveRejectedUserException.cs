namespace EventHub.Core.Exceptions.Oranizer.ForApprove
{
    public class ApproveRejectedUserException : Exception
    {
        public ApproveRejectedUserException()
            :base("Cannot approve rejected user!")
        {
            
        }
    }
}
