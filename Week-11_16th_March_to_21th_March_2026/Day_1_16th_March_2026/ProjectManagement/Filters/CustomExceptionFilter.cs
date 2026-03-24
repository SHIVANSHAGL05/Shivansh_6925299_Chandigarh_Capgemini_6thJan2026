using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace ProductManagement.Filters
{
    /// <summary>
    /// EXCEPTION FILTER — intercepts any unhandled exception thrown inside a
    /// controller action and returns a friendly error view instead of crashing.
    ///
    /// Registered GLOBALLY in Program.cs so it covers every controller
    /// automatically — no attribute needed on individual actions.
    ///
    /// IExceptionFilter requires one method:
    ///   OnException — called when an unhandled exception bubbles up from an action.
    ///
    /// Key property: context.ExceptionHandled = true
    ///   Setting this tells ASP.NET Core "I've handled it — don't rethrow."
    ///   Without it the exception keeps propagating and shows the default error page.
    /// </summary>
    public class CustomExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<CustomExceptionFilter> _logger;

        public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            // ── 1. Log the full exception details to the console ──────────────
            _logger.LogError(
                context.Exception,
                "[EXCEPTION FILTER] ✖ Unhandled exception caught | " +
                "Controller: {Controller} | Action: {Action} | Message: {Message}",
                context.RouteData.Values["controller"],
                context.RouteData.Values["action"],
                context.Exception.Message);

            // ── 2. Build a friendly ViewModel for the Error view ──────────────
            var errorModel = new ErrorViewModel
            {
                Message       = "Something went wrong. Please try again later.",
                ExceptionType = context.Exception.GetType().Name,
                // Only expose the real message in Development — hide it in Production.
                Detail        = context.Exception.Message
            };

            // ── 3. Return the Error view with a 500 status code ───────────────
            context.Result = new ViewResult
            {
                ViewName   = "Error",                      // Views/Shared/Error.cshtml
                ViewData   = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(
                                 new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
                                 new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary())
                             {
                                 Model = errorModel
                             },
                StatusCode = 500
            };

            // ── 4. Mark exception as handled so ASP.NET Core stops propagating ─
            context.ExceptionHandled = true;
        }
    }

    /// <summary>
    /// Simple ViewModel passed to the Error view.
    /// </summary>
    public class ErrorViewModel
    {
        public string Message       { get; set; } = string.Empty;
        public string ExceptionType { get; set; } = string.Empty;
        public string Detail        { get; set; } = string.Empty;
    }
}
