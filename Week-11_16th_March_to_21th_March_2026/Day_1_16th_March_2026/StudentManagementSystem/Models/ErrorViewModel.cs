namespace StudentManagementSystem.Models
{
    /// <summary>
    /// Passed to Views/Shared/Error.cshtml by the CustomExceptionFilter.
    /// Also used by the VS-generated HomeController.Error action.
    ///
    /// RequestId  — populated by HomeController (VS default), shown in the error view.
    /// ShowRequestId — true when RequestId is non-empty.
    /// Message    — friendly message set by CustomExceptionFilter.
    /// ExceptionType / Detail — technical info for developers.
    /// </summary>
    public class ErrorViewModel
    {
        // Required by Visual Studio's auto-generated HomeController.cs
        public string? RequestId    { get; set; }
        public bool ShowRequestId   => !string.IsNullOrEmpty(RequestId);

        // Used by our CustomExceptionFilter
        public string Message       { get; set; } = "Something went wrong. Please try again.";
        public string ExceptionType { get; set; } = string.Empty;
        public string Detail        { get; set; } = string.Empty;
    }
}
