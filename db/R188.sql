--
--  Start: Change for DEVCME-625 Remove Alias from the Attributes
--
IF EXISTS(SELECT * FROM sys.COLUMNS WHERE Name='Alias' AND OBJECT_ID = OBJECT_ID('dbo.Attributes'))
BEGIN
  DECLARE @ConstraintName nvarchar(200)
  SELECT @ConstraintName = Name FROM SYS.DEFAULT_CONSTRAINTS
  WHERE PARENT_OBJECT_ID = OBJECT_ID('Attributes')
  AND PARENT_COLUMN_ID = (SELECT column_id FROM sys.columns
                        WHERE NAME = N'Alias'
                        AND object_id = OBJECT_ID(N'Attributes'))
  IF @ConstraintName IS NOT NULL
    EXEC('ALTER TABLE Attributes DROP CONSTRAINT ' + @ConstraintName)
  DROP INDEX [IX_Attributes_Alias] ON [dbo].[Attributes]
  ALTER TABLE dbo.Attributes DROP COLUMN Alias
END

IF OBJECT_ID('dbo.Attribute_Add', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Attribute_Add as select 1')
GO
ALTER PROCEDURE Attribute_Add
	@Id NVARCHAR(max),
	@Name NVARCHAR(max) = null,
	@Shortname NVARCHAR(max) = null,
	@Label NVARCHAR(max) = null,
	@Type NVARCHAR(max) = null
AS
BEGIN
INSERT INTO Attributes(Id, Name, Shortname, Label, Type)
VALUES (@Id, @Name, @Shortname, @Label, @Type)
END
GO:
IF OBJECT_ID('dbo.Attribute_Update', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Attribute_Update as select 1')
GO
ALTER PROCEDURE Attribute_Update
	@Id NVARCHAR(max),
	@Name NVARCHAR(max) = null,
	@ShortName NVARCHAR(max) = null,
	@Label NVARCHAR(max) = null,
	@Type NVARCHAR(max) = null
AS
BEGIN

UPDATE Attributes
	SET Name = ISNULL(@Name, Name),
 ShortName = ISNULL(@ShortName, ShortName),
 Label = ISNULL(@Label, Label),
 Type = ISNULL(@Type, Type)
WHERE Id = @Id

END
GO

IF OBJECT_ID('dbo.Attribute_CheckAttribute', 'p') IS NULL
    EXEC ('DROP PROCEDURE Attribute_CheckAttribute as select 1')
GO
--
--  END: Change for DEVCME-625 Remove Alias from the Attributes
--


--- R188 DEVCME-635 Managing Providers table

IF EXISTS(SELECT * FROM sys.COLUMNS WHERE Name='Name' AND OBJECT_ID = OBJECT_ID('dbo.Providers'))
BEGIN
	ALTER TABLE dbo.Providers DROP COLUMN Name
END

IF EXISTS(SELECT * FROM sys.COLUMNS WHERE Name='ByPass' AND OBJECT_ID = OBJECT_ID('dbo.Providers'))
BEGIN
  DECLARE @ConstraintName nvarchar(200)
  SELECT @ConstraintName = Name FROM SYS.DEFAULT_CONSTRAINTS
  WHERE PARENT_OBJECT_ID = OBJECT_ID('Providers')
  AND PARENT_COLUMN_ID = (SELECT column_id FROM sys.columns
                        WHERE NAME = N'ByPass'
                        AND object_id = OBJECT_ID(N'Providers'))
  IF @ConstraintName IS NOT NULL
    EXEC('ALTER TABLE Providers DROP CONSTRAINT ' + @ConstraintName)
	ALTER TABLE dbo.Providers DROP COLUMN ByPass
END

IF EXISTS(SELECT * FROM sys.COLUMNS WHERE Name='ProviderCode' AND OBJECT_ID = OBJECT_ID('dbo.Providers'))
BEGIN
	EXEC sp_RENAME 'dbo.Providers.ProviderCode', 'WelcomeURLCode', 'COLUMN'
END
GO 

IF EXISTS(SELECT * FROM sys.COLUMNS WHERE Name='WelcomeURLCode' AND OBJECT_ID = OBJECT_ID('dbo.Providers'))
BEGIN
	ALTER TABLE Providers ALTER COLUMN WelcomeURLCode NVARCHAR(MAX) NULL
END
GO 

IF EXISTS(SELECT * FROM sys.COLUMNS WHERE Name='ApiUser' AND OBJECT_ID = OBJECT_ID('dbo.Providers'))
BEGIN
	EXEC sp_RENAME 'dbo.Providers.ApiUser', 'ProviderId', 'COLUMN'
END

IF EXISTS(SELECT * FROM sys.COLUMNS WHERE Name='Active' AND OBJECT_ID = OBJECT_ID('dbo.Providers'))
BEGIN
	EXEC sp_RENAME 'dbo.Providers.Active', 'Enabled', 'COLUMN'
END


IF OBJECT_ID('dbo.Provider_Add', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Provider_Add as select 1')
go
ALTER PROCEDURE dbo.Provider_Add 
	@ProviderId NVARCHAR(100),
	@WelcomeUrlCode NVARCHAR(100), 
	@Enabled BIT
AS
BEGIN
INSERT INTO Providers
(ProviderId, WelcomeUrlCode, Enabled)
VALUES (@ProviderId, @WelcomeUrlCode, @Enabled)
END
GO

IF OBJECT_ID('dbo.Provider_Update', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Provider_Update as select 1')
go
ALTER PROCEDURE dbo.Provider_Update 
	@Id INT,
	@ProviderId NVARCHAR(MAX),
	@WelcomeUrlCode NVARCHAR(MAX), 
	@Enabled BIT
AS
BEGIN

UPDATE Providers
SET ProviderId = ISNULL(@ProviderId, ProviderId),
	WelcomeUrlCode = ISNULL(@WelcomeUrlCode, WelcomeUrlCode),
	Enabled = ISNULL(@Enabled, Enabled)
WHERE Id = @Id
END
GO

IF OBJECT_ID('dbo.Provider_GetByProviderId', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Provider_GetByProviderId as select 1')
GO
ALTER PROCEDURE dbo.Provider_GetByProviderId
	@ProviderId NVARCHAR(MAX) 
AS
BEGIN
SELECT * FROM [dbo].[Providers] WHERE ProviderId = @ProviderId
END
GO
---DEVCME-633 Adding stored procedure to return the offers by studyID

IF OBJECT_ID('dbo.Offer_GetByStudyId', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Offer_GetByStudyId as select 1')
GO
ALTER PROCEDURE dbo.Offer_GetByStudyId
	@StudyId NVARCHAR(MAX) = NULL
AS
BEGIN
	DECLARE @Conditions NVARCHAR(MAX) = '';
	
	IF @StudyId IS NOT NULL
	BEGIN
		SET @Conditions = @Conditions + ' AND o.StudyId = ' + cast(@StudyId as varchar(10))
	END
	
	DECLARE @sql NVARCHAR(MAX) = 'SELECT
		o.StudyId as StudyId,
		o.SampleId as SampleId,
		o.Status as Status,
		T.CPI as CPI
		FROM Offers o
		LEFT JOIN [dbo].[Terms] T ON (o.[Id] = T.[OfferId]) AND T.Active = 1
		WHERE 1 = 1' + @Conditions

	EXEC(@sql)
END