--------------------------------------------------------
--------------------Database cleanup--------------------
--WARNING! THIS WILL DROP CURRENT DATABASE IF IT EXISTS!
--------------------------------------------------------

--USE [master]

--IF (EXISTS (SELECT [name] FROM [master].dbo.sysdatabases WHERE ([name] = 'DLDB2')))
--	ALTER DATABASE [DLDB2] SET SINGLE_USER WITH ROLLBACK IMMEDIATE

--GO

--IF (EXISTS (SELECT [name] FROM [master].dbo.sysdatabases WHERE ([name] = 'DLDB2')))
--	DROP DATABASE [DLDB2]

--GO

--IF NOT EXISTS(SELECT loginname FROM [master].dbo.syslogins WHERE [name] = 'dlservice')
--	CREATE LOGIN [dlservice] WITH PASSWORD = 'XXXX'

--GO

--CREATE DATABASE [DLDB2]
--GO

USE [DLDB2]

--CREATE USER [dlservice] FOR LOGIN [dlservice];
--ALTER ROLE [db_owner] ADD MEMBER [dlservice];

GO

-----------------------------------------------------
--------------------CREATE TABLES--------------------
-----------------------------------------------------

CREATE TABLE [OperatingSystem](
	[Id] INT IDENTITY(1, 1) PRIMARY KEY,
	[Name] VARCHAR(100) NOT NULL
)

CREATE TABLE [DiskType] (
	[Id] TINYINT PRIMARY KEY,
	[Description] VARCHAR(20) NOT NULL
)

CREATE TABLE [ComputerInfo] (
	[Id] INT IDENTITY(1, 1) PRIMARY KEY,
	[SerialNumber] VARCHAR(100) UNIQUE NOT NULL,
	[Name] VARCHAR(50) NULL,
	[Model] VARCHAR(50) NULL,
	[OS_Id] INT NULL,
	[CPU] VARCHAR(100) NULL,
	[CPUCores] TINYINT NULL,
	[RAMGB] INT NULL,
	[DiskType_Id] TINYINT NULL,
	[DiskSize] INT NULL,
	[GFX] VARCHAR(100) NULL,
	[TeamViewerId] VARCHAR(50) NULL,
	[LastUpdate] DATETIME NULL,
	[LastAlive] DATETIME NOT NULL DEFAULT GETDATE(),
	FOREIGN KEY ([OS_Id]) REFERENCES [OperatingSystem](Id),
	FOREIGN KEY ([DiskType_Id]) REFERENCES [DiskType](Id)
)

CREATE TABLE [ComputerLocation] (
	[Id] INT PRIMARY KEY,
	[MYN] NVARCHAR(10) NULL,
	[BYG] NVARCHAR(10) NULL,
	[LOK] NVARCHAR(10) NULL,
	[Owner] NVARCHAR(10) NULL,
	[LastUpdate] DATETIME NULL,
	FOREIGN KEY ([Id]) REFERENCES [ComputerInfo]([Id])
)

CREATE TABLE [Log] (
	[Id] BIGINT IDENTITY(1, 1) PRIMARY KEY,
	[Timestamp] DATETIME NOT NULL DEFAULT GETDATE(),
	[Computer_Id] INT NOT NULL,
	[Username] VARCHAR(50) NOT NULL,
	[Parameters] VARCHAR(255) NOT NULL,
	FOREIGN KEY ([Computer_Id]) REFERENCES [ComputerInfo](Id)
)

CREATE TABLE [Errors] (
	[Id] INT IDENTITY(1, 1) PRIMARY KEY,
	[Timestamp] DATETIME NOT NULL DEFAULT GETDATE(),
	[ComputerName] VARCHAR(50) NULL,
	[Message] VARCHAR(1000) NULL
)

GO

---------------------------------------------
--------------------Views--------------------
---------------------------------------------

ALTER VIEW [vwComputerInfo]
AS
SELECT		CI.Id AS Computer_Id,
			CI.SerialNumber,
			CI.[Name],
			CI.[Model],
			OS.[Name] AS [OS],
			CI.[CPU],
			CI.[CPUCores],
			CI.[RAMGB],
			CI.[DiskType_Id],
			DT.[Description] AS [DiskMediaType],
			CI.DiskSize,
			CI.GFX,
			CI.TeamViewerId,
			CI.LastUpdate,
			CI.LastAlive
FROM		[ComputerInfo] CI
LEFT JOIN	[DiskType] DT ON CI.DiskType_Id = DT.Id
LEFT JOIN	[OperatingSystem] OS ON CI.OS_Id = OS.Id

GO

ALTER VIEW [vwLogWithComputers]
AS
SELECT		L.[Id],
			L.[Timestamp],
			L.[Computer_Id],
			L.[Username],
			L.[Parameters],
			CI.[SerialNumber],
			CI.[Name],
			CI.[Model],
			CI.[OS],
			CI.[CPU],
			CI.[CPUCores],
			CI.[RAMGB],
			CI.[DiskType_Id],
			CI.[DiskMediaType],
			CI.DiskSize,
			CI.GFX,
			CI.TeamViewerId,
			CI.LastUpdate,
			CI.LastAlive
FROM		[Log] L
INNER JOIN	[vwComputerInfo] CI ON L.Computer_Id = CI.Computer_Id

GO

---------------------------------------------------------
--------------------Stored Procedures--------------------
---------------------------------------------------------

