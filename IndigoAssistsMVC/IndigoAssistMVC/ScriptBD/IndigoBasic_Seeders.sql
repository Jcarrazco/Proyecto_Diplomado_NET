USE IndigoBasic
GO

-- Seeders exclusivos para IndigoBasic (Identity)

IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetRoles] WHERE [NormalizedName] = 'ADMINISTRADOR')
BEGIN
    INSERT INTO [dbo].[AspNetRoles] (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(),'Administrador','ADMINISTRADOR',NEWID());
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetRoles] WHERE [NormalizedName] = 'SUPERVISOR')
BEGIN
    INSERT INTO [dbo].[AspNetRoles] (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(),'Supervisor','SUPERVISOR',NEWID());
END
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetRoles] WHERE [NormalizedName] = 'TECNICO')
BEGIN
    INSERT INTO [dbo].[AspNetRoles] (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(),'Tecnico','TECNICO',NEWID());
END
GO
