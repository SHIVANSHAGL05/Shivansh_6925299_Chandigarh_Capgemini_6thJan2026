namespace StudentPortal.Services
{
    public class RequestLogEntry
    {
        public string   Url        { get; set; } = string.Empty;
        public string   Method     { get; set; } = string.Empty;
        public long     ElapsedMs  { get; set; }
        public int      StatusCode { get; set; }
        public DateTime Timestamp  { get; set; }
    }
}
