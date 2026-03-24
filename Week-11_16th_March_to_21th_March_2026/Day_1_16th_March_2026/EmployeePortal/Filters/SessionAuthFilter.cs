using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmployeePortal.Filters
{
    /// <summary>
    /// SESSION AUTH FILTER — a guard that runs before any action on a
    /// controller it decorates.
    ///
    /// If the "Username" session key is absent (user not logged in),
    /// it immediately redirects to Account/Login and sets a TempData message.
    ///
    /// Apply with: [TypeFilter(typeof(SessionAuthFilter))]
    /// on any controller or action requiring a logged-in admin.
    /// </summary>
    public class SessionAuthFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var username = context.HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                if (context.Controller is Controller ctrl)
                    ctrl.TempData["InfoMessage"] = "Please log in to access this page.";

                // Short-circuit — action body never runs.
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
