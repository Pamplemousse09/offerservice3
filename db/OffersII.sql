
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 10/27/2014 09:20:09
-- Generated from EDMX file: C:\Source\BEL\KikaiVS2014\Kikai\OfferDbModel.edmx
-- --------------------------------------------------

USE master;
GO
IF DB_ID (N'OffersII') IS NULL
	create database OffersII;

SET QUOTED_IDENTIFIER OFF;
GO
USE OffersII;
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO


-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_MainstreamSample_Offers]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MainstreamSamples] DROP CONSTRAINT [FK_MainstreamSample_Offers];
GO
IF OBJECT_ID(N'[dbo].[FK_Attributes_Offers]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RespondentAttributes] DROP CONSTRAINT [FK_Attributes_Offers];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_Targets_dbo_Offers_OfferId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Targets] DROP CONSTRAINT [FK_dbo_Targets_dbo_Offers_OfferId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_Terms_dbo_Offers_OfferId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Terms] DROP CONSTRAINT [FK_dbo_Terms_dbo_Offers_OfferId];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Providers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Providers];
GO
IF OBJECT_ID(N'[dbo].[MainstreamSamples]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MainstreamSamples];
GO
IF OBJECT_ID(N'[dbo].[Offers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Offers];
GO
IF OBJECT_ID(N'[dbo].[Targets]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Targets];
GO
IF OBJECT_ID(N'[dbo].[Terms]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Terms];
GO
IF OBJECT_ID(N'[dbo].[RespondentAttributes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RespondentAttributes];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'MainstreamSamples'
CREATE TABLE [dbo].[MainstreamSamples] (
    [OfferId] uniqueidentifier  NOT NULL,
    [StudyId] int  NOT NULL,
    [SampleId] int  NOT NULL
);
GO

-- Creating table 'Offers'
CREATE TABLE [dbo].[Offers] (
    [Id] uniqueidentifier  NOT NULL,
    [LOI] int  NOT NULL,
    [IR] int  NOT NULL,
    [active] bit  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [Title] nvarchar(256)  NULL,
    [Topic] nvarchar(256)  NULL,
    [OfferLink] nvarchar(512)  NULL
);
GO

-- Creating table 'Targets'
CREATE TABLE [dbo].[Targets] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Age] nvarchar(max)  NULL,
    [Gender] nvarchar(max)  NULL,
    [Country] nvarchar(max)  NULL,
    [Language] nvarchar(max)  NULL,
    [OfferId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'Terms'
CREATE TABLE [dbo].[Terms] (
    [Id] uniqueidentifier  NOT NULL,
    [CPI] real  NOT NULL,
    [active] bit  NOT NULL,
    [Start] datetime  NOT NULL,
    [Expiration] datetime  NOT NULL,
    [OfferId] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'RespondentAttributes'
CREATE TABLE [dbo].[RespondentAttributes] (
    [OfferId] uniqueidentifier  NOT NULL,
    [Ident] nvarchar(50)  NOT NULL,
    [Values] nvarchar(max)  NULL
);
GO

-- Creating table 'Providers'
CREATE TABLE [dbo].[Providers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ApiUser] nvarchar(max)  NOT NULL,
    [LinkId] nvarchar(max)  NOT NULL
);
GO
SET QUOTED_IDENTIFIER ON;
-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [OfferId], [StudyId], [SampleId] in table 'MainstreamSamples'
ALTER TABLE [dbo].[MainstreamSamples]
ADD CONSTRAINT [PK_MainstreamSamples]
    PRIMARY KEY CLUSTERED ([OfferId], [StudyId], [SampleId] ASC);
GO

-- Creating primary key on [Id] in table 'Offers'
ALTER TABLE [dbo].[Offers]
ADD CONSTRAINT [PK_Offers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Targets'
ALTER TABLE [dbo].[Targets]
ADD CONSTRAINT [PK_Targets]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Terms'
ALTER TABLE [dbo].[Terms]
ADD CONSTRAINT [PK_Terms]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [OfferId], [Ident] in table 'RespondentAttributes'
ALTER TABLE [dbo].[RespondentAttributes]
ADD CONSTRAINT [PK_RespondentAttributes]
    PRIMARY KEY CLUSTERED ([OfferId], [Ident] ASC);
GO

-- Creating primary key on [Id] in table 'Providers'
ALTER TABLE [dbo].[Providers]
ADD CONSTRAINT [PK_Providers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [OfferId] in table 'MainstreamSamples'
ALTER TABLE [dbo].[MainstreamSamples]
ADD CONSTRAINT [FK_MainstreamSample_Offers]
    FOREIGN KEY ([OfferId])
    REFERENCES [dbo].[Offers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [OfferId] in table 'RespondentAttributes'
ALTER TABLE [dbo].[RespondentAttributes]
ADD CONSTRAINT [FK_Attributes_Offers]
    FOREIGN KEY ([OfferId])
    REFERENCES [dbo].[Offers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [OfferId] in table 'Targets'
ALTER TABLE [dbo].[Targets]
ADD CONSTRAINT [FK_dbo_Targets_dbo_Offers_OfferId]
    FOREIGN KEY ([OfferId])
    REFERENCES [dbo].[Offers]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_Targets_dbo_Offers_OfferId'
CREATE INDEX [IX_FK_dbo_Targets_dbo_Offers_OfferId]
ON [dbo].[Targets]
    ([OfferId]);
GO

-- Creating foreign key on [OfferId] in table 'Terms'
ALTER TABLE [dbo].[Terms]
ADD CONSTRAINT [FK_dbo_Terms_dbo_Offers_OfferId]
    FOREIGN KEY ([OfferId])
    REFERENCES [dbo].[Offers]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_Terms_dbo_Offers_OfferId'
CREATE INDEX [IX_FK_dbo_Terms_dbo_Offers_OfferId]
ON [dbo].[Terms]
    ([OfferId]);
GO

SET IDENTITY_INSERT [dbo].[Providers] ON;
insert into Providers ([Id],[Name],[ApiUser],[LinkId]) values('1','DevTestProvider','4b3a893ae1c35','QAhash170');
insert into Providers ([Id],[Name],[ApiUser],[LinkId]) values('2','DevBeldevProvider','Beldev','QAAutoTPLM1');
insert into Providers ([Id],[Name],[ApiUser],[LinkId]) values('3','IntBeldevProvider','BeldevINT','SomethingElse');
SET IDENTITY_INSERT [dbo].[Providers] OFF;
-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------