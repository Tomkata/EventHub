namespace EventHub.Core.Exceptions.UserProfile
{
    public class UserNotAppliedAnyInterestsException : Exception
    {
        public UserNotAppliedAnyInterestsException()
            :base("the user must have at least one added interest.")
        {
        }
    }
}
