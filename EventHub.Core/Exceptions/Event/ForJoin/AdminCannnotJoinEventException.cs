

namespace EventHub.Core.Exceptions.Event.ForJoin
{
    public  class AdminCannnotJoinEventException : Exception
    {
        public AdminCannnotJoinEventException()
            :base("Admin cannot join events!")        
        {
        }
    }
}
