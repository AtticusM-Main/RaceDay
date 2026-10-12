namespace RaceDay.Api.Models;

/// <summary>
/// An age or distance category inside an event, e.g. "Under 20", "Senior", "10km".
/// </summary>
public class Category
{
    public int CategoryId { get; set; }
    public int EventId { get; set; }
    public string Name { get; set; } = string.Empty;

    // Null for age-based categories that don't have their own distance.
    public decimal? Distance { get; set; }
    public int? MaxParticipants { get; set; }

    public Event Event { get; set; } = null!;
    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}
