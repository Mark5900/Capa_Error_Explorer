CREATE TABLE [dbo].[Capa_Errors]
(
    [UmitID] INT NOT NULL,
    [PackageID] INT NOT NULL,
    [Status] VARCHAR (10) NULL,
    [LastRunDate] INT NULL,
    [RunCount] INT NULL,
    [ErrorType] VARCHAR (200) NULL,
    [UnitGUID] UNIQUEIDENTIFIER NOT NULL,
    [PackageGUID] UNIQUEIDENTIFIER NOT NULL,
    [UnitName] VARCHAR (150) NULL,
    [PackageName] VARCHAR (50) NULL,
    [PackageVersion] VARCHAR (50) NULL,
    [CMPID] INT NULL,
    [TYPE] SMALLINT NULL
);