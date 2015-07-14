-- --------------------------------------------------------------------------------------
-- start of drop of all hangfire tables
-- --------------------------------------------------------------------------------------
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HangFire].[AggregatedCounter]') AND type in (N'U'))
DROP TABLE [HangFire].[AggregatedCounter]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HangFire].[Counter]') AND type in (N'U'))
DROP TABLE [HangFire].[Counter]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HangFire].[Hash]') AND type in (N'U'))
DROP TABLE [HangFire].[Hash]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[HangFire].[FK_HangFire_JobParameter_Job]') AND parent_object_id = OBJECT_ID(N'[HangFire].[JobParameter]'))
ALTER TABLE [HangFire].[JobParameter] DROP CONSTRAINT [FK_HangFire_JobParameter_Job]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HangFire].[JobParameter]') AND type in (N'U'))
DROP TABLE [HangFire].[JobParameter]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HangFire].[JobQueue]') AND type in (N'U'))
DROP TABLE [HangFire].[JobQueue]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HangFire].[List]') AND type in (N'U'))
DROP TABLE [HangFire].[List]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HangFire].[Schema]') AND type in (N'U'))
DROP TABLE [HangFire].[Schema]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HangFire].[Schema]') AND type in (N'U'))
DROP TABLE [HangFire].[Schema]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HangFire].[Server]') AND type in (N'U'))
DROP TABLE [HangFire].[Server]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HangFire].[Set]') AND type in (N'U'))
DROP TABLE [HangFire].[Set]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[HangFire].[FK_HangFire_State_Job]') AND parent_object_id = OBJECT_ID(N'[HangFire].[State]'))
ALTER TABLE [HangFire].[State] DROP CONSTRAINT [FK_HangFire_State_Job]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HangFire].[State]') AND type in (N'U'))
DROP TABLE [HangFire].[State]
GO

/****** Object:  Table [HangFire].[Job]    Script Date: 06/02/2015 15:21:25 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[HangFire].[Job]') AND type in (N'U'))
DROP TABLE [HangFire].[Job]
GO
-- --------------------------------------------------------------------------------------
-- End of drop of all hangfire tables
-- --------------------------------------------------------------------------------------


-- --------------------------------------------------------------------------------------
-- Updating previous database rows to have the default title
-- --------------------------------------------------------------------------------------
UPDATE Offers SET Title =('Survey - ' + CAST(SampleId AS VARCHAR)) WHERE Id IN (SELECT Id FROM Offers WHERE Title is NULL OR LTRIM(RTRIM(Title)) = '')

-- --------------------------------------------------------------------------------------
-- Add trigger on QuotaExpressions table to update Update_Date 
-- --------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE [type] = 'TR' AND [name] = 'trg_UpdateModified_QuotaExpressions')
BEGIN
DROP TRIGGER [dbo].[trg_UpdateModified_QuotaExpressions]
END

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[trg_UpdateModified_QuotaExpressions]
ON [dbo].[QuotaExpressions]
AFTER UPDATE
AS
    UPDATE t
            SET t.[Update_Date] = GETDATE()
            FROM [dbo].QuotaExpressions AS t 
            INNER JOIN inserted AS i 
            ON t.[SampleId] = i.[SampleId] ;
GO

-- --------------------------------------------------------------------------------------
-- Add trigger on QuotaMapping table to update Update_Date 
-- --------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE [type] = 'TR' AND [name] = 'trg_UpdateModified_QuotaMapping')
BEGIN
DROP TRIGGER [dbo].[trg_UpdateModified_QuotaMapping]
END

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[trg_UpdateModified_QuotaMapping]
ON [dbo].[QuotaMapping]
AFTER UPDATE
AS
    UPDATE t
            SET t.[Update_Date] = GETDATE()
            FROM [dbo].QuotaMapping AS t 
            INNER JOIN inserted AS i 
            ON t.[SampleId] = i.[SampleId] ;
GO

-- --------------------------------------------------------------------------------------
-- Add trigger on SampleMapping table to update Update_Date 
-- --------------------------------------------------------------------------------------
IF EXISTS (SELECT * FROM sys.objects WHERE [type] = 'TR' AND [name] = 'trg_UpdateModified_SampleMapping')
BEGIN
DROP TRIGGER [dbo].[trg_UpdateModified_SampleMapping]
END

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[trg_UpdateModified_SampleMapping]
ON [dbo].[SampleMapping]
AFTER UPDATE
AS
    UPDATE t
            SET t.[Update_Date] = GETDATE()
            FROM [dbo].SampleMapping AS t 
            INNER JOIN inserted AS i 
            ON t.[SampleId] = i.[SampleId] ;
GO

---[DEVCME-264]--------------------------------------------------------------------------
--USE [OffersII]
GO
/****** Object:  StoredProcedure [dbo].[Offer_GetSuspendedSampleIdsAndDates]    Script Date: 5/20/2015 12:37:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('Offer_GetSuspendedSampleIdsAndDates','p') IS NULL
	EXEC ('CREATE PROCEDURE Offer_GetSuspendedSampleIdsAndDates as select 1')
GO
ALTER PROCEDURE [dbo].[Offer_GetSuspendedSampleIdsAndDates] 
AS
BEGIN

EXEC('SELECT o.SampleId, o.Id, o.StudyStartDate, o.StudyEndDate
FROM Offers as o
WHERE o.Status = 2')

END
GO

-----------------------------------------------------------------------------------------
