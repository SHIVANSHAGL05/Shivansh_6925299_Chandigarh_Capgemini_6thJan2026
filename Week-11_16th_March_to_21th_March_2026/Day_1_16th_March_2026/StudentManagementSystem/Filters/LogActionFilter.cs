using Microsoft.AspNetCore.Mvc.Filters;

namespace StudentManagementSystem.Filters
{
    /// <summary>
    /// ACTION FILTER — logs the controller name, action name, and timestamp
    /// BEFORE and AFTER every action it is applied to.
    ///
    /// Applied globally in Program.cs so all controllers are covered.
    /// Can also be applied per-controller or per-action with:
    ///   [TypeFilter(typeof(LogActionFilter))]
    /// </summary>
    public class LogActionFilter : IActionFilter
    {
        private readonly ILogger<LogActionFilter> _logger;

        public LogActionFilter(ILogger<LogActionFilter> logger)
        {
            _logger = logger;
        }

        // Fires BEFORE the action method body executes.
        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation(
                "[LOG] ▶ BEFORE | Controller: {Controller} | Action: {Action} | Time: {Time}",
                context.RouteData.Values["controller"],
                context.ActionDescriptor.DisplayName,
                DateTime.Now.ToString("HH:mm:ss"));
        }

        // Fires AFTER the action method body executes.
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                _logger.LogWarning(
                    "[LOG] ✖ AFTER  | Controller: {Controller} | Action: {Action} | Time: {Time} | Exception: {Ex}",
                    context.RouteData.Values["controller"],
                    context.ActionDescriptor.DisplayName,
                    DateTime.Now.ToString("HH:mm:ss"),
                    context.Exception.Message);
            }
            else
            {
                _logger.LogInformation(
                    "[LOG] ✔ AFTER  | Controller: {Controller} | Action: {Action} | Time: {Time} | Status: OK",
                    context.RouteData.Values["controller"],
                    context.ActionDescriptor.DisplayName,
                    DateTime.Now.ToString("HH:mm:ss"));
            }
        }
    }
}
