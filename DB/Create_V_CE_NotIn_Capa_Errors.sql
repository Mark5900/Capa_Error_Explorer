USE [Test]
GO

/****** Object: View [dbo].[V_CE_NotIn_Capa_Errors] Script Date: 12/4/2023 11:01:37 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[V_CE_NotIn_Capa_Errors] AS
SELECT CMS.dbo.UNITJOB.UNITID, 
	CMS.dbo.UNITJOB.JOBID AS PackageID, 
	CMS.dbo.UNITJOB.[STATUS],  
	CMS.dbo.UNITJOB.LASTRUNDATE, 
	CMS.dbo.UNITJOB.[LOG],
	CMS.dbo.UNIT.UUID AS UnitUUID, 
	CMS.dbo.UNIT.[NAME] AS UnitName, 
	CMS.dbo.UNIT.[TYPE],
	CMS.dbo.JOB.[GUID] AS PackageGUID, 
	CMS.dbo.JOB.[NAME] AS PackageName, 
	CMS.dbo.JOB.[VERSION] AS PackageVersion,
	CMS.dbo.SCHEDULE.RECURRENCE,
	CMS.dbo.JOB.CMPID
FROM CMS.dbo.UNITJOB
LEFT JOIN Test.dbo.Capa_Errors ON
	CMS.dbo.UNITJOB.UNITID = Test.dbo.Capa_Errors.UnitID
		AND CMS.dbo.UNITJOB.JOBID = Test.dbo.Capa_Errors.PackageID
JOIN CMS.dbo.UNIT ON
	CMS.dbo.UNITJOB.UNITID = CMS.dbo.UNIT.UNITID
JOIN CMS.dbo.JOB ON
	CMS.dbo.UNITJOB.JOBID = CMS.dbo.JOB.JOBID
JOIN CMS.dbo.SCHEDULE ON
	CMS.dbo.UNITJOB.JOBID = CMS.dbo.SCHEDULE.ID
	WHERE Test.dbo.Capa_Errors.UnitID IS NULL
		AND Test.dbo.Capa_Errors.PackageID IS NULL
