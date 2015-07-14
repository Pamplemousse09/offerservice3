---DEVCME-403 Changing the column name active to status on Offers
IF EXISTS(SELECT * FROM sys.COLUMNS WHERE Name='active' AND OBJECT_ID = OBJECT_ID('dbo.Offers'))
BEGIN
EXEC sp_RENAME 'dbo.Offers.active', 'Status', 'COLUMN'
END


---Renaming the stored procedure to follow the project's naming convention
IF OBJECT_ID ( 'dbo.SteamStudy_Add', 'P' ) IS NOT NULL 
EXEC SP_RENAME 'dbo.SteamStudy_Add', 'QuotaExpression_Add'
GO
IF OBJECT_ID ( 'dbo.SteamStudy_Delete', 'P' ) IS NOT NULL 
EXEC SP_RENAME 'dbo.SteamStudy_Delete', 'QuotaExpression_Delete'
GO
IF OBJECT_ID ( 'dbo.SteamStudy_GetAll', 'P' ) IS NOT NULL 
EXEC SP_RENAME 'dbo.SteamStudy_GetAll', 'QuotaExpression_GetAll'
GO
IF OBJECT_ID ( 'dbo.SteamStudy_GetBySampleId', 'P' ) IS NOT NULL 
EXEC SP_RENAME 'dbo.SteamStudy_GetBySampleId', 'QuotaExpression_GetBySampleId'
GO
IF OBJECT_ID ( 'dbo.SteamStudy_Update', 'P' ) IS NOT NULL 
EXEC SP_RENAME 'dbo.SteamStudy_Update', 'QuotaExpression_Update'
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
WHERE r.OfferId = @OfferId AND r.Ident = a.AttributeId AND r.Ident = att.Id
END
GO

IF OBJECT_ID ( 'dbo.SteamQuotaStudy_Update', 'P' ) IS NOT NULL 
EXEC SP_RENAME 'dbo.SteamQuotaStudy_Update', 'QuotaExpressionQuota_Update'
GO


-- -------------------------------------------------------------------------
-- Start of the sql changes to remove IsInitialized column from Offers table
-- -------------------------------------------------------------------------
-- Delete the constraint 
IF EXISTS(SELECT * FROM sys.COLUMNS WHERE Name='IsInitialized' AND OBJECT_ID = OBJECT_ID('dbo.Offers'))
BEGIN
DECLARE @ConstraintName nvarchar(200)
SELECT @ConstraintName = Name FROM SYS.DEFAULT_CONSTRAINTS
WHERE PARENT_OBJECT_ID = OBJECT_ID('Offers')
AND PARENT_COLUMN_ID = (SELECT column_id FROM sys.columns
                        WHERE NAME = N'IsInitialized'
                        AND object_id = OBJECT_ID(N'Offers'))
IF @ConstraintName IS NOT NULL
EXEC('ALTER TABLE Offers DROP CONSTRAINT ' + @ConstraintName)
END

-- Drop the column 
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=N'IsInitialized' AND TABLE_NAME = N'Offers')
BEGIN
ALTER TABLE dbo.Offers DROP COLUMN [IsInitialized]
END

-- drop the procedure used to update the column
IF OBJECT_ID('dbo.Offer_UpdateIsInitialized', 'p') IS NOT NULL
    EXEC ('DROP PROCEDURE Offer_UpdateIsInitialized')

-- Remove the IsInitialized dependency from the PROCEDURE 
IF OBJECT_ID('dbo.Offer_GetActiveSampleIds', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Offer_GetActiveSampleIds as select 1')
GO
ALTER PROCEDURE Offer_GetActiveSampleIds 
AS
BEGIN

EXEC('SELECT o.SampleId, o.Id
FROM Offers as o
WHERE o.Status = 1')

END
GO

-- Remove the IsInitialized from the Trigger
IF EXISTS (SELECT name FROM sys.objects
      WHERE name = 'tr_Offers_Active' AND type = 'TR')
   DROP TRIGGER tr_Offers_Active;
GO
IF EXISTS (SELECT name FROM sys.objects
      WHERE name = 'tr_Offers_Status' AND type = 'TR')
   DROP TRIGGER tr_Offers_Status;
GO
Create TRIGGER tr_Offers_Status
   ON [dbo].[Offers] FOR UPDATE AS
  IF UPDATE(Status)
BEGIN
    Delete from [dbo].[QuotaMapping] where [SampleId] in(SELECT inserted.SampleId FROM inserted where inserted.Status <>1 );
	Delete from [dbo].[SampleMapping] where [SampleId] in(SELECT inserted.SampleId FROM inserted where inserted.Status <>1);
	Delete from [dbo].[QuotaExpressions] where [SampleId] in(SELECT inserted.SampleId FROM inserted where inserted.Status <>1);
END
GO
-- -------------------------------------------------------------------------
-- End of the sql changes to remove IsInitialized column from Offers table
-- -------------------------------------------------------------------------

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Offers]') AND name = N'IX_Offers_Active')
  DROP INDEX [dbo].[Offers].[IX_Offers_Active]
GO
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Offers]') AND name = N'IX_Offers_Status')
  CREATE INDEX [IX_Offers_Status] ON [dbo].[Offers] ([Status] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Offers]') AND name = N'IX_Offers_SampleId')
  DROP INDEX [dbo].[Offers].[IX_Offers_SampleId];
