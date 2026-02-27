

namespace EventHub.Core.AppException
{
    public class NotFoundException : DomainException
    {
        public NotFoundException() { }
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
