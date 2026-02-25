

namespace EventHub.Core.Exceptions.Oranizer.ForApply
{
    public class AdminCannotApplyException : Exception
    {
        public AdminCannotApplyException()
            :base("Admin cannot apply for organizer.")
        {
        }
    }
}
