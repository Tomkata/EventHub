namespace EventHub.Core.Exceptions.Oranizer.ForApply
{
    public class OrganizerCooldownNotExpiredException : Exception
    {
        public OrganizerCooldownNotExpiredException()
            :base("Organizer cooldown is not expired")
        {
        }
    }
}
