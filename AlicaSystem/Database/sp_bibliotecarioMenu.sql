-- ============================================================
-- CAMBIOS DE BASE DE DATOS - Menú principal del Bibliotecario
-- Autor: Camila
-- Fecha: 19/07/2026
--
-- INSTRUCCIONES:
-- 1. Abre SSMS y conéctate a tu base de datos local "alica_system"
-- 2. Abre una Nueva Consulta
-- 3. Pega TODO este archivo y presiona Ejecutar (F5)
-- 4. Debe decir "Commands completed successfully" al final
-- 5. Este script es seguro de correr aunque ya tengas datos:
--    usa CREATE OR ALTER, así que si el procedimiento ya existe,
--    simplemente lo actualiza sin borrar nada.
-- ============================================================

USE alica_system;
GO

-- ------------------------------------------------------------
-- 1) Permite que la fecha en que se agregó un libro a una lista
--    pueda quedar vacía (antes era obligatoria).
--    NOTA: Ya aplicado por Alani en ambas bases de datos (la suya y la de Camila).
--    Se deja aquí comentado solo como registro histórico del cambio.
-- ------------------------------------------------------------
--ALTER TABLE lista ALTER COLUMN fecha_libro_agregado DATE NULL;
--GO

-- ------------------------------------------------------------
-- 2) Login para empleados (Bibliotecario / Administrador)
--    Devuelve los datos del empleado junto con el nombre de su rol,
--    haciendo JOIN con la tabla "rol".
--    Se usa desde Datos/EmpleadoDatos.cs -> ValidarEmpleado()
-- ------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_LoginEmpleado]
    @Email    VARCHAR(150),
    @Password VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        e.id_empleado,
        e.nombre,
        e.apellido,
        e.email,
        e.telefono,
        e.area,
        e.estado,
        r.id_rol,
        r.nombre AS nombre_rol
    FROM empleado e
    JOIN rol r ON r.id_rol = e.id_rol
    WHERE e.email    = @Email
      AND e.password = @Password
      AND e.estado   = 1;
END
GO

-- ------------------------------------------------------------
-- 3) Cuenta los préstamos activos (los que no tienen fecha de
--    devolución real todavía, incluye los vencidos sin devolver).
--    Se usa en el menú principal del Bibliotecario.
-- ------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_ContarPrestamosActivos]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) AS Total
    FROM prestamo
    WHERE fecha_dev_real IS NULL;
END
GO

-- ------------------------------------------------------------
-- 4) Cuenta las reservas cuyo estado es "Pendiente"
--    (hace JOIN con estado_reserva para leer el nombre del estado).
-- ------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_ContarReservasPendientes]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) AS Total
    FROM reserva r
    JOIN estado_reserva er ON er.id_estado_reserva = r.id_estado_reserva
    WHERE er.nombre = 'Pendiente';
END
GO

-- ------------------------------------------------------------
-- 5) Cuenta cuántos USUARIOS distintos tienen al menos una multa
--    sin pagar (no cuenta multas, cuenta personas con saldo).
-- ------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_ContarMultasPendientes]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(DISTINCT id_usuario) AS Total
    FROM multa
    WHERE fecha_pago IS NULL;
END
GO

-- ------------------------------------------------------------
-- 6) Lista la actividad más reciente del sistema: últimos
--    préstamos, devoluciones y reservas, todo junto y ordenado
--    por fecha descendente. Se usa en la tabla "Actividad reciente"
--    del menú principal del Bibliotecario.
--
--    NOTA: las columnas de fecha son tipo DATE (sin hora), así que
--    el frontend debe mostrar solo la fecha, no una hora exacta.
-- ------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_ListarActividadReciente]
    @Top INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP (@Top) *
    FROM (
        SELECT p.fecha_prestamo AS Fecha, u.nombre + ' ' + u.apellido AS Usuario, l.titulo AS Libro, 'Prestamo' AS Accion
        FROM prestamo p
        JOIN usuario u ON u.id_usuario = p.id_usuario
        JOIN libro l ON l.id_libro = p.id_libro
        UNION ALL
        SELECT p.fecha_dev_real, u.nombre + ' ' + u.apellido, l.titulo, 'Devolucion'
        FROM prestamo p
        JOIN usuario u ON u.id_usuario = p.id_usuario
        JOIN libro l ON l.id_libro = p.id_libro
        WHERE p.fecha_dev_real IS NOT NULL
        UNION ALL
        SELECT r.fecha_reserva, u.nombre + ' ' + u.apellido, l.titulo, 'Reserva'
        FROM reserva r
        JOIN usuario u ON u.id_usuario = r.id_usuario
        JOIN libro l ON l.id_libro = r.id_libro
    ) actividad
    ORDER BY Fecha DESC;
END
GO

-- ------------------------------------------------------------
-- 7) (Opcional) Datos de prueba para poder loguearte como
--    bibliotecario. Solo corre esto si tu tabla "rol" o "empleado"
--    están vacías todavía - si ya tienes datos, sáltate este bloque.
-- ------------------------------------------------------------
-- INSERT INTO rol (nombre, descripcion) VALUES
-- ('Bibliotecario', 'Gestión operativa: préstamos, devoluciones, reservas y multas'),
-- ('Administrador', 'Gestión general del sistema bibliotecario');

-- INSERT INTO empleado (nombre, apellido, email, password, telefono, id_rol, area, estado)
-- VALUES ('Roberto', 'Pichardo', 'roberto@alica.com', '123456', '8091234567', 1, 'Circulación', 1);
