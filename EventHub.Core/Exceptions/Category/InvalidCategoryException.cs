
namespace EventHub.Core.Exceptions.Category
{
    using AppException;

    public class InvalidCategoryException : ValidationException
    {
        public InvalidCategoryException()
            :base("Invalid category!")
        {
            
        }
    }
}
