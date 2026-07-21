-- ============================================================
-- STORED PROCEDURES - Registrar prestamo y devolucion
-- Autor: Camila
-- Fecha: 20/07/2026
--
-- INSTRUCCIONES:
-- 1. Corre PRIMERO el archivo DATOS_BASE/valores_iniciales_estados.sql
--    (este script necesita que ya exista el estado "Activo" y "Devuelto")
-- 2. Abre SSMS, conectate a tu base de datos local "alica_system"
-- 3. Pega TODO este archivo y presiona Ejecutar (F5)
-- 4. Debe decir "Comandos completados correctamente" al final
-- 5. Es seguro correr este archivo varias veces: usa CREATE OR ALTER,
--    asi que si el procedimiento ya existe, simplemente lo actualiza
-- ============================================================

USE alica_system;
GO

-- ------------------------------------------------------------
-- 1) Registra un prestamo nuevo.
--    Que hace, paso a paso:
--    a) Revisa cuantas copias del libro quedan disponibles en inventario
--    b) Si no hay ninguna disponible, no crea el prestamo (devuelve 0)
--    c) Si hay copias, crea la fila en "prestamo" con estado "Activo"
--       y fecha de devolucion esperada (por defecto, 7 dias despues)
--    d) Le resta 1 a las copias disponibles en inventario
--    Devuelve: el id del prestamo creado, o 0 si no habia copias
-- ------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_RegistrarPrestamo]
    @IdUsuario  INT,
    @IdLibro    INT,
    @IdEmpleado INT,
    @DiasPlazo  INT = 7
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Disponible INT;
    SELECT @Disponible = cantidad_disponible FROM inventario WHERE id_libro = @IdLibro;

    IF @Disponible IS NULL OR @Disponible <= 0
    BEGIN
        SELECT 0 AS IdPrestamo; -- no hay copias disponibles de este libro
        RETURN;
    END

    DECLARE @IdEstadoActivo INT;
    SELECT @IdEstadoActivo = id_estado_prestamo FROM estado_prestamo WHERE nombre = 'Activo';

    INSERT INTO prestamo (id_usuario, id_libro, fecha_prestamo, fecha_dev_esperada, fecha_dev_real, id_empleado, id_estado_prestamo)
    VALUES (@IdUsuario, @IdLibro, GETDATE(), DATEADD(DAY, @DiasPlazo, GETDATE()), NULL, @IdEmpleado, @IdEstadoActivo);

    UPDATE inventario SET cantidad_disponible = cantidad_disponible - 1 WHERE id_libro = @IdLibro;

    SELECT SCOPE_IDENTITY() AS IdPrestamo; -- devuelve el id del prestamo recien creado
END
GO

-- ------------------------------------------------------------
-- 2) Registra la devolucion de un prestamo activo.
--    Que hace, paso a paso:
--    a) Busca el prestamo por su id, solo si todavia no tiene
--       fecha de devolucion real (o sea, si sigue activo)
--    b) Si no lo encuentra (no existe, o ya estaba devuelto), no hace nada
--    c) Si lo encuentra, marca la fecha de devolucion real como hoy
--       y cambia el estado a "Devuelto"
--    d) Le suma 1 de vuelta a las copias disponibles en inventario
--    Devuelve: 1 si la devolucion se registro bien, 0 si no se pudo
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
-- 3) Busca un libro por su codigo interno (el codigo que el
--    bibliotecario escribe o escanea al momento de prestar).
--    Devuelve el titulo del libro y cuantas copias hay disponibles
--    ahora mismo, para que el bibliotecario sepa si puede prestarlo.
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