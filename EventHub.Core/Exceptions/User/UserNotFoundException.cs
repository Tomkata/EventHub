
namespace EventHub.Core.Exceptions.User
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException()
            :base("User doesn't exist!")
        {
            
        }
    }
}
