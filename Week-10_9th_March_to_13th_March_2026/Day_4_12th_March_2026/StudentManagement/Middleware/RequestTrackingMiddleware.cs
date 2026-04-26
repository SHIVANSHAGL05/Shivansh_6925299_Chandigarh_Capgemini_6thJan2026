using System.Diagnostics;
using StudentPortal.Services;

namespace StudentPortal.Middleware
{
    public class RequestTrackingMiddleware
    {
        private readonly RequestDelegate    _next;
        private readonly IRequestLogService _logService;

        // IRequestLogService is Singleton — safe to inject into constructor
        public RequestTrackingMiddleware(
            RequestDelegate    next,
            IRequestLogService logService)
        {
            _next       = next;
            _logService = logService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            // Pass control to the next middleware in the pipeline
            await _next(context);

            stopwatch.Stop();

            // Record the log entry after response is generated
            var entry = new RequestLogEntry
            {
                Url        = context.Request.Path.ToString(),
                Method     = context.Request.Method,
                ElapsedMs  = stopwatch.ElapsedMilliseconds,
                StatusCode = context.Response.StatusCode,
                Timestamp  = DateTime.Now
            };

            _logService.Add(entry);
        }
    }
}
