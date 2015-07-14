--USE [OffersII]
GO

-- [DEVCME-262] Ability to Configure Test Studies in PROD 
IF NOT EXISTS(SELECT * FROM sys.columns WHERE Name = N'TestOffer' AND OBJECT_ID = OBJECT_ID(N'Offers'))
BEGIN
ALTER TABLE [dbo].Offers ADD [TestOffer] bit NOT NULL DEFAULT ((0))
END

-- [DEVCME-199] Admin should not be able to add more than one provider with the same APIuser
ALTER TABLE [dbo].Providers
ALTER COLUMN ApiUser NVARCHAR(450)

ALTER TABLE [dbo].Providers
ADD UNIQUE (ApiUser)