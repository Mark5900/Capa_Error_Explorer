USE [Test]
GO

/****** Object: View [dbo].[V_CE_NotIN_UNITJOB] Script Date: 12/4/2023 7:00:05 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW dbo.V_CE_NotIN_UNITJOB AS
SELECT Test.dbo.Capa_Errors.UnitID, Test.dbo.Capa_Errors.PackageID
FROM CMS.dbo.UNITJOB
RIGHT JOIN Test.dbo.Capa_Errors ON
	CMS.dbo.UNITJOB.UNITID = Test.dbo.Capa_Errors.UnitID
	AND CMS.dbo.UNITJOB.JOBID = Test.dbo.Capa_Errors.PackageID
	WHERE CMS.dbo.UNITJOB.UNITID IS NULL
		AND CMS.dbo.UNITJOB.JOBID IS NULL
