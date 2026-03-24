namespace EmployeePortal.Models
{
    /// <summary>
    /// Passed to Views/Shared/Error.cshtml.
    /// Contains both VS-generated fields (RequestId) and our
    /// CustomExceptionFilter fields (Message, ExceptionType, Detail).
    /// </summary>
    public class ErrorViewModel
    {
        // Used by Visual Studio's auto-generated HomeController.Error action.
        public string? RequestId  { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        // Used by our CustomExceptionFilter.
        public string Message       { get; set; } = "An unexpected error occurred. Please try again.";
        public string ExceptionType { get; set; } = string.Empty;
        public string Detail        { get; set; } = string.Empty;
    }
}
