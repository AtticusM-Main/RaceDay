# RaceDay

RaceDay is a backend system for organizing running events, managing participant enrollments, and tracking race results. I planned this project as a second year software development student to practice database design and API planning before writing any server code.

## Designed concepts

Users in two roles
- Organisers: create and manage events, categories, enrollments and results
- Participants: browse events and enroll in categories

Events
- Examples: "Cape Town Spring Run" and "Durban Beachfront Race"
- Each event can have multiple categories

Categories
- Examples: 5km, 10km, 21km
- Categories sit inside events and define race types and distances

Enrollments
- Links participants to a category for a specific event
- Stores enrollment status and profile info needed to race

Results
- Captures finish times and finishing positions
- Used to build leaderboards and event summaries

## Key components

Database Schema (RaceDaySchema.sql)
- SQL Server database with 6 tables: Role, User, Event, Category, Enrolment, Result
- Includes sample data that uses real South African event names and example participant profiles
- Schema is designed to be clear and extendable for future features

API Endpoint Plan (EndpointPlan.md)
- RESTful API contract covering the planned endpoints
- Authentication: register and login with JWT
- User profiles: view and update profile data
- Event CRUD: organisers can create, update, delete and list events
- Category management: create and manage race categories within events
- Enrollment workflow: participants can sign up for categories and manage their enrollments
- Results capture: organisers or authorised systems can submit results and generate leaderboards
- Role based access control for endpoints so organisers and participants get the right permissions

## Purpose and current status

This repo holds the data model and the API contract for the backend system. It is a planning stage project. The database schema and the endpoint plan are ready for implementation. Actual API code and server logic are not written yet.


## CI
<img width="1090" height="702" alt="image" src="https://github.com/user-attachments/assets/188a025f-3eb9-4ee6-9513-72a49d167d1e" />

## YOUTUBE LINK
https://youtu.be/al-fq-1F6W8 
