using BookStore.API.Dtos;
using FluentValidation;

namespace BookStore.API.Validators;

public sealed class UserRegisterDtoValidator : AbstractValidator<UserRegisterDto>
{
    public UserRegisterDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(200);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(25);
        RuleFor(x => x.Address).MaximumLength(250);
        RuleFor(x => x.City).MaximumLength(120);
        RuleFor(x => x.Pincode).MaximumLength(20);
    }
}

public sealed class BookCreateDtoValidator : AbstractValidator<BookCreateDto>
{
    public BookCreateDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(220);
        RuleFor(x => x.ISBN).NotEmpty().MaximumLength(20);
        RuleFor(x => string.IsNullOrWhiteSpace(x.AuthorName) ? x.Author : x.AuthorName).NotEmpty().MaximumLength(120);
        RuleFor(x => string.IsNullOrWhiteSpace(x.CategoryName) ? x.Category : x.CategoryName).NotEmpty().MaximumLength(120);
        RuleFor(x => string.IsNullOrWhiteSpace(x.PublisherName) ? x.Publisher : x.PublisherName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.ImageUrl).MaximumLength(600);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
    }
}

public sealed class BookUpdateDtoValidator : AbstractValidator<BookUpdateDto>
{
    public BookUpdateDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(220);
        RuleFor(x => x.ISBN).NotEmpty().MaximumLength(20);
        RuleFor(x => string.IsNullOrWhiteSpace(x.AuthorName) ? x.Author : x.AuthorName).NotEmpty().MaximumLength(120);
        RuleFor(x => string.IsNullOrWhiteSpace(x.CategoryName) ? x.Category : x.CategoryName).NotEmpty().MaximumLength(120);
        RuleFor(x => string.IsNullOrWhiteSpace(x.PublisherName) ? x.Publisher : x.PublisherName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.ImageUrl).MaximumLength(600);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
    }
}

public sealed class PlaceOrderDtoValidator : AbstractValidator<PlaceOrderDto>
{
    public PlaceOrderDtoValidator()
    {
        RuleFor(x => x.ShippingAddress).NotEmpty().MaximumLength(250);
        RuleFor(x => x.City).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Pincode).NotEmpty().MaximumLength(20);
        RuleFor(x => x.PaymentMethod).NotEmpty().MaximumLength(30);
        RuleFor(x => x.Items).NotEmpty();

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.BookId).GreaterThan(0);
            item.RuleFor(i => i.Qty).GreaterThan(0);
        });
    }
}

public sealed class ReviewCreateDtoValidator : AbstractValidator<ReviewCreateDto>
{
    public ReviewCreateDtoValidator()
    {
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Comment).NotEmpty().MaximumLength(800);
    }
}

public sealed class UpdateOrderStatusDtoValidator : AbstractValidator<UpdateOrderStatusDto>
{
    private static readonly string[] AllowedStatuses =
    [
        "Placed", "Processing", "Packed", "Shipped", "Delivered", "Cancelled", "ReturnRequested", "Returned", "Refunded"
    ];

    public UpdateOrderStatusDtoValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(status => AllowedStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Invalid order status.");
    }
}

public sealed class OrderActionDtoValidator : AbstractValidator<OrderActionDto>
{
    public OrderActionDtoValidator()
    {
        RuleFor(x => x.Reason).MaximumLength(300);
    }
}

public sealed class UpdateProfileDtoValidator : AbstractValidator<UpdateProfileDto>
{
    public UpdateProfileDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(25);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(250);
        RuleFor(x => x.City).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Pincode).NotEmpty().MaximumLength(20);
    }
}
