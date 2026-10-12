using System.Security.Claims;

namespace RaceDay.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>Gets the logged-in user's id from their token.</summary>
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var id)
            ? id
            : throw new InvalidOperationException("Authenticated user has no valid id claim.");
    }
}
