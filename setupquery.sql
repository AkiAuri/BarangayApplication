-- ===========================================================
-- DATABASE CREATION
-- ===========================================================

CREATE DATABASE ResidentsDB;
GO

CREATE DATABASE ResidentsArchiveDB;
GO

CREATE DATABASE ResidentsLogDB;
GO

-- ===========================================================
-- ResidentsDB (Main Database)
-- ===========================================================
USE ResidentsDB;
GO

-- Lookup table for Sex
CREATE TABLE Sexes (
    SexID BIT PRIMARY KEY,        -- 0 = Female, 1 = Male
    SexDescription NVARCHAR(10) NOT NULL
);
INSERT INTO Sexes (SexID, SexDescription) VALUES (0, 'Female'), (1, 'Male');

-- Lookup table for Civil Status
CREATE TABLE CivilStatuses (
    CivilStatusID INT PRIMARY KEY,
    CivilStatusDescription NVARCHAR(50) NOT NULL
);
INSERT INTO CivilStatuses (CivilStatusID, CivilStatusDescription) VALUES
(1, 'Single'),
(2, 'Married'),
(3, 'Widowed'),
(4, 'Legally Separated');
CREATE TABLE ResidenceTypes (
    ResidenceTypeID INT PRIMARY KEY IDENTITY(1,1),
    ResidenceTypeName NVARCHAR(50) NOT NULL UNIQUE
);

INSERT INTO ResidenceTypes (ResidenceTypeName) VALUES ('Owned'), ('Rented'), ('Boarders/Bedspacer');

-- Main Residents table
CREATE TABLE Residents (
    ResidentID INT PRIMARY KEY IDENTITY(1,1),
    LastName NVARCHAR(100) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    MiddleName NVARCHAR(100) NULL,
    Address NVARCHAR(200) NOT NULL,
    TelCelNo NVARCHAR(50) NOT NULL,
    SexID BIT NOT NULL,  -- Foreign key to Sexes
    DateOfBirth DATE NOT NULL,
    PlaceOfBirth NVARCHAR(100) NOT NULL,
    CivilStatusID INT NOT NULL, -- Foreign key to CivilStatuses
    VoterIDNo NVARCHAR(50) NOT NULL,
    PollingPlace NVARCHAR(100) NOT NULL,
    ResidenceTypeID INT NOT NULL,
    Height DECIMAL(10,2) NOT NULL,
    Weight DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_Residents_SexID FOREIGN KEY (SexID) REFERENCES Sexes(SexID),
    CONSTRAINT FK_Residents_ResidenceTypeID FOREIGN KEY (ResidenceTypeID) REFERENCES ResidenceTypes(ResidenceTypeID),
    CONSTRAINT FK_Residents_CivilStatusID FOREIGN KEY (CivilStatusID) REFERENCES CivilStatuses(CivilStatusID)
);


-- Employment table (for residents)
CREATE TABLE Employment (
    EmploymentID INT PRIMARY KEY IDENTITY(1,1),
    ResidentID INT NOT NULL FOREIGN KEY REFERENCES Residents(ResidentID),
    Company NVARCHAR(100) NOT NULL,
    Position NVARCHAR(100) NOT NULL,
    LengthOfService NVARCHAR(50) NOT NULL
);

-- Spouse table (no SpousePhone)
CREATE TABLE Spouse (
    SpouseID INT PRIMARY KEY IDENTITY(1,1),
    ResidentID INT NOT NULL FOREIGN KEY REFERENCES Residents(ResidentID),
    SpouseName NVARCHAR(100) NOT NULL
);

-- SpouseEmployment table
CREATE TABLE SpouseEmployment (
    SpouseEmploymentID INT PRIMARY KEY IDENTITY(1,1),
    SpouseID INT NOT NULL FOREIGN KEY REFERENCES Spouse(SpouseID),
    Company NVARCHAR(100) NOT NULL,
    Position NVARCHAR(100) NOT NULL,
    LengthOfService NVARCHAR(50) NOT NULL
);

