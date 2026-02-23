USE Indigo

IF OBJECT_ID('dbo.usp_Tickets_Status_List', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_Status_List
GO
CREATE PROCEDURE dbo.usp_Tickets_Status_List
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM (
        SELECT 0 AS [Status], 'Todos' AS StatusDes
        UNION
        SELECT [Status], StatusDes
        FROM mStatusTicket
    ) Status
    ORDER BY
        CASE Status
            WHEN 1 THEN 0
            WHEN 2 THEN 1
            WHEN 3 THEN 2
            WHEN 0 THEN 3
        END;
END
GO

IF OBJECT_ID('dbo.usp_Tickets_List', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_List
GO
CREATE PROCEDURE dbo.usp_Tickets_List
    @Historicos BIT,
    @Usuario NVARCHAR(50),
    @AdmoTickets BIT,
    @FiltrarporTecnico BIT,
    @Campo INT,
    @Status INT,
    @Filtro NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Tickets.IdTicket, Tickets.Titulo, Tickets.FeAlta, Tickets.Departamento, Tickets.Status,
           Tickets.StatusDes, Tickets.Solicitante, Tickets.Tecnico, Tickets.Historico
    FROM dbo.GetTickets(@Historicos, @Usuario, @AdmoTickets, @FiltrarporTecnico, @Campo, @Status, @Filtro) AS Tickets;
END
GO

IF OBJECT_ID('dbo.usp_Tickets_InsertDuracion', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_InsertDuracion
GO
CREATE PROCEDURE dbo.usp_Tickets_InsertDuracion
    @IdTicket INT,
    @Duracion INT,
    @Usuario NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT dTickets
    VALUES (@IdTicket, '', @Duracion, GETDATE(), @Usuario);
END
GO

IF OBJECT_ID('dbo.usp_Tickets_Detalle', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_Detalle
GO
CREATE PROCEDURE dbo.usp_Tickets_Detalle
    @Historicos BIT,
    @Usuario NVARCHAR(50),
    @IdTicket INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM dbo.GetDetalleTicket(@Historicos, @Usuario, @IdTicket) AS Ticket;
END
GO

IF OBJECT_ID('dbo.usp_Tickets_Insert', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_Insert
GO
CREATE PROCEDURE dbo.usp_Tickets_Insert
    @Usuario NVARCHAR(50),
    @IdSubCategoria INT,
    @Titulo NVARCHAR(50),
    @Descripcion NVARCHAR(MAX),
    @IdTicket BIGINT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdPersona INT = (SELECT IdPersona FROM mEmpleados WHERE Login = @Usuario);

    INSERT INTO mTickets (Usuario, IdSubCategoria, Titulo, Descripcion, Status, FeAlta)
    VALUES (@IdPersona, @IdSubCategoria, @Titulo, @Descripcion, 1, GETDATE());

    SET @IdTicket = SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.usp_Tickets_Insert_Human', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_Insert_Human
GO
CREATE PROCEDURE dbo.usp_Tickets_Insert_Human
    @Usuario NVARCHAR(50),
    @SubCategoria NVARCHAR(30),
    @Categoria NVARCHAR(30) = NULL,
    @TipoTicket NVARCHAR(20) = NULL,
    @Prioridad NVARCHAR(10) = NULL,
    @Titulo NVARCHAR(50),
    @Descripcion NVARCHAR(MAX),
    @IdTicket BIGINT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdPersona INT = (SELECT IdPersona FROM mEmpleados WHERE Login = @Usuario);
    IF @IdPersona IS NULL
    BEGIN
        THROW 50001, 'No se encontró el usuario solicitante en mEmpleados.', 1;
    END

    DECLARE @IdSubCategoria TINYINT;
    SELECT TOP 1 @IdSubCategoria = s.IdSubCategoria
    FROM mSubCategoriasTicket s
    LEFT JOIN mCategoriasTicket c ON c.IdCategoria = s.IdCategoria
    WHERE s.SubCategoria = @SubCategoria
      AND (@Categoria IS NULL OR c.Categoria = @Categoria);

    IF @IdSubCategoria IS NULL
    BEGIN
        THROW 50002, 'No se encontró la subcategoría indicada.', 1;
    END

    DECLARE @IdTipoTicket TINYINT = NULL;
    IF @TipoTicket IS NOT NULL AND LTRIM(RTRIM(@TipoTicket)) <> ''
    BEGIN
        SELECT @IdTipoTicket = IdTipoTicket FROM mTipoTicket WHERE TipoTicket = @TipoTicket;
        IF @IdTipoTicket IS NULL
        BEGIN
            THROW 50003, 'No se encontró el tipo de ticket indicado.', 1;
        END
    END

    DECLARE @IdPrioridad TINYINT = NULL;
    IF @Prioridad IS NOT NULL AND LTRIM(RTRIM(@Prioridad)) <> ''
    BEGIN
        SELECT @IdPrioridad = IdPrioridad FROM mPrioridadTicket WHERE Prioridad = @Prioridad;
        IF @IdPrioridad IS NULL
        BEGIN
            THROW 50004, 'No se encontró la prioridad indicada.', 1;
        END
    END

    INSERT INTO mTickets (Usuario, IdSubCategoria, Titulo, Descripcion, Status, FeAlta, IdTipoTicket, Prioridad)
    VALUES (@IdPersona, @IdSubCategoria, @Titulo, @Descripcion, 1, GETDATE(), @IdTipoTicket, @IdPrioridad);

    SET @IdTicket = SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.usp_Tickets_Update', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_Update
GO
CREATE PROCEDURE dbo.usp_Tickets_Update
    @IdTicket INT,
    @Titulo NVARCHAR(50),
    @IdSubCategoria INT,
    @IdTipoTicket NVARCHAR(10),
    @IdPrioridad NVARCHAR(10),
    @Descripcion NVARCHAR(MAX),
    @FeCompromiso NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE mTickets
    SET Titulo = @Titulo,
        IdSubCategoria = @IdSubCategoria,
        IdTipoTicket = CASE WHEN @IdTipoTicket = '' THEN NULL ELSE CONVERT(INT, @IdTipoTicket) END,
        Prioridad = CASE WHEN @IdPrioridad = '' THEN NULL ELSE CONVERT(INT, @IdPrioridad) END,
        Descripcion = @Descripcion,
        FeCompromiso = CASE WHEN @FeCompromiso = '' THEN NULL ELSE CONVERT(DATETIME, @FeCompromiso, 103) END
    WHERE IdTicket = @IdTicket;
END
GO

IF OBJECT_ID('dbo.usp_Tickets_ChangeStatus', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_ChangeStatus
GO
CREATE PROCEDURE dbo.usp_Tickets_ChangeStatus
    @IdTicket INT,
    @Opc INT,
    @ValEliminaTicket INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @Opc = 1
    BEGIN
        DELETE mTickets
        FROM mTickets
        WHERE IdTicket = @IdTicket
          AND Status = 1;

        SET @ValEliminaTicket = (
            SELECT COUNT(*)
            FROM mTickets
            WHERE IdTicket = @IdTicket
        );
    END

    IF @Opc = 2
    BEGIN
        UPDATE mTickets
        SET Status = 3,
            FeCierre = GETDATE()
        WHERE IdTicket = @IdTicket
          AND Status = 2;

        SET @ValEliminaTicket = 2;
    END

    IF @Opc = 3
    BEGIN
        UPDATE mTickets
        SET Status = 2,
            FeCierre = NULL
        WHERE IdTicket = @IdTicket
          AND Status = 3;

        SET @ValEliminaTicket = 3;
    END
END
GO

IF OBJECT_ID('dbo.usp_Tickets_Areas_List', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_Areas_List
GO
CREATE PROCEDURE dbo.usp_Tickets_Areas_List
AS
BEGIN
    SET NOCOUNT ON;

    SELECT mDepartamentos.IdDepto, mDepartamentos.Departamento
    FROM mDepartamentos
    WHERE mDepartamentos.Tickets = 1
    ORDER BY mDepartamentos.Departamento;
END
GO

IF OBJECT_ID('dbo.usp_Tickets_Categorias_ByArea', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_Categorias_ByArea
GO
CREATE PROCEDURE dbo.usp_Tickets_Categorias_ByArea
    @Area INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT mCategoriasTicket.IdCategoria, mCategoriasTicket.Categoria
    FROM mCategoriasTicket
    WHERE mCategoriasTicket.IdDepto = @Area
    ORDER BY mCategoriasTicket.Categoria;
END
GO

IF OBJECT_ID('dbo.usp_Tickets_SubCategorias_ByCategoria', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_SubCategorias_ByCategoria
GO
CREATE PROCEDURE dbo.usp_Tickets_SubCategorias_ByCategoria
    @Categoria INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT mSubCategoriasTicket.IdSubCategoria, mSubCategoriasTicket.SubCategoria
    FROM mSubCategoriasTicket
    WHERE mSubCategoriasTicket.IdCategoria = @Categoria
    ORDER BY mSubCategoriasTicket.SubCategoria;
END
GO

IF OBJECT_ID('dbo.usp_Tickets_Prioridad_List', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_Prioridad_List
GO
CREATE PROCEDURE dbo.usp_Tickets_Prioridad_List
AS
BEGIN
    SET NOCOUNT ON;

    SELECT NULL AS IdPrioridad, '...' AS Prioridad
    UNION
    SELECT mPrioridadTicket.IdPrioridad, mPrioridadTicket.Prioridad
    FROM mPrioridadTicket;
END
GO

IF OBJECT_ID('dbo.usp_Tickets_Tipo_List', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_Tipo_List
GO
CREATE PROCEDURE dbo.usp_Tickets_Tipo_List
AS
BEGIN
    SET NOCOUNT ON;

    SELECT NULL AS IdTipoTicket, '...' AS TipoTicket
    UNION
    SELECT mTipoTicket.IdTipoTicket, mTipoTicket.TipoTicket
    FROM mTipoTicket;
END
GO

IF OBJECT_ID('dbo.usp_Tickets_Categorias_ByDepto', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_Categorias_ByDepto
GO
CREATE PROCEDURE dbo.usp_Tickets_Categorias_ByDepto
    @IdDepto INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT mCategoriasTicket.IdCategoria, mCategoriasTicket.Categoria, mCategoriasTicket.IdDepto
    FROM mCategoriasTicket
    WHERE mCategoriasTicket.IdDepto = @IdDepto;
END
GO

IF OBJECT_ID('dbo.usp_Tickets_SubCategorias_Edit_ByCategoria', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_SubCategorias_Edit_ByCategoria
GO
CREATE PROCEDURE dbo.usp_Tickets_SubCategorias_Edit_ByCategoria
    @IdCategoria INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT mSubCategoriasTicket.IdSubCategoria, mSubCategoriasTicket.SubCategoria, mSubCategoriasTicket.IdCategoria
    FROM mSubCategoriasTicket
    WHERE mSubCategoriasTicket.IdCategoria = @IdCategoria;
END
GO

IF OBJECT_ID('dbo.usp_Tickets_Tecnicos_ByTicket', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_Tecnicos_ByTicket
GO
CREATE PROCEDURE dbo.usp_Tickets_Tecnicos_ByTicket
    @Historico BIT,
    @IdTicket INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Tecnicos.IdTecnico AS Tecnico, Tecnicos.Nombre, Tecnicos.Tiempo, Tecnicos.Usuario
    FROM dbo.GetTecnicosTicket(@Historico, @IdTicket) AS Tecnicos;
END
GO

IF OBJECT_ID('dbo.usp_Tickets_Tecnicos_Delete', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_Tecnicos_Delete
GO
CREATE PROCEDURE dbo.usp_Tickets_Tecnicos_Delete
    @IdTicket INT,
    @Tecnico INT
AS
BEGIN
    SET NOCOUNT ON;

    IF (SELECT COUNT(IdPersona) FROM dTicketsTecnicos WHERE IdTicket = @IdTicket) > 1
    BEGIN
        DELETE dTicketsTecnicos
        FROM dTicketsTecnicos
        WHERE IdTicket = @IdTicket
          AND IdPersona = @Tecnico
          AND IdPersona NOT IN (
              SELECT IdPersona
              FROM dTickets
              INNER JOIN mEmpleados ON dTickets.Usuario = mEmpleados.Login
              WHERE dTickets.IdTicket = @IdTicket
          );
    END
END
GO

IF OBJECT_ID('dbo.usp_Tickets_Anotaciones_ByTicket', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_Anotaciones_ByTicket
GO
CREATE PROCEDURE dbo.usp_Tickets_Anotaciones_ByTicket
    @Historico BIT,
    @IdTicket INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Anotaciones.IdDTicket, Anotaciones.Evento, Anotaciones.Tiempo, Anotaciones.Fecha, Anotaciones.Usuario
    FROM DBO.GetAnotacionesTecnicosTicket(@Historico, @IdTicket) AS Anotaciones;
END
GO

IF OBJECT_ID('dbo.usp_Tickets_Anotaciones_Insert', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_Anotaciones_Insert
GO
CREATE PROCEDURE dbo.usp_Tickets_Anotaciones_Insert
    @IdTicket INT,
    @Obvs NVARCHAR(MAX),
    @Duracion INT,
    @Usuario NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT dTickets
    VALUES (@IdTicket, @Obvs, @Duracion, GETDATE(), @Usuario);
END
GO

IF OBJECT_ID('dbo.usp_Tickets_Anotaciones_Delete', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_Anotaciones_Delete
GO
CREATE PROCEDURE dbo.usp_Tickets_Anotaciones_Delete
    @IdDTicket INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE dTickets
    WHERE IdDTicket = @IdDTicket;
END
GO

IF OBJECT_ID('dbo.usp_Tickets_DocTicket_List', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_DocTicket_List
GO
CREATE PROCEDURE dbo.usp_Tickets_DocTicket_List
    @IdTicket INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT mImgDocTicket.IdImgDoc, mTipoArchivo.TipoArchivo
    FROM mImgDocTicket
    INNER JOIN mTipoArchivo ON mImgDocTicket.IdTipoArchivo = mTipoArchivo.IdTipoArchivo
    WHERE mImgDocTicket.IdTicket = @IdTicket
      AND mImgDocTicket.Sucursal = 1;
END
GO

IF OBJECT_ID('dbo.usp_Tickets_TecnicosDisponibles', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_TecnicosDisponibles
GO
CREATE PROCEDURE dbo.usp_Tickets_TecnicosDisponibles
    @nombre NVARCHAR(200),
    @IdDepto INT,
    @IdTicket INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT *
    FROM (
        SELECT Usuarios.IdPersona, Usuarios.Nombre, Usuarios.Departamento
        FROM (
            SELECT mEmpleados.IdPersona, vPersonas.Nombre, mDepartamentos.Departamento, mEmpleados.Login
            FROM mEmpleados
            INNER JOIN dEmpleados ON mEmpleados.IdPersona = dEmpleados.IdPersona
            INNER JOIN mPuestos ON dEmpleados.IdPuesto = mPuestos.IdPuesto
            INNER JOIN vPersonas ON mEmpleados.IdPersona = vPersonas.IdPersona
            INNER JOIN mDepartamentos ON mPuestos.IdDepto = mDepartamentos.IdDepto
            WHERE (@nombre = '' AND mPuestos.IdDepto = @IdDepto)
               OR (vPersonas.Nombre LIKE '%' + @nombre + '%' AND mPuestos.IdDepto = @IdDepto)
            GROUP BY mEmpleados.IdPersona, vPersonas.Nombre, mDepartamentos.Departamento, mEmpleados.Login
        ) Usuarios
        INNER JOIN aspnet_Users Users ON Usuarios.Login = Users.LoweredUserName
        INNER JOIN aspnet_UsersInRoles URoles ON Users.UserId = URoles.UserId
        INNER JOIN aspnet_Roles Roles ON URoles.RoleId = Roles.RoleId
        WHERE Roles.RoleName = 'AdmoTickets'
    ) Tecnicos
    WHERE IdPersona NOT IN (
        SELECT IdPersona
        FROM dTicketsTecnicos
        WHERE IdTicket = @IdTicket
    )
    ORDER BY Tecnicos.Nombre;
END
GO

IF OBJECT_ID('dbo.usp_Tickets_AsignarTecnico', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_AsignarTecnico
GO
CREATE PROCEDURE dbo.usp_Tickets_AsignarTecnico
    @IdTicket INT,
    @Tecnico INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE mTickets
    SET Status = 2,
        FeAsignacion = GETDATE()
    WHERE IdTicket = @IdTicket
      AND Status = 1;

    INSERT dTicketsTecnicos
    VALUES (@IdTicket, @Tecnico);
END
GO

IF OBJECT_ID('dbo.usp_Tickets_Mail_NewTicketData', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_Mail_NewTicketData
GO
CREATE PROCEDURE dbo.usp_Tickets_Mail_NewTicketData
    @IdTicket INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT mTickets.Titulo, mTickets.Descripcion, UPPER(mStatusTicket.StatusDes) AS Status, mTickets.FeAlta,
           Solicitante.Nombre AS Solicitante, LOWER(dbo.GetEmailsByFunction(Solicitante.IdPersona, 9)) AS EmailSolicitante
    FROM mTickets
    INNER JOIN vPersonas AS Solicitante ON mTickets.Usuario = Solicitante.IdPersona
    INNER JOIN mStatusTicket ON mTickets.Status = mStatusTicket.Status
    WHERE mTickets.IdTicket = @IdTicket;
END
GO

IF OBJECT_ID('dbo.usp_Tickets_Mail_TecnicoAsignadoData', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_Mail_TecnicoAsignadoData
GO
CREATE PROCEDURE dbo.usp_Tickets_Mail_TecnicoAsignadoData
    @IdTicket INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT mTickets.Titulo, mTickets.Descripcion, UPPER(mStatusTicket.StatusDes) AS Status,
           mTickets.FeAlta, mTickets.FeAsignacion, mTickets.FeCompromiso,
           Solicitante.Nombre AS Solicitante, LOWER(dbo.GetEmailsByFunction(Solicitante.IdPersona, 9)) AS EmailSolicitante,
           Tecnicos.Nombre AS Tecnico, LOWER(dbo.GetEmailsByFunction(Tecnicos.IdPersona, 9)) AS EmailTecnico
    FROM mTickets
    INNER JOIN vPersonas AS Solicitante ON mTickets.Usuario = Solicitante.IdPersona
    INNER JOIN mStatusTicket ON mTickets.Status = mStatusTicket.Status
    INNER JOIN dTicketsTecnicos ON mTickets.IdTicket = dTicketsTecnicos.IdTicket
    INNER JOIN vPersonas AS Tecnicos ON dTicketsTecnicos.IdPersona = Tecnicos.IdPersona
    WHERE mTickets.IdTicket = @IdTicket;
END
GO

IF OBJECT_ID('dbo.usp_Tickets_Mail_TecnicosDepto', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_Mail_TecnicosDepto
GO
CREATE PROCEDURE dbo.usp_Tickets_Mail_TecnicosDepto
    @IdTicket INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT DISTINCT LOWER(dbo.GetEmailsByFunction(vPersonas.IdPersona, 9)) AS Email, UPPER(vPersonas.Nombre) AS Tecnico
    FROM (
        SELECT DeptoTecnicos.IdPersona, DeptoTecnicos.Login
        FROM mTickets
        INNER JOIN mSubCategoriasTicket ON mTickets.IdSubCategoria = mSubCategoriasTicket.IdSubCategoria
        INNER JOIN mCategoriasTicket ON mSubCategoriasTicket.IdCategoria = mCategoriasTicket.IdCategoria
        INNER JOIN (
            SELECT mPuestos.IdDepto, mEmpleados.IdPersona, mEmpleados.Login
            FROM mEmpleados
            INNER JOIN dEmpleados ON mEmpleados.IdPersona = dEmpleados.IdPersona
            INNER JOIN mPuestos ON dEmpleados.IdPuesto = mPuestos.IdPuesto
        ) AS DeptoTecnicos ON mCategoriasTicket.IdDepto = DeptoTecnicos.IdDepto
        INNER JOIN aspnet_Users Users ON DeptoTecnicos.Login = Users.LoweredUserName
        INNER JOIN aspnet_UsersInRoles Roles ON Users.UserId = Roles.UserId
        INNER JOIN aspnet_Roles ON Roles.RoleId = aspnet_Roles.RoleId
        WHERE mTickets.IdTicket = @IdTicket
          AND aspnet_Roles.RoleName = 'AdmoTickets'
    ) Usuarios
    INNER JOIN vPersonas ON Usuarios.IdPersona = vPersonas.IdPersona
    WHERE dbo.GetEmailsByFunction(vPersonas.IdPersona, 9) <> ''
    ORDER BY Tecnico;
END
GO

IF OBJECT_ID('dbo.usp_Tickets_Mail_TecnicosAsignados', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_Mail_TecnicosAsignados
GO
CREATE PROCEDURE dbo.usp_Tickets_Mail_TecnicosAsignados
    @IdTicket INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT LOWER(dbo.GetEmailsByFunction(vPersonas.IdPersona, 9)) AS Email, vPersonas.Nombre AS Tecnico
    FROM mTickets
    INNER JOIN dTicketsTecnicos ON mTickets.IdTicket = dTicketsTecnicos.IdTicket
    INNER JOIN vPersonas ON dTicketsTecnicos.IdPersona = vPersonas.IdPersona
    WHERE mTickets.IdTicket = @IdTicket
      AND dbo.GetEmailsByFunction(vPersonas.IdPersona, 9) <> ''
    ORDER BY vPersonas.Nombre;
END
GO

IF OBJECT_ID('dbo.usp_Tickets_Mail_TicketData', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_Mail_TicketData
GO
CREATE PROCEDURE dbo.usp_Tickets_Mail_TicketData
    @IdTicket INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT mTickets.Titulo, mTickets.Descripcion, UPPER(mStatusTicket.StatusDes) AS Status, mTickets.FeCierre,
           Solicitante.Nombre AS Solicitante, LOWER(dbo.GetEmailsByFunction(Solicitante.IdPersona, 9)) AS MailSolicitante,
           Tecnicos.Nombre AS Tecnico, LOWER(dbo.GetEmailsByFunction(Tecnicos.IdPersona, 9)) AS EmailTecnico
    FROM mTickets
    INNER JOIN vPersonas AS Solicitante ON mTickets.Usuario = Solicitante.IdPersona
    INNER JOIN mStatusTicket ON mTickets.Status = mStatusTicket.Status
    INNER JOIN dTicketsTecnicos ON mTickets.IdTicket = dTicketsTecnicos.IdTicket
    INNER JOIN vPersonas AS Tecnicos ON dTicketsTecnicos.IdPersona = Tecnicos.IdPersona
    WHERE mTickets.IdTicket = @IdTicket;
END
GO

IF OBJECT_ID('dbo.usp_Tickets_Mail_Anotaciones', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_Tickets_Mail_Anotaciones
GO
CREATE PROCEDURE dbo.usp_Tickets_Mail_Anotaciones
    @IdTicket INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT dTickets.Evento, dTickets.Fecha, dTickets.Usuario
    FROM dTickets
    WHERE dTickets.IdTicket = @IdTicket
      AND dTickets.Evento <> ''
    ORDER BY dTickets.IdDTicket;
END
GO
