using NUnit.Framework;
using System;

[TestFixture]
public class LibraryTests
{
    private Library library;

    [SetUp]
    public void Setup()
    {
        library = new Library();
        library.AddBook("C# Basics", 2);
    }

    [Test]
    public void AddBook_NewBook_IncreasesCount()
    {
        string title = "ASP.NET Core";
        int quantity = 3;

        library.AddBook(title, quantity);

        Assert.That(library.GetBookCount(title), Is.EqualTo(3));

    }

    [Test]
    public void BorrowBook_ExistingBook_DecreasesCount()
    {
        string title = "C# Basics";

        library.BorrowBook(title);

        Assert.That(library.GetBookCount(title), Is.EqualTo(1));

    }
        [Test]
    public void BorrowBook_NotAvailable_ThrowsException()
    {
        string title = "Java";

        Assert.Throws<InvalidOperationException>(() => library.BorrowBook(title));
    }
}