-- Purpose Types
CREATE TABLE PurposeTypes (
    PurposeTypeID INT PRIMARY KEY IDENTITY(1,1),
    PurposeName NVARCHAR(100) NOT NULL
);


SET IDENTITY_INSERT PurposeTypes ON;
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (1,  'Residency');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (2,  'PostalID');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (3,  'LocalEmployment');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (4,  'Marriage');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (5,  'Loan');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (6,  'Meralco');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (7,  'BankTransaction');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (8,  'TravelAbroad');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (9,  'SeniorCitizen');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (10, 'School');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (11, 'Medical');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (12, 'Burial');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (99, 'Others');
SET IDENTITY_INSERT PurposeTypes OFF;


-- ===========================================================
-- TransactionID Auto-Reset Logic (ResidentPurposes)
-- ===========================================================

-- Tracker table to keep track of yearly sequence
CREATE TABLE TransactionIDTracker (
    Year INT PRIMARY KEY,
    LastSeq INT NOT NULL
);

-- ResidentPurposes table with TransactionID
CREATE TABLE ResidentPurposes (
    TransactionID VARCHAR(9) NOT NULL PRIMARY KEY,
    ResidentID INT NOT NULL FOREIGN KEY REFERENCES Residents(ResidentID),
    PurposeTypeID INT NOT NULL FOREIGN KEY REFERENCES PurposeTypes(PurposeTypeID),
    PurposeOthers NVARCHAR(200) NULL
);

-- Stored procedure to insert with auto-resetting TransactionID
GO
CREATE PROCEDURE InsertResidentPurpose
    @ResidentID INT,
    @PurposeTypeID INT,
    @PurposeOthers NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Year INT = YEAR(GETDATE());
    DECLARE @Seq INT;
    DECLARE @TransactionID VARCHAR(9);

    BEGIN TRANSACTION;

    -- Check if current year exists in tracker
    IF EXISTS (SELECT 1 FROM TransactionIDTracker WHERE Year = @Year)
    BEGIN
        -- Increment LastSeq for the year
        UPDATE TransactionIDTracker
        SET LastSeq = LastSeq + 1
        WHERE Year = @Year;

        SELECT @Seq = LastSeq
        FROM TransactionIDTracker
        WHERE Year = @Year;
    END
    ELSE
    BEGIN
        -- New year, start at 1
        INSERT INTO TransactionIDTracker (Year, LastSeq) VALUES (@Year, 1);
        SET @Seq = 1;
    END

    -- Build TransactionID as 'YYYYXXXXX'
    SET @TransactionID = CAST(@Year AS VARCHAR(4)) + RIGHT('00000' + CAST(@Seq AS VARCHAR(5)), 5);

    -- Insert into ResidentPurposes
    INSERT INTO ResidentPurposes (TransactionID, ResidentID, PurposeTypeID, PurposeOthers)
    VALUES (@TransactionID, @ResidentID, @PurposeTypeID, @PurposeOthers);

    COMMIT TRANSACTION;
END
GO


-- ===========================================================
-- ResidentsLogDB (Logbook & Users)
-- ===========================================================
USE ResidentsLogDB;
GO

-- UserRoles table
CREATE TABLE UserRoles (
    roleID INT PRIMARY KEY IDENTITY(1,1),
    roleName NVARCHAR(50) NOT NULL UNIQUE
);

SET IDENTITY_INSERT UserRoles ON;
INSERT INTO UserRoles (roleID, roleName) VALUES (1, 'SuperAdmin');
INSERT INTO UserRoles (roleID, roleName) VALUES (2, 'Admin');
SET IDENTITY_INSERT UserRoles OFF;

-- users table
CREATE TABLE users (
    accountID INT PRIMARY KEY IDENTITY(1,1),
    accountName NVARCHAR(255) NOT NULL,
    passwordHash NVARCHAR(64) NOT NULL,
    roleID INT NOT NULL FOREIGN KEY REFERENCES UserRoles(roleID)
);

