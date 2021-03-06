USE [IVS_NewClientDB]
GO
/****** Object:  Table [dbo].[Devices]    Script Date: 11/14/2012 16:55:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Devices](
	[DeviceID] [int] IDENTITY(1,1) NOT NULL,
	[ClientID] [int] NULL,
	[DeviceType] [nvarchar](10) NULL,
	[ModelNo] [nvarchar](20) NULL,
	[SerialNo] [nvarchar](20) NULL,
	[FirmwareRev] [nvarchar](20) NULL,
	[FirmwareDate] [nvarchar](20) NULL,
	[HardwareRev] [nvarchar](20) NULL,
	[UpdateTS] [datetime] NULL,
 CONSTRAINT [PK_tblDevices] PRIMARY KEY CLUSTERED 
(
	[DeviceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Clients]    Script Date: 11/14/2012 16:55:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Clients](
	[ClientID] [int] IDENTITY(1,1) NOT NULL,
	[DeviceType] [nvarchar](10) NULL,
	[DeviceID] [int] NULL,
	[ComPort] [nvarchar](5) NULL,
	[IDecodeLicense] [nvarchar](50) NULL,
	[IDecodeVersion] [nvarchar](10) NULL,
	[IDecodeTrackFormat] [nvarchar](255) NULL,
	[IDecodeCardTypes] [nvarchar](20) NULL,
	[Location] [nvarchar](35) NULL,
	[Station] [nvarchar](35) NULL,
	[Phone] [nvarchar](20) NULL,
	[Email] [nvarchar](35) NULL,
	[SkipLogon] [bit] NOT NULL,
	[DisplayAdmin] [bit] NULL,
	[DefaultUser] [int] NULL,
	[AgeHighlight] [bit] NOT NULL,
	[AgePopup] [bit] NULL,
	[Age] [int] NULL,
	[ImageSave] [bit] NOT NULL,
	[ImageLocation] [nvarchar](255) NULL,
	[ViewingTime] [int] NULL,
	[CCDigits] [int] NULL,
	[DisableCCSave] [bit] NOT NULL,
	[DisableDBSave] [bit] NULL,
	[ClientHostName] [nvarchar](255) NULL,
	[ClientIPAddress] [nvarchar](255) NULL,
	[LogRetention] [int] NULL,
	[DBRetention] [int] NULL,
	[UpdateTS] [datetime] NULL,
 CONSTRAINT [PK_tblClients] PRIMARY KEY CLUSTERED 
(
	[ClientID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Alerts]    Script Date: 11/14/2012 16:55:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Alerts](
	[AlertID] [int] IDENTITY(1,1) NOT NULL,
	[IDNumber] [nvarchar](35) NULL,
	[NameFirst] [nvarchar](35) NULL,
	[NameLast] [nvarchar](35) NULL,
	[DateOfBirth] [nvarchar](10) NULL,
	[AlertContactName] [nvarchar](35) NULL,
	[AlertContactNumber] [nvarchar](20) NULL,
	[AlertNotes] [nvarchar](255) NULL,
	[ActiveFlag] [bit] NOT NULL,
	[UserID] [int] NULL,
	[UpdateTS] [datetime] NULL,
 CONSTRAINT [PK_tblAlerts] PRIMARY KEY CLUSTERED 
(
	[AlertID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WebService]    Script Date: 11/14/2012 16:55:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WebService](
	[IDecodeLicense] [nvarchar](50) NOT NULL,
	[IDecodeVersion] [nvarchar](10) NULL,
	[IDecodeTrackFormat] [nvarchar](255) NULL,
	[IDecodeCardTypes] [nvarchar](20) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 11/14/2012 16:55:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](15) NULL,
	[Password] [nvarchar](10) NULL,
	[UserNameFirst] [nvarchar](25) NULL,
	[UserNameLast] [nvarchar](25) NULL,
	[UserEmail] [nvarchar](35) NULL,
	[UserPhone] [nvarchar](20) NULL,
	[AdminFlag] [bit] NOT NULL,
	[AlertFlag] [bit] NOT NULL,
	[SearchFlag] [bit] NOT NULL,
	[ActiveFlag] [bit] NOT NULL,
	[UpdateTS] [datetime] NULL,
 CONSTRAINT [PK_tblUsers] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TEPViolations]    Script Date: 11/14/2012 16:55:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TEPViolations](
	[ViolationID] [int] IDENTITY(1,1) NOT NULL,
	[Statute] [nvarchar](30) NOT NULL,
	[Offense] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_tblViolations] PRIMARY KEY CLUSTERED 
(
	[ViolationID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TEPClients]    Script Date: 11/14/2012 16:55:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TEPClients](
	[ClientID] [tinyint] IDENTITY(1,1) NOT NULL,
	[CitationPrefix] [nvarchar](10) NULL,
	[ICRPrefix] [nvarchar](10) NULL,
 CONSTRAINT [PK_tblTEPClients] PRIMARY KEY CLUSTERED 
(
	[ClientID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SwipeScans_Violations]    Script Date: 11/14/2012 16:55:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SwipeScans_Violations](
	[SwipeScanID] [int] NOT NULL,
	[ViolationSequence] [tinyint] NULL,
	[ViolationStatute] [nvarchar](30) NULL,
	[ViolationOffense] [nvarchar](50) NULL,
 CONSTRAINT [PK_TEP_Violations] PRIMARY KEY CLUSTERED 
(
	[SwipeScanID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SwipeScans_TEP]    Script Date: 11/14/2012 16:55:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SwipeScans_TEP](
	[SwipeScanID] [int] NOT NULL,
	[ClientID] [tinyint] NULL,
	[CitationID] [nvarchar](50) NULL,
	[ICRID] [nvarchar](50) NULL,
	[Disposition] [nvarchar](50) NULL,
	[UpdateTS] [smalldatetime] NULL,
 CONSTRAINT [PK_tblSwipeScans_TEP_1] PRIMARY KEY CLUSTERED 
(
	[SwipeScanID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SwipeScans_MID]    Script Date: 11/14/2012 16:55:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SwipeScans_MID](
	[SwipeScanID] [int] NOT NULL,
	[IDNumber] [nvarchar](35) NULL,
	[NameFirst] [nvarchar](35) NULL,
	[NameLast] [nvarchar](35) NULL,
	[NameMiddle] [nvarchar](35) NULL,
	[DateOfBirth] [nvarchar](10) NULL,
	[Age] [smallint] NULL,
	[Height] [nvarchar](10) NULL,
	[Weight] [nvarchar](10) NULL,
	[Eyes] [nvarchar](10) NULL,
	[Hair] [nvarchar](10) NULL,
	[DateOfIssue] [nvarchar](10) NULL,
	[DateOfExpiration] [nvarchar](10) NULL,
	[SwipeRawData] [nvarchar](max) NULL,
 CONSTRAINT [PK_tblSwipeScans_MID] PRIMARY KEY CLUSTERED 
(
	[SwipeScanID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SwipeScans_INS]    Script Date: 11/14/2012 16:55:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SwipeScans_INS](
	[SwipeScanID] [int] NOT NULL,
	[IDNumber] [nvarchar](35) NULL,
	[NameFirst] [nvarchar](35) NULL,
	[NameLast] [nvarchar](35) NULL,
	[NameMiddle] [nvarchar](35) NULL,
	[DateOfBirth] [nvarchar](10) NULL,
	[Age] [smallint] NULL,
	[Sex] [nvarchar](1) NULL,
	[DateOfIssue] [nvarchar](10) NULL,
	[DateOfExpiration] [nvarchar](10) NULL,
	[SwipeRawData] [nvarchar](max) NULL,
 CONSTRAINT [PK_tblSwipeScans_INS] PRIMARY KEY CLUSTERED 
(
	[SwipeScanID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SwipeScans_DLID]    Script Date: 11/14/2012 16:55:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SwipeScans_DLID](
	[SwipeScanID] [int] NOT NULL,
	[IDNumber] [nvarchar](35) NULL,
	[NameFirst] [nvarchar](35) NULL,
	[NameLast] [nvarchar](35) NULL,
	[NameMiddle] [nvarchar](35) NULL,
	[DateOfBirth] [nvarchar](10) NULL,
	[Age] [smallint] NULL,
	[Sex] [nvarchar](1) NULL,
	[Height] [nvarchar](10) NULL,
	[Weight] [nvarchar](10) NULL,
	[Eyes] [nvarchar](10) NULL,
	[Hair] [nvarchar](10) NULL,
	[DateOfIssue] [nvarchar](10) NULL,
	[DateOfExpiration] [nvarchar](10) NULL,
	[AddressStreet] [nvarchar](35) NULL,
	[AddressCity] [nvarchar](35) NULL,
	[AddressState] [nvarchar](2) NULL,
	[AddressZip] [nvarchar](10) NULL,
	[SwipeRawData] [nvarchar](max) NULL,
 CONSTRAINT [PK_tblSwipeScans_DLID] PRIMARY KEY CLUSTERED 
(
	[SwipeScanID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SwipeScans_CK_Batch]    Script Date: 11/14/2012 16:55:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SwipeScans_CK_Batch](
	[SwipeScanID] [int] IDENTITY(1,1) NOT NULL,
	[BatchID] [nvarchar](50) NULL,
	[ClientID] [int] NULL,
	[UserID] [int] NULL,
	[RoutingNumber] [nvarchar](9) NULL,
	[AccountNumber] [nvarchar](15) NULL,
	[CheckNumber] [int] NULL,
	[ImagePath] [nvarchar](255) NULL,
	[SwipeRawData] [nvarchar](75) NULL,
	[SwipeScanTS] [datetime] NULL,
 CONSTRAINT [PK_tblSwipeScans_CK_Batch] PRIMARY KEY CLUSTERED 
(
	[SwipeScanID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SwipeScans_CK]    Script Date: 11/14/2012 16:55:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SwipeScans_CK](
	[SwipeScanID] [int] NOT NULL,
	[RoutingNumber] [nvarchar](9) NULL,
	[AccountNumber] [nvarchar](15) NULL,
	[CheckNumber] [int] NULL,
	[SwipeRawData] [nvarchar](75) NULL,
 CONSTRAINT [PK_tblSwipeScans_CK] PRIMARY KEY CLUSTERED 
(
	[SwipeScanID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SwipeScans_CC]    Script Date: 11/14/2012 16:55:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SwipeScans_CC](
	[SwipeScanID] [int] NOT NULL,
	[CCNumber] [nvarchar](35) NULL,
	[CCIssuer] [nvarchar](35) NULL,
	[NameFirst] [nvarchar](35) NULL,
	[NameLast] [nvarchar](35) NULL,
	[NameMiddle] [nvarchar](35) NULL,
	[DateOfExpiration] [nvarchar](10) NULL,
	[SwipeRawData] [nvarchar](255) NULL,
 CONSTRAINT [PK_tblSwipeScans_CC] PRIMARY KEY CLUSTERED 
(
	[SwipeScanID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SwipeScans]    Script Date: 11/14/2012 16:55:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SwipeScans](
	[SwipeScanID] [int] IDENTITY(1,1) NOT NULL,
	[SwipeScanType] [nvarchar](40) NULL,
	[ClientID] [int] NULL,
	[UserID] [int] NULL,
	[CaseID] [nvarchar](35) NULL,
	[DataSource] [nvarchar](10) NULL,
	[ImageAvailable] [bit] NULL,
	[SwipeScanTS] [datetime] NULL,
 CONSTRAINT [PK_tblSwipeScans] PRIMARY KEY CLUSTERED 
(
	[SwipeScanID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[qryUpdateWSIDecode]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryUpdateWSIDecode]

@IDecodeVersion		NVARCHAR(10)

AS
UPDATE WebService SET IDecodeVersion = @IDecodeVersion
GO
/****** Object:  StoredProcedure [dbo].[qryUpdateUser]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryUpdateUser]

@UserName				NVARCHAR(15),
@Password				NVARCHAR(10),
@UserNameFirst			NVARCHAR(25),
@UserNameLast			NVARCHAR(25),
@UserEmail				NVARCHAR(35),
@UserPhone				NVARCHAR(20),
@AdminFlag				BIT,
@AlertFlag				BIT,
@ActiveFlag				BIT,
@SearchFlag				BIT,
@UserID					INTEGER

AS

UPDATE Users SET Users.UserName = @UserName
					, Users.[Password] = @Password
					, Users.UserNameFirst = @UserNameFirst
					, Users.UserNameLast = @UserNameLast
					, Users.UserEmail = @UserEmail
					, Users.UserPhone = @UserPhone
					, Users.AdminFlag = @AdminFlag
					, Users.AlertFlag = @AlertFlag
					, Users.SearchFlag = @SearchFlag
					, Users.ActiveFlag = @ActiveFlag
					, Users.UpdateTS = GETDATE()
WHERE (((Users.UserID)=@UserID));
GO
/****** Object:  StoredProcedure [dbo].[qryUpdateImageAvailable]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryUpdateImageAvailable]

@SwipeScanID			INTEGER

AS

UPDATE SwipeScans SET ImageAvailable = 1
WHERE SwipeScanID=@SwipeScanID;
GO
/****** Object:  StoredProcedure [dbo].[qryUpdateIDecodeLicense]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryUpdateIDecodeLicense]

@IDecodeLicense		NVARCHAR(50),
@IDecodeVersion		NVARCHAR(10),
@ClientID			INTEGER

AS
UPDATE Clients SET IDecodeLicense = @IDecodeLicense, IDecodeVersion = @IDecodeVersion, UpdateTS = GETDATE()
WHERE ClientID=@ClientID
GO
/****** Object:  StoredProcedure [dbo].[qryUpdateDevice]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryUpdateDevice]

@DeviceType			NVARCHAR(10),
@ModelNo			NVARCHAR(20),
@SerialNo			NVARCHAR(20),
@FirmwareRev		NVARCHAR(20),
@FirmwareDate		NVARCHAR(20),
@HardwareRev		NVARCHAR(20),
@DeviceID			INTEGER

AS

UPDATE Devices SET Devices.DeviceType = @DeviceType
					, Devices.ModelNo = @ModelNo
					, Devices.SerialNo = @SerialNo
					, Devices.FirmwareRev = @FirmwareRev
					, Devices.FirmwareDate = @FirmwareDate
					, Devices.HardwareRev = @HardwareRev
					, Devices.UpdateTS = GETDATE()
WHERE Devices.DeviceID=@DeviceID;
GO
/****** Object:  StoredProcedure [dbo].[qryUpdateClientSettings]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryUpdateClientSettings]

@Location			NVARCHAR(35),
@Station			NVARCHAR(35),
@Phone				NVARCHAR(20),
@Email				NVARCHAR(35),
@SkipLogon			BIT,
@DisplayAdmin		BIT,
@DefaultUser		INTEGER,
@AgeHighlight		BIT,
@AgePopup			BIT,
@Age				INTEGER,
@ImageSave			BIT,
@ImageLocation		NVARCHAR(255),
@ViewingTime		INTEGER,
@CCDigits			INTEGER,
@DisableCCSave		BIT,
@DisableDBSave		BIT,
@ClientID			INTEGER

AS

UPDATE Clients SET Clients.Location = @Location
					, Clients.Station = @Station
					, Clients.Phone = @Phone
					, Clients.Email = @Email
					, Clients.SkipLogon = @SkipLogon
					, Clients.DisplayAdmin = @DisplayAdmin
					, Clients.DefaultUser = @DefaultUser
					, Clients.AgeHighlight = @AgeHighlight
					, clients.AgePopup=@AgePopup
					, Clients.Age = @Age
					, Clients.ImageSave = @ImageSave
					, Clients.ImageLocation = @ImageLocation
					, Clients.ViewingTime = @ViewingTime
					, Clients.CCDigits = @CCDigits
					, Clients.DisableCCSave = @DisableCCSave
					, Clients.DisableDBSave = @DisableDBSave
WHERE ClientID=@ClientID;
GO
/****** Object:  StoredProcedure [dbo].[qryUpdateClientIPAddress]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryUpdateClientIPAddress]

@ClientIPAddress	NVARCHAR(255),
@ClientID			INTEGER

AS

UPDATE Clients SET ClientIPAddress = @ClientIPAddress
WHERE ClientID=@ClientID
GO
/****** Object:  StoredProcedure [dbo].[qryUpdateClientDevice]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryUpdateClientDevice]

@DeviceType			NVARCHAR(10),
@DeviceID			INTEGER,
@ComPort			NVARCHAR(5),
@ClientID			INTEGER

AS

UPDATE Clients SET DeviceType = @DeviceType, DeviceID = @DeviceID, ComPort = @ComPort, UpdateTS = GETDATE()
WHERE ClientID=@ClientID
GO
/****** Object:  StoredProcedure [dbo].[qryUpdateAlert]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryUpdateAlert]

@IDNumber			NVARCHAR(35),
@NameFirst			NVARCHAR(35),
@NameLast			NVARCHAR(35),
@DateOfBirth		NVARCHAR(10),
@AlertContactName	NVARCHAR(35),
@AlertContactNumber NVARCHAR(20),
@AlertNotes			NVARCHAR(255),
@ActiveFlag			BIT,
@UserID				INTEGER,
@AlertID			INTEGER

AS

UPDATE Alerts SET Alerts.IDNumber = @IDNumber
					, Alerts.AlertContactName = @AlertContactName
					, Alerts.DateOfBirth = @DateOfBirth
					, Alerts.AlertContactNumber = @AlertContactNumber
					, Alerts.AlertNotes = @AlertNotes
					, Alerts.ActiveFlag = @ActiveFlag
					, Alerts.UserID = @UserID
					, Alerts.UpdateTS = GETDATE()
					, Alerts.NameFirst = @NameFirst
					, Alerts.NameLast = @NameLast
WHERE (((Alerts.AlertID)=@AlertID));
GO
/****** Object:  StoredProcedure [dbo].[qrySwipeScansNavigatePrevious]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qrySwipeScansNavigatePrevious]

@UserID		INTEGER,
@ClientID	INTEGER,
@SwipeScanID	INTEGER

AS

SELECT TOP 1 SwipeScanID, SwipeScanType, SwipeScanTS
FROM SwipeScans
WHERE ClientID=@ClientID and UserID=@UserID AND SwipeScanID < @SwipeScanID
ORDER BY SwipeScanID DESC;
GO
/****** Object:  StoredProcedure [dbo].[qrySwipeScansNavigatePosition]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qrySwipeScansNavigatePosition]

@SwipeScanID	INTEGER

AS

SELECT TOP 1 SwipeScanID, SwipeScanType, SwipeScanTS
FROM SwipeScans
WHERE SwipeScanID=@SwipeScanID
ORDER BY SwipeScanID DESC;
GO
/****** Object:  StoredProcedure [dbo].[qrySwipeScansNavigateNext]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qrySwipeScansNavigateNext]

@UserID		INTEGER,
@ClientID	INTEGER,
@SwipeScanID	INTEGER

AS

SELECT TOP 1 SwipeScanID, SwipeScanType, SwipeScanTS
FROM SwipeScans
WHERE ClientID=@ClientID and UserID=@UserID AND SwipeScanID > @SwipeScanID
ORDER BY SwipeScanID;
GO
/****** Object:  StoredProcedure [dbo].[qrySwipeScansNavigateLast]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qrySwipeScansNavigateLast]

@UserID		INTEGER,
@ClientID	INTEGER

AS

SELECT TOP 1 SwipeScanID, SwipeScanType, SwipeScanTS
FROM SwipeScans
WHERE ClientID=@ClientID and UserID=@UserID
ORDER BY SwipeScanID DESC;
GO
/****** Object:  StoredProcedure [dbo].[qrySwipeScansNavigateFirst]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qrySwipeScansNavigateFirst]

@UserID		INTEGER,
@ClientID	INTEGER

AS

SELECT TOP 1 SwipeScanID, SwipeScanType, SwipeScanTS
FROM SwipeScans
WHERE ClientID=@ClientID and UserID=@UserID
ORDER BY SwipeScanID;
GO
/****** Object:  StoredProcedure [dbo].[qryNewUser]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryNewUser]

@UserName				NVARCHAR(15),
@Password				NVARCHAR(10),
@UserNameFirst			NVARCHAR(25),
@UserNameLast			NVARCHAR(25),
@UserEmail				NVARCHAR(35),
@UserPhone				NVARCHAR(20),
@AdminFlag				BIT,
@AlertFlag				BIT,
@SearchFlag				BIT,
@ActiveFlag				BIT

AS

INSERT INTO Users ( UserName
						, [Password]
						, UserNameFirst
						, UserNameLast
						, UserEmail
						, UserPhone
						, AdminFlag
						, AlertFlag
						, ActiveFlag
						, SearchFlag
						, UpdateTS )
				VALUES (@UserName
						, @Password
						, @UserNameFirst
						, @UserNameLast
						, @UserEmail
						, @UserPhone
						, @AdminFlag
						, @AlertFlag
						, @SearchFlag
						, @ActiveFlag
						, GETDATE())
GO
/****** Object:  StoredProcedure [dbo].[qryNewDevice]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryNewDevice]

@DeviceType				NVARCHAR(10),
@ModelNo				NVARCHAR(20),
@SerialNo				NVARCHAR(20),
@FirmwareRev			NVARCHAR(20),
@FirmwareDate			NVARCHAR(20),
@HardwareRev			NVARCHAR(20),
@ClientID				INTEGER

AS

INSERT INTO Devices ( ClientID
						, DeviceType
						, ModelNo
						, SerialNo
						, FirmwareRev
						, FirmwareDate
						, HardwareRev
						, UpdateTS )
				VALUES (@ClientID
						, @DeviceType
						, @ModelNo
						, @SerialNo
						, @FirmwareRev
						, @FirmwareDate
						, @HardwareRev
						, GETDATE())
GO
/****** Object:  StoredProcedure [dbo].[qryNewDataSwipeScan_Violation]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryNewDataSwipeScan_Violation]

@SwipeScanID			INTEGER,
@ViolationSequence		TINYINT,
@ViolationStatute		NVARCHAR(30),
@ViolationOffense		NVARCHAR(50)

AS

INSERT INTO [dbo].[SwipeScans_Violations]
           ([SwipeScanID]
           ,[ViolationSequence]
           ,[ViolationStatute]
           ,[ViolationOffense])
     VALUES
           (@SwipeScanID
           ,@ViolationSequence
           ,@ViolationStatute
           ,@ViolationOffense)
GO
/****** Object:  StoredProcedure [dbo].[qryNewDataSwipeScan_TEP]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryNewDataSwipeScan_TEP]

@SwipeScanID			INTEGER,
@ClientID				TINYINT,
@CitationID				NVARCHAR(50),
@ICRID					NVARCHAR(50),
@Disposition			NVARCHAR(50)

AS

INSERT INTO SwipeScans_TEP		( SwipeScanID
								, ClientID
								, CitationID
								, ICRID
								, Disposition
								, UpdateTS )
						VALUES (@SwipeScanID
								, @ClientID
								, @CitationID
								, @ICRID
								, @Disposition
								, GETDATE())
GO
/****** Object:  StoredProcedure [dbo].[qryNewDataSwipeScan_MID]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryNewDataSwipeScan_MID]

@SwipeScanID			INTEGER,
@IDNumber				NVARCHAR(35),
@NameFirst				NVARCHAR(35),
@NameLast				NVARCHAR(35),
@NameMiddle				NVARCHAR(35),
@DateOfBirth			NVARCHAR(10),
@Age					SMALLINT,
@Height					NVARCHAR(10),
@Weight					NVARCHAR(10),
@Eyes					NVARCHAR(10),
@Hair					NVARCHAR(10),
@DateOfIssue			NVARCHAR(10),
@DateOfExpiration		NVARCHAR(10),
@SwipeRawData			NVARCHAR(max)

AS

INSERT INTO SwipeScans_MID ( SwipeScanID
								, IDNumber
								, NameFirst
								, NameLast
								, NameMiddle
								, DateOfBirth
								, Age
								, [Height]
								, [Weight]
								, Eyes
								, Hair
								, DateOfIssue
								, DateOfExpiration
								, SwipeRawData )
						VALUES (@SwipeScanID
								, @IDNumber
								, @NameFirst
								, @NameLast
								, @NameMiddle
								, @DateOfBirth
								, @Age
								, @Height
								, @Weight
								, @Eyes
								, @Hair
								, @DateOfIssue
								, @DateOfExpiration
								, @SwipeRawData)
GO
/****** Object:  StoredProcedure [dbo].[qryNewDataSwipeScan_INS]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryNewDataSwipeScan_INS]

@SwipeScanID			INTEGER,
@IDNumber				NVARCHAR(35),
@NameFirst				NVARCHAR(35),
@NameLast				NVARCHAR(35),
@NameMiddle				NVARCHAR(35),
@DateOfBirth			NVARCHAR(10),
@Age					SMALLINT,
@Sex					NVARCHAR(1),
@DateOfIssue			NVARCHAR(10),
@DateOfExpiration		NVARCHAR(10),
@SwipeRawData			NVARCHAR(max)

AS

INSERT INTO SwipeScans_INS ( SwipeScanID
								, IDNumber
								, NameFirst
								, NameLast
								, NameMiddle
								, DateOfBirth
								, Age
								, Sex
								, DateOfIssue
								, DateOfExpiration
								, SwipeRawData )
						VALUES (@SwipeScanID
								, @IDNumber
								, @NameFirst
								, @NameLast
								, @NameMiddle
								, @DateOfBirth
								, @Age
								, @Sex
								, @DateOfIssue
								, @DateOfExpiration
								, @SwipeRawData)
GO
/****** Object:  StoredProcedure [dbo].[qryNewDataSwipeScan_DLID]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryNewDataSwipeScan_DLID]

@SwipeScanID			INTEGER,
@IDNumber				NVARCHAR(35),
@NameFirst				NVARCHAR(35),
@NameLast				NVARCHAR(35),
@NameMiddle				NVARCHAR(35),
@DateOfBirth			NVARCHAR(10),
@Age					SMALLINT,
@Sex					NVARCHAR(1),
@Height					NVARCHAR(10),
@Weight					NVARCHAR(10),
@Eyes					NVARCHAR(10),
@Hair					NVARCHAR(10),
@DateOfIssue			NVARCHAR(10),
@DateOfExpiration		NVARCHAR(10),
@AddressStreet			NVARCHAR(35),
@AddressCity			NVARCHAR(35),
@AddressState			NVARCHAR(2),
@AddressZip				NVARCHAR(10),
@SwipeRawData			NVARCHAR(max)

AS

INSERT INTO SwipeScans_DLID ( SwipeScanID
								, IDNumber
								, NameFirst
								, NameLast
								, NameMiddle
								, DateOfBirth
								, Age
								, Sex
								, [Height]
								, [Weight]
								, Eyes
								, Hair
								, DateOfIssue
								, DateOfExpiration
								, AddressStreet
								, AddressCity
								, AddressState
								, AddressZip
								, SwipeRawData )
						VALUES (@SwipeScanID
								, @IDNumber
								, @NameFirst
								, @NameLast
								, @NameMiddle
								, @DateOfBirth
								, @Age
								, @Sex
								, @Height
								, @Weight
								, @Eyes
								, @Hair
								, @DateOfIssue
								, @DateOfExpiration
								, @AddressStreet
								, @AddressCity
								, @AddressState
								, @AddressZip
								, @SwipeRawData)
GO
/****** Object:  StoredProcedure [dbo].[qryNewDataSwipeScan_CK]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryNewDataSwipeScan_CK]

@SwipeScanID			INTEGER,
@RoutingNumber			NVARCHAR(9),
@AccountNumber			NVARCHAR(15),
@CheckNumber			INTEGER,
@SwipeRawData			NVARCHAR(75)

AS
INSERT INTO SwipeScans_CK ( SwipeScanID
								, RoutingNumber
								, AccountNumber
								, CheckNumber
								, SwipeRawData )
						VALUES (@SwipeScanID
								, @RoutingNumber
								, @AccountNumber
								, @CheckNumber
								, @SwipeRawData)
GO
/****** Object:  StoredProcedure [dbo].[qryNewDataSwipeScan_CC]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryNewDataSwipeScan_CC]

@SwipeScanID			INTEGER,
@CCNumber				NVARCHAR(35),
@CCIssuer				NVARCHAR(35),
@NameFirst				NVARCHAR(35),
@NameLast				NVARCHAR(35),
@NameMiddle				NVARCHAR(35),
@DateOfExpiration		NVARCHAR(10),
@SwipeRawData			NVARCHAR(255)

AS

INSERT INTO SwipeScans_CC ( SwipeScanID
								, CCNumber
								, CCIssuer
								, NameFirst
								, NameLast
								, NameMiddle
								, DateOfExpiration
								, SwipeRawData )
						VALUES (@SwipeScanID
								, @CCNumber
								, @CCIssuer
								, @NameFirst
								, @NameLast
								, @NameMiddle
								, @DateOfExpiration
								, @SwipeRawData)
GO
/****** Object:  StoredProcedure [dbo].[qryNewDataSwipeScan]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryNewDataSwipeScan]

@SwipeScanType			NVARCHAR(255),
@ClientID				INTEGER,
@UserID					INTEGER,
@DataSource				NVARCHAR(10)

AS

INSERT INTO SwipeScans ( SwipeScanType
							, ClientID
							, UserID
							, SwipeScanTS
							, DataSource
							, ImageAvailable)
					VALUES (@SwipeScanType
							, @ClientID
							, @UserID
							, GETDATE()
							, @DataSource
							, 0)
GO
/****** Object:  StoredProcedure [dbo].[qryNewClient]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryNewClient]

@ImageLocation			NVARCHAR(255),
@ClientHostName			NVARCHAR(255),
@ClientIPAddress		NVARCHAR(255)

AS

INSERT INTO Clients ( DeviceType
						, DeviceID
						, ComPort
						, IDecodeLicense
						, IDecodeTrackFormat
						, IDecodeCardTypes
						, Location
						, Station
						, Phone
						, Email
						, SkipLogon
						, DisplayAdmin
						, DefaultUser
						, AgeHighlight
						, AgePopup
						, Age
						, ImageSave
						, ImageLocation
						, ViewingTime
						, CCDigits
						, DisableCCSave
						, DisableDBSave
						, ClientHostName
						, ClientIPAddress
						, LogRetention
						, DBRetention
						, UpdateTS )
				VALUES( 'UNK'
						, 0 
						, 'UNK' 
						, 'IDN2-XXXX-XXXX-XXXX-XXXX-XXXXX' 
						, '1,T1,S"%",D,E"?",T2,S";",D,E"?",T3,S"%",S"+",S"#",S"!",S";",D,E"?"' 
						, 'DL,CC,DOD,INS' 
						, 'IVS Location' 
						, 'IVS Station' 
						, '612-555-1234'
						, 'IVS@something.com' 
						, 1 
						, 0
						, 3 
						, 1
						, 0
						, 21
						, 1
						, @ImageLocation
						, 30
						, 4
						, 0
						, 0
						, @ClientHostName
						, @ClientIPAddress
						, 30
						, 1
						, GETDATE())
GO
/****** Object:  StoredProcedure [dbo].[qryNewBatchSwipeScan]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryNewBatchSwipeScan]

@BatchID				NVARCHAR(50),
@ClientID				INTEGER,
@UserID					INTEGER,
@RoutingNumber			NVARCHAR(9),
@AccountNumber			NVARCHAR(15),
@CheckNumber			INTEGER,
@ImagePath				NVARCHAR(75),
@SwipeRawData			NVARCHAR(75)

AS

INSERT INTO SwipeScans_CK_Batch ( BatchID
							, ClientID
							, UserID
							, RoutingNumber
							, AccountNumber
							, CheckNumber
							, ImagePath
							, SwipeRawData
							, SwipeScanTS)
					VALUES (@BatchID
							, @ClientID
							, @UserID
							, @RoutingNumber
							, @AccountNumber
							, @CheckNumber
							, @ImagePath
							, @SwipeRawData
							, GETDATE())
GO
/****** Object:  StoredProcedure [dbo].[qryNewAlert]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryNewAlert]

@IDNumber			NVARCHAR(35),
@NameFirst			NVARCHAR(35),
@NameLast			NVARCHAR(35),
@DateOfBirth		NVARCHAR(10),
@AlertContactName	NVARCHAR(35),
@AlertContactNumber NVARCHAR(20),
@AlertNotes			NVARCHAR(255),
@ActiveFlag			BIT,
@UserID				INTEGER

AS

INSERT INTO Alerts ( IDNumber
						, NameFirst
						, NameLast
						, DateOfBirth
						, AlertContactName
						, AlertContactNumber
						, AlertNotes
						, ActiveFlag
						, UserID
						, UpdateTS )
				VALUES	( @IDNumber
						, @NameFirst
						, @NameLast
						, @DateOfBirth
						, @AlertContactName
						, @AlertContactNumber
						, @AlertNotes
						, @ActiveFlag
						, @UserID
						, GETDATE())
GO
/****** Object:  StoredProcedure [dbo].[qryIsUserNameAvailable]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryIsUserNameAvailable]

@UserName				NVARCHAR(15)

AS

SELECT Count(UserID) AS CountOfUserID
FROM Users
GROUP BY UserName
HAVING UserName=@UserName
GO
/****** Object:  StoredProcedure [dbo].[qryIsUserAuthenticated]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryIsUserAuthenticated]

@UserName				NVARCHAR(15),
@Password				NVARCHAR(10)

AS

SELECT UserID
FROM Users
WHERE [UserName]=@UserName AND [Password]=@Password
GO
/****** Object:  StoredProcedure [dbo].[qryGetWebServiceSettings]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetWebServiceSettings]

AS

SELECT IDecodeLicense, IDecodeTrackFormat, IDecodeCardTypes
FROM WebService
GO
/****** Object:  StoredProcedure [dbo].[qryGetUsers]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetUsers]

AS

SELECT UserID, UserName, UserNameFirst, UserNameLast, ActiveFlag
FROM Users
WHERE UserID>2
ORDER BY UserName
GO
/****** Object:  StoredProcedure [dbo].[qryGetUserPhone]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetUserPhone]

@UserID	INTEGER

AS

SELECT [UserPhone]
FROM Users
WHERE UserID=@UserID
GO
/****** Object:  StoredProcedure [dbo].[qryGetUserNames]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetUserNames]

AS

SELECT UserID, [UserNameFirst] + ' ' + [UserNameLast] AS UserName, UserNameFirst
FROM Users
WHERE UserID>2 AND ActiveFlag=1
ORDER BY [UserNameFirst] + ' ' + [UserNameLast];
GO
/****** Object:  StoredProcedure [dbo].[qryGetUserName]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetUserName]

@UserID	INTEGER

AS

SELECT [UserNameFirst] + ' ' + [UserNameLast] AS UserName
FROM Users
WHERE UserID=@UserID
GO
/****** Object:  StoredProcedure [dbo].[qryGetUserDetail]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetUserDetail]

@UserID	INTEGER

AS

SELECT UserName, [Password], UserNameFirst, UserNameLast, UserEmail, UserPhone
, AdminFlag, AlertFlag,SearchFlag, ActiveFlag, UpdateTS
FROM Users
WHERE UserID=@UserID
GO
/****** Object:  StoredProcedure [dbo].[qryGetUserContactInfo]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetUserContactInfo]

@UserID	INTEGER

AS

SELECT [UserNameFirst] + ' ' + [UserNameLast] AS UserName, UserPhone
FROM Users
WHERE UserID=@UserID
GO
/****** Object:  StoredProcedure [dbo].[qryGetTEPViolations]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetTEPViolations]

AS

SELECT [ViolationID],[Statute],[Offense]
  FROM [dbo].[TEPViolations]
ORDER BY [Statute]
GO
/****** Object:  StoredProcedure [dbo].[qryGetTEPClientSettings]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetTEPClientSettings]

@ClientID			INTEGER

AS

SELECT [CitationPrefix]
      ,[ICRPrefix]
  FROM [dbo].[TEPClients]
WHERE ClientID=@ClientID
GO
/****** Object:  StoredProcedure [dbo].[qryGetSwipeScanType]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetSwipeScanType]

@SwipeScanID	INTEGER

AS

SELECT SwipeScanType
FROM SwipeScans
WHERE SwipeScanID=@SwipeScanID
GO
/****** Object:  StoredProcedure [dbo].[qryGetSwipeScanSearch_MID]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetSwipeScanSearch_MID]

AS

SELECT SwipeScans.SwipeScanID, SwipeScans.SwipeScanType, Clients.Location, Clients.Station, Users.[UserNameFirst] + ' ' + [UserNameLast] AS UserName
, SwipeScans.SwipeScanTS, SwipeScans_MID.IDNumber AS IDAccountNumber, SwipeScans_MID.NameFirst, SwipeScans_MID.NameLast, SwipeScans_MID.DateOfBirth
, SwipeScans.ImageAvailable, SwipeScans.ClientID
FROM SwipeScans
INNER JOIN Clients ON SwipeScans.ClientID = Clients.ClientID
INNER JOIN Users ON SwipeScans.UserID = Users.UserID
INNER JOIN SwipeScans_MID ON SwipeScans.SwipeScanID = SwipeScans_MID.SwipeScanID
ORDER BY SwipeScans.SwipeScanID DESC;
GO
/****** Object:  StoredProcedure [dbo].[qryGetSwipeScanSearch_INS]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetSwipeScanSearch_INS]

AS

SELECT SwipeScans.SwipeScanID, SwipeScans.SwipeScanType, Clients.Location, Clients.Station, Users.[UserNameFirst] + ' ' + [UserNameLast] AS UserName
, SwipeScans.SwipeScanTS, SwipeScans_INS.IDNumber AS IDAccountNumber, SwipeScans_INS.NameFirst, SwipeScans_INS.NameLast, SwipeScans_INS.DateOfBirth
, SwipeScans.ImageAvailable, SwipeScans.ClientID
FROM SwipeScans
INNER JOIN Clients ON SwipeScans.ClientID = Clients.ClientID
INNER JOIN Users ON SwipeScans.UserID = Users.UserID
INNER JOIN SwipeScans_INS ON SwipeScans.SwipeScanID = SwipeScans_INS.SwipeScanID
ORDER BY SwipeScans.SwipeScanID DESC;
GO
/****** Object:  StoredProcedure [dbo].[qryGetSwipeScanSearch_DLID]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetSwipeScanSearch_DLID]

AS

SELECT SwipeScans.SwipeScanID, SwipeScans.SwipeScanType, Clients.Location, Clients.Station, Users.[UserNameFirst] + ' ' + [UserNameLast] AS UserName
, SwipeScans.SwipeScanTS, SwipeScans_DLID.IDNumber AS IDAccountNumber, SwipeScans_DLID.NameFirst,SwipeScans_DLID.NameLast, SwipeScans_DLID.DateOfBirth
, SwipeScans.ImageAvailable, SwipeScans.ClientID
FROM SwipeScans
INNER JOIN Clients ON SwipeScans.ClientID = Clients.ClientID
INNER JOIN Users ON SwipeScans.UserID = Users.UserID
INNER JOIN SwipeScans_DLID ON SwipeScans.SwipeScanID = SwipeScans_DLID.SwipeScanID
ORDER BY SwipeScans.SwipeScanID DESC;
GO
/****** Object:  StoredProcedure [dbo].[qryGetSwipeScanSearch_CK]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetSwipeScanSearch_CK]

AS

SELECT SwipeScans.SwipeScanID,SwipeScans.SwipeScanType, Clients.Location, Clients.Station, Users.[UserNameFirst] + ' ' + [UserNameLast] AS UserName
, SwipeScans.SwipeScanTS, [RoutingNumber] + ':' + [AccountNumber] AS IDAccountNumber, '' AS NameFirst, '' AS NameLast, '' AS DateOfBirth, SwipeScans.ImageAvailable
, SwipeScans.ClientID
FROM SwipeScans
INNER JOIN Clients ON SwipeScans.ClientID = Clients.ClientID
INNER JOIN Users ON SwipeScans.UserID = Users.UserID
INNER JOIN SwipeScans_CK ON SwipeScans.SwipeScanID = SwipeScans_CK.SwipeScanID
ORDER BY SwipeScans.SwipeScanID DESC;
GO
/****** Object:  StoredProcedure [dbo].[qryGetSwipeScanSearch_CC]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetSwipeScanSearch_CC]

AS

SELECT SwipeScans.SwipeScanID, SwipeScans.SwipeScanType, Clients.Location, Clients.Station, Users.[UserNameFirst] + ' ' + [UserNameLast] AS UserName
, SwipeScans.SwipeScanTS, SwipeScans_CC.CCNumber AS IDAccountNumber, SwipeScans_CC.NameFirst, SwipeScans_CC.NameLast, '' AS DateOfBirth, SwipeScans.ImageAvailable
, SwipeScans.ClientID
FROM SwipeScans
INNER JOIN Clients ON SwipeScans.ClientID = Clients.ClientID
INNER JOIN Users ON SwipeScans.UserID = Users.UserID
INNER JOIN SwipeScans_CC ON SwipeScans.SwipeScanID = SwipeScans_CC.SwipeScanID
ORDER BY SwipeScans.SwipeScanID DESC;
GO
/****** Object:  StoredProcedure [dbo].[qryGetSwipeScanHistory_MID]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetSwipeScanHistory_MID]

@IDAccountNumber			NVARCHAR(35)

AS

SELECT SwipeScans.SwipeScanID, Clients.Location, Clients.Station, Clients.Phone, Users.[UserNameFirst] + ' ' + [UserNameLast] AS UserName, SwipeScans.SwipeScanTS
FROM SwipeScans
INNER JOIN Clients ON SwipeScans.ClientID = Clients.ClientID
INNER JOIN Users ON SwipeScans.UserID = Users.UserID
INNER JOIN SwipeScans_MID ON SwipeScans.SwipeScanID = SwipeScans_MID.SwipeScanID
WHERE SwipeScans_MID.IDNumber=@IDAccountNumber
ORDER BY SwipeScans.SwipeScanID DESC;
GO
/****** Object:  StoredProcedure [dbo].[qryGetSwipeScanHistory_INS]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetSwipeScanHistory_INS]

@IDAccountNumber			NVARCHAR(35)

AS

SELECT SwipeScans.SwipeScanID, Clients.Location, Clients.Station, Clients.Phone, Users.[UserNameFirst] + ' ' + [UserNameLast] AS UserName, SwipeScans.SwipeScanTS
FROM SwipeScans
INNER JOIN Clients ON SwipeScans.ClientID = Clients.ClientID
INNER JOIN Users ON SwipeScans.UserID = Users.UserID
INNER JOIN SwipeScans_INS ON SwipeScans.SwipeScanID = SwipeScans_INS.SwipeScanID
WHERE SwipeScans_INS.IDNumber=@IDAccountNumber
ORDER BY SwipeScans.SwipeScanID DESC;
GO
/****** Object:  StoredProcedure [dbo].[qryGetSwipeScanHistory_DLID]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetSwipeScanHistory_DLID]

@IDAccountNumber			NVARCHAR(35)

AS

SELECT SwipeScans.SwipeScanID, Clients.Location, Clients.Station, Clients.Phone, Users.[UserNameFirst] + ' ' + [UserNameLast] AS UserName, SwipeScans.SwipeScanTS
FROM SwipeScans
INNER JOIN Clients ON SwipeScans.ClientID = Clients.ClientID
INNER JOIN Users ON SwipeScans.UserID = Users.UserID
INNER JOIN SwipeScans_DLID ON SwipeScans.SwipeScanID = SwipeScans_DLID.SwipeScanID
WHERE SwipeScans_DLID.IDNumber=@IDAccountNumber
ORDER BY SwipeScans.SwipeScanID DESC;
GO
/****** Object:  StoredProcedure [dbo].[qryGetSwipeScanHistory_CK]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetSwipeScanHistory_CK]

@RoutingNumber			NVARCHAR(35),
@AccountNumber			NVARCHAR(35)

AS

SELECT SwipeScans.SwipeScanID, Clients.Location, Clients.Station, Clients.Phone, Users.[UserNameFirst] + ' ' + [UserNameLast] AS UserName, SwipeScans.SwipeScanTS
FROM SwipeScans
INNER JOIN Clients ON SwipeScans.ClientID = Clients.ClientID
INNER JOIN Users ON SwipeScans.UserID = Users.UserID
INNER JOIN SwipeScans_CK ON SwipeScans.SwipeScanID = SwipeScans_CK.SwipeScanID
WHERE SwipeScans_CK.RoutingNumber=@RoutingNumber AND SwipeScans_CK.AccountNumber=@AccountNumber
ORDER BY SwipeScans.SwipeScanID DESC;
GO
/****** Object:  StoredProcedure [dbo].[qryGetSwipeScanHistory_CC]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetSwipeScanHistory_CC]

@IDAccountNumber			NVARCHAR(35)

AS

SELECT SwipeScans.SwipeScanID, Clients.Location, Clients.Station, Clients.Phone, Users.[UserNameFirst] + ' ' + [UserNameLast] AS UserName, SwipeScans.SwipeScanTS
FROM SwipeScans
INNER JOIN Clients ON SwipeScans.ClientID = Clients.ClientID
INNER JOIN Users ON SwipeScans.UserID = Users.UserID
INNER JOIN SwipeScans_CC ON SwipeScans.SwipeScanID = SwipeScans_CC.SwipeScanID
WHERE SwipeScans_CC.CCNumber=@IDAccountNumber
ORDER BY SwipeScans.SwipeScanID DESC
GO
/****** Object:  StoredProcedure [dbo].[qryGetSwipeScanDetail_Violations]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetSwipeScanDetail_Violations]

@SwipeScanID			INTEGER

AS

SELECT [ViolationSequence]
      ,[ViolationStatute]
      ,[ViolationOffense]
  FROM [dbo].[SwipeScans_Violations]
WHERE SwipeScanID=@SwipeScanID
GO
/****** Object:  StoredProcedure [dbo].[qryGetSwipeScanDetail_TEP]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetSwipeScanDetail_TEP]

@SwipeScanID			INTEGER

AS

SELECT [ClientID]
      ,[CitationID]
      ,[ICRID]
      ,[Disposition]
      ,[UpdateTS]
  FROM [dbo].[SwipeScans_TEP]
WHERE SwipeScanID=@SwipeScanID
GO
/****** Object:  StoredProcedure [dbo].[qryGetSwipeScanDetail_MID]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetSwipeScanDetail_MID]

@SwipeScanID			INTEGER

AS

SELECT SwipeScans_MID.IDNumber, SwipeScans_MID.NameFirst, SwipeScans_MID.NameLast, SwipeScans_MID.NameMiddle, SwipeScans_MID.DateOfBirth
, SwipeScans_MID.Age, SwipeScans_MID.Height, SwipeScans_MID.Weight, SwipeScans_MID.Eyes, SwipeScans_MID.Hair, SwipeScans_MID.DateOfIssue
, SwipeScans_MID.DateOfExpiration, [UserNameFirst] + ' ' + [UserNameLast] AS UserName, Clients.Location, SwipeScans_MID.SwipeRawData,SwipeScans.DataSource, CaseID
FROM SwipeScans 
INNER JOIN Clients ON SwipeScans.ClientID = Clients.ClientID
INNER JOIN Users ON SwipeScans.UserID = Users.UserID
INNER JOIN SwipeScans_MID ON SwipeScans.SwipeScanID = SwipeScans_MID.SwipeScanID
WHERE SwipeScans.SwipeScanID=@SwipeScanID
GO
/****** Object:  StoredProcedure [dbo].[qryGetSwipeScanDetail_INS]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetSwipeScanDetail_INS]

@SwipeScanID			INTEGER

AS

SELECT SwipeScans_INS.IDNumber, SwipeScans_INS.NameFirst, SwipeScans_INS.NameLast, SwipeScans_INS.NameMiddle, SwipeScans_INS.DateOfBirth
, SwipeScans_INS.Age, SwipeScans_INS.Sex, SwipeScans_INS.DateOfIssue, SwipeScans_INS.DateOfExpiration
, [UserNameFirst] + ' ' + [UserNameLast] AS UserName, Clients.Location, SwipeScans_INS.SwipeRawData,SwipeScans.DataSource, CaseID
FROM SwipeScans 
INNER JOIN Clients ON SwipeScans.ClientID = Clients.ClientID
INNER JOIN Users ON SwipeScans.UserID = Users.UserID
INNER JOIN SwipeScans_INS ON SwipeScans.SwipeScanID = SwipeScans_INS.SwipeScanID
WHERE SwipeScans.SwipeScanID=@SwipeScanID
GO
/****** Object:  StoredProcedure [dbo].[qryGetSwipeScanDetail_DLID]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetSwipeScanDetail_DLID]

@SwipeScanID			INTEGER

AS

SELECT SwipeScans_DLID.IDNumber, SwipeScans_DLID.NameFirst,SwipeScans_DLID.NameLast, SwipeScans_DLID.NameMiddle, SwipeScans_DLID.DateOfBirth, SwipeScans_DLID.Age
, SwipeScans_DLID.Sex, SwipeScans_DLID.Height, SwipeScans_DLID.Weight, SwipeScans_DLID.Eyes
, SwipeScans_DLID.Hair, SwipeScans_DLID.DateOfIssue, SwipeScans_DLID.DateOfExpiration, SwipeScans_DLID.AddressStreet, SwipeScans_DLID.AddressCity
, SwipeScans_DLID.AddressState, SwipeScans_DLID.AddressZip, [UserNameFirst] + ' ' + [UserNameLast] AS UserName, Clients.Location
, SwipeScans_DLID.SwipeRawData,SwipeScans.DataSource, CaseID
FROM SwipeScans 
INNER JOIN Clients ON SwipeScans.ClientID = Clients.ClientID
INNER JOIN Users ON SwipeScans.UserID = Users.UserID
INNER JOIN SwipeScans_DLID ON SwipeScans.SwipeScanID = SwipeScans_DLID.SwipeScanID
WHERE SwipeScans.SwipeScanID=@SwipeScanID
GO
/****** Object:  StoredProcedure [dbo].[qryGetSwipeScanDetail_CK]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetSwipeScanDetail_CK]

@SwipeScanID			INTEGER

AS

SELECT [RoutingNumber] + ':' + [AccountNumber] AS IDAccountNumber, SwipeScans_CK.CheckNumber, [UserNameFirst] + ' ' + [UserNameLast] AS UserName
, Clients.Location, SwipeScans_CK.SwipeRawData ,SwipeScans.DataSource, CaseID
FROM SwipeScans 
INNER JOIN Clients ON SwipeScans.ClientID = Clients.ClientID
INNER JOIN Users ON SwipeScans.UserID = Users.UserID
INNER JOIN SwipeScans_CK ON SwipeScans.SwipeScanID = SwipeScans_CK.SwipeScanID
WHERE SwipeScans_CK.SwipeScanID=@SwipeScanID
GO
/****** Object:  StoredProcedure [dbo].[qryGetSwipeScanDetail_CC]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetSwipeScanDetail_CC]

@SwipeScanID			INTEGER

AS

SELECT SwipeScans_CC.CCNumber, SwipeScans_CC.CCIssuer, SwipeScans_CC.NameFirst, SwipeScans_CC.NameLast, SwipeScans_CC.NameMiddle
, SwipeScans_CC.DateOfExpiration, [UserNameFirst] + ' ' + [UserNameLast] AS UserName, Clients.Location, SwipeScans_CC.SwipeRawData ,SwipeScans.DataSource, CaseID
FROM SwipeScans 
INNER JOIN Clients ON SwipeScans.ClientID = Clients.ClientID 
INNER JOIN Users ON SwipeScans.UserID = Users.UserID 
INNER JOIN SwipeScans_CC ON SwipeScans.SwipeScanID = SwipeScans_CC.SwipeScanID
WHERE SwipeScans_CC.SwipeScanID=@SwipeScanID
GO
/****** Object:  StoredProcedure [dbo].[qryGetSwipeScanAlerts]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetSwipeScanAlerts]

@IDNumber			NVARCHAR(35),
@NameFirst			NVARCHAR(35),
@NameLast			NVARCHAR(35)

AS

SELECT AlertID,UpdateTS, NameFirst, NameLast, IDNumber
, case when NameLast=@NameLast then 'TRUE' else'FALSE' end as 'MatchLast'
, case when IDNumber=@IDNumber then 'TRUE' else'FALSE' end as 'MatchID'
FROM Alerts
WHERE (IDNumber=@IDNumber AND ActiveFlag=1) OR (NameLast=@NameLast AND ActiveFlag=1)
ORDER BY 6 DESC, AlertID DESC;
GO
/****** Object:  StoredProcedure [dbo].[qryGetStationName]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetStationName]

@ClientID			INTEGER

AS

SELECT Station
FROM Clients
WHERE ClientID=@ClientID
GO
/****** Object:  StoredProcedure [dbo].[qryGetLocations]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetLocations]

AS

SELECT ClientID, [Location] + '-' + [Station] AS IVSLocation
FROM Clients;
GO
/****** Object:  StoredProcedure [dbo].[qryGetDeviceInfo]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetDeviceInfo]

@ClientID			INTEGER

AS

SELECT Devices.DeviceID, Devices.DeviceType,Devices.ModelNo, Devices.SerialNo, Devices.FirmwareRev, Devices.FirmwareDate
, Devices.HardwareRev, Clients.ComPort, Devices.UpdateTS
FROM Devices INNER JOIN Clients ON Devices.ClientID = Clients.ClientID
WHERE Devices.ClientID=@ClientID
GO
/****** Object:  StoredProcedure [dbo].[qryGetClientSettings]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetClientSettings]

@ClientID			INTEGER

AS

SELECT DeviceType, DeviceID, ComPort, IDecodeLicense, IDecodeTrackFormat, IDecodeCardTypes
, Location, Station, Phone, Email, SkipLogon, DisplayAdmin, DefaultUser, AgeHighlight
, AgePopup, Age, ImageSave, ImageLocation, ViewingTime, CCDigits, DisableCCSave, DisableDBSave, LogRetention
FROM Clients
WHERE ClientID=@ClientID
GO
/****** Object:  StoredProcedure [dbo].[qryGetClients]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetClients]

AS

SELECT ClientID, Location, Station
FROM Clients
GO
/****** Object:  StoredProcedure [dbo].[qryGetClientID]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetClientID]

@ClientHostName			NVARCHAR(255)

AS

SELECT ClientID
FROM Clients
WHERE ClientHostName=@ClientHostName
GO
/****** Object:  StoredProcedure [dbo].[qryGetAlerts]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetAlerts]

AS

SELECT AlertID, IDNumber,NameLast,ActiveFlag, UpdateTS
FROM Alerts
ORDER BY AlertID
GO
/****** Object:  StoredProcedure [dbo].[qryGetAlertDetail]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryGetAlertDetail]

@AlertID			INTEGER

AS

SELECT	IDNumber
		, NameFirst
		, NameLast
		, DateOfBirth
		, AlertContactName
		, AlertContactNumber
		, AlertNotes
		, Alerts.ActiveFlag
		, Alerts.UserID
		, [UserNameFirst] + ' ' + [UsernameLast] AS UserName
		, Alerts.UpdateTS
FROM Alerts INNER JOIN Users ON Alerts.UserID = Users.UserID
WHERE Alerts.AlertID=@AlertID
GO
/****** Object:  StoredProcedure [dbo].[qryEnableUser]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryEnableUser]

@ActiveFlag				BIT,
@UserID					INTEGER

AS

UPDATE Users SET ActiveFlag = @ActiveFlag, UpdateTS = GETDATE()
WHERE UserID=@UserID
GO
/****** Object:  StoredProcedure [dbo].[qryEnableAlert]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryEnableAlert]

@ActiveFlag				BIT,
@AlertID				INTEGER

AS

UPDATE Alerts SET ActiveFlag = @ActiveFlag, UpdateTS = GETDATE()
WHERE AlertID=@AlertID
GO
/****** Object:  StoredProcedure [dbo].[qryDeleteUser]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryDeleteUser]

@UserID				INTEGER

AS

DELETE FROM Users
WHERE [UserID]=@UserID
GO
/****** Object:  StoredProcedure [dbo].[qryDeleteSwipeScan_MID]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryDeleteSwipeScan_MID]

@SwipeScanID				INTEGER

AS

DELETE FROM SwipeScans_MID
WHERE [SwipeScanID]=@SwipeScanID
GO
/****** Object:  StoredProcedure [dbo].[qryDeleteSwipeScan_INS]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryDeleteSwipeScan_INS]

@SwipeScanID				INTEGER

AS

DELETE FROM SwipeScans_INS
WHERE [SwipeScanID]=@SwipeScanID
GO
/****** Object:  StoredProcedure [dbo].[qryDeleteSwipeScan_DLID]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryDeleteSwipeScan_DLID]

@SwipeScanID				INTEGER

AS

DELETE FROM SwipeScans_DLID
WHERE [SwipeScanID]=@SwipeScanID
GO
/****** Object:  StoredProcedure [dbo].[qryDeleteSwipeScan_CK]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryDeleteSwipeScan_CK]

@SwipeScanID				INTEGER

AS

DELETE FROM SwipeScans_CK
WHERE [SwipeScanID]=@SwipeScanID
GO
/****** Object:  StoredProcedure [dbo].[qryDeleteSwipeScan_CC]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryDeleteSwipeScan_CC]

@SwipeScanID				INTEGER

AS

DELETE FROM SwipeScans_CC
WHERE [SwipeScanID]=@SwipeScanID
GO
/****** Object:  StoredProcedure [dbo].[qryDeleteSwipeScan]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryDeleteSwipeScan]

@SwipeScanID				INTEGER

AS

DELETE FROM SwipeScans
WHERE [SwipeScanID]=@SwipeScanID
GO
/****** Object:  StoredProcedure [dbo].[qryDeleteAlert]    Script Date: 11/14/2012 16:55:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[qryDeleteAlert]

@AlertID				INTEGER

AS

DELETE FROM Alerts
WHERE [AlertID]=@AlertID
GO

INSERT INTO [dbo].[Users]
           ([UserName]
           ,[Password]
           ,[UserNameFirst]
           ,[UserNameLast]
           ,[UserEmail]
           ,[UserPhone]
           ,[AdminFlag]
           ,[AlertFlag]
           ,[SearchFlag]
           ,[ActiveFlag]
           ,[UpdateTS])
     VALUES
           ('IVS','1234','IVS',NULL,NULL,NULL,1,1,1,1,GETDATE())
GO
INSERT INTO [dbo].[Users]
           ([UserName]
           ,[Password]
           ,[UserNameFirst]
           ,[UserNameLast]
           ,[UserEmail]
           ,[UserPhone]
           ,[AdminFlag]
           ,[AlertFlag]
           ,[SearchFlag]
           ,[ActiveFlag]
           ,[UpdateTS])
     VALUES
           ('IVS','1234','admin','Administrator',NULL,NULL,1,1,1,1,GETDATE())
GO
INSERT INTO [dbo].[Users]
           ([UserName]
           ,[Password]
           ,[UserNameFirst]
           ,[UserNameLast]
           ,[UserEmail]
           ,[UserPhone]
           ,[AdminFlag]
           ,[AlertFlag]
           ,[SearchFlag]
           ,[ActiveFlag]
           ,[UpdateTS])
     VALUES
           ('IVS','1234','user','User',NULL,NULL,0,1,1,1,GETDATE())
GO
