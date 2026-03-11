namespace EventHub.Web.Hubs
{
    public class RateLimitData
    {
        private int _count = 0;
        private DateTime _windowStart = DateTime.UtcNow;
        private readonly object _lock = new();

        public bool TryConsume()
        {
            lock (_lock)
            {
                if (DateTime.UtcNow - _windowStart > TimeSpan.FromMinutes(1))
                {
                    _count = 0;
                    _windowStart = DateTime.UtcNow;
                }
                if (_count >= 30) return false;
                _count++;
                return true;
            }
        }
    }
}
