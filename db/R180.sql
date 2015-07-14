--USE [OffersII];

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO

-- --------------------------------------------------
-- Dropping table Targets if exists
-- --------------------------------------------------
IF OBJECT_ID('[dbo].[Targets]', 'U') IS NOT NULL
  DROP TABLE [dbo].[Targets]
  
-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------
IF OBJECT_ID(N'[dbo].[FK_RequiredAttributes_AttributeId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RequiredAttributes] DROP CONSTRAINT [FK_RequiredAttributes_AttributeId];
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='RequiredAttributes' AND COLUMN_NAME ='AttributeId')
    ALTER TABLE [dbo].[RequiredAttributes] ALTER COLUMN AttributeId nvarchar(50) not null;
	
Go
-- --------------------------------------------------
-- Add FOREIGN KEY constraints for RequiredAttributes
-- --------------------------------------------------
GO
ALTER TABLE [dbo].[RequiredAttributes] WITH CHECK ADD CONSTRAINT [FK_RequiredAttributes_AttributeId] FOREIGN KEY([AttributeId]) REFERENCES [dbo].[Attributes] ([Id]) ON UPDATE CASCADE
ALTER TABLE [dbo].[RequiredAttributes] CHECK CONSTRAINT [FK_RequiredAttributes_AttributeId]
GO

-- ----------------------------------------------------------------------------------------------------------
-- Change the startdate and enddate to datetime type, but first drop the constraints so column can be altered
-- ----------------------------------------------------------------------------------------------------------
update [dbo].[Offers] set StudyStartDate = GETDATE() where StudyStartDate is null or StudyStartDate = '';
update [dbo].[Offers] set StudyEndDate = DATEADD(Year, 1, GETDATE()) where StudyEndDate is null or StudyEndDate = '' or convert(varchar,StudyEndDate) = '0001-01-01T00:00:00';

declare @table_name nvarchar(256)
declare @col_name nvarchar(256)
declare @Command  nvarchar(100);

set @table_name = N'Offers'
set @col_name = N'StudyStartDate'

select @Command = 'ALTER TABLE ' + @table_name + ' drop constraint ' + d.name
 from sys.tables t   
  join sys.default_constraints d on d.parent_object_id = t.object_id  
  join sys.columns c on c.object_id = t.object_id and c.column_id = d.parent_column_id
 where t.name = @table_name
  and c.name = @col_name
execute (@Command)

set @col_name = N'StudyEndDate'

select @Command = 'ALTER TABLE ' + @table_name + ' drop constraint ' + d.name
 from sys.tables t   
  join sys.default_constraints d on d.parent_object_id = t.object_id  
  join sys.columns c on c.object_id = t.object_id and c.column_id = d.parent_column_id
 where t.name = @table_name
  and c.name = @col_name
execute (@Command)

ALTER TABLE [dbo].[Offers] ALTER COLUMN StudyStartDate datetime not null;
ALTER TABLE [dbo].[Offers] ADD CONSTRAINT DF_Offers_StudyStartDate DEFAULT GETDATE() FOR StudyStartDate;

ALTER TABLE [dbo].[Offers] ALTER COLUMN StudyEndDate datetime not null;
ALTER TABLE [dbo].[Offers] ADD CONSTRAINT DF_Offers_StudyEndDate DEFAULT GETDATE() FOR StudyEndDate;
GO


GO

IF OBJECT_ID('dbo.GetFilteredAttributes', 'p') IS NULL
    EXEC ('CREATE PROCEDURE GetFilteredAttributes AS SELECT 1')
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[GetFilteredAttributes]
	-- Add the parameters for the stored procedure here
	@AttributeId NVARCHAR(50) = null, 
	@Status BIT = NULL,
	@Page INT = 0,
	@RecordsPerPage INT = 40
