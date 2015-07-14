 --USE [OffersII]


---DEVCME-346 Adapting the database changes

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=N'Type' AND TABLE_NAME = N'Attributes')
BEGIN
ALTER TABLE Attributes
ADD Type VARCHAR(100) NULL 
END

---DEVCME-346 Statement to add columns to Offers Update them and Remove the table MainstreamSamples

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=N'StudyId' AND TABLE_NAME = N'Offers')
BEGIN
ALTER TABLE Offers
ADD StudyId INT NOT NULL DEFAULT(0)
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=N'SampleId' AND TABLE_NAME = N'Offers')
BEGIN
ALTER TABLE Offers
ADD SampleId INT NOT NULL DEFAULT(0)
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'MainstreamSamples')
BEGIN
EXEC('UPDATE Offers
SET StudyId = m.StudyId, SampleId = m.SampleId
FROM Offers o
INNER JOIN MainstreamSamples m on o.Id = m.OfferId')

DROP TABLE MainstreamSamples
END

---End of statement for MainstreamSamples

---DEVCME-273 Updating the offer table with the new column IsInitialized
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=N'IsInitialized' AND TABLE_NAME = N'Offers')
BEGIN
ALTER TABLE dbo.Offers 
ADD [IsInitialized] bit not null DEFAULT(0)
END
GO
IF OBJECT_ID('dbo.Offer_UpdateIsInitialized', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Offer_UpdateIsInitialized as select 1')
GO
ALTER PROCEDURE Offer_UpdateIsInitialized 
	@SampleId int,
	@IsInitialized bit
AS
BEGIN
 UPDATE Offers
SET IsInitialized = ISNULL(@IsInitialized, IsInitialized)
WHERE SampleId = @SampleId
END
GO
---End of statement DEVCME-273

---DEVCME-346 Statement to add column to AttributeSettings Update them and remove the Table RequiredAttributes 

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=N'Required' AND TABLE_NAME = N'AttributeSettings')
BEGIN
ALTER TABLE AttributeSettings
ADD [Required] BIT NOT NULL DEFAULT(0)
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'RequiredAttributes')
BEGIN
EXEC('UPDATE AttributeSettings
SET [Required] = 1
FROM AttributeSettings a
INNER JOIN RequiredAttributes r on a.AttributeId = r.AttributeId')

DROP TABLE RequiredAttributes
END

---End of Statement for RequiredAttributes

---Adding CRUD and Needed stored procedures for terms table

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.Term_GetAll', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Term_GetAll as select 1')
go
ALTER PROCEDURE Term_GetAll 
AS
BEGIN
SELECT * FROM [dbo].[Terms]
END
GO

