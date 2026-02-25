namespace EventHub.Core.Exceptions.Event.ForJoin
{
    public class UserDontHavePrfileException : Exception
    {
        public UserDontHavePrfileException()
            :base("Please, complete your profile to join events.")
        {
        }
    }
}
