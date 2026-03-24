using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StudentManagementSystem.Filters
{
    /// <summary>
    /// SESSION AUTH FILTER — a resource filter that runs before any action
    /// in a controller it decorates.
    ///
    /// If the "Username" session key is missing (user not logged in),
    /// it immediately redirects to the Login page.
    ///
    /// Apply with: [TypeFilter(typeof(SessionAuthFilter))]
    /// on any controller or action that requires a logged-in user.
    ///
    /// This replaces the manual session-check boilerplate that would
    /// otherwise be repeated at the top of every protected action.
    /// </summary>
    public class SessionAuthFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var username = context.HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                // Store a message for the Login page via TempData.
                if (context.Controller is Controller ctrl)
                {
                    ctrl.TempData["InfoMessage"] =
                        "Please log in to access this page.";
                }

                // Redirect to Login — stops the action from executing.
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
