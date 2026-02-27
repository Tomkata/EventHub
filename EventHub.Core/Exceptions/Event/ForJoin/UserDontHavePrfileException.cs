
namespace EventHub.Core.Exceptions.Event.ForJoin
{
    using EventHub.Core.AppException;

    public class UserDontHavePrfileException : ConflictException
    {
        public UserDontHavePrfileException()
            :base("Please, complete your profile to join events.")
        {
        }
    }
}
