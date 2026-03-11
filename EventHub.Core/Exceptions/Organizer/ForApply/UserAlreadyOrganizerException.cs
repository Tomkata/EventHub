
namespace EventHub.Core.Exceptions.Oranizer.ForApply
{
using EventHub.Core.AppException;
    public class UserAlreadyOrganizerException : ConflictException
    {
        public UserAlreadyOrganizerException()
            :base("User is already an organizer!")
        {
            
        }
    }
}
