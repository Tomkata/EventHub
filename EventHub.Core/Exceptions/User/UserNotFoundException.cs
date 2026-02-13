
namespace EventHub.Core.Exceptions.User
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException()
            :base("You need to log in!")
        {
            
        }
    }
}
