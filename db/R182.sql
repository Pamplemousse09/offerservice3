--USE [OffersII]
GO

IF EXISTS(SELECT * FROM sys.COLUMNS WHERE Name = 'LinkId' AND Object_ID = Object_ID('dbo.Providers')) 
BEGIN
EXEC sp_RENAME 'dbo.Providers.LinkId', 'ProviderCode', 'COLUMN'
END

IF NOT EXISTS(SELECT * FROM sys.COLUMNS WHERE Name='Created_By' AND OBJECT_ID = OBJECT_ID('dbo.Terms'))
BEGIN
ALTER TABLE dbo.Terms ADD Created_By NVARCHAR (50) NOT NULL DEFAULT ('System')
END

IF NOT EXISTS(SELECT * FROM sys.COLUMNS WHERE Name='Last_Updated_By' AND OBJECT_ID = OBJECT_ID('dbo.Terms'))
BEGIN
ALTER TABLE dbo.Terms ADD Last_Updated_By NVARCHAR (50) NOT NULL DEFAULT ('System')
END

--DEVCME-156 Start
IF EXISTS(SELECT * FROM sys.COLUMNS WHERE Name='Attribute_Id' AND OBJECT_ID = OBJECT_ID('dbo.AttributeSettings'))
BEGIN
EXEC sp_RENAME 'dbo.AttributeSettings.Attribute_Id', 'AttributeId', 'COLUMN'
END

IF NOT EXISTS (SELECT * FROM sys.objects 
WHERE object_id = OBJECT_ID(N'[dbo].AttributeSettings') AND type in (N'U'))
BEGIN

CREATE TABLE [dbo].[AttributeSettings](
	[AttributeId] [nvarchar](50) NOT NULL,
	[Creation_Date] [datetime] NOT NULL DEFAULT(GETDATE()),
	[Created_By] [nvarchar](50) NULL DEFAULT('System'),
	[Update_Date] [datetime] NOT NULL DEFAULT(GETDATE()),
	[Last_Updated_By] [nvarchar](50) NULL DEFAULT('System'),
	[Publish] [bit] NOT NULL DEFAULT((0)),
 CONSTRAINT [PK_AttributeSettings] PRIMARY KEY CLUSTERED 
(
	[AttributeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
CREATE NONCLUSTERED INDEX [IX_Attributes_Publish] ON [dbo].[AttributeSettings]
(
	[Publish] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

END

IF EXISTS(SELECT * FROM sys.COLUMNS WHERE Name='Publish' AND OBJECT_ID = OBJECT_ID('dbo.Attributes'))
BEGIN
EXEC('INSERT INTO dbo.AttributeSettings (AttributeId, Publish)
SELECT a.Id, a.Publish
FROM dbo.Attributes a')
DECLARE @ConstraintName nvarchar(200)
SELECT @ConstraintName = Name FROM SYS.DEFAULT_CONSTRAINTS
WHERE PARENT_OBJECT_ID = OBJECT_ID('Attributes')
AND PARENT_COLUMN_ID = (SELECT column_id FROM sys.columns
                        WHERE NAME = N'Publish'
                        AND object_id = OBJECT_ID(N'Attributes'))
IF @ConstraintName IS NOT NULL
EXEC('ALTER TABLE Attributes DROP CONSTRAINT ' + @ConstraintName)
DROP INDEX [IX_Attributes_Publish] ON [dbo].[Attributes]
ALTER TABLE dbo.Attributes DROP COLUMN Publish
END




/****** Object:  StoredProcedure [dbo].[GetFilteredAttributes]    Script Date: 4/20/2015 3:01:18 PM ******/
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
	DECLARE @Publish NVARCHAR(MAX)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

	DECLARE @Conditions NVARCHAR(MAX) = ''

	IF @AttributeId <> ''
		SET @Conditions = 'AND a.Id LIKE ''%' + @AttributeId + '%'' '	
	IF @Status IS NOT NULL
	BEGIN
		IF cast(@Status as char(1))=0
		BEGIN
			SET @Conditions = @Conditions + 'AND (S.[Publish] = '+ cast(@Status as char(1))+' OR S.[Publish] is NULL)'
		END
		ELSE
			SET @Conditions = @Conditions + 'AND S.[Publish] = '+ cast(@Status as char(1))+''
	END


	DECLARE @RowCount NVARCHAR(MAX) =
	        '
			DECLARE @TotalRows INT 
			
		SELECT COUNT(1) AS TotalRows INTO #Temp_count
		FROM (SELECT ROW_NUMBER() OVER (ORDER BY Id) AS RowNum, 
			@TotalRows as TotalRows,
			a.Id as attributeId,
			a.Label as label,
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
			A.[Id] as attributeId,
			A.[Label] as label,
			CASE WHEN (S.[Publish] IS NULL) THEN ''False'' ELSE S.[Publish] END as status,
			@TotalRows as TotalRows,
			@FromTotalRows as FromTotalRows,
			CASE WHEN (R.[Ident] IS NULL) THEN 0 ELSE 1 END AS used ,
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

--End DEVCME-156