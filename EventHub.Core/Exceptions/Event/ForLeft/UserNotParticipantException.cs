namespace EventHub.Core.Exceptions.Event.ForLeft
{
    public class UserNotParticipantException : Exception
    {
        public UserNotParticipantException()
            :base("User is not participant in this event!")
        {
        }
    }
}
