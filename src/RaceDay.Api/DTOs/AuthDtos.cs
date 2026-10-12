using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.DTOs;

public class RegisterRequest
{
    /// <summary>The user's full name.</summary>
    /// <example>Lerato Mokoena</example>
    [Required, StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    /// <summary>Email address, used to log in. Must be unique.</summary>
    /// <example>lerato.mokoena@example.com</example>
    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    /// <summary>At least 8 characters, with an uppercase letter, a lowercase letter and a digit.</summary>
    /// <example>Password123!</example>
    [Required, StringLength(100, MinimumLength = 8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
        ErrorMessage = "Password must contain an uppercase letter, a lowercase letter and a digit.")]
    public string Password { get; set; } = string.Empty;

    /// <summary>The role to register as: "Organiser" or "Participant".</summary>
    /// <example>Participant</example>
    [Required]
    public string Role { get; set; } = string.Empty;
}

public class RegisterResponse
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class LoginRequest
{
    /// <example>thandiwe.nkosi@example.com</example>
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <example>Password123!</example>
    [Required]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    /// <summary>JWT bearer token. Send it as "Authorization: Bearer {token}" on later requests.</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>When the token (session) expires, in UTC.</summary>
    public DateTime ExpiresAt { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
