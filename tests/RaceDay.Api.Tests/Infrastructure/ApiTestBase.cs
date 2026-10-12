using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using RaceDay.Api.DTOs;

namespace RaceDay.Api.Tests.Infrastructure;

/// <summary>
/// Shared helpers for registering/logging in users and creating test data through the API.
/// Every test uses unique emails and its own events, so tests don't depend on each other.
/// </summary>
public abstract class ApiTestBase : IClassFixture<RaceDayApiFactory>
{
    protected const string Password = "Password123!";

    protected static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    protected readonly RaceDayApiFactory Factory;

    protected ApiTestBase(RaceDayApiFactory factory)
    {
        Factory = factory;
    }

    /// <summary>A client with no token, i.e. an anonymous visitor.</summary>
    protected HttpClient AnonymousClient() => Factory.CreateClient();

    protected HttpClient ClientWithToken(string token)
    {
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    protected static string UniqueEmail(string prefix) => $"{prefix}.{Guid.NewGuid():N}@test.co.za";

    protected async Task<HttpResponseMessage> RegisterAsync(string email, string role, string password = Password) =>
        await AnonymousClient().PostAsJsonAsync("/api/auth/register", new
        {
            fullName = "Test User",
            email,
            password,
            role
        }, Json);

    protected async Task<HttpResponseMessage> LoginAsync(string email, string password = Password) =>
        await AnonymousClient().PostAsJsonAsync("/api/auth/login", new { email, password }, Json);

    /// <summary>Registers a brand new user with the given role and returns a client logged in as them.</summary>
    protected async Task<(HttpClient Client, int UserId)> NewUserClientAsync(string role)
    {
        var email = UniqueEmail(role.ToLowerInvariant());
        var register = await RegisterAsync(email, role);
        Assert.Equal(HttpStatusCode.Created, register.StatusCode);

        var login = await LoginAsync(email);
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        var body = (await login.Content.ReadFromJsonAsync<LoginResponse>(Json))!;

        return (ClientWithToken(body.Token), body.UserId);
    }

    protected Task<(HttpClient Client, int UserId)> NewOrganiserAsync() => NewUserClientAsync("Organiser");
    protected Task<(HttpClient Client, int UserId)> NewParticipantAsync() => NewUserClientAsync("Participant");

    protected static object ValidEventBody(string name = "Test City Marathon", DateTime? date = null) => new
    {
        name,
        description = "An event created by a unit test.",
        eventDate = date ?? DateTime.Now.AddMonths(2),
        location = "Cape Town, South Africa",
        distance = 42.2m,
        eventType = "Run"
    };

    protected static async Task<EventResponse> CreateEventAsync(HttpClient organiser, DateTime? date = null)
    {
        var response = await organiser.PostAsJsonAsync("/api/events", ValidEventBody(date: date), Json);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<EventResponse>(Json))!;
    }

    protected static async Task<CategoryResponse> CreateCategoryAsync(HttpClient organiser, int eventId,
        string name = "10km", int? maxParticipants = null)
    {
        var response = await organiser.PostAsJsonAsync($"/api/events/{eventId}/categories",
            new { name, distance = 10m, maxParticipants }, Json);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<CategoryResponse>(Json))!;
    }

    protected static async Task<EnrolmentResponse> EnrolAsync(HttpClient participant, int categoryId)
    {
        var response = await participant.PostAsJsonAsync("/api/enrolments", new { categoryId }, Json);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<EnrolmentResponse>(Json))!;
    }
}
