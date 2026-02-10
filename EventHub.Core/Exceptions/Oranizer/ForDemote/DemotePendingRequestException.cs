namespace EventHub.Core.Exceptions.Oranizer.ForDemote
{
    public class DemotePendingRequestException : Exception
    {
        public DemotePendingRequestException()
            :base("Cannot demote user with pending request!")
        {
        }
    }
}
