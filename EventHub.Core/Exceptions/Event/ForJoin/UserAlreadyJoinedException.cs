namespace EventHub.Core.Exceptions.Event.ForJoin
{
    public class UserAlreadyJoinedException: Exception
    {
        public UserAlreadyJoinedException()
            :base("The user is already a participant in this event!")
        {
        }
    }
}
