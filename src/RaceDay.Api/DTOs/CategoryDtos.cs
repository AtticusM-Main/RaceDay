using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.DTOs;

public class CategoryRequest
{
    /// <summary>Category name, e.g. "Under 20", "Senior", "10km".</summary>
    /// <example>10km</example>
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Distance in km. Leave empty for age-based categories.</summary>
    /// <example>10</example>
    [Range(0.1, 999.99)]
    public decimal? Distance { get; set; }

    /// <summary>Maximum number of entries allowed. Leave empty for no limit.</summary>
    /// <example>300</example>
    [Range(1, 100000)]
    public int? MaxParticipants { get; set; }
}

public class CategoryResponse
{
    public int CategoryId { get; set; }
    public int EventId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal? Distance { get; set; }
    public int? MaxParticipants { get; set; }

    /// <summary>How many participants have enrolled so far.</summary>
    public int EnrolledCount { get; set; }
}
