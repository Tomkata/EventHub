namespace EventHub.Core.Exceptions.Oranizer.ForDemote
{
    public class DemoteRejectedException : Exception
    {
        public DemoteRejectedException()
            :base("Cannot demote user with rejected status!")
        {
        }
    }
}