GO
CREATE UNIQUE INDEX [IX_Offers_SampleId] ON [dbo].[Offers] ([SampleId] ASC)
  WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Offers]') AND name = N'IX_Offers_StudyId')
  CREATE INDEX [IX_Offers_StudyId] ON [dbo].[Offers] ([StudyId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Terms]') AND name = N'IX_Terms_Active')
  CREATE INDEX [IX_Terms_Active] ON [dbo].[Terms] ([Active] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[QuotaExpressions]') AND name = N'IX_QuotaExpressions_SampleId')
  CREATE INDEX [IX_QuotaExpressions_SampleId] ON [dbo].[QuotaExpressions] ([SampleId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SampleMapping]') AND name = N'IX_SampleMapping_SampleId')
  CREATE INDEX [IX_SampleMapping_SampleId] ON [dbo].[SampleMapping] ([SampleId] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


---[DEVCME-403]--------------------------------------------------------------------

--USE [OffersII]
GO

/****** Object:  Table [dbo].[Offers]    Script Date: 5/20/2015 12:37:40 PM ******/
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE COLUMN_NAME=N'RetryCount' AND TABLE_NAME = N'Offers')
BEGIN
ALTER TABLE dbo.Offers 
ADD [RetryCount] int not null DEFAULT(0)
END
GO

/****** Object:  StoredProcedure [dbo].[Offer_GetPendingSampleIdsAndRetryCount]    Script Date: 5/20/2015 12:37:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('Offer_GetPendingSampleIdsAndRetryCount','p') IS NULL
	EXEC ('CREATE PROCEDURE Offer_GetPendingSampleIdsAndRetryCount as select 1')
GO
ALTER PROCEDURE Offer_GetPendingSampleIdsAndRetryCount
AS
BEGIN

EXEC('SELECT o.SampleId, o.Id, o.RetryCount
FROM Offers as o
WHERE o.Status = 3')

END
GO

/****** Object:  StoredProcedure [dbo].[Offer_UpdateRetryCount]    Script Date: 5/20/2015 12:37:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('Offer_UpdateRetryCount','p') IS NULL
	EXEC ('CREATE PROCEDURE Offer_UpdateRetryCount as select 1')
GO
ALTER PROCEDURE Offer_UpdateRetryCount
	@SampleId int,
	@RetryCount int
AS
BEGIN

UPDATE Offers
SET RetryCount = ISNULL(@RetryCount, RetryCount)
WHERE SampleId = @SampleId
END

/****** Object:  StoredProcedure [dbo].[Offer_Update]    Script Date: 5/20/2015 1:58:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.Offer_Update','p') IS NULL
	EXEC ('CREATE PROCEDURE Offer_Update as select 1')
GO
ALTER PROCEDURE Offer_Update
	@Id UNIQUEIDENTIFIER,
	@StudyId INT = null,
	@SampleId INT = null, 
	@LOI INT = null, 
	@IR REAL = null, 
	@Status INT = null, 
	@Test NVARCHAR(max) = null,
	@Description NVARCHAR(MAX) = null,
	@Title NVARCHAR(256) = null,
	@Topic NVARCHAR(256) = null,
	@OfferLink NVARCHAR(512) = null,
	@QuotaRemaining INT = null,
	@StudyStartDate DATETIME = null,
	@StudyEndDate DATETIME = null,
	@RetryCount INT = null
AS
BEGIN

UPDATE Offers
SET StudyId = ISNULL(@StudyId, StudyId),
	SampleId = ISNULL(@SampleId, SampleId),
	LOI = ISNULL(@LOI, LOI),
	IR = ISNULL(@IR, IR),
	Status = ISNULL(@Status, Status),
	Description = ISNULL(@Description, Description),
	Title = ISNULL(@Title, Title),
	Topic = ISNULL(@Topic, Topic),
	OfferLink = ISNULL(@OfferLink, OfferLink),
	QuotaRemaining = ISNULL(@QuotaRemaining, QuotaRemaining),
	StudyStartDate = ISNULL(@StudyStartDate, StudyStartDate),
	StudyEndDate = ISNULL(@StudyEndDate, StudyEndDate),
	TestOffer = ISNULL(@Test, TestOffer),
	RetryCount = ISNULL(@RetryCount, RetryCount)
WHERE Id = @Id
END
GO

--USE [OffersII]
GO
/****** Object:  StoredProcedure [dbo].[Offer_GetFilteredOffers]    Script Date: 5/25/2015 5:36:39 PM ******/
IF OBJECT_ID('dbo.Offer_GetFilteredOffers','p') IS NULL
	EXEC ('CREATE PROCEDURE Offer_GetFilteredOffers as select 1')
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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
		IF @OfferStatus <> 4
		BEGIN
			SET @Conditions = @Conditions + ' AND o.Status = ' + cast(@OfferStatus as varchar(10))
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
			CASE WHEN (o.Status IS NULL) THEN ''0'' ELSE o.Status END as status
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
			CASE WHEN (o.Status IS NULL) THEN 0 ELSE o.Status END as Status,
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

IF OBJECT_ID('dbo.Offer_Add', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Offer_Add as select 1')
go
ALTER PROCEDURE Offer_Add 
	@StudyId INT,
	@SampleId INT, 
	@LOI INT, 
	@IR REAL, 
	@Status INT = null, 
	@Description NVARCHAR(MAX) = null,
	@Title NVARCHAR(256) = null,
	@Topic NVARCHAR(256) = null,
	@OfferLink NVARCHAR(512) = null,
	@QuotaRemaining INT,
	@StudyStartDate DATETIME,
	@StudyEndDate DATETIME = null
AS
BEGIN
INSERT INTO Offers (StudyId, SampleId, LOI, IR, Status, Description, Title, Topic, OfferLink, QuotaRemaining, StudyStartDate, StudyEndDate)
VALUES (@StudyId, @SampleId, @LOI, @IR, @Status, @Description, @Title, @Topic, @OfferLink, @QuotaRemaining, @StudyStartDate, @StudyEndDate)
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
WHERE o.OfferLink IS NOT NULL AND o.Status = 1 AND
((@OfferType IS NULL) OR (o.TestOffer = @OfferType))

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
AND o.Status = 1'

EXEC(@SQL)
END
GO


IF OBJECT_ID('dbo.Offer_GetActiveSampleIds', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Offer_GetActiveSampleIds as select 1')
GO
ALTER PROCEDURE Offer_GetActiveSampleIds 
AS
BEGIN

EXEC('SELECT o.SampleId, o.Id
FROM Offers as o
WHERE o.Status = 1')

END
GO

-- DROP the procedures created with the earlier versions of this file possibly exist on developers DB
IF OBJECT_ID('dbo.Offer_Suspend', 'p') IS NOT NULL
    EXEC ('DROP PROCEDURE Offer_Suspend')
IF OBJECT_ID('dbo.Offer_Activate', 'p') IS NOT NULL
    EXEC ('DROP PROCEDURE Offer_Activate')

-- Define procedure to update offer status based on sample id
IF OBJECT_ID('dbo.Offer_UpdateStatus', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Offer_UpdateStatus as select 1')
GO
ALTER PROCEDURE Offer_UpdateStatus
	@SampleId INT,
	@Status INT 
AS
BEGIN
UPDATE Offers SET Status = @Status WHERE SampleId = @SampleId
END
GO

-- Define procedure to update offer quota remaining based on sample id
IF OBJECT_ID('dbo.Offer_UpdateQuotaRemaining', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Offer_UpdateQuotaRemaining as select 1')
GO
ALTER PROCEDURE Offer_UpdateQuotaRemaining
	@SampleId INT,
	@QuotaRemaining INT 
AS
BEGIN
UPDATE Offers SET QuotaRemaining = @QuotaRemaining WHERE SampleId = @SampleId
END
GO
---End of stored procedures for Offers table
