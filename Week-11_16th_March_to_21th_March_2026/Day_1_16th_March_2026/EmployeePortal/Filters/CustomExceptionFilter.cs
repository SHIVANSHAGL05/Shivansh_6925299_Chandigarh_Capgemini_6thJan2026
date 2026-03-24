using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using EmployeePortal.Models;

namespace EmployeePortal.Filters
{
    /// <summary>
    /// EXCEPTION FILTER — intercepts any unhandled exception thrown inside a
    /// controller action and returns a friendly Error view instead of crashing.
    ///
    /// Registered GLOBALLY in Program.cs — covers every controller without
    /// needing any attribute on individual actions.
    ///
    /// IExceptionFilter requires:
    ///   OnException — called when an unhandled exception bubbles up.
    ///
    /// Key: context.ExceptionHandled = true tells ASP.NET Core to stop
    /// propagating the exception after we have handled it here.
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
            // 1. Log the full exception to the console.
            _logger.LogError(
                context.Exception,
                "[EXCEPTION FILTER] ✖ Caught | Controller: {Controller} | Action: {Action} | Message: {Message}",
                context.RouteData.Values["controller"],
                context.RouteData.Values["action"],
                context.Exception.Message);

            // 2. Build a friendly ErrorViewModel.
            var model = new ErrorViewModel
            {
                Message       = "Something went wrong. Our team has been notified.",
                ExceptionType = context.Exception.GetType().Name,
                Detail        = context.Exception.Message
            };

            // 3. Return the Shared/Error view with HTTP 500.
            context.Result = new ViewResult
            {
                ViewName   = "Error",
                ViewData   = new ViewDataDictionary(
                                 new EmptyModelMetadataProvider(),
                                 new ModelStateDictionary())
                             { Model = model },
                StatusCode = 500
            };

            // 4. Mark as handled — stops further propagation.
            context.ExceptionHandled = true;
        }
    }
}