IF OBJECT_ID('dbo.Term_GetByOfferId', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Term_GetByOfferId as select 1')
go
ALTER PROCEDURE [dbo].[Term_GetByOfferId] 
	@OfferId UNIQUEIDENTIFIER 
AS
BEGIN
SELECT * FROM [dbo].[Terms] WHERE OfferId = @OfferId
END
GO

IF OBJECT_ID('dbo.Term_GetById', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Term_GetById as select 1')
go
ALTER PROCEDURE Term_GetById 
	@TermId UNIQUEIDENTIFIER 
AS
BEGIN
SELECT * FROM [dbo].[Terms] WHERE Id = @TermId
END
GO

IF OBJECT_ID('dbo.Term_Add', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Term_Add as select 1')
go
ALTER PROCEDURE Term_Add 
	@CPI REAL,
	@Active BIT, 
	@Start DATETIME, 
	@Expiration DATETIME, 
	@OfferId UNIQUEIDENTIFIER, 
	@Created_By NVARCHAR(50),
	@Last_Updated_By NVARCHAR(50)
AS
BEGIN
INSERT INTO Terms (CPI, Active, Start, Expiration, OfferId, Created_By, Last_Updated_By)
VALUES (@CPI, @Active, @Start, @Expiration, @OfferId, @Created_By, @Last_Updated_By)
END
GO

IF OBJECT_ID('dbo.Term_Update', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Term_Update as select 1')
go
ALTER PROCEDURE Term_Update 
	@Id UNIQUEIDENTIFIER,
	@CPI REAL,
	@Active BIT, 
	@Start DATETIME, 
	@Expiration DATETIME, 
	@OfferId UNIQUEIDENTIFIER, 
	@Created_By NVARCHAR(50),
	@Last_Updated_By NVARCHAR(50)
AS
BEGIN

UPDATE Terms
SET CPI = ISNULL(@CPI, CPI),
	Active = ISNULL(@Active, Active),
	Start = ISNULL(@Start, Start),
	Expiration = ISNULL(@Expiration, Expiration),
	OfferId = ISNULL(@OfferId, OfferId),
	Created_By = ISNULL(@Created_By, Created_By),
	Last_Updated_By = ISNULL(@Last_Updated_By, Last_Updated_By)
WHERE Id = @Id
END
GO


IF OBJECT_ID('dbo.Term_Delete', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Term_Delete as select 1')
go
ALTER PROCEDURE Term_Delete 
	@Id UNIQUEIDENTIFIER
AS
BEGIN
DELETE FROM Terms
WHERE Id = @Id
END
GO


IF OBJECT_ID('dbo.Term_CheckTermForOffer', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Term_CheckTermForOffer as select 1')
go
ALTER PROCEDURE Term_CheckTermForOffer
	@TermId UNIQUEIDENTIFIER,
	@OfferId UNIQUEIDENTIFIER
AS
BEGIN
SELECT * FROM dbo.Terms where Id=@TermId AND OfferId=@OfferId
END
GO

IF OBJECT_ID('dbo.Term_GetActiveTermForOffer', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Term_GetActiveTermForOffer as select 1')
go
ALTER PROCEDURE Term_GetActiveTermForOffer
	@OfferId UNIQUEIDENTIFIER
AS
BEGIN
SELECT * FROM dbo.Terms where OfferId=@OfferId And active=1
END
GO

IF OBJECT_ID('dbo.Term_GetTermFromOfferId', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Term_GetTermFromOfferId as select 1')
go
ALTER PROCEDURE Term_GetTermFromOfferId
	@OfferId UNIQUEIDENTIFIER
AS
BEGIN
SELECT * FROM dbo.Terms where OfferId=@OfferId ORDER BY Start DESC
END
GO

IF OBJECT_ID('dbo.Term_CheckTermValidity', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Term_CheckTermValidity as select 1')
go
ALTER PROCEDURE Term_CheckTermValidity
	@TermId UNIQUEIDENTIFIER,
	@GracePeriod int
AS
BEGIN
SELECT * from Terms where DATEDIFF(MINUTE, Expiration,GETDATE() ) <= @GracePeriod  and Id= @TermId
END
GO

IF OBJECT_ID('dbo.Term_SetNewCPI', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Term_SetNewCPI as select 1')
go
ALTER PROCEDURE Term_SetNewCPI
	@OfferId UNIQUEIDENTIFIER,
	@UpdatedBy VARCHAR(50),
	@CPI FLOAT
AS
BEGIN
UPDATE Terms SET active = 0, Expiration = DATEADD(MINUTE, 30, GETDATE()), Last_Updated_By = @UpdatedBy WHERE OfferId = @OfferId AND active = 1
INSERT INTO Terms (CPI, active, Start, Expiration, OfferId, Created_By, Last_Updated_By) VALUES (@CPI, 1, GETDATE(), DATEADD(YEAR, 1, GETDATE()), @OfferId, @UpdatedBy, @UpdatedBy)
END
GO

---End of stored procedures for terms table

---Adding CRUD and needed stored procedures for RespondentAttributes

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.RespondentAttribute_GetAll', 'p') IS NULL
    EXEC ('CREATE PROCEDURE RespondentAttribute_GetAll as select 1')
go
ALTER PROCEDURE RespondentAttribute_GetAll 
AS
BEGIN
SELECT * FROM [dbo].[RespondentAttributes]
END
GO

IF OBJECT_ID('dbo.RespondentAttribute_GetById', 'p') IS NULL
    EXEC ('CREATE PROCEDURE RespondentAttribute_GetById as select 1')
go
ALTER PROCEDURE RespondentAttribute_GetById 
	@Id INT 
AS
BEGIN
SELECT * FROM [dbo].[RespondentAttributes] WHERE Id = @Id
END
GO

IF OBJECT_ID('dbo.RespondentAttribute_GetRespondentAttribute', 'p') IS NULL
    EXEC ('CREATE PROCEDURE RespondentAttribute_GetRespondentAttribute as select 1')
go
ALTER PROCEDURE RespondentAttribute_GetRespondentAttribute 
	@OfferId UNIQUEIDENTIFIER,
	@Ident NVARCHAR(50)
AS
BEGIN
SELECT * FROM [dbo].[RespondentAttributes] WHERE OfferId = @OfferId AND Ident = @Ident
END
GO


IF OBJECT_ID('dbo.RespondentAttribute_GetById', 'p') IS NULL
    EXEC ('CREATE PROCEDURE RespondentAttribute_GetById as select 1')
go
ALTER PROCEDURE RespondentAttribute_GetById 
	@Id INT 
AS
BEGIN
SELECT * FROM [dbo].[RespondentAttributes] WHERE Id = @Id
END
GO

IF OBJECT_ID('dbo.RespondentAttribute_Add', 'p') IS NULL
    EXEC ('CREATE PROCEDURE RespondentAttribute_Add as select 1')
go
ALTER PROCEDURE RespondentAttribute_Add 
	@OfferId UNIQUEIDENTIFIER,
	@Ident NVARCHAR(50), 
	@Values NVARCHAR(MAX)
AS
BEGIN
INSERT INTO RespondentAttributes (OfferId, Ident, [Values])
VALUES (@OfferId, @Ident, @Values)
END
GO

IF OBJECT_ID('dbo.RespondentAttribute_Update', 'p') IS NULL
    EXEC ('CREATE PROCEDURE RespondentAttribute_Update as select 1')
go
ALTER PROCEDURE RespondentAttribute_Update 
	@Id INT,
	@OfferId UNIQUEIDENTIFIER,
	@Ident NVARCHAR(50), 
	@Values NVARCHAR(MAX)
AS
BEGIN

UPDATE RespondentAttributes
SET OfferId = ISNULL(@OfferId, OfferId),
	Ident = ISNULL(@Ident, Ident),
	[Values] = ISNULL(@Values, [Values])
WHERE Id = @Id
END
GO


IF OBJECT_ID('dbo.RespondentAttribute_Delete', 'p') IS NULL
    EXEC ('CREATE PROCEDURE RespondentAttribute_Delete as select 1')
go
ALTER PROCEDURE RespondentAttribute_Delete 
	@Id INT
AS
BEGIN
DELETE FROM RespondentAttributes
WHERE Id = @Id
END
GO

IF OBJECT_ID('dbo.RespondentAttribute_GetOfferAttributes', 'p') IS NULL
    EXEC ('CREATE PROCEDURE RespondentAttribute_GetOfferAttributes as select 1')
go
ALTER PROCEDURE RespondentAttribute_GetOfferAttributes 
	@OfferId UNIQUEIDENTIFIER
AS
BEGIN
SELECT r.*,a.*,att.ShortName
FROM RespondentAttributes as r, AttributeSettings as a, Attributes as att
WHERE r.OfferId = @OfferId AND r.Ident = a.AttributeId AND r.Ident = att.Id AND a.Publish = 1
END
GO

IF OBJECT_ID('dbo.RespondentAttribute_GetRpcOfferAttributes', 'p') IS NULL
    EXEC ('CREATE PROCEDURE RespondentAttribute_GetRpcOfferAttributes as select 1')
go
ALTER PROCEDURE RespondentAttribute_GetRpcOfferAttributes 
	@OfferId UNIQUEIDENTIFIER
AS
BEGIN
SELECT r.Ident
FROM RespondentAttributes as r, AttributeSettings as a
WHERE r.OfferId = @OfferId AND r.Ident = a.AttributeId AND a.Publish = 1
END
GO

IF OBJECT_ID('dbo.RespondentAttribute_GetOfferAttributesApi', 'p') IS NULL
    EXEC ('CREATE PROCEDURE RespondentAttribute_GetOfferAttributesApi as select 1')
go
ALTER PROCEDURE RespondentAttribute_GetOfferAttributesApi 
	@OfferId UNIQUEIDENTIFIER
AS
BEGIN
SELECT r.Ident as Name, r.[Values] as Value
FROM RespondentAttributes as r, AttributeSettings as a
WHERE r.OfferId = @OfferId AND r.Ident = a.AttributeId AND a.Publish = 1
END
GO

---End of stored procedures for RespondentAttributes table

---Adding CRUD and needed stored procedures for Providers

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.Provider_GetAll', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Provider_GetAll as select 1')
go
ALTER PROCEDURE Provider_GetAll 
AS
BEGIN
SELECT * FROM [dbo].[Providers]
END
GO

IF OBJECT_ID('dbo.Provider_GetById', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Provider_GetById as select 1')
go
ALTER PROCEDURE Provider_GetById 
	@Id INT 
AS
BEGIN
SELECT * FROM [dbo].[Providers] WHERE Id = @Id
END
GO

IF OBJECT_ID('dbo.Provider_GetByApiUser', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Provider_GetByApiUser as select 1')
go
ALTER PROCEDURE Provider_GetByApiUser 
	@ApiUser NVARCHAR(MAX) 
AS
BEGIN
SELECT * FROM [dbo].[Providers] WHERE ApiUser = @ApiUser
END
GO

IF OBJECT_ID('dbo.Provider_Add', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Provider_Add as select 1')
go
ALTER PROCEDURE Provider_Add 
	@Name NVARCHAR(MAX),
	@ApiUser NVARCHAR(MAX), 
	@ProviderCode NVARCHAR(MAX),
	@Active BIT,
	@ByPass BIT
AS
BEGIN
INSERT INTO Providers(Name, ApiUser, ProviderCode, Active, ByPass)
VALUES (@Name, @ApiUser, @ProviderCode, @Active,@ByPass)
END
GO

IF OBJECT_ID('dbo.Provider_Update', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Provider_Update as select 1')
go
ALTER PROCEDURE Provider_Update 
	@Id INT,
	@Name NVARCHAR(MAX),
	@ApiUser NVARCHAR(MAX), 
	@ProviderCode NVARCHAR(MAX),
	@Active BIT,
	@ByPass BIT
AS
BEGIN

UPDATE Providers
SET Name = ISNULL(@Name, Name),
	ApiUser = ISNULL(@ApiUser, ApiUser),
	ProviderCode = ISNULL(@ProviderCode, ProviderCode),
	Active = ISNULL(@Active, Active),
	ByPass = ISNULL(@ByPass, ByPass)
WHERE Id = @Id
END
GO


IF OBJECT_ID('dbo.Provider_Delete', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Provider_Delete as select 1')
go
ALTER PROCEDURE Provider_Delete 
	@Id INT
AS
BEGIN
DELETE FROM Providers
WHERE Id = @Id
END
GO

---End of stored procedures for Providers table

---Adding CRUD and needed stored procedures for Offers
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.Offer_GetAll', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Offer_GetAll as select 1')
go
ALTER PROCEDURE Offer_GetAll 
AS
BEGIN
SELECT * FROM [dbo].[Offers]
END
GO

IF OBJECT_ID('dbo.Offer_GetById', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Offer_GetById as select 1')
go
ALTER PROCEDURE Offer_GetById 
	@OfferId UNIQUEIDENTIFIER 
AS
BEGIN
SELECT * FROM [dbo].[Offers] WHERE Id = @OfferId
END
GO


IF OBJECT_ID('dbo.Offer_GetBySampleId', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Offer_GetBySampleId as select 1')
go
ALTER PROCEDURE Offer_GetBySampleId 
	@SampleId INT
AS
BEGIN
SELECT * FROM [dbo].[Offers] WHERE SampleId = @SampleId
END
GO

IF OBJECT_ID('dbo.Offer_Add', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Offer_Add as select 1')
go
ALTER PROCEDURE Offer_Add 
	@StudyId INT,
	@SampleId INT, 
	@LOI INT, 
	@IR REAL, 
	@Active INT = null, 
	@Description NVARCHAR(MAX) = null,
	@Title NVARCHAR(256) = null,
	@Topic NVARCHAR(256) = null,
	@OfferLink NVARCHAR(512) = null,
	@QuotaRemaining INT,
	@StudyStartDate DATETIME,
	@StudyEndDate DATETIME = null
AS
BEGIN
INSERT INTO Offers (StudyId, SampleId, LOI, IR, active, Description, Title, Topic, OfferLink, QuotaRemaining, StudyStartDate, StudyEndDate)
VALUES (@StudyId, @SampleId, @LOI, @IR, @Active, @Description, @Title, @Topic, @OfferLink, @QuotaRemaining, @StudyStartDate, @StudyEndDate)
END
GO

IF OBJECT_ID('dbo.Offer_Update', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Offer_Update as select 1')
go
ALTER PROCEDURE [dbo].[Offer_Update] 
	@Id UNIQUEIDENTIFIER,
	@StudyId INT = null,
	@SampleId INT = null, 
	@LOI INT = null, 
	@IR REAL = null, 
	@Active INT = null, 
	@Test NVARCHAR(max) = null,
	@Description NVARCHAR(MAX) = null,
	@Title NVARCHAR(256) = null,
	@Topic NVARCHAR(256) = null,
	@OfferLink NVARCHAR(512) = null,
	@QuotaRemaining INT = null,
	@StudyStartDate DATETIME = null,
	@StudyEndDate DATETIME = null
AS
BEGIN

UPDATE Offers
SET StudyId = ISNULL(@StudyId, StudyId),
 SampleId = ISNULL(@SampleId, SampleId),
 LOI = ISNULL(@LOI, LOI),
 IR = ISNULL(@IR, IR),
 Active = ISNULL(@Active, Active),
 Description = ISNULL(@Description, Description),
 Title = ISNULL(@Title, Title),
 Topic = ISNULL(@Topic, Topic),
 OfferLink = ISNULL(@OfferLink, OfferLink),
 QuotaRemaining = ISNULL(@QuotaRemaining, QuotaRemaining),
 StudyStartDate = ISNULL(@StudyStartDate, StudyStartDate),
 StudyEndDate = ISNULL(@StudyEndDate, StudyEndDate),
 TestOffer = ISNULL(@Test, TestOffer)
WHERE Id = @Id

END
GO

IF OBJECT_ID('dbo.Offer_Delete', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Offer_Delete as select 1')
go
ALTER PROCEDURE Offer_Delete 
	@Id UNIQUEIDENTIFIER
AS
BEGIN
DELETE FROM Offers
WHERE Id = @Id
END
GO

IF OBJECT_ID('dbo.Offer_GetActiveOffers', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Offer_GetActiveOffers as select 1')
go
ALTER PROCEDURE Offer_GetActiveOffers 
@OfferType BIT = null
AS
BEGIN
SELECT o.Id as OfferId, o.Title, o.Topic, o.Description, o.LOI, o.IR, o.QuotaRemaining, o.OfferLink, t.Id as TermId, t.CPI
FROM Offers as o
JOIN Terms t on o.Id = t.OfferId AND t.active = 1 and t.Expiration > GETDATE()
WHERE o.OfferLink IS NOT NULL AND o.active = 1 AND
((@OfferType IS NULL) OR (o.TestOffer = @OfferType))

END
GO

IF OBJECT_ID('dbo.Offer_GetStudyIdsFromOfferIds', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Offer_GetStudyIdsFromOfferIds as select 1')
GO
ALTER PROCEDURE Offer_GetStudyIdsFromOfferIds 
	@OfferIds NVARCHAR(MAX)
AS
BEGIN
DECLARE @SQL NVARCHAR(MAX)

SET @SQL = 'SELECT StudyId, Id
FROM Offers as o
WHERE o.Id in ('+@OfferIds+')'

EXEC(@SQL)
END
GO

IF OBJECT_ID('dbo.Offer_GetOfferIdsFromStudyIds', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Offer_GetOfferIdsFromStudyIds as select 1')
GO
ALTER PROCEDURE Offer_GetOfferIdsFromStudyIds 
	@StudyIds NVARCHAR(MAX)
AS
BEGIN
DECLARE @SQL NVARCHAR(MAX)

SET @SQL = 'SELECT Id as OfferId, StudyId
FROM Offers as o
WHERE o.StudyId in ('+@StudyIds+')
AND o.Active = 1'

EXEC(@SQL)
END
GO

IF OBJECT_ID('dbo.Offer_GetActiveSampleIds', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Offer_GetActiveSampleIds as select 1')
GO
ALTER PROCEDURE Offer_GetActiveSampleIds 
AS
BEGIN
DECLARE @SQL NVARCHAR(MAX)

EXEC('SELECT o.SampleId, o.Id, o.IsInitialized
FROM Offers as o
WHERE o.active = 1')

END
GO

IF OBJECT_ID('dbo.Offer_Suspend', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Offer_Suspend as select 1')
GO
ALTER PROCEDURE Offer_Suspend
	@SampleId INT 
AS
BEGIN
DECLARE @SQL NVARCHAR(MAX)

UPDATE Offers
SET active = 2
WHERE SampleId = @SampleId

END
GO


IF OBJECT_ID('dbo.Offer_GetFilteredOffers', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Offer_GetFilteredOffers as select 1')
GO
ALTER PROCEDURE [dbo].[Offer_GetFilteredOffers]
	-- Add the parameters for the stored procedure here
	@StudyId INT = null, 
	@OfferTitle NVARCHAR(MAX) = NULL,
	@OfferStatus INT = NULL,
	@OfferType INT = NULL,
	@Page INT = 0,
	@RecordsPerPage INT = 40,
	@SortBy NVARCHAR(20) = '',
	@SortDir NVARCHAR(5) = ''
AS
BEGIN
	DECLARE @Sql NVARCHAR(MAX)
	DECLARE @Publish NVARCHAR(MAX)
	DECLARE @Sort NVARCHAR(MAX)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

	DECLARE @Conditions NVARCHAR(MAX) = ''

	SET @Sort = 'ORDER BY StudyId'
	
	IF @OfferTitle <> ''
		SET @Conditions = 'AND o.Title LIKE ''%' + @OfferTitle + '%'' '
			
	IF @StudyId IS NOT NULL
	BEGIN
		SET @Conditions = @Conditions + ' AND o.StudyId = ' + cast(@StudyId as varchar(10))
	END
		
	IF @OfferStatus IS NOT NULL
	BEGIN
		IF @OfferStatus <> 3
		BEGIN
			SET @Conditions = @Conditions + ' AND o.active = ' + cast(@OfferStatus as varchar(10))
		END
	END
		
	IF @OfferType IS NOT NULL
	BEGIN
		IF @OfferType <> 2
		BEGIN
			SET @Conditions = @Conditions + ' AND o.TestOffer = ' + cast(@OfferType as varchar(10))
		END
	END

	IF(@SortBy IS NOT NULL AND @SortBy <> '')
	BEGIN
		SET @Sort = ' ORDER BY ' + @SortBy + ' ' + @SortDir;
	END


	DECLARE @RowCount NVARCHAR(MAX) =
	        '
			DECLARE @TotalRows INT 
			
		SELECT COUNT(1) AS TotalRows INTO #Temp_count
		FROM (SELECT ROW_NUMBER() OVER (ORDER BY o.StudyId) AS RowNum, 
			@TotalRows as TotalRows,
			o.StudyId as StudyId,
			o.Title as Title,
			CASE WHEN (o.active IS NULL) THEN ''0'' ELSE o.active END as status
			FROM dbo.Offers o 
			WHERE 1 = 1 ' + @Conditions + ') AS PaginatedOffers
			WHERE RowNum > ' + cast((@Page * @RecordsPerPage) as varchar(20)) + '
			AND RowNum <= ' + cast((@Page * @RecordsPerPage + @RecordsPerPage) as varchar(20)) + '
			SET @TotalRows = (SELECT * FROM #Temp_count)'

		DECLARE @FromRowCount NVARCHAR(MAX) =
	        '
			DECLARE @FromTotalRows INT 
			
		SELECT COUNT(1) AS FromTotalRows INTO #Temp_total_count
		FROM dbo.Offers o 
		WHERE 1 = 1 ' + @Conditions + '
			SET @FromTotalRows = (SELECT * FROM #Temp_total_count)'


	SET @Sql = '
		SELECT *
		FROM (SELECT ROW_NUMBER() OVER (' + @Sort + ') AS RowNum, 
			o.[Id] as OfferId,
			o.[StudyId] as StudyId,
			o.[SampleId] as SampleId,
			o.[Title] as Title,
			o.[Topic] as Topic,
			o.[Description] as Description,
			o.[QuotaRemaining] as QuotaRemaining,
			o.[StudyStartDate] as StudyStartDate,
			o.[StudyEndDate] as StudyEndDate,
			o.[LOI] as LOI,
			o.[IR] as IR,
			o.[TestOffer] as TestOffer,
			T.[Id] as TermId,
			T.[CPI] as CPI,
			CASE WHEN (o.Active IS NULL) THEN 0 ELSE o.Active END as Active,
			@TotalRows as TotalRows,
			@FromTotalRows as FromTotalRows
			FROM [dbo].[Offers] o
			LEFT JOIN [dbo].[Terms] T ON (o.[Id] = T.[OfferId]) AND T.Active = 1
	'

	SET @Sql = @Sql + 'WHERE 1 = 1 '

	
	SET @Sql = @Sql + @Conditions + ' ) AS PaginatedAttributes
			WHERE RowNum > ' + cast((@Page * @RecordsPerPage) as varchar(20)) + '
			AND RowNum <= ' + cast((@Page * @RecordsPerPage + @RecordsPerPage) as varchar(20)) + '
			ORDER BY ROWNUM
			';

	print(@Sql)
	EXEC(@RowCount + @FromRowCount +  @Sql)
END

---End of stored procedures for Offers table

---Adding CRUD and needed stored procedures for Attributes
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.Attribute_Add', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Attribute_Add as select 1')
go
ALTER PROCEDURE Attribute_Add
	@Id NVARCHAR(max),
	@Alias NVARCHAR(max),
	@Name NVARCHAR(max) = null,
	@Shortname NVARCHAR(max) = null,
	@Label NVARCHAR(max) = null,
	@Type NVARCHAR(max) = null
AS
BEGIN
INSERT INTO Attributes(Id, Alias, Name, Shortname, Label, Type)
VALUES (@Id, @Alias, @Name, @Shortname, @Label, @Type)
END
GO

IF OBJECT_ID('dbo.Attribute_Delete', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Attribute_Delete as select 1')
go
ALTER PROCEDURE Attribute_Delete
	@Id NVARCHAR(max)
AS
BEGIN
DELETE FROM Attributes
WHERE Id = @Id
END
GO

IF OBJECT_ID('dbo.Attribute_GetAll', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Attribute_GetAll as select 1')
go
ALTER PROCEDURE Attribute_GetAll
AS
BEGIN
SELECT * FROM [dbo].[Attributes]
END
GO

IF OBJECT_ID('dbo.Attribute_GetById', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Attribute_GetById as select 1')
go
ALTER PROCEDURE Attribute_GetById
	@Id NVARCHAR(max) = null 
AS
BEGIN
SELECT * FROM [dbo].[Attributes] WHERE Id = @Id
END

GO

IF OBJECT_ID('dbo.Attribute_GetPublishedAttributes', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Attribute_GetPublishedAttributes as select 1')
go
ALTER PROCEDURE Attribute_GetPublishedAttributes
AS
BEGIN
SELECT attr.* 
FROM [dbo].[Attributes] as attr, [dbo].[AttributeSettings] as attrset 
WHERE attr.Id = attrset.AttributeId AND attrset.Publish = 1
END

GO

IF OBJECT_ID('dbo.Attribute_Update', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Attribute_Update as select 1')
go
ALTER PROCEDURE Attribute_Update
	@Id NVARCHAR(max),
	@Alias NVARCHAR(max) = null,
	@Name NVARCHAR(max) = null,
	@ShortName NVARCHAR(max) = null,
	@Label NVARCHAR(max) = null,
	@Type NVARCHAR(max) = null
AS
BEGIN

UPDATE Attributes
SET Alias = ISNULL(@Alias, Alias),
	Name = ISNULL(@Name, Name),
 ShortName = ISNULL(@ShortName, ShortName),
 Label = ISNULL(@Label, Label),
 Type = ISNULL(@Type, Type)
WHERE Id = @Id

END
GO

IF OBJECT_ID('dbo.Attribute_GetRequired', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Attribute_GetRequired as select 1')
go
ALTER PROCEDURE Attribute_GetRequired
AS
BEGIN

SELECT attr.*
FROM Attributes as attr, AttributeSettings as attrset
WHERE attr.Id = attrset.AttributeId AND attrset.Required = 1
END
GO

IF OBJECT_ID('dbo.Attribute_CheckAttribute', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Attribute_CheckAttribute as select 1')
go
ALTER PROCEDURE Attribute_CheckAttribute
	@Attribute NVARCHAR(MAX)
AS
BEGIN

SELECT *
FROM Attributes
WHERE UPPER(Id) = UPPER(@Attribute) OR UPPER(Alias) = UPPER(@Attribute)
END
GO

/****** Object:  StoredProcedure [dbo].[Attribute_GetFilteredAttributes]    Script Date: 5/6/2015 11:20:09 AM ******/
IF OBJECT_ID('dbo.Attribute_GetFilteredAttributes', 'p') IS NULL
    EXEC ('CREATE PROCEDURE dbo.Attribute_GetFilteredAttributes as select 1')
go
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[Attribute_GetFilteredAttributes]
	-- Add the parameters for the stored procedure here
	@AttributeId NVARCHAR(50) = null, 
	@Status INT = NULL,
	@Page INT = 0,
	@RecordsPerPage INT = 40
AS
BEGIN
	DECLARE @Sql NVARCHAR(MAX)
	DECLARE @Publish NVARCHAR(MAX)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

	DECLARE @Conditions NVARCHAR(MAX) = ''

	IF @AttributeId <> ''
		SET @Conditions = 'AND a.Id LIKE ''%' + @AttributeId + '%'' '	
	IF @Status IS NOT NULL
	BEGIN
		IF @Status = 0
		BEGIN
			SET @Conditions = @Conditions + 'AND (S.[Publish] = 0 OR S.[Publish] is NULL)'
		END
		IF @Status = 1
		BEGIN
			print(@Status)
			SET @Conditions = @Conditions + 'AND S.[Publish] = 1'
		END
	END


	DECLARE @RowCount NVARCHAR(MAX) =
	        '
			DECLARE @TotalRows INT 
			
		SELECT COUNT(1) AS TotalRows INTO #Temp_count
		FROM (SELECT ROW_NUMBER() OVER (ORDER BY Id) AS RowNum, 
			@TotalRows as TotalRows,
			a.Id as AttributeId,
			a.Label as Label,
			CASE WHEN (S.[Publish] IS NULL) THEN ''False'' ELSE S.[Publish] END as status
			FROM dbo.Attributes a 
			LEFT JOIN [dbo].[AttributeSettings] S ON (A.[Id] = S.[AttributeId])
			WHERE 1 = 1 ' + @Conditions + ') AS PaginatedAttributes
			WHERE RowNum > ' + cast((@Page * @RecordsPerPage) as varchar(20)) + '
			AND RowNum <= ' + cast((@Page * @RecordsPerPage + @RecordsPerPage) as varchar(20)) + '
			SET @TotalRows = (SELECT * FROM #Temp_count)'

		DECLARE @FromRowCount NVARCHAR(MAX) =
	        '
			DECLARE @FromTotalRows INT 
			
		SELECT COUNT(1) AS FromTotalRows INTO #Temp_total_count
		FROM dbo.Attributes a 
		LEFT JOIN [dbo].[AttributeSettings] S ON (A.[Id] = S.[AttributeId])
		WHERE 1 = 1 ' + @Conditions + '
			SET @FromTotalRows = (SELECT * FROM #Temp_total_count)'


	SET @Sql = '
		SELECT *
		FROM (SELECT ROW_NUMBER() OVER (ORDER BY A.Id) AS RowNum, 
			A.[Id] as AttributeId,
			A.[Label] as Label,
			CASE WHEN (S.[Publish] IS NULL) THEN ''False'' ELSE S.[Publish] END as Status,
			@TotalRows as TotalRows,
			@FromTotalRows as FromTotalRows,
			CASE WHEN (R.[Ident] IS NULL) THEN 0 ELSE 1 END AS Used ,
			CASE WHEN (S.[Last_Updated_By] IS NULL) THEN ''System'' ELSE S.[Last_Updated_By] END as Last_Updated_By,
			ISNULL(CONVERT(varchar(50),  S.[Update_Date] ,101) +'' '' +LTRIM(RIGHT(CONVERT(varchar(50),  S.[Update_Date] ,22),11)),''N/A'') as Update_Date
			FROM [dbo].[Attributes] A
			LEFT JOIN [dbo].[RespondentAttributes] R ON (A.[Id] = R.[Ident])
			LEFT JOIN [dbo].[AttributeSettings] S ON (A.[Id] = S.[AttributeId])
	'

	SET @Sql = @Sql + 'WHERE 1 = 1 '

	
	SET @Sql = @Sql + @Conditions + ' GROUP BY A.[Id], A.[Label], S.[Publish],S.[Last_Updated_By] ,S.[Update_Date], R.[Ident]) AS PaginatedAttributes
			WHERE RowNum > ' + cast((@Page * @RecordsPerPage) as varchar(20)) + '
			AND RowNum <= ' + cast((@Page * @RecordsPerPage + @RecordsPerPage) as varchar(20)) + '
			ORDER BY RowNum
			'; 
	print(@Sql)
	EXEC( @RowCount + @FromRowCount + @Sql )
END
GO

---End of stored procedures for Attributes table

---Adding CRUD and needed stored procedures for AttributeSettings
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.AttributeSetting_Add', 'p') IS NULL
    EXEC ('CREATE PROCEDURE AttributeSetting_Add as select 1')
go
ALTER PROCEDURE AttributeSetting_Add
	@AttributeId NVARCHAR(max),
	@Creation_Date DATETIME = null,
	@Created_By NVARCHAR(max) = null,
	@Update_Date DATETIME = null,
	@Last_Updated_By NVARCHAR(max) = null,
	@Publish bit = null,
	@Required bit = null
AS
BEGIN
INSERT INTO AttributeSettings (AttributeId, Creation_Date,  Created_By, Update_Date, Last_Updated_By, Publish, Required)
VALUES (@AttributeId, ISNULL(@Creation_Date,GETDATE()),ISNULL(@Created_By,'System'), ISNULL(@Update_Date,GETDATE()), ISNULL(@Last_Updated_By,'System'), ISNULL(@Publish, 0), ISNULL(@Required, 0))
END
GO

IF OBJECT_ID('dbo.AttributeSetting_Delete', 'p') IS NULL
    EXEC ('CREATE PROCEDURE AttributeSetting_Delete as select 1')
go
ALTER PROCEDURE AttributeSetting_Delete
	@AttributeId NVARCHAR(max)
AS
BEGIN
DELETE FROM AttributeSettings
WHERE AttributeId = @AttributeId
END
GO

IF OBJECT_ID('dbo.AttributeSetting_GetAll', 'p') IS NULL
    EXEC ('CREATE PROCEDURE AttributeSetting_GetAll as select 1')
go
ALTER PROCEDURE AttributeSetting_GetAll
AS
BEGIN
SELECT * FROM [dbo].[AttributeSettings]
END
GO

IF OBJECT_ID('dbo.AttributeSetting_GetById', 'p') IS NULL
    EXEC ('CREATE PROCEDURE AttributeSetting_GetById as select 1')
go
ALTER PROCEDURE AttributeSetting_GetById
	@AttributeId NVARCHAR(max) = null 
AS
BEGIN
SELECT * FROM [dbo].[AttributeSettings] WHERE AttributeId = @AttributeId
END

GO

IF OBJECT_ID('dbo.AttributeSetting_Update', 'p') IS NULL
    EXEC ('CREATE PROCEDURE AttributeSetting_Update as select 1')
go
ALTER PROCEDURE AttributeSetting_Update
	@AttributeId NVARCHAR(max),
	@Creation_Date DateTime = null,
	@Created_By NVARCHAR(max) = null,
	@Update_Date DATETIME = null,
	@Last_Updated_By NVARCHAR(max) = null,
	@Publish NVARCHAR(max) = null,
	@Required NVARCHAR(max) = null
AS
BEGIN

UPDATE AttributeSettings
SET Creation_Date = ISNULL(@Creation_Date, GETDATE()),
	Created_By = ISNULL(@Created_By,Created_By),
	Update_Date = ISNULL(@Update_Date, GetDate()),
	Last_Updated_By = ISNULL(@Last_Updated_By, Last_Updated_By),
	Publish = ISNULL(@Publish, Publish),
	Required = ISNULL(@Required, Required)
WHERE AttributeId = @AttributeId

END
GO

IF OBJECT_ID('dbo.AttributeSetting_Publish', 'p') IS NULL
    EXEC ('CREATE PROCEDURE AttributeSetting_Publish as select 1')
go
ALTER PROCEDURE AttributeSetting_Publish
	@AttributeId NVARCHAR(max),
	@Status BIT,
	@User NVARCHAR(max)
AS
BEGIN

IF EXISTS(SELECT * FROM AttributeSettings WHERE AttributeId = @AttributeId)
BEGIN
	UPDATE AttributeSettings SET Publish = @Status, Last_Updated_By = @User WHERE AttributeId = @AttributeId
END
ELSE 
BEGIN
	INSERT INTO AttributeSettings (AttributeId, Publish, Last_Updated_By) VALUES (@AttributeId, @Status, @User)
END

END

---Adding CRUD and needed stored procedures for AttributeOptions
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.AttributeOption_Add', 'p') IS NULL
    EXEC ('CREATE PROCEDURE AttributeOption_Add as select 1')
go
ALTER PROCEDURE AttributeOption_Add
	@AttributeId NVARCHAR(max),
	@Code INT, 
	@Description NVARCHAR(max)
AS
BEGIN
INSERT INTO AttributeOptions(AttributeId, Code, Description)
VALUES (@AttributeId, @Code, @Description)
END
GO

IF OBJECT_ID('dbo.AttributeOption_Delete', 'p') IS NULL
    EXEC ('CREATE PROCEDURE AttributeOption_Delete as select 1')
go
ALTER PROCEDURE AttributeOption_Delete 
	@AttributeId NVARCHAR(max),
	@Code int
AS
BEGIN
DELETE FROM AttributeOptions
WHERE AttributeId = @AttributeId AND Code=@Code
END
GO

IF OBJECT_ID('dbo.AttributeOption_GetAll', 'p') IS NULL
    EXEC ('CREATE PROCEDURE AttributeOption_GetAll as select 1')
go
ALTER PROCEDURE AttributeOption_GetAll
AS
BEGIN
SELECT * FROM [dbo].[AttributeOptions]
END
GO

IF OBJECT_ID('dbo.AttributeOption_GetById', 'p') IS NULL
    EXEC ('CREATE PROCEDURE AttributeOption_GetById as select 1')
go
ALTER PROCEDURE AttributeOption_GetById
	@AttributeId NVARCHAR(max) = null 
AS
BEGIN
SELECT [dbo].[AttributeOptions].[AttributeId] as AttributeId, [dbo].[AttributeOptions].[Code] as Code,[dbo].[AttributeOptions].[Description] as Description FROM [dbo].[AttributeOptions] WHERE AttributeId = @AttributeId ORDER BY CASE WHEN ISNUMERIC(Code) = 1 THEN CAST(Code AS INT) ELSE 0
END
END
GO

IF OBJECT_ID('dbo.AttributeOption_Update', 'p') IS NULL
    EXEC ('CREATE PROCEDURE AttributeOption_Update as select 1')
go
ALTER PROCEDURE AttributeOption_Update
	@AttributeId NVARCHAR(max),
	@Code INT,
	@Description NVARCHAR(max) = null
AS
BEGIN

UPDATE AttributeOptions
SET Description = ISNULL(@Description, Description)
WHERE AttributeId = @AttributeId AND Code = @Code
END

GO
---End of stored procedures for AttributeOptions table

---Adding triggers to proper columns

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sys.objects WHERE [type] = 'TR' AND [name] = 'trg_UpdateModifiedAttributeSettings_Setting')
BEGIN
DROP TRIGGER [dbo].[trg_UpdateModifiedAttributeSettings_Setting]
END
/****** Object:  Trigger [dbo].[trg_UpdateModifiedAttributeSettings_Setting]    Script Date: 4/23/2015 10:16:28 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[trg_UpdateModifiedAttributeSettings_Setting]
ON [dbo].[AttributeSettings]
AFTER UPDATE
AS
    UPDATE t
            SET t.[Update_Date] = GETDATE()
            FROM [dbo].AttributeSettings AS t 
            INNER JOIN inserted AS i 
            ON t.[AttributeId] = i.[AttributeId] ;


GO

/****** Object:  Trigger [trigger_InsertAttributeSetting_Attributes]    Script Date: 4/23/2015 11:01:22 AM ******/
IF EXISTS (SELECT * FROM sys.objects WHERE [type] = 'TR' AND [name] = 'trigger_InsertAttributeSetting_Attributes')
BEGIN
DROP TRIGGER [dbo].[trigger_InsertAttributeSetting_Attributes]
END

/****** Object:  Trigger [dbo].[trigger_InsertAttributeSetting_Attributes]    Script Date: 4/23/2015 11:01:22 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE TRIGGER [dbo].[trigger_InsertAttributeSetting_Attributes] ON [dbo].[Attributes]
FOR INSERT

AS
DECLARE @AttributeId nvarchar(max)
SET @AttributeId = (SELECT Id FROM Inserted)
	
IF NOT EXISTS (SELECT * FROM AttributeSettings WHERE AttributeId = @AttributeId)
BEGIN
INSERT INTO AttributeSettings
        (AttributeId)
    SELECT
        Id
        FROM inserted
END

GO

IF EXISTS (SELECT * FROM sys.objects WHERE [type] = 'TR' AND [name] = 'trigger_InsertTerms_Terms')
BEGIN
DROP TRIGGER [dbo].[trigger_InsertTerms_Terms]
END
GO

CREATE TRIGGER [dbo].[trigger_InsertTerms_Terms] ON [dbo].Terms
FOR INSERT

AS
DECLARE @TermId UNIQUEIDENTIFIER
SET @TermId = (SELECT Id FROM Inserted)
	
BEGIN
UPDATE Terms SET Creation_Date = GETDATE(), Update_Date = GETDATE() 
WHERE Id = @TermId
END

GO

IF EXISTS (SELECT * FROM sys.objects WHERE [type] = 'TR' AND [name] = 'trigger_UpdateTerms_Terms')
BEGIN
DROP TRIGGER [dbo].[trigger_UpdateTerms_Terms]
END
GO

CREATE TRIGGER [dbo].[trigger_UpdateTerms_Terms] ON [dbo].Terms
AFTER UPDATE
AS
	
BEGIN
    UPDATE t
            SET t.[Update_Date] = GETDATE()
            FROM [dbo].Terms AS t 
            INNER JOIN inserted AS i 
            ON t.Id = i.Id ;
END
GO

IF EXISTS(SELECT default_constraints.name FROM sys.all_columns 
			INNER JOIN sys.tables ON all_columns.object_id = tables.object_id 
			INNER JOIN sys.schemas ON tables.schema_id = schemas.schema_id
			INNER JOIN sys.default_constraints ON all_columns.default_object_id = default_constraints.object_id
			WHERE schemas.name = 'dbo'
				AND tables.name = 'Terms'
				AND all_columns.name = 'Id')
BEGIN
ALTER TABLE Terms
DROP CONSTRAINT DF_Terms_Id
END

ALTER TABLE Terms
ADD CONSTRAINT DF_Terms_Id DEFAULT NEWID() FOR Id


IF NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].QuotaExpressions') AND type in (N'U'))
BEGIN

CREATE TABLE [dbo].[QuotaExpressions](
[SampleId] int NOT NULL,
[ExpressionsXML] [nvarchar](Max) NOT NULL,
[QuotaExpressionsXML] [nvarchar](Max) NOT NULL,
[OfferId] uniqueidentifier NOT NULL,
[Creation_Date] [datetime] NOT NULL DEFAULT(GETDATE()),
[Update_Date] [datetime] NOT NULL DEFAULT(GETDATE())
 CONSTRAINT [PK_QuotaExpressions] PRIMARY KEY CLUSTERED 
(
	[SampleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
End
GO

IF NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].SampleMapping') AND type in (N'U'))
BEGIN

CREATE TABLE [dbo].[SampleMapping](
[SampleId] INT NOT NULL,
[InternalSampleId] INT NOT NULL,
[OfferId] uniqueidentifier NOT NULL,
[Creation_Date] [datetime] NOT NULL DEFAULT(GETDATE()),
[Update_Date] [datetime] NOT NULL DEFAULT(GETDATE())
 CONSTRAINT [PK_SampleMapping] PRIMARY KEY CLUSTERED 
(
	[SampleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

End
GO

IF NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].QuotaMapping') AND type in (N'U'))
BEGIN

CREATE TABLE [dbo].[QuotaMapping](
[SampleId] INT NOT NULL,
[QuotaId] INT NOT NULL,
[InternalQuotaId] INT,
[QuotaRemaining] INT NOT NULL,
[OfferId] uniqueidentifier NOT NULL,
[Creation_Date] [datetime] NOT NULL DEFAULT(GETDATE()),
[Update_Date] [datetime] NOT NULL DEFAULT(GETDATE())
 CONSTRAINT [PK_QuotaMapping] PRIMARY KEY CLUSTERED 
(
	[QuotaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

End
GO
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[QuotaMapping]') AND name = N'IX_QuotaMapping_SampleId')
DROP INDEX [dbo].[QuotaMapping].IX_QuotaMapping_SampleId
GO
IF  NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[QuotaMapping]') AND name = N'IX_QuotaMapping_SampleId')
CREATE INDEX IX_QuotaMapping_SampleId ON [dbo].[QuotaMapping] (SampleId)
GO

---Adding CRUD and needed stored procedures for QuotaExpressions table

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.SteamStudy_GetAll', 'p') IS NULL
    EXEC ('CREATE PROCEDURE SteamStudy_GetAll as select 1')
GO
ALTER PROCEDURE SteamStudy_GetAll 
AS
BEGIN
SELECT * FROM [dbo].[QuotaExpressions]
END
GO

IF OBJECT_ID('dbo.SteamStudy_GetBySampleId', 'p') IS NULL
    EXEC ('CREATE PROCEDURE SteamStudy_GetBySampleId as select 1')
GO
ALTER PROCEDURE SteamStudy_GetBySampleId 
	@SampleId INT  
AS
BEGIN
SELECT * FROM [dbo].[QuotaExpressions] WHERE SampleId = @SampleId
END
GO


IF OBJECT_ID('dbo.SteamStudy_Add', 'p') IS NULL
    EXEC ('CREATE PROCEDURE SteamStudy_Add as select 1')
GO
ALTER PROCEDURE SteamStudy_Add 
	@SampleId INT,
	@ExpressionsXML NVARCHAR(MAX),
	@QuotaExpressionsXML NVARCHAR(MAX),
	@OfferId uniqueidentifier
	
AS
BEGIN
INSERT INTO QuotaExpressions(SampleId, ExpressionsXML,QuotaExpressionsXML,OfferId)
VALUES (@SampleId, @ExpressionsXML,@QuotaExpressionsXML,@OfferId)
END
GO


IF OBJECT_ID('dbo.SteamStudy_Update', 'p') IS NULL
    EXEC ('CREATE PROCEDURE SteamStudy_Update as select 1')
GO
ALTER PROCEDURE SteamStudy_Update 
	@SampleId INT,
	@ExpressionsXML NVARCHAR(MAX)
AS
BEGIN
UPDATE QuotaExpressions
SET ExpressionsXML = ISNULL(@ExpressionsXML, ExpressionsXML)
WHERE SampleId = @SampleId
END
GO

IF OBJECT_ID('dbo.SteamQuotaStudy_Update', 'p') IS NULL
    EXEC ('CREATE PROCEDURE SteamQuotaStudy_Update as select 1')
GO
ALTER PROCEDURE SteamQuotaStudy_Update 
	@SampleId INT,
	@QuotaExpressionsXML NVARCHAR(MAX)
AS
BEGIN
UPDATE QuotaExpressions
SET QuotaExpressionsXML = ISNULL(@QuotaExpressionsXML, QuotaExpressionsXML)
WHERE SampleId = @SampleId
END
GO


IF OBJECT_ID('dbo.SteamStudy_Delete', 'p') IS NULL
    EXEC ('CREATE PROCEDURE SteamStudy_Delete as select 1')
GO
ALTER PROCEDURE SteamStudy_Delete 
	@SampleId INT
AS
BEGIN
DELETE FROM QuotaExpressions
WHERE SampleId = @SampleId
END
GO

---End of stored procedures for QuotaExpressions table


---Adding CRUD and needed stored procedures for SampleMapping table

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.SampleMapping_GetAll', 'p') IS NULL
    EXEC ('CREATE PROCEDURE SampleMapping_GetAll as select 1')
GO
ALTER PROCEDURE SampleMapping_GetAll 
AS
BEGIN
SELECT * FROM [dbo].[SampleMapping]
END
GO

IF OBJECT_ID('dbo.SampleMapping_GetBySampleId', 'p') IS NULL
    EXEC ('CREATE PROCEDURE SampleMapping_GetBySampleId as select 1')
GO
ALTER PROCEDURE SampleMapping_GetBySampleId 
	@SampleId INT  
AS
BEGIN
SELECT * FROM [dbo].[SampleMapping] WHERE SampleId = @SampleId
END
GO

IF OBJECT_ID('dbo.SampleMapping_GetByInternalSampleId', 'p') IS NULL
    EXEC ('CREATE PROCEDURE SampleMapping_GetByInternalSampleId as select 1')
GO
ALTER PROCEDURE SampleMapping_GetByInternalSampleId 
	@InternalSampleId INT  
AS
BEGIN
SELECT * FROM [dbo].[SampleMapping] WHERE InternalSampleId = @InternalSampleId
END
GO

IF OBJECT_ID('dbo.SampleMapping_Add', 'p') IS NULL
    EXEC ('CREATE PROCEDURE SampleMapping_Add as select 1')
GO
ALTER PROCEDURE SampleMapping_Add 
	@SampleId INT,
	@InternalSampleId INT,
	@OfferId uniqueidentifier
	
AS
BEGIN
INSERT INTO SampleMapping(SampleId, InternalSampleId,OfferId)
VALUES (@SampleId, @InternalSampleId,@OfferId)
END
GO


IF OBJECT_ID('dbo.SampleMapping_Delete', 'p') IS NULL
    EXEC ('CREATE PROCEDURE SampleMapping_Delete as select 1')
GO
ALTER PROCEDURE SampleMapping_Delete 
	@SampleId INT
AS
BEGIN
DELETE FROM SampleMapping
WHERE SampleId = @SampleId
END
GO

---End of stored procedures for QuotaExpressions table
---Adding CRUD and needed stored procedures for QuotaMapping table

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.QuotaMapping_GetAll', 'p') IS NULL
    EXEC ('CREATE PROCEDURE QuotaMapping_GetAll as select 1')
GO
ALTER PROCEDURE QuotaMapping_GetAll 
AS
BEGIN
SELECT * FROM [dbo].[QuotaMapping]
END
GO

IF OBJECT_ID('dbo.QuotaMapping_GetBySampleId', 'p') IS NULL
    EXEC ('CREATE PROCEDURE QuotaMapping_GetBySampleId as select 1')
GO
ALTER PROCEDURE QuotaMapping_GetBySampleId 
	@SampleId INT
AS
BEGIN
SELECT * FROM [dbo].[QuotaMapping] WHERE SampleId = @SampleId
END
GO

IF OBJECT_ID('dbo.QuotaMapping_GetByInternalQuotaId', 'p') IS NULL
    EXEC ('CREATE PROCEDURE QuotaMapping_GetByInternalQuotaId as select 1')
GO
ALTER PROCEDURE QuotaMapping_GetByInternalQuotaId 
	@InternalQuotaId INT
AS
BEGIN
SELECT * FROM [dbo].[QuotaMapping] WHERE InternalQuotaId = @InternalQuotaId
END
GO

IF OBJECT_ID('dbo.QuotaMapping_GetByQuotaId', 'p') IS NULL
    EXEC ('CREATE PROCEDURE QuotaMapping_GetByQuotaId as select 1')
GO
ALTER PROCEDURE QuotaMapping_GetByQuotaId 
	@QuotaId INT  
AS
BEGIN
SELECT * FROM [dbo].[QuotaMapping] WHERE QuotaId = @QuotaId
END
GO

IF OBJECT_ID('dbo.QuotaMapping_Add', 'p') IS NULL
    EXEC ('CREATE PROCEDURE QuotaMapping_Add as select 1')
GO
ALTER PROCEDURE QuotaMapping_Add 
	@QuotaId INT,
	@SampleId INT,
	@InternalQuotaId INT,
	@QuotaRemaining int,
	@OfferId uniqueidentifier
	
AS
BEGIN
INSERT INTO QuotaMapping(SampleId, QuotaId,InternalQuotaId,QuotaRemaining,OfferId)
VALUES (@SampleId, @QuotaId, @InternalQuotaId, @QuotaRemaining, @OfferId)
END
GO
IF OBJECT_ID('dbo.QuotaMapping_Update', 'p') IS NULL
    EXEC ('CREATE PROCEDURE QuotaMapping_Update as select 1')
GO
ALTER PROCEDURE QuotaMapping_Update 
	@QuotaId INT,
	@QuotaRemaining INT
	
AS
BEGIN
 UPDATE QuotaMapping
SET QuotaRemaining = ISNULL(@QuotaRemaining, QuotaRemaining)
WHERE QuotaId = @QuotaId
END
GO

IF OBJECT_ID('dbo.QuotaMapping_Delete', 'p') IS NULL
    EXEC ('CREATE PROCEDURE QuotaMapping_Delete as select 1')
GO
ALTER PROCEDURE QuotaMapping_Delete 
	@QuotaId INT
AS
BEGIN
DELETE FROM QuotaMapping
WHERE QuotaId = @QuotaId 
END
GO

IF OBJECT_ID('dbo.QuotaMapping_DeleteBySample', 'p') IS NULL
    EXEC ('CREATE PROCEDURE QuotaMapping_DeleteBySample as select 1')
GO
ALTER PROCEDURE QuotaMapping_DeleteBySample 
	@SampleId INT
AS
BEGIN
DELETE FROM QuotaMapping
WHERE SampleId = @SampleId
END
GO

---End of stored procedures for QuotaExpressions table

IF EXISTS (SELECT name FROM sys.objects
      WHERE name = 'tr_Offers_Active' AND type = 'TR')
   DROP TRIGGER tr_Offers_Active;
GO
Create TRIGGER tr_Offers_Active
   ON [dbo].[Offers] FOR UPDATE AS
  IF UPDATE(active)
BEGIN
    Delete from [dbo].[QuotaMapping] where [SampleId] in(SELECT inserted.SampleId FROM inserted where inserted.active <>1 );
	Delete from [dbo].[SampleMapping] where [SampleId] in(SELECT inserted.SampleId FROM inserted where inserted.active <>1);
	Delete from [dbo].[QuotaExpressions] where [SampleId] in(SELECT inserted.SampleId FROM inserted where inserted.active <>1);
	Update [dbo].[Offers] set [IsInitialized]=0 where [SampleId] in(SELECT inserted.SampleId FROM inserted where inserted.active <>1);
END
GO

-- -----------------------------------------------------------------------------------------------------------
-- Start of manage all the foreign key constraints to the Offers table
-- -----------------------------------------------------------------------------------------------------------
-- Creating foreign key on [OfferId] in table 'QuotaExpressions'
IF OBJECT_ID(N'[dbo].[FK_QuotaExpressions_OfferId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[QuotaExpressions] DROP CONSTRAINT [FK_QuotaExpressions_OfferId]
GO
ALTER TABLE [dbo].[QuotaExpressions] WITH CHECK ADD CONSTRAINT [FK_QuotaExpressions_OfferId] FOREIGN KEY([OfferId]) REFERENCES [dbo].[Offers] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[QuotaExpressions] CHECK CONSTRAINT [FK_QuotaExpressions_OfferId]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[QuotaExpressions]') AND name = N'IX_FK_QuotaExpressions_Offers_OfferId')
  CREATE INDEX [IX_FK_QuotaExpressions_Offers_OfferId] ON [dbo].[QuotaExpressions] ([OfferId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

-- Creating foreign key on [OfferId] in table 'SampleMapping'
IF OBJECT_ID(N'[dbo].[FK_SampleMapping_OfferId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SampleMapping] DROP CONSTRAINT [FK_SampleMapping_OfferId]
GO
ALTER TABLE [dbo].[SampleMapping] WITH CHECK ADD CONSTRAINT [FK_SampleMapping_OfferId] FOREIGN KEY([OfferId]) REFERENCES [dbo].[Offers] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[SampleMapping] CHECK CONSTRAINT [FK_SampleMapping_OfferId]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SampleMapping]') AND name = N'IX_FK_SampleMapping_Offers_OfferId')
  CREATE INDEX [IX_FK_SampleMapping_Offers_OfferId] ON [dbo].[SampleMapping] ([OfferId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

-- Creating foreign key on [OfferId] in table 'QuotaMapping'
IF OBJECT_ID(N'[dbo].[FK_QuotaMapping_OfferId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[QuotaMapping] DROP CONSTRAINT [FK_QuotaMapping_OfferId]
GO
ALTER TABLE [dbo].[QuotaMapping] WITH CHECK ADD CONSTRAINT [FK_QuotaMapping_OfferId] FOREIGN KEY([OfferId]) REFERENCES [dbo].[Offers] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[QuotaMapping] CHECK CONSTRAINT [FK_QuotaMapping_OfferId]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[QuotaMapping]') AND name = N'IX_FK_QuotaMapping_Offers_OfferId')
  CREATE INDEX [IX_FK_QuotaMapping_Offers_OfferId] ON [dbo].[QuotaMapping] ([OfferId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

-- Creating foreign key on [OfferId] in table 'RespondentAttributes'
IF OBJECT_ID(N'[dbo].[FK_Attributes_Offers]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RespondentAttributes] DROP CONSTRAINT [FK_Attributes_Offers]
GO
IF OBJECT_ID(N'[dbo].[FK_RespondentAttributes_OfferId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RespondentAttributes] DROP CONSTRAINT [FK_RespondentAttributes_OfferId]
GO
ALTER TABLE [dbo].[RespondentAttributes] WITH CHECK ADD CONSTRAINT [FK_RespondentAttributes_OfferId] FOREIGN KEY([OfferId]) REFERENCES [dbo].[Offers] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[RespondentAttributes] CHECK CONSTRAINT [FK_RespondentAttributes_OfferId]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[RespondentAttributes]') AND name = N'IX_FK_RespondentAttributes_Offers_OfferId')
  CREATE INDEX [IX_FK_RespondentAttributes_Offers_OfferId] ON [dbo].[RespondentAttributes] ([OfferId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

-- Creating foreign key on [OfferId] in table 'Terms'
IF OBJECT_ID(N'[dbo].[FK_dbo_Terms_dbo_Offers_OfferId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Terms] DROP CONSTRAINT [FK_dbo_Terms_dbo_Offers_OfferId]
GO
IF OBJECT_ID(N'[dbo].[FK_Terms_OfferId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Terms] DROP CONSTRAINT [FK_Terms_OfferId]
GO
ALTER TABLE [dbo].[Terms] WITH CHECK ADD CONSTRAINT [FK_Terms_OfferId] FOREIGN KEY([OfferId]) REFERENCES [dbo].[Offers] ([Id]) ON DELETE CASCADE ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[Terms] CHECK CONSTRAINT [FK_Terms_OfferId]
GO
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Terms]') AND name = N'IX_FK_dbo_Terms_dbo_Offers_OfferId')
  DROP INDEX [dbo].[Terms].IX_FK_dbo_Terms_dbo_Offers_OfferId
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Terms]') AND name = N'IX_FK_Terms_Offers_OfferId')
  CREATE INDEX [IX_FK_Terms_Offers_OfferId] ON [dbo].[Terms] ([OfferId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
-- -----------------------------------------------------------------------------------------------------------
-- End of manage all the foreign key constraints to the Offers table
-- -----------------------------------------------------------------------------------------------------------

