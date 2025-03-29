CREATE TABLE [Department] (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    name VARCHAR(45) NOT NULL
)

INSERT INTO Department (name) VALUES ('Accounting')

CREATE TABLE [Rol] (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    name VARCHAR(45) NOT NULL
)

INSERT INTO Rol (name) VALUES ('Supervisor')

CREATE TABLE [User] (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    fullName VARCHAR(80) NOT NULL, 
    email VARCHAR(80) NOT NULL, 
    rolId INT NOT NULL, 
    departmentId INT NOT NULL, 
    passwordHash VARBINARY(MAX) NOT NULL, 
    passwordSalt VARBINARY(MAX) NOT NULL
    FOREIGN KEY (rolId) REFERENCES Rol (id), 
    FOREIGN KEY (departmentId) REFERENCES Department (id)
)