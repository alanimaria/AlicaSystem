-- ============================================================
-- STORED PROCEDURES - Registrar prestamo y devolucion
-- Autor: Camila
-- Fecha: 20/07/2026 (actualizado 21/07/2026)
--
-- INSTRUCCIONES:
-- 1. Corre PRIMERO el archivo DATOS_BASE/valores_iniciales_estados.sql
--    (este script necesita que ya exista el estado "Activo" y "Devuelto")
-- 2. Abre SSMS, conectate a tu base de datos local "alica_system"
-- 3. Pega TODO este archivo y presiona Ejecutar (F5)
-- 4. Debe decir "Comandos completados correctamente" al final
-- 5. Es seguro correr este archivo varias veces: usa CREATE OR ALTER,
--    asi que si el procedimiento ya existe, simplemente lo actualiza
--
-- CAMBIOS DEL 21/07/2026:
-- - sp_RegistrarPrestamo ahora valida ADEMAS de las copias disponibles:
--     a) que el usuario no tenga multas pendientes (fecha_pago IS NULL
--        en la tabla multa)
--     b) que el usuario no tenga ya 3 prestamos activos (maximo permitido)
--   Ahora devuelve tambien una columna "Mensaje" explicando el motivo
--   exacto si el prestamo no se pudo registrar, para mostrarlo directo
--   en pantalla sin tener que adivinarlo en el codigo C#.
-- - sp_BuscarUsuarioPorMatricula ahora devuelve tambien cuantos
--   prestamos activos tiene el usuario (PrestamosActivos) y si tiene
--   alguna multa pendiente (TieneMultaPendiente), para mostrar esa
--   info en el formulario apenas se busca al usuario (igual que en
--   el mockup: "3 prestamos activos permitidos - actualmente tiene 1")
-- ============================================================

USE alica_system;
GO

-- ------------------------------------------------------------
-- 1) Registra un prestamo nuevo.
--    Que hace, paso a paso:
--    a) Si el usuario tiene alguna multa pendiente de pagar, no
--       deja registrar el prestamo (regla: "usuarios con multas
--       pendientes no pueden pedir prestamos")
--    b) Si el usuario ya tiene 3 o mas prestamos activos (libros
--       que todavia no ha devuelto), no deja registrar otro
--       (regla: "maximo 3 prestamos activos por usuario")
--    c) Revisa cuantas copias del libro quedan disponibles en
--       inventario; si no hay ninguna, no crea el prestamo
--    d) Si paso todas las validaciones, crea la fila en "prestamo"
--       con estado "Activo" y fecha de devolucion esperada (por
--       defecto, 7 dias despues)
--    e) Le resta 1 a las copias disponibles en inventario
--    Devuelve: IdPrestamo (el id creado, o 0 si fallo alguna regla)
--              Mensaje (texto explicando el resultado, sea exito o
--              el motivo exacto del fallo)
-- ------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_RegistrarPrestamo]
    @IdUsuario  INT,
    @IdLibro    INT,
    @IdEmpleado INT,
    @DiasPlazo  INT = 7
