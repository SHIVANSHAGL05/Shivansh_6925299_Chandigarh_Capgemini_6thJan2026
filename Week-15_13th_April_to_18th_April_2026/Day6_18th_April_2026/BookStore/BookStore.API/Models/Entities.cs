using System.ComponentModel.DataAnnotations;

namespace BookStore.API.Models;

public sealed class Role
{
    public int RoleId { get; set; }

    [MaxLength(50)]
    public string RoleName { get; set; } = string.Empty;

    public ICollection<AppUser> Users { get; set; } = [];
}

public sealed class AppUser
{
    public int AppUserId { get; set; }

    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(25)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(300)]
    public string PasswordHash { get; set; } = string.Empty;

    public int RoleId { get; set; }
    public Role? RoleRef { get; set; }

    [MaxLength(20)]
    public string Role { get; set; } = "Customer";

    public UserProfile? Profile { get; set; }
    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<Wishlist> Wishlists { get; set; } = [];
}

public sealed class UserProfile
{
    public int ProfileId { get; set; }
    public int UserId { get; set; }

    [MaxLength(250)]
    public string Address { get; set; } = string.Empty;

    [MaxLength(120)]
    public string City { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Pincode { get; set; } = string.Empty;

    public AppUser? User { get; set; }
}

public sealed class Category
{
    public int CategoryId { get; set; }

    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Book> Books { get; set; } = [];
}

public sealed class Author
{
    public int AuthorId { get; set; }

    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Book> Books { get; set; } = [];
}

public sealed class Publisher
{
    public int PublisherId { get; set; }

    [MaxLength(120)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Book> Books { get; set; } = [];
}

public sealed class Book
{
    public int BookId { get; set; }

    [MaxLength(220)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(20)]
    public string ISBN { get; set; } = string.Empty;

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public int AuthorId { get; set; }
    public Author? Author { get; set; }

    public int PublisherId { get; set; }
    public Publisher? Publisher { get; set; }

    [MaxLength(600)]
    public string ImageUrl { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public ICollection<OrderItem> OrderItems { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<Wishlist> Wishlists { get; set; } = [];
}

public sealed class Order
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public AppUser? User { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }

    [MaxLength(40)]
    public string Status { get; set; } = "Placed";

    [MaxLength(250)]
    public string ShippingAddress { get; set; } = string.Empty;

    [MaxLength(120)]
    public string ShippingCity { get; set; } = string.Empty;

    [MaxLength(20)]
    public string ShippingPincode { get; set; } = string.Empty;

    [MaxLength(30)]
    public string PaymentMethod { get; set; } = "card";

    public ICollection<OrderItem> Items { get; set; } = [];
}

public sealed class OrderItem
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public int BookId { get; set; }
    public Book? Book { get; set; }
    public int Qty { get; set; }
    public decimal Price { get; set; }
}

public sealed class Review
{
    public int ReviewId { get; set; }
    public int UserId { get; set; }
    public AppUser? User { get; set; }
    public int BookId { get; set; }
    public Book? Book { get; set; }
    public int Rating { get; set; }

    [MaxLength(800)]
    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}

public sealed class Wishlist
{
    public int UserId { get; set; }
    public AppUser? User { get; set; }
    public int BookId { get; set; }
    public Book? Book { get; set; }
    public DateTime AddedUtc { get; set; } = DateTime.UtcNow;
}

public sealed class EmailLog
{
    public int EmailLogId { get; set; }

    [MaxLength(200)]
    public string ToEmail { get; set; } = string.Empty;

    [MaxLength(180)]
    public string Subject { get; set; } = string.Empty;

    public DateTime SentDate { get; set; } = DateTime.UtcNow;

    [MaxLength(40)]
    public string Status { get; set; } = "Queued";

    [MaxLength(500)]
    public string Note { get; set; } = string.Empty;
}
