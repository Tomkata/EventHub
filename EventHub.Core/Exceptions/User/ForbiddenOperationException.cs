
namespace EventHub.Core.Exceptions.User
{
using EventHub.Core.AppException;
    public class ForbiddenOperationException : ForbiddenException
    {
        public ForbiddenOperationException()
            :base("User doesn't have that permissions!")    
        {
        }
    }
}
