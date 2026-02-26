
namespace EventHub.Core.Exceptions.Location
{
using EventHub.Core.AppException;
    public class InvalidLocationException : ValidationException
    {
        public InvalidLocationException()
            :base("Invalid Location!")
        {
            
        }
    }
}
