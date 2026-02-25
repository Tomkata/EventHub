using System.Drawing;

namespace EventHub.Core.Exceptions.UserProfile
{
    public class ProfileRequiredException : Exception
    {
        public ProfileRequiredException()
            :base("You need to complete your profile.")
        {
            
        }
    }
}
