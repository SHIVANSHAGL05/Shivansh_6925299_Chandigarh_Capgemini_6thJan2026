using Microsoft.AspNetCore.Mvc.Filters;

namespace ProductManagement.Filters
{
    /// <summary>
    /// ACTION FILTER — runs code BEFORE and AFTER every controller action it decorates.
    ///
    /// How to use:
    ///   Option A — per action:     [LogAction]  on a single action method
    ///   Option B — per controller: [LogAction]  on the controller class
    ///   Option C — globally:       builder.Services.AddControllersWithViews(o =>
    ///                                  o.Filters.Add<LogActionFilter>());
    ///
    /// IActionFilter interface requires two methods:
    ///   OnActionExecuting  — fires BEFORE the action body runs
    ///   OnActionExecuted   — fires AFTER  the action body runs
    /// </summary>
    public class LogActionFilter : IActionFilter
    {
        // ILogger<T> is injected by ASP.NET Core's built-in DI container.
        // Log output goes to the console (and any other configured providers).
        private readonly ILogger<LogActionFilter> _logger;

        public LogActionFilter(ILogger<LogActionFilter> logger)
        {
            _logger = logger;
        }

        // ── Runs BEFORE the action ────────────────────────────────────────────
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var actionName     = context.ActionDescriptor.DisplayName;
            var controllerName = context.RouteData.Values["controller"];
            var timestamp      = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // LogInformation writes a coloured INFO line to the console window.
            _logger.LogInformation(
                "[LOG] ▶ BEFORE | Controller: {Controller} | Action: {Action} | Time: {Time}",
                controllerName, actionName, timestamp);
        }

        // ── Runs AFTER the action ─────────────────────────────────────────────
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var actionName     = context.ActionDescriptor.DisplayName;
            var controllerName = context.RouteData.Values["controller"];
            var timestamp      = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // If an exception was thrown, the Exception property is non-null.
            if (context.Exception != null)
            {
                _logger.LogWarning(
                    "[LOG] ✖ AFTER  | Controller: {Controller} | Action: {Action} | Time: {Time} | Exception: {Ex}",
                    controllerName, actionName, timestamp, context.Exception.Message);
            }
            else
            {
                _logger.LogInformation(
                    "[LOG] ✔ AFTER  | Controller: {Controller} | Action: {Action} | Time: {Time} | Status: OK",
                    controllerName, actionName, timestamp);
            }
        }
    }
}
