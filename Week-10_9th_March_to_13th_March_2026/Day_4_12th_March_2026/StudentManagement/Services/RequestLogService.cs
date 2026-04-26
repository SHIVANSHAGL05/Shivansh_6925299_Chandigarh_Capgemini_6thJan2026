namespace StudentPortal.Services
{
    public class RequestLogService : IRequestLogService
    {
        private readonly List<RequestLogEntry> _logs = new();
        private readonly object _lock = new();

        public void Add(RequestLogEntry entry)
        {
            lock (_lock)
            {
                // Keep only the last 50 entries to prevent unbounded memory growth
                if (_logs.Count >= 50)
                    _logs.RemoveAt(0);

                _logs.Add(entry);
            }
        }

        public IReadOnlyList<RequestLogEntry> GetAll()
        {
            lock (_lock)
            {
                return _logs.AsReadOnly();
            }
        }

        public void Clear()
        {
            lock (_lock) { _logs.Clear(); }
        }
    }
}
