namespace EventHub.Core.Exceptions.Event.ForJoin
{
    public class UserAlreadyJoinedException: Exception
    {
        public UserAlreadyJoinedException()
            :base("You have already joined this event.")
        {
        }
    }
}
