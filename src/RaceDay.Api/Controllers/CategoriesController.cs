using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Extensions;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers;

/// <summary>
/// Age or distance categories inside an event (e.g. "Under 20", "Senior", "10km").
/// Anyone can view them; only the Organiser who owns the event can manage them.
/// </summary>
[ApiController]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly RaceDayDbContext _db;

    public CategoriesController(RaceDayDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// List all categories for an event.
    /// </summary>
    /// <param name="eventId">The event id.</param>
    /// <response code="200">Array of categories, including how many have enrolled in each.</response>
    /// <response code="404">No event with this id.</response>
    [HttpGet("api/events/{eventId:int}/categories")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<CategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetCategories(int eventId)
    {
        if (!await _db.Events.AnyAsync(e => e.EventId == eventId)) return NotFound();

        var categories = await _db.Categories
            .Include(c => c.Enrolments)
            .AsNoTracking()
            .Where(c => c.EventId == eventId)
            .OrderBy(c => c.CategoryId)
            .ToListAsync();

        return Ok(categories.Select(c => c.ToResponse()));
    }

    /// <summary>
    /// Add a category to an event owned by the logged-in Organiser.
    /// </summary>
    /// <param name="eventId">The event id.</param>
    /// <param name="request">The category's name, optional distance (km) and optional entry limit.</param>
    /// <response code="201">Category created. Returns the new category.</response>
    /// <response code="400">Invalid data.</response>
    /// <response code="401">Not logged in.</response>
    /// <response code="403">Not an Organiser, or not the Organiser who owns this event.</response>
    /// <response code="404">No event with this id.</response>
    /// <response code="409">The event already has a category with this name.</response>
    [HttpPost("api/events/{eventId:int}/categories")]
    [Authorize(Roles = Role.Organiser)]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CategoryResponse>> CreateCategory(int eventId, CategoryRequest request)
    {
        var ev = await _db.Events.Include(e => e.Categories).FirstOrDefaultAsync(e => e.EventId == eventId);
        if (ev is null) return NotFound();
        if (ev.OrganiserId != User.GetUserId()) return Forbid();

        var name = request.Name.Trim();
        if (ev.Categories.Any(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            return DuplicateName(name);
        }

        var category = new Category
        {
            EventId = eventId,
            Name = name,
            Distance = request.Distance,
            MaxParticipants = request.MaxParticipants
        };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return Created($"/api/events/{eventId}/categories", category.ToResponse());
    }

    /// <summary>
    /// Update a category in an event owned by the logged-in Organiser.
    /// </summary>
    /// <param name="id">The category id.</param>
    /// <param name="request">The category's new name, distance (km) and entry limit.</param>
    /// <response code="200">The updated category.</response>
    /// <response code="400">Invalid data, or MaxParticipants is lower than the number already enrolled.</response>
    /// <response code="401">Not logged in.</response>
    /// <response code="403">Not an Organiser, or not the Organiser who owns the event.</response>
    /// <response code="404">No category with this id.</response>
    /// <response code="409">The event already has another category with this name.</response>
    [HttpPut("api/categories/{id:int}")]
    [Authorize(Roles = Role.Organiser)]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CategoryResponse>> UpdateCategory(int id, CategoryRequest request)
    {
        var category = await _db.Categories
            .Include(c => c.Event).ThenInclude(e => e.Categories)
            .Include(c => c.Enrolments)
            .FirstOrDefaultAsync(c => c.CategoryId == id);
        if (category is null) return NotFound();
        if (category.Event.OrganiserId != User.GetUserId()) return Forbid();

        var name = request.Name.Trim();
        if (category.Event.Categories.Any(c => c.CategoryId != id &&
                                               string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            return DuplicateName(name);
        }

        if (request.MaxParticipants is not null && request.MaxParticipants < category.Enrolments.Count)
        {
            ModelState.AddModelError(nameof(request.MaxParticipants),
                $"{category.Enrolments.Count} participants are already enrolled, so the limit can't be lower than that.");
            return ValidationProblem(ModelState);
        }

        category.Name = name;
        category.Distance = request.Distance;
        category.MaxParticipants = request.MaxParticipants;
        await _db.SaveChangesAsync();

        return Ok(category.ToResponse());
    }

    /// <summary>
    /// Delete a category from an event owned by the logged-in Organiser.
    /// </summary>
    /// <remarks>Enrolments and results in the category are deleted with it.</remarks>
    /// <param name="id">The category id.</param>
    /// <response code="204">Category deleted.</response>
    /// <response code="401">Not logged in.</response>
    /// <response code="403">Not an Organiser, or not the Organiser who owns the event.</response>
    /// <response code="404">No category with this id.</response>
    [HttpDelete("api/categories/{id:int}")]
    [Authorize(Roles = Role.Organiser)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _db.Categories.Include(c => c.Event).FirstOrDefaultAsync(c => c.CategoryId == id);
        if (category is null) return NotFound();
        if (category.Event.OrganiserId != User.GetUserId()) return Forbid();

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private ObjectResult DuplicateName(string name) =>
        Problem(statusCode: StatusCodes.Status409Conflict, title: "Duplicate category",
            detail: $"This event already has a category called '{name}'.");
}
