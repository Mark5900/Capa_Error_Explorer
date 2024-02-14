USE [Test]
GO

/****** Object:  Table [dbo].[Capa_Errors_Excluded_GUI]    Script Date: 2/14/2024 4:08:57 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Capa_Errors_Excluded_GUI]') AND type in (N'U'))
DROP TABLE [dbo].[Capa_Errors_Excluded_GUI]
GO

/****** Object:  Table [dbo].[Capa_Errors_Excluded_GUI]    Script Date: 2/14/2024 4:08:57 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Capa_Errors_Excluded_GUI](
	[PackageName] [varchar](50) NULL,
	[PackageVersion] [varchar](50) NULL,
	[TYPE] [smallint] NULL
) ON [PRIMARY]
GO


