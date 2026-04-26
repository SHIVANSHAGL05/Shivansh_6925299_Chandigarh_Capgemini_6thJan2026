using System.ComponentModel.DataAnnotations;

namespace SmartHealthcare.Core.Common;

/// <summary>Ensures the date is in the future (tomorrow or later).</summary>
public class FutureDateAttribute : ValidationAttribute
{
    public FutureDateAttribute()
        => ErrorMessage = "Appointment date must be a future date.";

    public override bool IsValid(object? value)
    {
        if (value is DateTime date)
            return date.Date > DateTime.Today;
        return true; // let [Required] handle null
    }
}

/// <summary>Validates common blood group strings: A+, A-, B+, B-, AB+, AB-, O+, O-.</summary>
public class ValidBloodGroupAttribute : ValidationAttribute
{
    private static readonly HashSet<string> _valid =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-"
        };

    public ValidBloodGroupAttribute()
        => ErrorMessage = "Invalid blood group. Must be one of: A+, A-, B+, B-, AB+, AB-, O+, O-.";

    public override bool IsValid(object? value)
    {
        if (value is null) return true;          // optional field — let [Required] handle null
        return _valid.Contains(value.ToString()!);
    }
}

/// <summary>Ensures a password meets the strength policy (min 8 chars, digit, uppercase).</summary>
public class StrongPasswordAttribute : ValidationAttribute
{
    public int MinLength { get; set; } = 8;

    public StrongPasswordAttribute()
        => ErrorMessage = "Password must be at least 8 characters, contain a digit and an uppercase letter.";

    public override bool IsValid(object? value)
    {
        if (value is not string pwd) return true;
        return pwd.Length >= MinLength
               && pwd.Any(char.IsDigit)
               && pwd.Any(char.IsUpper);
    }
}

/// <summary>Ensures phone number contains only digits, spaces, +, -, (, ).</summary>
public class InternationalPhoneAttribute : ValidationAttribute
{
    public InternationalPhoneAttribute()
        => ErrorMessage = "Phone number contains invalid characters.";

    public override bool IsValid(object? value)
    {
        if (value is null) return true;
        var s = value.ToString()!;
        return s.All(c => char.IsDigit(c) || c is '+' or '-' or ' ' or '(' or ')');
    }
}
