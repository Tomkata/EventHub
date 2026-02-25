using EventHub.Core.Common;

namespace EventHub.Core.DTOs.Location
{
    public class LocationDto
    {
        public Guid Id { get; set; }
        public string City { get; set; } = null !;
        public int ZipCode { get; set; }
    }
}
