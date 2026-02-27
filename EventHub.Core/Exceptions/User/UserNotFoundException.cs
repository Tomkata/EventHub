
using EventHub.Core.AppException;

namespace EventHub.Core.Exceptions.User
{
    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException()
            : base("User doesnt exist!")
        {
        }
    }
}
