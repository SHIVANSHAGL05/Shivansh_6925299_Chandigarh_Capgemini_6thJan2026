namespace BookStore.Web.Services.Books;

public sealed class BookApiEnvelope<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }
}

public sealed class BookApiDto
{
    public int BookId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Isbn { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Publisher { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public decimal AverageRating { get; init; }
    public int ReviewCount { get; init; }
}

public sealed class BookApiResult<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }

    public static BookApiResult<T> Ok(T data, string message = "Success") =>
        new() { Success = true, Message = message, Data = data };

    public static BookApiResult<T> Fail(string message) =>
        new() { Success = false, Message = message };
}
