
namespace EventHub.Core.Exceptions.Oranizer.ForApprove
{
using EventHub.Core.AppException;
    public class ApproveRejectedUserException : ConflictException
    {
        public ApproveRejectedUserException()
            :base("Cannot approve rejected user!")
        { 
        }
    }
}
