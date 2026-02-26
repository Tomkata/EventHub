

using EventHub.Core.AppException;

namespace EventHub.Core.Exceptions.Event.ForJoin
{
    public  class AdminCannnotJoinEventException : ForbiddenException
    {
        public AdminCannnotJoinEventException()
            :base("Admin cannot join events!")        
        {
        }
    }
}
