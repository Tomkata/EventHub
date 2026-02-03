namespace EventHub.Core.Exceptions.Oranizer.ForApply
{
    public class UserAlreadyOrganizerException : Exception
    {
        public UserAlreadyOrganizerException()
            :base("User is already an organizer!")
        {
            
        }
    }
}
