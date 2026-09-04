
CREATE DATABASE RaceDay;
END
GO

USE RaceDay;
GO


CREATE TABLE Role (
    RoleId      INT IDENTITY(1,1) NOT NULL,
    RoleName    VARCHAR(20)       NOT NULL,
    CONSTRAINT PK_Role PRIMARY KEY (RoleId),
    CONSTRAINT UQ_Role_RoleName UNIQUE (RoleName)
);
GO


CREATE TABLE User(
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

CREATE TABLE Event (
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

CREATE TABLE Category (
    CategoryId      INT IDENTITY(1,1)   NOT NULL,
    EventId         INT                 NOT NULL,
    Name            VARCHAR(100)        NOT NULL,
    Distance        DECIMAL(5,2)        NULL,
    MaxParticipants INT                 NULL,
    CONSTRAINT PK_Category PRIMARY KEY (CategoryId),
    CONSTRAINT FK_Category_Event FOREIGN KEY (EventId) REFERENCES dbo.Event (EventId)
);
GO

CREATE TABLE Enrolment (
    EnrolmentId     INT IDENTITY(1,1)   NOT NULL,
    ParticipantId   INT                 NOT NULL,
    CategoryId      INT                 NOT NULL,
    EnrolmentDate   DATETIME            NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_Enrolment PRIMARY KEY (EnrolmentId),
    CONSTRAINT FK_Enrolment_Participant FOREIGN KEY (ParticipantId) REFERENCES dbo.[User] (UserId),
    CONSTRAINT FK_Enrolment_Category FOREIGN KEY (CategoryId) REFERENCES dbo.Category (CategoryId),
);
GO

CREATE TABLE Result (
    ResultId        INT IDENTITY(1,1)   NOT NULL,
    EnrolmentId     INT                 NOT NULL,
    FinishTime      TIME                NOT NULL,
    Position        INT                 NULL,
    RecordedAt      DATETIME            NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_Result PRIMARY KEY (ResultId),
    CONSTRAINT FK_Result_Enrolment FOREIGN KEY (EnrolmentId) REFERENCES dbo.Enrolment (EnrolmentId),
);
GO

