USE [Test]
GO

/****** Object: View [dbo].[V_CE_NotIn_UnitInstallDate] Script Date: 12/4/2023 11:01:50 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[V_CE_NotIn_UnitInstallDate] AS
SELECT [CMS].[dbo].[INV].UNITID, [CMS].[dbo].[INV].[VALUE]
FROM [CMS].[dbo].[INV]
LEFT JOIN Test.dbo.UnitInstallDate ON 
	[CMS].[dbo].[INV].UNITID = Test.dbo.UnitInstallDate.UNITID
WHERE Test.dbo.UnitInstallDate.UNITID IS NULL
	AND [CMS].[dbo].[INV].[SECTION] = 'Operating System'
	AND [CMS].[dbo].[INV].[NAME] ='InstallDate'
