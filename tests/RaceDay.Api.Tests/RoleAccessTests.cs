using System.Net;
using System.Net.Http.Json;
using RaceDay.Api.Tests.Infrastructure;

namespace RaceDay.Api.Tests;

/// <summary>
/// Checks that every role-restricted endpoint rejects the other role (403)
/// and rejects anonymous callers (401). Ids refer to the seeded sample data.
/// </summary>
public class RoleAccessTests : ApiTestBase
{
    public RoleAccessTests(RaceDayApiFactory factory) : base(factory)
    {
    }

    private const string EventBody =
        """{"name":"Blocked","eventDate":"2030-01-01T07:00:00","location":"Cape Town","distance":5,"eventType":"Run"}""";
    private const string CategoryBody = """{"name":"Blocked","distance":5}""";
    private const string ResultBody = """{"enrolmentId":1,"finishTime":"00:40:00","position":1}""";

    /// <summary>Endpoints only Organisers may use.</summary>
    public static TheoryData<string, string, string?> OrganiserOnlyEndpoints => new()
    {
        { "POST", "/api/events", EventBody },
        { "PUT", "/api/events/1", EventBody },
        { "DELETE", "/api/events/1", null },
        { "POST", "/api/events/1/categories", CategoryBody },
        { "PUT", "/api/categories/1", CategoryBody },
        { "DELETE", "/api/categories/1", null },
        { "GET", "/api/events/1/enrolments", null },
        { "POST", "/api/results", ResultBody },
        { "PUT", "/api/results/1", """{"finishTime":"00:40:00","position":1}""" }
    };

    /// <summary>Endpoints only Participants may use.</summary>
    public static TheoryData<string, string, string?> ParticipantOnlyEndpoints => new()
    {
        { "POST", "/api/enrolments", """{"categoryId":1}""" },
        { "GET", "/api/enrolments/me", null },
        { "DELETE", "/api/enrolments/1", null },
        { "GET", "/api/results/me", null }
    };

    /// <summary>Endpoints any logged-in user may use, but anonymous users may not.</summary>
    public static TheoryData<string, string, string?> AnyLoggedInUserEndpoints => new()
    {
        { "GET", "/api/users/me", null },
        { "PUT", "/api/users/me", """{"fullName":"Anon","email":"anon@example.com"}""" }
    };

    [Theory]
    [MemberData(nameof(OrganiserOnlyEndpoints))]
    public async Task Participant_CallingOrganiserEndpoint_Returns403(string method, string url, string? body)
    {
        var (participant, _) = await NewParticipantAsync();

        var response = await participant.SendAsync(BuildRequest(method, url, body));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(ParticipantOnlyEndpoints))]
    public async Task Organiser_CallingParticipantEndpoint_Returns403(string method, string url, string? body)
    {
        var (organiser, _) = await NewOrganiserAsync();

        var response = await organiser.SendAsync(BuildRequest(method, url, body));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(OrganiserOnlyEndpoints))]
    [MemberData(nameof(ParticipantOnlyEndpoints))]
    [MemberData(nameof(AnyLoggedInUserEndpoints))]
    public async Task Anonymous_CallingProtectedEndpoint_Returns401(string method, string url, string? body)
    {
        var response = await AnonymousClient().SendAsync(BuildRequest(method, url, body));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Participant_RejectedFromCreatingEvent_NothingIsSaved()
    {
        var (participant, _) = await NewParticipantAsync();
        var before = await AnonymousClient().GetFromJsonAsync<List<object>>("/api/events", Json);

        await participant.SendAsync(BuildRequest("POST", "/api/events", EventBody));

        var after = await AnonymousClient().GetFromJsonAsync<List<object>>("/api/events", Json);
        Assert.Equal(before!.Count, after!.Count);
    }

    [Fact]
    public async Task Organiser_CannotSeeEnrolmentsForAnotherOrganisersEvent()
    {
        // Seeded event 1 belongs to seeded organiser Naledi, not to this new organiser.
        var (organiser, _) = await NewOrganiserAsync();

        var response = await organiser.GetAsync("/api/events/1/enrolments");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/events")]
    [InlineData("/api/events/1")]
    [InlineData("/api/events/1/categories")]
    [InlineData("/api/categories/2/results")]
    public async Task PublicEndpoints_AreOpenToEveryone(string url)
    {
        var (organiser, _) = await NewOrganiserAsync();
        var (participant, _) = await NewParticipantAsync();

        Assert.Equal(HttpStatusCode.OK, (await AnonymousClient().GetAsync(url)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await organiser.GetAsync(url)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await participant.GetAsync(url)).StatusCode);
    }

    private static HttpRequestMessage BuildRequest(string method, string url, string? body)
    {
        var request = new HttpRequestMessage(new HttpMethod(method), url);
        if (body is not null)
        {
            request.Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
        }
        return request;
    }
}