GO

CREATE PROCEDURE [GetOSId]
	@Name VARCHAR(100),
	@Id INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT @Id = [Id] FROM [OperatingSystem] WHERE [Name] = @Name

	IF (@Id IS NULL)
	BEGIN
		INSERT INTO [OperatingSystem] ([Name]) VALUES (@Name)
		SET @Id = SCOPE_IDENTITY()
	END
END

GO

CREATE PROCEDURE [SaveComputerInfo]
	@SerialNumber VARCHAR(100),
	@Name VARCHAR(50),
	@Model VARCHAR(50),
	@OS VARCHAR(100),
	@CPU VARCHAR(100),
	@CPUCores TINYINT,
	@RAMGB INT,
	@DiskTypeId TINYINT,
	@DiskSize INT,
	@GFX VARCHAR(100),
	@TeamViewerId VARCHAR(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SET @SerialNumber = UPPER(@SerialNumber)

	DECLARE @OSId TINYINT
	DECLARE @ComputerId INT = (SELECT [Id] FROM [ComputerInfo] WHERE [SerialNumber] = @SerialNumber)

	EXEC dbo.GetOSId @OS, @OSId OUTPUT

	IF @ComputerId IS NOT NULL
	BEGIN
		UPDATE	[ComputerInfo]
		SET		[Name] = UPPER(@Name),
				[Model] = @Model,
				[OS_Id] = @OSId,
				[CPU] = @CPU,
				[CPUCores] = @CPUCores,
				[RAMGB] = @RAMGB,
				[DiskType_Id] = @DiskTypeId,
				[DiskSize] = @DiskSize,
				[GFX] = @GFX,
				[TeamViewerId] = @TeamViewerId,
				[LastUpdate] = GETDATE()
		WHERE	[Id] = @ComputerId
	END
	ELSE
	BEGIN
		INSERT INTO	[ComputerInfo] ([SerialNumber], [Name], [Model], [OS_Id], [CPU], [CPUCores], [RAMGB], [DiskType_Id], [DiskSize], [GFX], [TeamViewerId], [LastUpdate])
		VALUES		(@SerialNumber, @Name, @Model, @OSId, @CPU, @CPUCores, @RAMGB, @DiskTypeId, @DiskSize, @GFX, @TeamViewerId, GETDATE())
	END
END

GO

CREATE PROCEDURE SaveComputerLocation
	@SerialNumber VARCHAR(100),
	@MYN NVARCHAR(10),
	@BYG NVARCHAR(10),
	@LOK NVARCHAR(10),
	@Owner NVARCHAR(10)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	SET @SerialNumber = UPPER(@SerialNumber)

	DECLARE @ComputerId INT
	SELECT @ComputerId = [Id] FROM [ComputerInfo] WHERE [SerialNumber] = @SerialNumber

	IF @ComputerId IS NULL
	BEGIN
		INSERT INTO [ComputerInfo] ([SerialNumber])
		VALUES (@SerialNumber)

		SET @ComputerId = SCOPE_IDENTITY()
	END
	ELSE
	BEGIN
		IF EXISTS(SELECT TOP 1 [Id] FROM [ComputerLocation] WHERE [Id] = @ComputerId)
		BEGIN
			UPDATE	[ComputerLocation]
			SET		[MYN] = @MYN,
					[BYG] = @BYG,
					[LOK] = @LOK,
					[Owner] = @Owner,
					[LastUpdate] = GETDATE()
			WHERE	[Id] = @ComputerId
		END
		ELSE
		BEGIN
			INSERT INTO [ComputerLocation]
			([Id], [MYN], [BYG], [LOK], [Owner], [LastUpdate])
			VALUES (@ComputerId, @MYN, @BYG, @LOK, @Owner, GETDATE())
		END
	END
END

GO

CREATE PROCEDURE [WriteLog]
	@SerialNumber VARCHAR(100),
	@Username VARCHAR(50),
	@Parameters VARCHAR(255),
	@LastUpdate DATETIME OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	SET @SerialNumber = UPPER(@SerialNumber)
	SET @Username = UPPER(@Username)

	DECLARE @ComputerId INT

	SELECT @ComputerId = [Id], @LastUpdate = [LastUpdate] FROM [ComputerInfo] WHERE [SerialNumber] = @SerialNumber

	IF (@ComputerId IS NOT NULL)
		UPDATE [ComputerInfo] SET [LastAlive] = GETDATE() WHERE [Id] = @ComputerId
	ELSE
	BEGIN
		INSERT INTO [ComputerInfo] ([SerialNumber])
		VALUES (@SerialNumber)

		SET @ComputerId = SCOPE_IDENTITY()
	END

	INSERT INTO [Log] ([Computer_Id], [Username], [Parameters])
	VALUES (@ComputerId, @Username, @Parameters)
END

GO

CREATE PROCEDURE [WriteErrorLog]
	@ComputerName VARCHAR(50),
	@Message VARCHAR(1000)
AS
BEGIN
	INSERT INTO [Errors] ([ComputerName], [Message])
	VALUES (UPPER(@ComputerName), @Message)
END

GO

----------------------------------------------------------
--------------------INSERT STATIC DATA--------------------
----------------------------------------------------------

INSERT INTO [DiskType]
VALUES	(1, 'Unknown'), (2, 'Unspecified'), (3, 'HDD'), (4, 'SSD'), (5, 'SCM')