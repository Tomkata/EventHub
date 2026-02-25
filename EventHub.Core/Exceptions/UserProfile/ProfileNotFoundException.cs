namespace EventHub.Core.Exceptions.UserProfile
{
    public class ProfileNotFoundException : Exception
    {
        public ProfileNotFoundException()
            :base("The profile doesn`t exist.")
        {
        }
    }
}
