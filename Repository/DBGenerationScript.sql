USE [master]
GO
/****** Object:  Database [demo.notocol.com]    Script Date: 12/18/2015 2:53:52 PM ******/
CREATE DATABASE [demo.notocol.com]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'demo.notocol.com', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.LOCALDB\MSSQL\DATA\demo.notocol.com.mdf' , SIZE = 4096KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'demo.notocol.com_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.LOCALDB\MSSQL\DATA\demo.notocol.com_log.ldf' , SIZE = 1536KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [demo.notocol.com] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [demo.notocol.com].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [demo.notocol.com] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [demo.notocol.com] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [demo.notocol.com] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [demo.notocol.com] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [demo.notocol.com] SET ARITHABORT OFF 
GO
ALTER DATABASE [demo.notocol.com] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [demo.notocol.com] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [demo.notocol.com] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [demo.notocol.com] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [demo.notocol.com] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [demo.notocol.com] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [demo.notocol.com] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [demo.notocol.com] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [demo.notocol.com] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [demo.notocol.com] SET  DISABLE_BROKER 
GO
ALTER DATABASE [demo.notocol.com] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [demo.notocol.com] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [demo.notocol.com] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [demo.notocol.com] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [demo.notocol.com] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [demo.notocol.com] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [demo.notocol.com] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [demo.notocol.com] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [demo.notocol.com] SET  MULTI_USER 
GO
ALTER DATABASE [demo.notocol.com] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [demo.notocol.com] SET DB_CHAINING OFF 
GO
ALTER DATABASE [demo.notocol.com] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [demo.notocol.com] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [demo.notocol.com] SET DELAYED_DURABILITY = DISABLED 
GO
USE [demo.notocol.com]
GO
/****** Object:  FullTextCatalog [NotocolSourceFTS]    Script Date: 12/18/2015 2:53:53 PM ******/
CREATE FULLTEXT CATALOG [NotocolSourceFTS]
GO
/****** Object:  FullTextCatalog [Testft]    Script Date: 12/18/2015 2:53:53 PM ******/
CREATE FULLTEXT CATALOG [Testft]AS DEFAULT

