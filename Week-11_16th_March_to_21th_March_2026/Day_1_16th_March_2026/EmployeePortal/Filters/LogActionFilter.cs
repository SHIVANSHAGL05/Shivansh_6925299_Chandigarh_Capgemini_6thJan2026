using Microsoft.AspNetCore.Mvc.Filters;

namespace EmployeePortal.Filters
{
    /// <summary>
    /// ACTION FILTER — logs action name, logged-in user, and timestamp
    /// BEFORE and AFTER every action it is applied to.
    ///
    /// Registered GLOBALLY in Program.cs so all controllers are covered.
    /// Can also be applied per-controller with [TypeFilter(typeof(LogActionFilter))].
    ///
    /// IActionFilter requires:
    ///   OnActionExecuting  — fires BEFORE the action body
    ///   OnActionExecuted   — fires AFTER  the action body
    /// </summary>
    public class LogActionFilter : IActionFilter
    {
        private readonly ILogger<LogActionFilter> _logger;

        // ILogger<T> is injected by ASP.NET Core's DI container.
        public LogActionFilter(ILogger<LogActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Read the logged-in username from Session (null if not logged in).
            var user = context.HttpContext.Session.GetString("Username") ?? "Anonymous";

            _logger.LogInformation(
                "[LOG] ▶ BEFORE | Controller: {Controller} | Action: {Action} | User: {User} | Time: {Time}",
                context.RouteData.Values["controller"],
                context.ActionDescriptor.DisplayName,
                user,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var user = context.HttpContext.Session.GetString("Username") ?? "Anonymous";

            if (context.Exception != null)
            {
                _logger.LogWarning(
                    "[LOG] ✖ AFTER  | Controller: {Controller} | Action: {Action} | User: {User} | Time: {Time} | Exception: {Ex}",
                    context.RouteData.Values["controller"],
                    context.ActionDescriptor.DisplayName,
                    user,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    context.Exception.Message);
            }
            else
            {
                _logger.LogInformation(
                    "[LOG] ✔ AFTER  | Controller: {Controller} | Action: {Action} | User: {User} | Time: {Time} | Status: OK",
                    context.RouteData.Values["controller"],
                    context.ActionDescriptor.DisplayName,
                    user,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }
    }
}
