-- Define procedure to update offer status based on sample id only is status is changing
IF OBJECT_ID('dbo.Offer_UpdateStatus', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Offer_UpdateStatus as select 1')
GO
ALTER PROCEDURE Offer_UpdateStatus
	@SampleId INT,
	@Status INT 
AS
BEGIN
UPDATE Offers SET Status = @Status WHERE SampleId = @SampleId AND Status <> @Status
END
GO

-- Define procedure to update offer quota remaining based on sample id only if quota remaining is changing
IF OBJECT_ID('dbo.Offer_UpdateQuotaRemaining', 'p') IS NULL
    EXEC ('CREATE PROCEDURE Offer_UpdateQuotaRemaining as select 1')
GO
ALTER PROCEDURE Offer_UpdateQuotaRemaining
	@SampleId INT,
	@QuotaRemaining INT 
AS
BEGIN
UPDATE Offers SET QuotaRemaining = @QuotaRemaining WHERE SampleId = @SampleId AND QuotaRemaining <> @QuotaRemaining
END
GO
---End of stored procedures for Offers table


--- [DEVCME-599] Set 0 as default value for Status  and making the status column not nullable---
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Offers]') AND name = N'IX_Offers_Status')
	DROP INDEX [IX_Offers_Status] ON [dbo].[Offers]    
GO

UPDATE [Offers]
SET [Status] = 0 
WHERE [Status] IS NULL

ALTER TABLE [Offers]
ALTER COLUMN [Status] INTEGER NOT NULL

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Offers' AND COLUMN_NAME = 'Status' AND COLUMN_DEFAULT IS NULL)
BEGIN
ALTER TABLE [Offers]
ADD CONSTRAINT DF_Offer_Status DEFAULT 0 FOR [Status]
END 

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Offers]') AND name = N'IX_Offers_Status')
  CREATE INDEX [IX_Offers_Status] ON [dbo].[Offers] ([Status] ASC)
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO