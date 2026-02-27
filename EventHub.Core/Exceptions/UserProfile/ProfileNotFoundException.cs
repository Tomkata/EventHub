
namespace EventHub.Core.Exceptions.UserProfile
{
using EventHub.Core.AppException;
    public class ProfileNotFoundException : NotFoundException
    {
        public ProfileNotFoundException()
            :base("The profile doesn`t exist.")
        {
        }
    }
}