GO
/****** Object:  FullTextCatalog [TestFTCat]    Script Date: 12/18/2015 2:53:53 PM ******/
CREATE FULLTEXT CATALOG [TestFTCat]
GO
/****** Object:  UserDefinedTableType [dbo].[StringList]    Script Date: 12/18/2015 2:53:53 PM ******/
CREATE TYPE [dbo].[StringList] AS TABLE(
	[Item] [nvarchar](max) NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[TagIDList]    Script Date: 12/18/2015 2:53:53 PM ******/
CREATE TYPE [dbo].[TagIDList] AS TABLE(
	[TagID] [bigint] NULL
)
GO
/****** Object:  Table [dbo].[Annotation]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Annotation](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Updated] [nvarchar](max) NOT NULL,
	[Target] [nvarchar](max) NULL,
	[Created] [nvarchar](max) NOT NULL,
	[Text] [nvarchar](max) NULL,
	[Uri] [nvarchar](max) NOT NULL,
	[Document] [nvarchar](max) NOT NULL,
	[Consumer] [nvarchar](max) NOT NULL,
	[Permissions] [nvarchar](max) NOT NULL,
	[User] [nvarchar](max) NOT NULL,
	[UserID] [bigint] NOT NULL,
	[SourceUserID] [bigint] NOT NULL,
	[SourceID] [bigint] NOT NULL,
 CONSTRAINT [PK__Annotati__3214EC27BABC99AD] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AnnotationTag]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AnnotationTag](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[annotationID] [bigint] NOT NULL,
	[tagID] [bigint] NOT NULL,
 CONSTRAINT [PK_AnnotationTag] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Folder]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Folder](
	[name] [varchar](50) NULL,
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[description] [nvarchar](50) NULL,
	[parentID] [bigint] NOT NULL,
	[created] [datetime] NOT NULL,
	[updated] [datetime] NOT NULL,
	[userID] [bigint] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Follow]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Follow](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[follower] [bigint] NOT NULL,
	[followee] [bigint] NOT NULL,
	[lastUpdated] [datetime] NOT NULL,
 CONSTRAINT [PK_Follower] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Notification]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Notification](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ReadStatus] [bit] NOT NULL,
	[Type] [int] NOT NULL,
	[Receiver] [bigint] NOT NULL,
	[SecondaryUser] [bigint] NULL,
	[Created] [datetime] NOT NULL,
	[SourceUserID] [bigint] NULL,
	[ReasonCode] [int] NOT NULL,
	[SourceID] [bigint] NOT NULL,
	[AdditionalText] [varchar](max) NULL,
	[tags] [varchar](max) NULL,
	[note] [varchar](max) NULL,
 CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[NotificationTemp]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[NotificationTemp](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[ReadStatus] [bit] NOT NULL,
	[Type] [int] NOT NULL,
	[Receiver] [bigint] NOT NULL,
	[SecondaryUser] [bigint] NULL,
	[Created] [datetime] NOT NULL,
	[SourceUserID] [bigint] NULL,
	[ReasonCode] [int] NOT NULL,
	[SourceID] [bigint] NOT NULL,
	[AdditionalText] [varchar](max) NULL,
	[tags] [varchar](max) NULL,
	[note] [varchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SolarGroupUsers]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SolarGroupUsers](
	[userID] [bigint] NOT NULL,
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_SolarGroupUsers] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Source]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Source](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[url] [nvarchar](2083) NOT NULL,
	[uriHash] [binary](160) NOT NULL,
	[faviconURL] [nvarchar](2083) NULL,
	[title] [nvarchar](1024) NULL,
	[created] [datetime] NOT NULL,
 CONSTRAINT [PK_Source_1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SourceUser]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SourceUser](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [bigint] NOT NULL,
	[Summary] [nvarchar](2000) NULL,
	[Privacy] [bit] NULL,
	[Rating] [int] NULL,
	[ModifiedAt] [datetime] NULL,
	[thumbnailImageUrl] [varchar](max) NULL,
	[thumbnailText] [nvarchar](1000) NULL,
	[FolderID] [bigint] NULL,
	[noteCount] [int] NOT NULL,
	[SourceID] [bigint] NULL,
	[PrivacyOverride] [bit] NULL,
	[PrivateNoteCount] [int] NULL,
 CONSTRAINT [PK_Source] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SourceUserTag]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SourceUserTag](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[SourceUserID] [bigint] NOT NULL,
	[TagID] [bigint] NOT NULL,
 CONSTRAINT [PK_SourceUserTag] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tag]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Tag](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[updated] [datetime] NOT NULL,
 CONSTRAINT [PK_Tag] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UploadedFileMapping]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UploadedFileMapping](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[userID] [bigint] NOT NULL,
	[FileNameForLink] [varchar](max) NOT NULL,
	[LocalFileName] [varchar](max) NOT NULL,
	[UploadedDate] [datetime] NOT NULL,
	[Title] [varchar](max) NULL,
	[Uri] [varchar](max) NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_UploadedFileMapping] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[User]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[User](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](200) NOT NULL,
	[Password] [varchar](50) NULL,
	[Identifier] [varchar](500) NULL,
	[ModifiedAt] [datetime] NULL,
	[Email] [varchar](200) NULL,
	[Provider] [varchar](500) NULL,
	[Gender] [char](1) NULL,
	[DOB] [date] NULL,
	[Address] [varchar](200) NULL,
	[Name] [varchar](50) NULL,
	[Photo] [nvarchar](2083) NULL,
	[UnsubscribeNotification] [bit] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserTagUsage]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserTagUsage](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[userID] [bigint] NOT NULL,
	[tagID] [bigint] NOT NULL,
	[lastUsed] [datetime] NOT NULL,
 CONSTRAINT [PK_UserTagUsage] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Index [ui_sourceid]    Script Date: 12/18/2015 2:53:53 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [ui_sourceid] ON [dbo].[SourceUser]
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Folder] ADD  CONSTRAINT [DF_Folder_parentID]  DEFAULT ((0)) FOR [parentID]
GO
ALTER TABLE [dbo].[Notification] ADD  CONSTRAINT [DF_Notification_ReadStatus]  DEFAULT ((0)) FOR [ReadStatus]
GO
ALTER TABLE [dbo].[SourceUser] ADD  CONSTRAINT [DF_Source_SaveOffline1]  DEFAULT ((0)) FOR [Privacy]
GO
ALTER TABLE [dbo].[SourceUser] ADD  CONSTRAINT [DF_Source_Rating]  DEFAULT ((0)) FOR [Rating]
GO
ALTER TABLE [dbo].[SourceUser] ADD  CONSTRAINT [DF_Source_noteCount]  DEFAULT ((0)) FOR [noteCount]
GO
ALTER TABLE [dbo].[SourceUser] ADD  CONSTRAINT [DF_SourceUser_PrivateNoteCount]  DEFAULT ((0)) FOR [PrivateNoteCount]
GO
ALTER TABLE [dbo].[Tag] ADD  CONSTRAINT [DF__Tag__updated__2A164134]  DEFAULT (getdate()) FOR [updated]
GO
ALTER TABLE [dbo].[UploadedFileMapping] ADD  CONSTRAINT [DF_UploadedFileMapping_Version]  DEFAULT ((0)) FOR [Version]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_ModifiedDateTime]  DEFAULT (getdate()) FOR [ModifiedAt]
GO
ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_UnsubscribeNotification]  DEFAULT ((0)) FOR [UnsubscribeNotification]
GO
ALTER TABLE [dbo].[Annotation]  WITH CHECK ADD FOREIGN KEY([SourceID])
REFERENCES [dbo].[Source] ([ID])
GO
ALTER TABLE [dbo].[Annotation]  WITH CHECK ADD FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([ID])
GO
ALTER TABLE [dbo].[AnnotationTag]  WITH CHECK ADD  CONSTRAINT [FK__Annotatio__annot__0E8E2250] FOREIGN KEY([annotationID])
REFERENCES [dbo].[Annotation] ([ID])
GO
ALTER TABLE [dbo].[AnnotationTag] CHECK CONSTRAINT [FK__Annotatio__annot__0E8E2250]
GO
ALTER TABLE [dbo].[AnnotationTag]  WITH CHECK ADD  CONSTRAINT [FK__Annotatio__tagID__0F824689] FOREIGN KEY([tagID])
REFERENCES [dbo].[Tag] ([ID])
GO
ALTER TABLE [dbo].[AnnotationTag] CHECK CONSTRAINT [FK__Annotatio__tagID__0F824689]
GO
ALTER TABLE [dbo].[Folder]  WITH CHECK ADD  CONSTRAINT [fk_user] FOREIGN KEY([userID])
REFERENCES [dbo].[User] ([ID])
GO
ALTER TABLE [dbo].[Folder] CHECK CONSTRAINT [fk_user]
GO
ALTER TABLE [dbo].[Follow]  WITH CHECK ADD  CONSTRAINT [FK_Follower_Followee] FOREIGN KEY([followee])
REFERENCES [dbo].[User] ([ID])
GO
ALTER TABLE [dbo].[Follow] CHECK CONSTRAINT [FK_Follower_Followee]
GO
ALTER TABLE [dbo].[Follow]  WITH CHECK ADD  CONSTRAINT [FK_Follower_Follower] FOREIGN KEY([follower])
REFERENCES [dbo].[User] ([ID])
GO
ALTER TABLE [dbo].[Follow] CHECK CONSTRAINT [FK_Follower_Follower]
GO
ALTER TABLE [dbo].[Notification]  WITH NOCHECK ADD  CONSTRAINT [FK_Notification_SecUser] FOREIGN KEY([SecondaryUser])
REFERENCES [dbo].[User] ([ID])
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_SecUser]
GO
ALTER TABLE [dbo].[Notification]  WITH NOCHECK ADD  CONSTRAINT [FK_Notification_Source] FOREIGN KEY([SourceID])
REFERENCES [dbo].[Source] ([ID])
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_Source]
GO
ALTER TABLE [dbo].[Notification]  WITH NOCHECK ADD  CONSTRAINT [FK_Notification_SourceUser] FOREIGN KEY([SourceUserID])
REFERENCES [dbo].[SourceUser] ([ID])
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_SourceUser]
GO
ALTER TABLE [dbo].[Notification]  WITH NOCHECK ADD  CONSTRAINT [FK_Notification_User] FOREIGN KEY([Receiver])
REFERENCES [dbo].[User] ([ID])
GO
ALTER TABLE [dbo].[Notification] CHECK CONSTRAINT [FK_Notification_User]
GO
ALTER TABLE [dbo].[SourceUser]  WITH CHECK ADD  CONSTRAINT [FK__SourceUse__Sourc__758D6A5C] FOREIGN KEY([SourceID])
REFERENCES [dbo].[Source] ([ID])
GO
ALTER TABLE [dbo].[SourceUser] CHECK CONSTRAINT [FK__SourceUse__Sourc__758D6A5C]
GO
ALTER TABLE [dbo].[SourceUser]  WITH CHECK ADD  CONSTRAINT [fk_folder] FOREIGN KEY([FolderID])
REFERENCES [dbo].[Folder] ([ID])
GO
ALTER TABLE [dbo].[SourceUser] CHECK CONSTRAINT [fk_folder]
GO
ALTER TABLE [dbo].[SourceUser]  WITH CHECK ADD  CONSTRAINT [FK_Source_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([ID])
GO
ALTER TABLE [dbo].[SourceUser] CHECK CONSTRAINT [FK_Source_User]
GO
ALTER TABLE [dbo].[SourceUserTag]  WITH CHECK ADD  CONSTRAINT [FK_SourceUserTag_SourceUser] FOREIGN KEY([SourceUserID])
REFERENCES [dbo].[SourceUser] ([ID])
GO
ALTER TABLE [dbo].[SourceUserTag] CHECK CONSTRAINT [FK_SourceUserTag_SourceUser]
GO
ALTER TABLE [dbo].[SourceUserTag]  WITH CHECK ADD  CONSTRAINT [FK_SourceUserTag_Tag] FOREIGN KEY([TagID])
REFERENCES [dbo].[Tag] ([ID])
GO
ALTER TABLE [dbo].[SourceUserTag] CHECK CONSTRAINT [FK_SourceUserTag_Tag]
GO
ALTER TABLE [dbo].[UserTagUsage]  WITH CHECK ADD FOREIGN KEY([tagID])
REFERENCES [dbo].[Tag] ([ID])
GO
ALTER TABLE [dbo].[UserTagUsage]  WITH CHECK ADD FOREIGN KEY([userID])
REFERENCES [dbo].[User] ([ID])
GO
/****** Object:  StoredProcedure [dbo].[DeleteAnnotation]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DeleteAnnotation]
	-- Add the parameters for the stored procedure here
	@AnnotationID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	delete from AnnotationTag where annotationID = @AnnotationID
	update annotation set [Text] = null, [SourceUserID] = 0, [Permissions] = '{}' where ID = @AnnotationID;
	
	
	return 0
   
END

GO
/****** Object:  StoredProcedure [dbo].[DeleteSourceUser]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[DeleteSourceUser]
	-- Add the parameters for the stored procedure here
	@SourceUserID bigint,
	@SourceID bigint,
	@DeleteSource BIT output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		
	select * from sourceuser where SourceID = @SourceID;
	
	IF @@ROWCOUNT = 1 
	BEGIN
		delete from AnnotationTag where annotationID in (select ID from Annotation where SourceUserID = @SourceUserID);
		delete annotation where SourceUserID = @SourceUserID;
	
		delete from SourceUserTag where SourceUserID = @SourceUserID
		delete from Notification where SourceUserID = @SourceUserID

		delete from SourceUser where ID = @SourceUserID 
		delete from Source where ID = @SourceID
		SET @DeleteSource = 1
	END

	ELSE
	BEGIN
		delete from AnnotationTag where annotationID in (select ID from Annotation where SourceUserID = @SourceUserID);
		update annotation set [Text] = null, [SourceUserID] = 0, [Permissions] = '{}' where SourceUserID = @SourceUserID;
	
		delete from SourceUserTag where SourceUserID = @SourceUserID
		delete from SourceUser where ID = @SourceUserID 
		SET @DeleteSource = 0

	END
	
   
END


GO
/****** Object:  StoredProcedure [dbo].[GetTagID]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Name
-- Create date: 
-- Description:	
-- =============================================
CREATE PROCEDURE [dbo].[GetTagID] 
	-- Add the parameters for the stored procedure here
	@TagName as varchar(MAX), 
	@TagID int OUTPUT,
	@TagNameOut as varchar(MAX) out
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	select @TagNameOut=Name, @TagID=ID from Tag where Name = @TagName;

	IF NULLIF(@TagID, '') IS NULL
	BEGIN
		DECLARE @OutputTbl TABLE (ID INT, Name varchar(MAX))
		
		INSERT INTO Tag(Name)
		OUTPUT INSERTED.ID, INSERTED.Name INTO @OutputTbl(ID, Name)
		VALUES (@TagName);

		select @TagNameOut=Name, @TagID=ID from @OutputTbl
			
		
	END

	select @TagID, @TagNameOut
END




GO
/****** Object:  StoredProcedure [dbo].[GetUserProfileData]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--CREATE TYPE [dbo].[StringList] AS TABLE(
--    [Item] [NVARCHAR](MAX) NULL
--);

CREATE PROCEDURE [dbo].[GetUserProfileData]
    @username varchar(MAX) ,
	@userID bigint output,
	@name varchar(MAX) output,
	@followers int output,
	@follows int output,
	@sourceUser int output,
	@noteCount int output

AS
BEGIN
    select @userID=ID, @name = Name from [User] where Username = @username;
	select @followers = count(*) from Follow where followee = @userID;
	select @follows = count(*) from Follow where follower = @userID;
	select @sourceUser = count(*) from SourceUser where UserID = @userID;
	select @noteCount = count(*) from Annotation where UserID = @userID and SourceUserID != 0;
END

GO
/****** Object:  StoredProcedure [dbo].[GetUserProfileSourcePages]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--CREATE TYPE [dbo].[StringList] AS TABLE(
--    [Item] [NVARCHAR](MAX) NULL
--);

CREATE PROCEDURE [dbo].[GetUserProfileSourcePages]
   	@userID bigint,
	@offset int,
	@size int 
AS
BEGIN
    select s.ID as sourceID,
		su.ID as sourceUserID, 
		a.ID as annotationID, 
		s.faviconURL, 
		s.url,
		s.title, 
		su.thumbnailImageUrl,
		su.thumbnailText,
		su.Summary,
		t.Name as tag,
		a.Text,
		a.Target,
		a.Permissions 


	from sourceUser as su
	left join Source as s on su.SourceID = s.ID
	left join Annotation as a on su.ID = a.SourceUserID
	left join Tag as t on t.ID in 
		(select TagID from SourceUserTag where SourceUserID = su.ID
			UNION 
		  select TagID from AnnotationTag where annotationID = a.ID	) 
	where su.ID in 
		(select ID from sourceUser 
		 where UserID = @userID and Privacy != 1 
		 order by ModifiedAt desc
		 offset @offset rows
		 FETCH NEXT @size ROWS ONLY)
	
	order by su.ModifiedAt desc
END

GO
/****** Object:  StoredProcedure [dbo].[GetUsersForTags]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--CREATE TYPE [dbo].[StringList] AS TABLE(
--    [Item] [NVARCHAR](MAX) NULL
--);

CREATE PROCEDURE [dbo].[GetUsersForTags]
    @tagList StringList READONLY
AS
BEGIN
    
	select DISTINCT(userID) as Item
	from SourceUser 
	where ID in 
		(select SourceUserID 
		from SourceUserTag join Tag on SourceUserTag.TagID = Tag.ID 
		where Tag.Name in (select Item from @tagList))

		

END

GO
/****** Object:  StoredProcedure [dbo].[MarkNotificationAsRead]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[MarkNotificationAsRead]
	-- Add the parameters for the stored procedure here
	@notificationID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	update Notification set [ReadStatus] = 1 where ID = @notificationID;
	
	
	return 0
   
END

GO
/****** Object:  StoredProcedure [dbo].[MarkNotificationAsUnread]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[MarkNotificationAsUnread]
	-- Add the parameters for the stored procedure here
	@notificationID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	update Notification set [ReadStatus] = 0 where ID = @notificationID;
	
	
	return 0
   
END

GO
/****** Object:  StoredProcedure [dbo].[SaveSource]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SaveSource]
	-- Add the parameters for the stored procedure here
	@tags varchar(max),
	@UserID bigint,
	@title varchar(500),
	@link nvarchar(1000),
	@summary nvarchar(2000),
	@readLater bit,
	@saveOffline bit,
	@privacy bit,
	@rating int
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	return 0
   
END


GO
/****** Object:  StoredProcedure [dbo].[sp_UseStringList]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--CREATE TYPE [dbo].[StringList] AS TABLE(
--    [Item] [NVARCHAR](MAX) NULL
--);

CREATE PROCEDURE [dbo].[sp_UseStringList]
    @list TagIDList READONLY
AS
BEGIN
    -- Just return the items we passed in
    SELECT l.TagID FROM @list l;
END


GO
/****** Object:  StoredProcedure [dbo].[spTestSourceData]    Script Date: 12/18/2015 2:53:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create proc [dbo].[spTestSourceData]
AS
begin
set nocount on
select * 
from Source
inner join  SourceTag on Source.ID = SourceTag.SourceID
inner join Tag on SourceTag.TagsID = tag.ID

End


GO
USE [master]
GO
ALTER DATABASE [demo.notocol.com] SET  READ_WRITE 
GO
