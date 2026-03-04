namespace EventHub.Core.Models.Common
{
    public abstract class BaseModel
    {

        protected BaseModel()
        {
            this.CreatedAt = DateTime.UtcNow;
        }

        public DateTime CreatedAt { get; set; }
    }
}
