
namespace EventHub.Core.Exceptions.UserProfile
{
using EventHub.Core.AppException;
    public class InvalidInterestException : ValidationException
    {
        public InvalidInterestException()
            :base("Invalid interest provided.")
        {
        }
    }
}
