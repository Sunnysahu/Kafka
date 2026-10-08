CREATE DATABASE TrackingDb;
GO

USE TrackingDb;
GO

CREATE TABLE ShipmentTracking
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ShipmentId INT NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    CreatedAtUtc DATETIME2 NOT NULL,
    UpdatedAtUtc DATETIME2 NOT NULL
);
GO

CREATE TABLE ProcessedEvents
(
    EventId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    ProcessedAt DATETIME2 NOT NULL
);
GO


SELECT * FROM ShipmentTracking;
SELECT * FROM ProcessedEvents;
