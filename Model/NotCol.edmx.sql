
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 06/02/2015 15:00:05
-- Generated from EDMX file: C:\Users\apurva.jalit\Workspace\Notocol\Model\NotCol.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [nileshgarg.com];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[nileshgsa].[FK_Annotaion_Source]', 'F') IS NOT NULL
    ALTER TABLE [nileshgsa].[Annotation] DROP CONSTRAINT [FK_Annotaion_Source];
GO
IF OBJECT_ID(N'[nileshgsa].[FK_Source_User]', 'F') IS NOT NULL
    ALTER TABLE [nileshgsa].[Source] DROP CONSTRAINT [FK_Source_User];
GO
IF OBJECT_ID(N'[nileshgsa].[FK_SourceTags_Source]', 'F') IS NOT NULL
    ALTER TABLE [nileshgsa].[SourceTag] DROP CONSTRAINT [FK_SourceTags_Source];
GO
IF OBJECT_ID(N'[nileshgsa].[FK_SourceTags_Tag]', 'F') IS NOT NULL
    ALTER TABLE [nileshgsa].[SourceTag] DROP CONSTRAINT [FK_SourceTags_Tag];
GO
IF OBJECT_ID(N'[nileshgsa].[FK_Tag_User]', 'F') IS NOT NULL
    ALTER TABLE [nileshgsa].[Tag] DROP CONSTRAINT [FK_Tag_User];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[nileshgsa].[Annotation]', 'U') IS NOT NULL
    DROP TABLE [nileshgsa].[Annotation];
GO
IF OBJECT_ID(N'[nileshgsa].[Source]', 'U') IS NOT NULL
    DROP TABLE [nileshgsa].[Source];
GO
IF OBJECT_ID(N'[nileshgsa].[SourceTag]', 'U') IS NOT NULL
    DROP TABLE [nileshgsa].[SourceTag];
GO
IF OBJECT_ID(N'[nileshgsa].[Tag]', 'U') IS NOT NULL
    DROP TABLE [nileshgsa].[Tag];
GO
IF OBJECT_ID(N'[nileshgsa].[User]', 'U') IS NOT NULL
    DROP TABLE [nileshgsa].[User];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'SourceTags'
CREATE TABLE [dbo].[SourceTags] (
    [ID] bigint IDENTITY(1,1) NOT NULL,
    [SourceID] bigint  NOT NULL,
    [TagsID] bigint  NOT NULL
);
GO

-- Creating table 'Tags'
CREATE TABLE [dbo].[Tags] (
    [ID] bigint IDENTITY(1,1) NOT NULL,
    [Name] varchar(50)  NOT NULL,
    [ParentID] bigint  NOT NULL,
    [UserID] bigint  NOT NULL,
    [Description] nvarchar(500)  NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [ID] bigint IDENTITY(1,1) NOT NULL,
    [Username] varchar(200)  NOT NULL,
    [Password] varchar(50)  NOT NULL,
    [Identifier] varchar(500)  NULL,
    [ModifiedAt] datetime  NULL,
    [Email] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Sources'
CREATE TABLE [dbo].[Sources] (
    [ID] bigint IDENTITY(1,1) NOT NULL,
    [UserID] bigint  NOT NULL,
    [Title] varchar(500)  NULL,
    [Link] nvarchar(1000)  NULL,
    [Summary] nvarchar(2000)  NULL,
    [ReadLater] bit  NULL,
    [SaveOffline] bit  NULL,
    [Privacy] bit  NULL,
    [Rating] int  NULL,
    [TagNames] varchar(1000)  NULL,
    [TagIDs] varchar(1000)  NULL,
    [ModifiedAt] datetime  NULL
);
GO

-- Creating table 'Annotations'
CREATE TABLE [dbo].[Annotations] (
    [ID] bigint  NOT NULL,
    [SourceID] bigint  NOT NULL,
    [Annotator_schema_version] varchar(10)  NOT NULL,
    [Created] datetime  NULL,
    [Updated] datetime  NULL,
    [Text] nvarchar(max)  NULL,
    [Quote] nvarchar(max)  NULL,
    [Uri] nvarchar(max)  NULL,
    [Ranges] nvarchar(max)  NULL,
    [User] bigint  NULL,
    [Consumer] varchar(10)  NULL,
    [Tags] nvarchar(max)  NULL,
    [Permissions] nvarchar(max)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ID] in table 'SourceTags'
ALTER TABLE [dbo].[SourceTags]
ADD CONSTRAINT [PK_SourceTags]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Tags'
ALTER TABLE [dbo].[Tags]
ADD CONSTRAINT [PK_Tags]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Sources'
ALTER TABLE [dbo].[Sources]
ADD CONSTRAINT [PK_Sources]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- Creating primary key on [ID] in table 'Annotations'
ALTER TABLE [dbo].[Annotations]
ADD CONSTRAINT [PK_Annotations]
    PRIMARY KEY CLUSTERED ([ID] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [TagsID] in table 'SourceTags'
ALTER TABLE [dbo].[SourceTags]
ADD CONSTRAINT [FK_SourceTags_Tag]
    FOREIGN KEY ([TagsID])
    REFERENCES [dbo].[Tags]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SourceTags_Tag'
CREATE INDEX [IX_FK_SourceTags_Tag]
ON [dbo].[SourceTags]
    ([TagsID]);
GO

-- Creating foreign key on [UserID] in table 'Tags'
ALTER TABLE [dbo].[Tags]
ADD CONSTRAINT [FK_Tag_User]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[Users]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Tag_User'
CREATE INDEX [IX_FK_Tag_User]
ON [dbo].[Tags]
    ([UserID]);
GO

-- Creating foreign key on [UserID] in table 'Sources'
ALTER TABLE [dbo].[Sources]
ADD CONSTRAINT [FK_Source_User]
    FOREIGN KEY ([UserID])
    REFERENCES [dbo].[Users]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Source_User'
CREATE INDEX [IX_FK_Source_User]
ON [dbo].[Sources]
    ([UserID]);
GO

-- Creating foreign key on [SourceID] in table 'SourceTags'
ALTER TABLE [dbo].[SourceTags]
ADD CONSTRAINT [FK_SourceTags_Source]
    FOREIGN KEY ([SourceID])
    REFERENCES [dbo].[Sources]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SourceTags_Source'
CREATE INDEX [IX_FK_SourceTags_Source]
ON [dbo].[SourceTags]
    ([SourceID]);
GO

-- Creating foreign key on [SourceID] in table 'Annotations'
ALTER TABLE [dbo].[Annotations]
ADD CONSTRAINT [FK_Annotaion_Source]
    FOREIGN KEY ([SourceID])
    REFERENCES [dbo].[Sources]
        ([ID])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Annotaion_Source'
CREATE INDEX [IX_FK_Annotaion_Source]
ON [dbo].[Annotations]
    ([SourceID]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------