CREATE DATABASE RaceDay;
GO

USE RaceDay;
GO

CREATE TABLE dbo.Role (
    RoleId      INT IDENTITY(1,1) NOT NULL,
    RoleName    VARCHAR(20)       NOT NULL,
    CONSTRAINT PK_Role PRIMARY KEY (RoleId),
    CONSTRAINT UQ_Role_RoleName UNIQUE (RoleName)
);
GO

CREATE TABLE dbo.[User] (
    UserId          INT IDENTITY(1,1)   NOT NULL,
    RoleId          INT                 NOT NULL,
    FullName        VARCHAR(100)        NOT NULL,
    Email           VARCHAR(150)        NOT NULL,
    PasswordHash    VARCHAR(255)        NOT NULL,
    CreatedAt       DATETIME            NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_User PRIMARY KEY (UserId),
    CONSTRAINT UQ_User_Email UNIQUE (Email),
    CONSTRAINT FK_User_Role FOREIGN KEY (RoleId) REFERENCES dbo.Role (RoleId)
);
GO

CREATE TABLE dbo.Event (
    EventId         INT IDENTITY(1,1)   NOT NULL,
    OrganiserId     INT                 NOT NULL,
    Name            VARCHAR(150)        NOT NULL,
    Description     VARCHAR(500)        NULL,
    EventDate       DATETIME            NOT NULL,
    Location        VARCHAR(150)        NOT NULL,
    CreatedAt       DATETIME            NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_Event PRIMARY KEY (EventId),
    CONSTRAINT FK_Event_Organiser FOREIGN KEY (OrganiserId) REFERENCES dbo.[User] (UserId)
);
GO

CREATE TABLE dbo.Category (
    CategoryId      INT IDENTITY(1,1)   NOT NULL,
    EventId         INT                 NOT NULL,
    Name            VARCHAR(100)        NOT NULL,
    Distance        DECIMAL(5,2)        NULL,
    MaxParticipants INT                 NULL,
    CONSTRAINT PK_Category PRIMARY KEY (CategoryId),
    CONSTRAINT FK_Category_Event FOREIGN KEY (EventId) REFERENCES dbo.Event (EventId)
);
GO

CREATE TABLE dbo.Enrolment (
    EnrolmentId     INT IDENTITY(1,1)   NOT NULL,
    ParticipantId   INT                 NOT NULL,
    CategoryId      INT                 NOT NULL,
    EnrolmentDate   DATETIME            NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_Enrolment PRIMARY KEY (EnrolmentId),
    CONSTRAINT FK_Enrolment_Participant FOREIGN KEY (ParticipantId) REFERENCES dbo.[User] (UserId),
    CONSTRAINT FK_Enrolment_Category FOREIGN KEY (CategoryId) REFERENCES dbo.Category (CategoryId)
);
GO

CREATE TABLE dbo.Result (
    ResultId        INT IDENTITY(1,1)   NOT NULL,
    EnrolmentId     INT                 NOT NULL,
    FinishTime      TIME                NOT NULL,
    Position        INT                 NULL,
    RecordedAt      DATETIME            NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_Result PRIMARY KEY (ResultId),
    CONSTRAINT FK_Result_Enrolment FOREIGN KEY (EnrolmentId) REFERENCES dbo.Enrolment (EnrolmentId)
);
GO

-- Seed Roles
INSERT INTO dbo.Role (RoleName) 
VALUES ('Organiser'), ('Participant');
GO

-- Seed Users
INSERT INTO dbo.[User] (RoleId, FullName, Email, PasswordHash) 
VALUES
    ((SELECT RoleId FROM dbo.Role WHERE RoleName = 'Organiser'),   'Naledi Khumalo', 'naledi.khumalo@raceday.co.za', 'HASHED_PASSWORD_1'),
    ((SELECT RoleId FROM dbo.Role WHERE RoleName = 'Organiser'),   'Johan Botha',    'johan.botha@raceday.co.za',    'HASHED_PASSWORD_2'),
    ((SELECT RoleId FROM dbo.Role WHERE RoleName = 'Participant'), 'Thandiwe Nkosi', 'thandiwe.nkosi@example.com',   'HASHED_PASSWORD_3'),
    ((SELECT RoleId FROM dbo.Role WHERE RoleName = 'Participant'), 'Ryan Pillay',    'ryan.pillay@example.com',      'HASHED_PASSWORD_4');
