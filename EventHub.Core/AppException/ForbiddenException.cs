
namespace EventHub.Core.AppException
{
    public class ForbiddenException : DomainException
    {
        public ForbiddenException() { }
        public ForbiddenException(string message) : base(message) { }
        public ForbiddenException(string message, Exception innerException) : base(message, innerException) { }
    }
}
