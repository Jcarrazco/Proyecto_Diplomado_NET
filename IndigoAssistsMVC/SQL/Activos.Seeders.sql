USE [Indigo]
GO

IF NOT EXISTS (SELECT 1 FROM dbo.mStatusActivo)
BEGIN
    INSERT INTO dbo.mStatusActivo (Status)
    VALUES
        ('Activo'),
        ('Mantenimiento'),
        ('Baja'),
        ('Garantia');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.mTiposActivo)
BEGIN
    INSERT INTO dbo.mTiposActivo (TipoActivo)
    VALUES
        ('Laptop'),
        ('PC Escritorio'),
        ('Servidor'),
        ('Impresora'),
        ('Monitor'),
        ('Telefono'),
        ('Otro');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.mProveedoresActivos)
BEGIN
    INSERT INTO dbo.mProveedoresActivos (Proveedor)
    VALUES
        ('Sin proveedor'),
        ('Proveedor Generico');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.mComponentesActivos)
BEGIN
    INSERT INTO dbo.mComponentesActivos (Componente, ValorBit)
    VALUES
        ('SSD', 1),
        ('HDD', 2),
        ('RAM', 4),
        ('GPU', 8),
        ('WiFi', 16),
        ('Bluetooth', 32),
        ('Dock', 64);
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.mActivos)
BEGIN
    DECLARE @IdDepto TINYINT = (SELECT TOP 1 IdDepto FROM dbo.mDepartamentos ORDER BY IdDepto);
    DECLARE @IdProveedor TINYINT = (SELECT TOP 1 IdProveedor FROM dbo.mProveedoresActivos ORDER BY IdProveedor);
    DECLARE @IdStatusActivo TINYINT = (SELECT TOP 1 StatusId FROM dbo.mStatusActivo WHERE Status = 'Activo');
    DECLARE @IdStatusMantenimiento TINYINT = (SELECT TOP 1 StatusId FROM dbo.mStatusActivo WHERE Status = 'Mantenimiento');
    DECLARE @IdTipoLaptop TINYINT = (SELECT TOP 1 IdTipoActivo FROM dbo.mTiposActivo WHERE TipoActivo = 'Laptop');
    DECLARE @IdTipoPC TINYINT = (SELECT TOP 1 IdTipoActivo FROM dbo.mTiposActivo WHERE TipoActivo = 'PC Escritorio');
    DECLARE @IdTipoMonitor TINYINT = (SELECT TOP 1 IdTipoActivo FROM dbo.mTiposActivo WHERE TipoActivo = 'Monitor');
    DECLARE @IdTipoImpresora TINYINT = (SELECT TOP 1 IdTipoActivo FROM dbo.mTiposActivo WHERE TipoActivo = 'Impresora');

    INSERT INTO dbo.mActivos (
        Codigo,
        Marca,
        Modelo,
        Serie,
        Nombre,
        PersonaAsign,
        Ubicacion,
        FeAlta,
        FeCompra,
        CostoCompra,
        Notas,
        CodificacionComponentes,
        TieneSoftwareOP,
        IdTipoActivo,
        IdDepartamento,
        IdStatus,
        IdProveedor
    )
    VALUES
        ('ACT-0001', 'Lenovo', 'ThinkPad T14', 'SN-T14-0001', 'Laptop Desarrollo', 'Admin Demo', 'Oficina 1', CAST(GETDATE() AS DATE), CAST(GETDATE() AS DATE), 32000.00, 'Equipo demo para desarrollo', 1 + 4 + 16, 1, @IdTipoLaptop, @IdDepto, @IdStatusActivo, @IdProveedor),
        ('ACT-0002', 'Dell', 'OptiPlex 7090', 'SN-OP-0002', 'PC Contabilidad', 'Usuario Demo', 'Oficina 2', CAST(GETDATE() AS DATE), CAST(GETDATE() AS DATE), 18000.00, 'PC de escritorio demo', 2 + 4 + 16, 0, @IdTipoPC, @IdDepto, @IdStatusActivo, @IdProveedor),
        ('ACT-0003', 'HP', 'LaserJet Pro M404', 'SN-LJ-0003', 'Impresora Recepcion', NULL, 'Recepcion', CAST(GETDATE() AS DATE), CAST(GETDATE() AS DATE), 6500.00, 'Impresora demo', 0, 0, @IdTipoImpresora, @IdDepto, @IdStatusActivo, @IdProveedor),
        ('ACT-0004', 'LG', '27UL500', 'SN-MN-0004', 'Monitor Diseño', NULL, 'Diseno', CAST(GETDATE() AS DATE), CAST(GETDATE() AS DATE), 7000.00, 'Monitor 4K demo', 0, 0, @IdTipoMonitor, @IdDepto, @IdStatusActivo, @IdProveedor),
        ('ACT-0005', 'Lenovo', 'ThinkPad T480', 'SN-T48-0005', 'Laptop Soporte', 'Tecnico Demo', 'Soporte', CAST(GETDATE() AS DATE), CAST(GETDATE() AS DATE), 21000.00, 'Equipo en mantenimiento', 1 + 4 + 16 + 32, 1, @IdTipoLaptop, @IdDepto, @IdStatusMantenimiento, @IdProveedor);
END
GO