AS
BEGIN
    SET NOCOUNT ON;

    -- Regla: multas pendientes
    IF EXISTS (SELECT 1 FROM multa WHERE id_usuario = @IdUsuario AND fecha_pago IS NULL)
    BEGIN
        SELECT 0 AS IdPrestamo, 'El usuario tiene multas pendientes y no puede pedir prestamos.' AS Mensaje;
        RETURN;
    END

    -- Regla: maximo 3 prestamos activos
    DECLARE @PrestamosActivos INT;
    SELECT @PrestamosActivos = COUNT(*) FROM prestamo WHERE id_usuario = @IdUsuario AND fecha_dev_real IS NULL;

    IF @PrestamosActivos >= 3
    BEGIN
        SELECT 0 AS IdPrestamo, 'El usuario ya alcanzo el maximo de 3 prestamos activos.' AS Mensaje;
        RETURN;
    END

    -- Regla: copias disponibles
    DECLARE @Disponible INT;
    SELECT @Disponible = cantidad_disponible FROM inventario WHERE id_libro = @IdLibro;

    IF @Disponible IS NULL OR @Disponible <= 0
    BEGIN
        SELECT 0 AS IdPrestamo, 'No hay copias disponibles de este libro.' AS Mensaje;
        RETURN;
    END

    DECLARE @IdEstadoActivo INT;
    SELECT @IdEstadoActivo = id_estado_prestamo FROM estado_prestamo WHERE nombre = 'Activo';

    INSERT INTO prestamo (id_usuario, id_libro, fecha_prestamo, fecha_dev_esperada, fecha_dev_real, id_empleado, id_estado_prestamo)
    VALUES (@IdUsuario, @IdLibro, GETDATE(), DATEADD(DAY, @DiasPlazo, GETDATE()), NULL, @IdEmpleado, @IdEstadoActivo);

    UPDATE inventario SET cantidad_disponible = cantidad_disponible - 1 WHERE id_libro = @IdLibro;

    SELECT SCOPE_IDENTITY() AS IdPrestamo, 'Prestamo registrado con exito.' AS Mensaje;
END
GO

-- ------------------------------------------------------------
-- 2) Registra la devolucion de un prestamo activo.
--    (sin cambios respecto a la version anterior)
-- ------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_RegistrarDevolucion]
    @IdPrestamo INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdLibro INT;
    DECLARE @IdEstadoDevuelto INT;
    SELECT @IdEstadoDevuelto = id_estado_prestamo FROM estado_prestamo WHERE nombre = 'Devuelto';

    SELECT @IdLibro = id_libro FROM prestamo WHERE id_prestamo = @IdPrestamo AND fecha_dev_real IS NULL;

    IF @IdLibro IS NULL
    BEGIN
        SELECT 0 AS FilasAfectadas; -- el prestamo no existe o ya estaba devuelto
        RETURN;
    END

    UPDATE prestamo
    SET fecha_dev_real = GETDATE(), id_estado_prestamo = @IdEstadoDevuelto
    WHERE id_prestamo = @IdPrestamo;

    UPDATE inventario SET cantidad_disponible = cantidad_disponible + 1 WHERE id_libro = @IdLibro;

    SELECT 1 AS FilasAfectadas;
END
GO

-- ------------------------------------------------------------
-- 3) Busca un libro por su codigo interno.
--    (sin cambios respecto a la version anterior)
-- ------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_BuscarLibroPorCodigo]
    @CodigoInterno VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT l.id_libro, l.titulo, l.codigo_interno, i.cantidad_disponible
    FROM libro l
    JOIN inventario i ON i.id_libro = l.id_libro
    WHERE l.codigo_interno = @CodigoInterno;
END
GO

-- ------------------------------------------------------------
-- 4) Busca un usuario (lector) por su matricula.
--    Devuelve sus datos basicos, mas ahora tambien:
--    - PrestamosActivos: cuantos libros tiene prestados sin devolver
--    - TieneMultaPendiente: 1 si tiene alguna multa sin pagar, 0 si no
--    Esto es para mostrar en pantalla, apenas se busca al usuario,
--    la info de "3 prestamos activos permitidos - actualmente tiene X"
--    igual que en el mockup, y para poder avisar de una vez si tiene
--    multas pendientes antes de intentar registrar el prestamo.
--    Solo busca entre usuarios activos (estado = 1).
-- ------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_BuscarUsuarioPorMatricula]
    @Matricula VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        u.id_usuario,
        u.nombre,
        u.apellido,
        u.matricula,
        (SELECT COUNT(*) FROM prestamo p WHERE p.id_usuario = u.id_usuario AND p.fecha_dev_real IS NULL) AS PrestamosActivos,
        CASE WHEN EXISTS (SELECT 1 FROM multa m WHERE m.id_usuario = u.id_usuario AND m.fecha_pago IS NULL)
             THEN 1 ELSE 0 END AS TieneMultaPendiente
    FROM usuario u
    WHERE u.matricula = @Matricula
      AND u.estado = 1;
END
GO
