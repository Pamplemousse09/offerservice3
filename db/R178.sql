--USE [OffersII]
GO

-- --------------------------------------------------------------------------------------------------
-- DEVCME-22 Add default value for Offers.Id column
-- --------------------------------------------------------------------------------------------------
IF not EXISTS(
SELECT object_definition(default_object_id) AS definition
FROM   sys.columns
WHERE  name      ='Id'
AND    object_id = object_id('Offers') AND object_definition(default_object_id) = '(newid())'
)
Alter table [dbo].[Offers] ADD CONSTRAINT Offers_Id DEFAULT (newid()) FOR [Id]
GO

-- --------------------------------------------------------------------------------------------------
-- DEVCME-22 Add QuotaRemaining column to Offers table
-- --------------------------------------------------------------------------------------------------
IF  Not  EXISTS(
SELECT  *  FROM  INFORMATION_SCHEMA.COLUMNS
WHERE  TABLE_NAME='Offers'
AND  COLUMN_NAME  ='QuotaRemaining')
ALTER  TABLE  [dbo].[Offers]
ADD   [QuotaRemaining]  int   NOT  NULL  DEFAULT  0
GO

