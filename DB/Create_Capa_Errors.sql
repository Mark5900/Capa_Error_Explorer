USE [Test]
GO

/****** Object: Table [dbo].[Capa_Errors] Script Date: 12/4/2023 7:00:28 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Capa_Errors] (
    [UnitID]            INT              NOT NULL,
    [PackageID]         INT              NOT NULL,
    [Status]            VARCHAR (20)     NULL,
    [LastRunDate]       INT              NULL,
    [RunCount]          INT              NULL,
    [CurrentErrorType]  VARCHAR (200)    NULL,
    [UnitUUID]          UNIQUEIDENTIFIER NOT NULL,
    [PackageGUID]       UNIQUEIDENTIFIER NOT NULL,
    [UnitName]          VARCHAR (150)    NULL,
    [PackageName]       VARCHAR (50)     NULL,
    [PackageVersion]    VARCHAR (50)     NULL,
    [CMPID]             INT              NULL,
    [TYPE]              SMALLINT         NULL,
    [ErrorCount]        INT              NULL,
    [LastErrorType]     VARCHAR (200)    NULL,
    [CancelledCount]    INT              NULL,
    [PackageRecurrence] NCHAR (10)       NULL
);


