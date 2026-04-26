namespace StudentPortal.Services
{
    public interface IRequestLogService
    {
        void Add(RequestLogEntry entry);
        IReadOnlyList<RequestLogEntry> GetAll();
        void Clear();
    }
}
