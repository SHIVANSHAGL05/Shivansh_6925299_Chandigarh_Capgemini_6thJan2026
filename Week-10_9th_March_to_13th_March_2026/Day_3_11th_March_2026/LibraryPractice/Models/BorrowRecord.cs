using System;
using System.Collections.Generic;

namespace LibraryPractice.Models;

public partial class BorrowRecord
{
    public int RecordId { get; set; }

    public int? MemberId { get; set; }

    public int? BookId { get; set; }

    public DateOnly? BorrowDate { get; set; }

    public DateOnly? ReturnDate { get; set; }

    public virtual Book? Book { get; set; }

    public virtual Member? Member { get; set; }
}
