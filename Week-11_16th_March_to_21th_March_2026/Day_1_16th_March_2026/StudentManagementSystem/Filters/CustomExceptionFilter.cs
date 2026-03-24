using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Filters
{
    /// <summary>
    /// EXCEPTION FILTER — intercepts any unhandled exception from any controller
    /// action and returns a friendly Error view instead of crashing the app.
    ///
    /// Registered GLOBALLY in Program.cs — no attribute needed on controllers.
    ///
    /// Key: setting context.ExceptionHandled = true stops the exception
    /// from propagating further up the middleware pipeline.
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
            // Log the full exception details to the console.
            _logger.LogError(
                context.Exception,
                "[EXCEPTION FILTER] ✖ Caught | Controller: {Controller} | Action: {Action} | {Message}",
                context.RouteData.Values["controller"],
                context.RouteData.Values["action"],
                context.Exception.Message);

            // Build a user-friendly error model.
            var errorModel = new ErrorViewModel
            {
                Message       = "An unexpected error occurred. Please try again.",
                ExceptionType = context.Exception.GetType().Name,
                Detail        = context.Exception.Message
            };

            // Return the Error view with HTTP 500.
            context.Result = new ViewResult
            {
                ViewName   = "Error",
                ViewData   = new ViewDataDictionary(
                                 new EmptyModelMetadataProvider(),
                                 new ModelStateDictionary())
                             { Model = errorModel },
                StatusCode = 500
            };

            // Mark as handled — no further propagation.
            context.ExceptionHandled = true;
        }
    }
}
