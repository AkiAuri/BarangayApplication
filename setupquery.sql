CREATE TABLE Residents (
    ResidentID INT PRIMARY KEY IDENTITY(1,1),
    LastName NVARCHAR(100) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    MiddleName NVARCHAR(100) NULL,
    Address NVARCHAR(200) NOT NULL,
    TelCelNo NVARCHAR(50) NOT NULL,
    Sex NVARCHAR(10) NOT NULL,
    DateOfBirth DATETIME NOT NULL,
    PlaceOfBirth NVARCHAR(100) NOT NULL,
    CivilStatus NVARCHAR(50) NOT NULL,
    VoterIDNo NVARCHAR(50) NOT NULL,
    PollingPlace NVARCHAR(100) NOT NULL,
    ResidenceType NVARCHAR(50) NOT NULL,
    PaymentAmount DECIMAL(10,2) NOT NULL,
    PaymentFrequency NVARCHAR(50) NOT NULL,
    Height DECIMAL(10,2) NOT NULL,
    Weight DECIMAL(10,2) NOT NULL,
    isArchived BIT NOT NULL DEFAULT 0
);

CREATE TABLE Employment (
    EmploymentID INT PRIMARY KEY IDENTITY(1,1),
    ResidentID INT NOT NULL FOREIGN KEY REFERENCES Residents(ResidentID),
    Company NVARCHAR(100) NOT NULL,
    Position NVARCHAR(100) NOT NULL,
    LengthOfService NVARCHAR(50) NOT NULL
);

CREATE TABLE PreviousEmployment (
    PreviousEmploymentID INT PRIMARY KEY IDENTITY(1,1),
    ResidentID INT NOT NULL FOREIGN KEY REFERENCES Residents(ResidentID),
    Company NVARCHAR(100) NOT NULL,
    Position NVARCHAR(100) NOT NULL,
    LengthOfService NVARCHAR(50) NOT NULL    
);

CREATE TABLE Spouse (
    SpouseID INT PRIMARY KEY IDENTITY(1,1),
    ResidentID INT NOT NULL FOREIGN KEY REFERENCES Residents(ResidentID),
    SpouseName NVARCHAR(100) NOT NULL,
    SpousePhone NVARCHAR(20) NOT NULL
);

CREATE TABLE SpouseEmployment (
    SpouseEmploymentID INT PRIMARY KEY IDENTITY(1,1),
    SpouseID INT NOT NULL FOREIGN KEY REFERENCES Spouse(SpouseID),
    Company NVARCHAR(100) NOT NULL,
    Position NVARCHAR(100) NOT NULL,
    LengthOfService NVARCHAR(50) NOT NULL
);

CREATE TABLE SpousePreviousEmployment (
    SpousePrevEmploymentID INT PRIMARY KEY IDENTITY(1,1),
    SpouseID INT NOT NULL FOREIGN KEY REFERENCES Spouse(SpouseID),
    Company NVARCHAR(100) NOT NULL,
    Position NVARCHAR(100) NOT NULL,
    LengthOfService NVARCHAR(50) NOT NULL
);

CREATE TABLE PurposeTypes (
    PurposeTypeID INT PRIMARY KEY IDENTITY(1,1),
    PurposeName NVARCHAR(100) NOT NULL
); 

CREATE TABLE ResidentPurposes (
    ResidentPurposeID INT PRIMARY KEY IDENTITY(1,1),
    ResidentID INT NOT NULL FOREIGN KEY REFERENCES Residents(ResidentID),
    PurposeTypeID INT NOT NULL FOREIGN KEY REFERENCES PurposeTypes(PurposeTypeID),
    PurposeOthers NVARCHAR(200) NULL
);

CREATE TABLE UserLogs
(
    LogID INT IDENTITY(1,1) PRIMARY KEY,
    Timestamp DATETIME NOT NULL,
    UserName NVARCHAR(50) NOT NULL,
    Action NVARCHAR(50) NOT NULL,
    Description NVARCHAR(500)
); 

CREATE TABLE UserRoles (
    roleID INT PRIMARY KEY IDENTITY(1,1),
    roleName NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE users (
    accountID INT PRIMARY KEY IDENTITY(1,1),
    accountName NVARCHAR(255) NOT NULL,
    passwordHash NVARCHAR(64) NOT NULL,
    roleID INT NOT NULL FOREIGN KEY REFERENCES UserRoles(roleID)
);

-- Add to PurposeTypes
SET IDENTITY_INSERT PurposeTypes ON;

INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (901, 'Residency');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (902, 'PostalID');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (903, 'LocalEmployment');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (904, 'Marriage');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (905, 'Loan');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (906, 'Meralco');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (907, 'BankTransaction');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (908, 'TravelAbroad');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (909, 'SeniorCitizen');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (910, 'School');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (911, 'Medical');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (912, 'Burial');
INSERT INTO PurposeTypes (PurposeTypeID, PurposeName) VALUES (913, 'Others');

SET IDENTITY_INSERT PurposeTypes OFF;

-- Add to UserRoles
SET IDENTITY_INSERT UserRoles ON;

INSERT INTO UserRoles (roleID, roleName) VALUES (1, 'SuperAdmin');
INSERT INTO UserRoles (roleID, roleName) VALUES (2, 'Admin');

SET IDENTITY_INSERT UserRoles OFF;

-- DUMMY USERS
-- Insert 10 random Admin accounts (roleID = 2)
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
