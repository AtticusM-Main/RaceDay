namespace RaceDay.Api.Models;

/// <summary>
/// A user role. Only two roles exist: Organiser and Participant.
/// </summary>
public class Role
{
    public const string Organiser = "Organiser";
    public const string Participant = "Participant";

    public int RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;

    public ICollection<User> Users { get; set; } = new List<User>();
}
