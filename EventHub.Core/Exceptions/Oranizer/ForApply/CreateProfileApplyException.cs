

using EventHub.Core.AppException;

namespace EventHub.Core.Exceptions.Oranizer.ForApply
{
    public class CreateProfileApplyException : ConflictException
    {
        public CreateProfileApplyException()
            :base("Please complete your profile.")
        {
            
        }
    }
}
