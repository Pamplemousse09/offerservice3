--USE [OffersII]
GO

IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'Active' AND OBJECT_ID = OBJECT_ID(N'Providers'))
BEGIN
ALTER TABLE [dbo].[Providers] ADD [Active] bit NOT NULL DEFAULT ((1))
END

IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'ByPass' AND OBJECT_ID = OBJECT_ID(N'Providers'))
BEGIN
ALTER TABLE [dbo].[Providers] ADD [ByPass] bit NOT NULL DEFAULT ((0))
END

ALTER TABLE [dbo].[Offers] ALTER COLUMN [StudyEndDate] DateTime NULL
