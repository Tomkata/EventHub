namespace EventHub.Core.DTOs
{
    public class CreateEventDto
    {
        public string Title { get; set; }
        public int MaxParticipants { get;   set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid CategoryId { get; set; }
        public Guid LocationId { get; set; }    
        public string OrganizerId { get; set; }
        public string ImagePath { get; set; }
            

    }
}