GO

-- Seed Events
INSERT INTO dbo.Event (OrganiserId, Name, Description, EventDate, Location) 
VALUES
    ((SELECT UserId FROM dbo.[User] WHERE Email = 'naledi.khumalo@raceday.co.za'), 'Cape Town Spring Run',   'Annual road running event through the city bowl.', '2026-10-18 07:00:00', 'Cape Town, South Africa'),
    ((SELECT UserId FROM dbo.[User] WHERE Email = 'naledi.khumalo@raceday.co.za'), 'Durban Beachfront Race', 'Coastal race along the Golden Mile.',               '2026-11-08 06:30:00', 'Durban, South Africa'),
    ((SELECT UserId FROM dbo.[User] WHERE Email = 'johan.botha@raceday.co.za'),    'Joburg Trail Challenge', 'Off-road trail run through the Northern suburbs.',  '2026-11-22 06:00:00', 'Johannesburg, South Africa');
GO

-- Seed Categories
INSERT INTO dbo.Category (EventId, Name, Distance, MaxParticipants) 
VALUES
    ((SELECT EventId FROM dbo.Event WHERE Name = 'Cape Town Spring Run'),   '5km',  5.00,  500),
    ((SELECT EventId FROM dbo.Event WHERE Name = 'Cape Town Spring Run'),   '10km', 10.00, 300),
    ((SELECT EventId FROM dbo.Event WHERE Name = 'Durban Beachfront Race'), '5km',  5.00,  400),
    ((SELECT EventId FROM dbo.Event WHERE Name = 'Durban Beachfront Race'), '21km', 21.10, 150),
    ((SELECT EventId FROM dbo.Event WHERE Name = 'Joburg Trail Challenge'), '15km', 15.00, 200);
GO

-- Seed Enrolments
INSERT INTO dbo.Enrolment (ParticipantId, CategoryId) 
VALUES
    (
        (SELECT UserId FROM dbo.[User] WHERE Email = 'thandiwe.nkosi@example.com'),
        (SELECT CategoryId FROM dbo.Category c JOIN dbo.Event e ON c.EventId = e.EventId WHERE e.Name = 'Cape Town Spring Run' AND c.Name = '10km')
    ),
    (
        (SELECT UserId FROM dbo.[User] WHERE Email = 'ryan.pillay@example.com'),
        (SELECT CategoryId FROM dbo.Category c JOIN dbo.Event e ON c.EventId = e.EventId WHERE e.Name = 'Cape Town Spring Run' AND c.Name = '10km')
    ),
    (
        (SELECT UserId FROM dbo.[User] WHERE Email = 'thandiwe.nkosi@example.com'),
        (SELECT CategoryId FROM dbo.Category c JOIN dbo.Event e ON c.EventId = e.EventId WHERE e.Name = 'Durban Beachfront Race' AND c.Name = '21km')
    );
GO

-- Seed Result
INSERT INTO dbo.Result (EnrolmentId, FinishTime, Position) 
VALUES
    (
        (SELECT EnrolmentId 
         FROM dbo.Enrolment 
         WHERE ParticipantId = (SELECT UserId FROM dbo.[User] WHERE Email = 'thandiwe.nkosi@example.com')
           AND CategoryId = (SELECT CategoryId FROM dbo.Category c JOIN dbo.Event e ON c.EventId = e.EventId WHERE e.Name = 'Cape Town Spring Run' AND c.Name = '10km')),
        '00:52:14', 
        3
    );
GO
