# RaceDay — API Endpoint Plan

Planned before any API code is written (Part 2 implementation must match this plan; deviations are explained in the PR/README).

Roles: **None** = public, no token required. **Any** = any authenticated user. **Organiser** / **Participant** = that role only.

## Authentication

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| POST | /api/auth/register | Registers a new account as Organiser or Participant. | None | `{ fullName, email, password, role }` | 201 Created – new user id and role.<br>400 Bad Request – validation failed.<br>409 Conflict – email already registered. |
| POST | /api/auth/login | Authenticates a user and issues a JWT. | None | `{ email, password }` | 200 OK – JWT token and role.<br>401 Unauthorized – invalid credentials. |

## User Profile

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| GET | /api/users/me | Returns the logged-in user's own profile. | Any | None | 200 OK – profile details.<br>401 Unauthorized. |
| PUT | /api/users/me | Updates the logged-in user's own profile. | Any | `{ fullName, email }` | 200 OK – updated profile.<br>400 Bad Request.<br>401 Unauthorized. |

## Events

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| GET | /api/events | Lists all events so anyone can browse. | None | None | 200 OK – array of events. |
| GET | /api/events/{id} | Returns details for a single event. | None | None | 200 OK – event details.<br>404 Not Found. |
| POST | /api/events | Creates a new event owned by the logged-in organiser. | Organiser | `{ name, description, eventDate, location }` | 201 Created – new event id.<br>400 Bad Request.<br>403 Forbidden. |
| PUT | /api/events/{id} | Updates an event owned by the logged-in organiser. | Organiser | `{ name, description, eventDate, location }` | 200 OK – updated event.<br>403 Forbidden – not the owning organiser.<br>404 Not Found. |
| DELETE | /api/events/{id} | Deletes an event owned by the logged-in organiser. | Organiser | None | 204 No Content.<br>403 Forbidden.<br>404 Not Found. |

## Categories

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| GET | /api/events/{eventId}/categories | Lists the categories under an event. | None | None | 200 OK – array of categories.<br>404 Not Found – event doesn't exist. |
| POST | /api/events/{eventId}/categories | Adds a race category to an event. | Organiser | `{ name, distance, maxParticipants }` | 201 Created – new category id.<br>403 Forbidden.<br>404 Not Found. |
| PUT | /api/categories/{id} | Updates a category. | Organiser | `{ name, distance, maxParticipants }` | 200 OK – updated category.<br>403 Forbidden.<br>404 Not Found. |
| DELETE | /api/categories/{id} | Removes a category. | Organiser | None | 204 No Content.<br>403 Forbidden.<br>404 Not Found. |

## Event Enrolments

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| POST | /api/enrolments | Enrols the logged-in participant into a category. | Participant | `{ categoryId }` | 201 Created – new enrolment id.<br>404 Not Found – category doesn't exist.<br>409 Conflict – already enrolled in this category. |
| GET | /api/enrolments/me | Lists the logged-in participant's own enrolments. | Participant | None | 200 OK – array of enrolments. |
| DELETE | /api/enrolments/{id} | Cancels the logged-in participant's own enrolment. | Participant | None | 204 No Content.<br>403 Forbidden – not the owning participant.<br>404 Not Found. |
| GET | /api/events/{eventId}/enrolments | Lists every enrolment across an event's categories, for organiser oversight. | Organiser | None | 200 OK – array of enrolments with participant details.<br>403 Forbidden.<br>404 Not Found. |

## Results

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| POST | /api/results | Captures a result against a participant's enrolment. | Organiser | `{ enrolmentId, finishTime, position }` | 201 Created – new result id.<br>403 Forbidden.<br>404 Not Found – enrolment doesn't exist.<br>409 Conflict – result already captured for this enrolment. |
| PUT | /api/results/{id} | Updates a previously captured result. | Organiser | `{ finishTime, position }` | 200 OK – updated result.<br>403 Forbidden.<br>404 Not Found. |
| GET | /api/results/me | Lists the logged-in participant's own results, to track personal performance. | Participant | None | 200 OK – array of results with event/category context. |

## Additional endpoint identified as necessary

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| GET | /api/categories/{id}/results | Public leaderboard: results for a category ordered by finishing position. | None | None | 200 OK – ordered array of results.<br>404 Not Found. |

Added beyond the minimum because a race system without a visible leaderboard has no way to show captured results back to entrants who aren't the one who ran that specific category — this is the natural read-side counterpart to `POST /api/results`.
