using LibraryApp.Models;

namespace LibraryApp.ViewModels
{
    public class BookViewModel
    {
        // Task 3: Core book + availability
        public Book Book { get; set; }
        public bool IsAvailable { get; set; }

        // Task 6: Borrower extension
        public string BorrowerName { get; set; }
        public bool IsBorrowed => !IsAvailable && !string.IsNullOrEmpty(BorrowerName);
    }
}
