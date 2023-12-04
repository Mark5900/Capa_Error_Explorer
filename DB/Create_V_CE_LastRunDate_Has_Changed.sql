USE [Test]
GO

/****** Object: View [dbo].[V_CE_LastRunDate_Has_Changed] Script Date: 12/4/2023 6:58:42 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW dbo.V_CE_LastRunDate_Has_Changed AS
SELECT CMS.dbo.UNITJOB.UNITID, CMS.dbo.UNITJOB.JOBID AS PackageID, CMS.dbo.UNITJOB.[STATUS],  CMS.dbo.UNITJOB.LASTRUNDATE, CMS.dbo.UNITJOB.[LOG]
FROM CMS.dbo.UNITJOB
Inner Join Test.dbo.Capa_Errors ON
	CMS.dbo.UNITJOB.UNITID = Test.dbo.Capa_Errors.UnitID
	AND CMS.dbo.UNITJOB.JOBID = Test.dbo.Capa_Errors.PackageID
WHERE Test.dbo.Capa_Errors.LastRunDate <>  CMS.dbo.UNITJOB.LASTRUNDATE