AS
BEGIN
	DECLARE @Sql NVARCHAR(MAX)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

	DECLARE @Conditions NVARCHAR(MAX) = ''

	IF @AttributeId <> ''
		SET @Conditions = 'AND a.Id LIKE ''%' + @AttributeId + '%'' '	

	IF @Status IS NOT NULL
		SET @Conditions = @Conditions + 'AND a.Publish = ' + cast(@Status as char(1)) + ' '

	DECLARE @RowCount NVARCHAR(MAX) =
	        '
			DECLARE @TotalRows INT 
			
		SELECT COUNT(1) AS TotalRows INTO #Temp_count
		FROM (SELECT ROW_NUMBER() OVER (ORDER BY Id) AS RowNum, 
			@TotalRows as TotalRows,
			a.Id as attributeId,
			a.Label as label,
			a.Publish as status
			FROM dbo.Attributes a WHERE 1 = 1 ' + @Conditions + ') AS PaginatedAttributes
			WHERE RowNum > ' + cast((@Page * @RecordsPerPage) as varchar(20)) + '
			AND RowNum <= ' + cast((@Page * @RecordsPerPage + @RecordsPerPage) as varchar(20)) + '
			SET @TotalRows = (SELECT * FROM #Temp_count)'

		DECLARE @FromRowCount NVARCHAR(MAX) =
	        '
			DECLARE @FromTotalRows INT 
			
		SELECT COUNT(1) AS FromTotalRows INTO #Temp_total_count
		FROM dbo.Attributes a WHERE 1 = 1 ' + @Conditions + '
			SET @FromTotalRows = (SELECT * FROM #Temp_total_count)'


	SET @Sql = '
		SELECT *
		FROM (SELECT ROW_NUMBER() OVER (ORDER BY A.Id) AS RowNum, 
			A.[Id] as attributeId,
			A.[Label] as label,
			A.[Publish] as status,
			@TotalRows as TotalRows,
			@FromTotalRows as FromTotalRows,
			CASE WHEN (R.[Ident] IS NULL) THEN 0 ELSE 1 END AS used 
			FROM [dbo].[Attributes] A
			LEFT JOIN [dbo].[RespondentAttributes] R ON (A.[Id] = R.[Ident])
	'

	SET @Sql = @Sql + 'WHERE 1 = 1 '

	
	SET @Sql = @Sql + @Conditions + ' GROUP BY A.[Id], A.[Label], A.[Publish], R.[Ident]) AS PaginatedAttributes
			WHERE RowNum > ' + cast((@Page * @RecordsPerPage) as varchar(20)) + '
			AND RowNum <= ' + cast((@Page * @RecordsPerPage + @RecordsPerPage) as varchar(20)) + '
			ORDER BY RowNum
			'; 
	print(@Sql)
	EXEC( @RowCount + @FromRowCount + @Sql )
END



-- ----------------------------------------------------------------
-- Add the Id column to RespondentAttribute to allow for REST API
-- ----------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[RespondentAttributes]') AND name = N'IX_RespondentAttributes_OfferId_Ident')
DROP INDEX [dbo].[RespondentAttributes].IX_RespondentAttributes_OfferId_Ident
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[RespondentAttributes]') AND name = N'PK_RespondentAttributes')
ALTER TABLE [dbo].[RespondentAttributes] DROP CONSTRAINT [PK_RespondentAttributes]
GO

IF EXISTS(SELECT * from sys.columns WHERE Name = N'Id' and Object_ID = Object_ID(N'RespondentAttributes'))
BEGIN
Alter Table [dbo].[RespondentAttributes] DROP COLUMN [Id]
END

Alter Table [dbo].[RespondentAttributes] Add [Id] [int] IDENTITY(1,1) NOT NULL
Go

ALTER TABLE [dbo].[RespondentAttributes] ADD CONSTRAINT PK_RespondentAttributes  PRIMARY KEY ([Id])
Go

IF  NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[RespondentAttributes]') AND name = N'IX_RespondentAttributes_OfferId_Ident')
CREATE UNIQUE INDEX IX_RespondentAttributes_OfferId_Ident ON [dbo].[RespondentAttributes] (OfferId,Ident)
GO

ALTER TABLE [dbo].[Offers] ALTER COLUMN IR real NOT NULL

DELETE [dbo].[RespondentAttributes] WHERE [Values] IS NULL OR [Values] = '';

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[MainstreamSamples]') AND name = N'PK_MainstreamSamples')
  ALTER TABLE [dbo].[MainstreamSamples] DROP CONSTRAINT [PK_MainstreamSamples]

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[MainstreamSamples]') AND name = N'PK_MainstreamSample')
  ALTER TABLE [dbo].[MainstreamSamples] DROP CONSTRAINT [PK_MainstreamSample]

ALTER TABLE [dbo].[MainstreamSamples] ADD CONSTRAINT [PK_MainstreamSamples] PRIMARY KEY ([OfferId]);

GO