-- UserLogs table
CREATE TABLE UserLogs (
    LogID INT IDENTITY(1,1) PRIMARY KEY,
    Timestamp DATETIME NOT NULL,
    AccountID INT NOT NULL FOREIGN KEY REFERENCES users(accountID),
    Action NVARCHAR(50) NOT NULL,
    Description NVARCHAR(500)
);
-- DUMMY USERS
INSERT INTO users (accountName, passwordHash, roleID) VALUES ('admin_alex',   '$2a$16$FfKdgtztEVNEKjOowHOsxeXCk6LJmzYOCNXVWMCmIE3ct8Bke6/lO', 2); -- password: password1
INSERT INTO users (accountName, passwordHash, roleID) VALUES ('admin_jane',   '$2a$16$EhN27LuzYAskEKl0IiXe1OyT/Fr5gbLpuc12yShodR2P1Wwxcdoze', 2); -- password: admin321
INSERT INTO users (accountName, passwordHash, roleID) VALUES ('admin_carl',   '$2a$16$3wEBda/gxuZ38ImPHhEl1OG6O8WzA0niYChJc/rUcJeTCFOmSr/tu', 2); -- password: 12345678
INSERT INTO users (accountName, passwordHash, roleID) VALUES ('admin_emma',   '$2a$16$iGNGZu3.24Y1gSUAkPKF0OMUKm3mcpy9B/077Il.wz08kyZ6WGIXG', 2); -- password: test
INSERT INTO users (accountName, passwordHash, roleID) VALUES ('admin_oliver', '$2a$16$RB.nwZovAezQzOY1KGGxo.6feJdN0gMcvLDPUvJw6jkGydIJ9LlSq', 2); -- password: abc123
INSERT INTO users (accountName, passwordHash, roleID) VALUES ('admin_lucas',  '$2a$16$8jJo631SekIFnGPKt/b2Fu49u9xKenGzAJIPIhtEIBDSIli89PjSu', 2); -- password: 12345
INSERT INTO users (accountName, passwordHash, roleID) VALUES ('admin_eva',    '$2a$16$hAWZev6akTZVZu.J7YVTUOT5X8mwQQ6u6K8UoLiA77k9kFf4Peybq', 2); -- password: qwerty
INSERT INTO users (accountName, passwordHash, roleID) VALUES ('admin_mia',    '$2a$16$DK.OtZ6LR16CUjqLPTp6jO1EB1J0sMRkqAQGqVI5sL2IUMulzu70a', 2); -- password: 111111
INSERT INTO users (accountName, passwordHash, roleID) VALUES ('admin_liam',   '$2a$16$pwpuRrpsp0m8AqzONjDgG.SqNHs1v1c1QdJD2eDBfql/E5BPuSiQW', 2); -- password: iloveyou
INSERT INTO users (accountName, passwordHash, roleID) VALUES ('admin_sophia', '$2a$16$ElL6FZnspDvY7ecBDNi3ievx/.YrJz3cLNsZ2n.xp/9ynzV8RgPlm', 2); -- password: qwertyuiop

-- Insert 1 SuperAdmin account (roleID = 1)
INSERT INTO users (accountName, passwordHash, roleID) VALUES ('superadmin_root', '$2a$16$IxLXznB26GHEBsoxlMmcR.tjl9vhNBE8nXUVMapcdB8Ix1WclmvbW', 1); -- password: admin

-- ===========================================================
-- ResidentsArchiveDB (Archive Database)
-- ===========================================================
USE ResidentsArchiveDB;
GO

-- Lookup table for Sex
CREATE TABLE Sexes (
    SexID BIT PRIMARY KEY,        -- 0 = Female, 1 = Male
    SexDescription NVARCHAR(10) NOT NULL
);
INSERT INTO Sexes (SexID, SexDescription) VALUES (0, 'Female'), (1, 'Male');

-- Lookup table for Civil Status
CREATE TABLE CivilStatuses (
    CivilStatusID INT PRIMARY KEY,
    CivilStatusDescription NVARCHAR(50) NOT NULL
);
INSERT INTO CivilStatuses (CivilStatusID, CivilStatusDescription) VALUES
(1, 'Single'),
(2, 'Married'),
(3, 'Widowed'),
(4, 'Legally Separated');


