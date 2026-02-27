

using EventHub.Core.AppException;

namespace EventHub.Core.Exceptions.Oranizer.ForApply
{
    public class AdminCannotApplyException : ForbiddenException
    {
        public AdminCannotApplyException()
            :base("Admin cannot apply for organizer.")
        {
        }
    }
}
