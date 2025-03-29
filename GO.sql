USE Travidia

DROP TABLE IF EXISTS TripRequest
DROP TABLE IF EXISTS Trip

CREATE TABLE Trip (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    departureDate DATETIME NOT NULL, 
    returnDate DATETIME NOT NULL, 
    destiny VARCHAR(60) NOT NULL, 
    origin VARCHAR(60) NOT NULL, 
    purpose TEXT NOT NULL,
    originatorId INT NOT NULL, 
    FOREIGN KEY (originatorId) REFERENCES [User] (id)
)

CREATE TABLE System (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    name VARCHAR(45) NOT NULL
)

CREATE TABLE RequestFlow (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    name VARCHAR(45) NOT NULL, 
    systemId INT NOT NULL, 
    FOREIGN KEY (SystemId) REFERENCES System (id)
)

CREATE TABLE RequestStep (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    requestFlowId INT NOT NULL, 
    isFinish BIT NOT NULL, 
    nextStepId INT NULL, 
    departmentId INT NULL, 
    rolId INT NULL, 
    userId INT NULL, 
    FOREIGN KEY (requestFlowId) REFERENCES RequestFlow (id), 
    FOREIGN KEY (nextStepId) REFERENCES RequestStep (id),
    FOREIGN KEY (departmentId) REFERENCES Department (id), 
    FOREIGN KEY (rolId) REFERENCES Rol (id), 
    FOREIGN KEY (userId) REFERENCES [User] (id)
)

DROP TABLE IF EXISTS Request
CREATE TABLE Request (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    itemId INT NOT NULL, 
    requestStepId INT NOT NULL, 
    response BIT NULL, 
    departmentId INT NULL, 
    rolId INT NULL, 
    userId INT NULL, 
    FOREIGN KEY (requestStepId) REFERENCES RequestStep (id),
    FOREIGN KEY (departmentId) REFERENCES Department (id), 
    FOREIGN KEY (rolId) REFERENCES Rol (id), 
    FOREIGN KEY (userId) REFERENCES [User] (id)
)

CREATE TABLE ExpenseType (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    name VARCHAR(45) NOT NULL
)

CREATE TABLE TripExpense (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    tripId INT NOT NULL, 
    expenseTypeId INT NOT NULL, 
    amount DECIMAL (10,2) NOT NULL, 
    reportedAt DATETIME NOT NULL
    FOREIGN KEY (tripId) REFERENCES Trip (id), 
    FOREIGN KEY (expenseTypeId) REFERENCES ExpenseType (id)
)

GO
CREATE OR ALTER PROCEDURE Spu_UserUpset
    @id INT = NULL, 
    @fullName VARCHAR(80) = NULL, 
    @email VARCHAR(100) = NULL, 
    @rolId INT = NULL, 
    @departmentId INT = NULL,
    @passwordHash VARBINARY(MAX) = NULL, 
    @passwordSalt VARBINARY(MAX) = NULL
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM [User] WHERE id = @id)
    BEGIN
        INSERT INTO [User] (fullName, email, rolId, departmentId, passwordHash, passwordSalt)
        VALUES (@fullName, @email, @rolId, @departmentId, @passwordHash, @passwordSalt)
    END
    ELSE
    BEGIN
        UPDATE [User]
        SET fullName = COALESCE(@fullName, fullName),
            email = COALESCE(@email, email), 
            rolId = COALESCE(@rolId, rolId), 
            departmentId = COALESCE(@departmentId, departmentId),
            passwordHash = COALESCE(@passwordHash, passwordHash),
            passwordSalt = COALESCE(@passwordSalt, passwordSalt)
        WHERE id = @id
    END
END
GO
CREATE OR ALTER PROCEDURE Spu_TripUpsert
    @id INT = NULL,
    @departureDate DATETIME, 
    @returnDate DATETIME, 
    @destiny VARCHAR(60), 
    @origin VARCHAR(60), 
    @purpose TEXT, 
    @originatorId INT
AS
BEGIN
    IF NOT EXISTS (SELECT * FROM Trip WHERE id = @id)
    BEGIN
        INSERT INTO Trip (departureDate, returnDate, destiny, origin, purpose, originatorId)
        VALUES (@departureDate, @returnDate, @destiny, @origin, @purpose, @originatorId)
    END
    ELSE
    BEGIN 
        UPDATE Trip 
        SET departureDate = COALESCE(@departureDate, departureDate),
            returnDate = COALESCE(@returnDate, returnDate),
            destiny = COALESCE(@destiny, destiny),
            origin = COALESCE(@origin, origin),
            purpose = COALESCE(@purpose, purpose),
            originatorId = COALESCE(@originatorId, originatorId)
        WHERE id = @id
    END
END