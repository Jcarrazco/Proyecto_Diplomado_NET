USE [Indigo]
GO

IF OBJECT_ID('dbo.mTiposActivo', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.mTiposActivo (
        IdTipoActivo TINYINT IDENTITY(1,1) NOT NULL,
        TipoActivo NVARCHAR(50) NOT NULL,
        CONSTRAINT PK_mTiposActivo PRIMARY KEY CLUSTERED (IdTipoActivo)
    );
END
GO

IF OBJECT_ID('dbo.mStatusActivo', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.mStatusActivo (
        StatusId TINYINT IDENTITY(1,1) NOT NULL,
        Status NVARCHAR(20) NOT NULL,
        CONSTRAINT PK_mStatusActivo PRIMARY KEY CLUSTERED (StatusId)
    );
END
GO

IF OBJECT_ID('dbo.mProveedoresActivos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.mProveedoresActivos (
        IdProveedor TINYINT IDENTITY(1,1) NOT NULL,
        Proveedor NVARCHAR(120) NOT NULL,
        CONSTRAINT PK_mProveedoresActivos PRIMARY KEY CLUSTERED (IdProveedor)
    );
END
GO

IF OBJECT_ID('dbo.mComponentesActivos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.mComponentesActivos (
        IdComponente TINYINT IDENTITY(1,1) NOT NULL,
        Componente NVARCHAR(80) NOT NULL,
        ValorBit INT NULL,
        CONSTRAINT PK_mComponentesActivos PRIMARY KEY CLUSTERED (IdComponente)
    );
END
GO

IF OBJECT_ID('dbo.mActivos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.mActivos (
        IdActivo INT IDENTITY(1,1) NOT NULL,
        Codigo NVARCHAR(40) NOT NULL,
        Marca NVARCHAR(50) NULL,
        Modelo NVARCHAR(80) NULL,
        Serie NVARCHAR(80) NULL,
        Nombre NVARCHAR(120) NOT NULL,
        PersonaAsign NVARCHAR(120) NULL,
        Ubicacion NVARCHAR(120) NULL,
        FeAlta DATE NOT NULL,
        FeCompra DATE NULL,
        FeBaja DATE NULL,
        CostoCompra DECIMAL(12,2) NULL,
        Notas NVARCHAR(400) NULL,
        CodificacionComponentes INT NULL CONSTRAINT DF_mActivos_CodificacionComponentes DEFAULT (0),
        TieneSoftwareOP BIT NULL CONSTRAINT DF_mActivos_TieneSoftwareOP DEFAULT (0),
        IdTipoActivo TINYINT NULL,
        IdDepartamento TINYINT NULL,
        IdStatus TINYINT NULL,
        IdProveedor TINYINT NULL,
        CONSTRAINT PK_mActivos PRIMARY KEY CLUSTERED (IdActivo)
    );

    IF COL_LENGTH('dbo.mActivos', 'FeAlta') IS NOT NULL
    BEGIN
        ALTER TABLE dbo.mActivos ADD CONSTRAINT DF_mActivos_FeAlta DEFAULT (CAST(GETDATE() AS DATE)) FOR FeAlta;
    END
END
GO

IF OBJECT_ID('dbo.FK_mActivos_TiposActivo', 'F') IS NULL
BEGIN
    ALTER TABLE dbo.mActivos WITH CHECK ADD CONSTRAINT FK_mActivos_TiposActivo
        FOREIGN KEY (IdTipoActivo) REFERENCES dbo.mTiposActivo (IdTipoActivo);
END
GO

IF OBJECT_ID('dbo.FK_mActivos_Status', 'F') IS NULL
BEGIN
    ALTER TABLE dbo.mActivos WITH CHECK ADD CONSTRAINT FK_mActivos_Status
        FOREIGN KEY (IdStatus) REFERENCES dbo.mStatusActivo (StatusId);
END
GO

IF OBJECT_ID('dbo.FK_mActivos_Proveedores', 'F') IS NULL
BEGIN
    ALTER TABLE dbo.mActivos WITH CHECK ADD CONSTRAINT FK_mActivos_Proveedores
        FOREIGN KEY (IdProveedor) REFERENCES dbo.mProveedoresActivos (IdProveedor);
END
GO

IF OBJECT_ID('dbo.FK_mActivos_Departamentos', 'F') IS NULL
BEGIN
    ALTER TABLE dbo.mActivos WITH CHECK ADD CONSTRAINT FK_mActivos_Departamentos
        FOREIGN KEY (IdDepartamento) REFERENCES dbo.mDepartamentos (IdDepto);
END
GO
