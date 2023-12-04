USE [Test]
GO

/****** Object: View [dbo].[V_CE_UnitInstallDate_Has_Changed] Script Date: 12/4/2023 11:15:14 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW dbo.V_CE_UnitInstallDate_Has_Changed AS
SELECT [CMS].[dbo].[INV].UNITID, [CMS].[dbo].[INV].[VALUE]
FROM [CMS].[dbo].[INV]
INNER JOIN Test.dbo.UnitInstallDate ON 
	[CMS].[dbo].[INV].UNITID = Test.dbo.UnitInstallDate.UNITID
WHERE [CMS].[dbo].[INV].[SECTION] = 'Operating System'
	AND [CMS].[dbo].[INV].[NAME] ='InstallDate'
	AND Test.dbo.UnitInstallDate.[VALUE] <> [CMS].[dbo].[INV].[VALUE]