CREATE TABLE ResidenceTypes (
    ResidenceTypeID INT PRIMARY KEY IDENTITY(1,1),
    ResidenceTypeName NVARCHAR(50) NOT NULL UNIQUE
);

INSERT INTO ResidenceTypes (ResidenceTypeName) VALUES ('Owned'), ('Rented'), ('Boarders/Bedspacer');


-- Main Residents table
CREATE TABLE Residents (
    ResidentID INT PRIMARY KEY IDENTITY(1,1),
    LastName NVARCHAR(100) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    MiddleName NVARCHAR(100) NULL,
    Address NVARCHAR(200) NOT NULL,
    TelCelNo NVARCHAR(50) NOT NULL,
    SexID BIT NOT NULL,  -- Foreign key to Sexes
    DateOfBirth DATE NOT NULL,
    PlaceOfBirth NVARCHAR(100) NOT NULL,
    CivilStatusID INT NOT NULL, -- Foreign key to CivilStatuses
    VoterIDNo NVARCHAR(50) NOT NULL,
    PollingPlace NVARCHAR(100) NOT NULL,
    ResidenceTypeID INT NOT NULL,
    Height DECIMAL(10,2) NOT NULL,
    Weight DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_Residents_SexID FOREIGN KEY (SexID) REFERENCES Sexes(SexID),
    CONSTRAINT FK_Residents_ResidenceTypeID FOREIGN KEY (ResidenceTypeID) REFERENCES ResidenceTypes(ResidenceTypeID),
    CONSTRAINT FK_Residents_CivilStatusID FOREIGN KEY (CivilStatusID) REFERENCES CivilStatuses(CivilStatusID)
);


-- Employment table (for residents)
CREATE TABLE Employment (
    EmploymentID INT PRIMARY KEY IDENTITY(1,1),
    ResidentID INT NOT NULL FOREIGN KEY REFERENCES Residents(ResidentID),
    Company NVARCHAR(100) NOT NULL,
    Position NVARCHAR(100) NOT NULL,
    LengthOfService NVARCHAR(50) NOT NULL
);

-- Spouse table (no SpousePhone)
CREATE TABLE Spouse (
    SpouseID INT PRIMARY KEY IDENTITY(1,1),
    ResidentID INT NOT NULL FOREIGN KEY REFERENCES Residents(ResidentID),
    SpouseName NVARCHAR(100) NOT NULL
);

-- SpouseEmployment table
CREATE TABLE SpouseEmployment (
    SpouseEmploymentID INT PRIMARY KEY IDENTITY(1,1),
    SpouseID INT NOT NULL FOREIGN KEY REFERENCES Spouse(SpouseID),
    Company NVARCHAR(100) NOT NULL,
    Position NVARCHAR(100) NOT NULL,
    LengthOfService NVARCHAR(50) NOT NULL
);

-- Purpose Types
CREATE TABLE PurposeTypes (
    PurposeTypeID INT PRIMARY KEY IDENTITY(1,1),
    PurposeName NVARCHAR(100) NOT NULL
);


SET IDENTITY_INSERT PurposeTypes ON;
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (1,  'Residency');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (2,  'PostalID');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (3,  'LocalEmployment');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (4,  'Marriage');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (5,  'Loan');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (6,  'Meralco');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (7,  'BankTransaction');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (8,  'TravelAbroad');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (9,  'SeniorCitizen');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (10, 'School');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (11, 'Medical');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (12, 'Burial');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (99, 'Others');
SET IDENTITY_INSERT PurposeTypes OFF;


-- ResidentPurposes table
CREATE TABLE ResidentPurposes (
    TransactionID VARCHAR(9) NOT NULL PRIMARY KEY,
    ResidentID INT NOT NULL FOREIGN KEY REFERENCES Residents(ResidentID),
    PurposeTypeID INT NOT NULL FOREIGN KEY REFERENCES PurposeTypes(PurposeTypeID),
    PurposeOthers NVARCHAR(200) NULL
);