-- ============================================================
-- STORED PROCEDURES - Reservas pendientes
-- Autor: Camila
-- Fecha: 21/07/2026
--
-- INSTRUCCIONES: igual que siempre - USE alica_system, pegar todo,
-- F5, y "Comandos completados correctamente" al final.
-- ============================================================

USE alica_system;
GO

-- ------------------------------------------------------------
-- 1) Lista las reservas que estan en estado "Pendiente"
--    (esperando ser recogidas por el usuario), con los datos
--    del usuario y el libro ya resueltos para mostrar en pantalla.
--    Ordenadas por fecha de expiracion (las que vencen primero,
--    arriba).
-- ------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_ListarReservasPendientes]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        r.id_reserva,
        u.nombre + ' ' + u.apellido AS Usuario,
        l.titulo AS Libro,
        r.fecha_reserva,
        r.fecha_expiracion
    FROM reserva r
    JOIN usuario u ON u.id_usuario = r.id_usuario
    JOIN libro l ON l.id_libro = r.id_libro
    JOIN estado_reserva er ON er.id_estado_reserva = r.id_estado_reserva
    WHERE er.nombre = 'Pendiente'
    ORDER BY r.fecha_expiracion ASC;
END
GO

-- ------------------------------------------------------------
-- 2) Cambia el estado de una reserva (usado para "Marcar
--    entregado" -> Cumplida, o "Cancelar" -> Cancelada).
--    Solo actualiza si la reserva sigue en "Pendiente" (para no
--    reescribir una reserva que ya se cerro antes).
--    Devuelve: 1 si se actualizo, 0 si no se pudo (no existia,
--    o ya no estaba pendiente).
-- ------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_ActualizarEstadoReserva]
    @IdReserva   INT,
    @NuevoEstado VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdNuevoEstado INT;
    SELECT @IdNuevoEstado = id_estado_reserva FROM estado_reserva WHERE nombre = @NuevoEstado;

    IF @IdNuevoEstado IS NULL
    BEGIN
        SELECT 0 AS FilasAfectadas;
        RETURN;
    END

    UPDATE reserva
    SET id_estado_reserva = @IdNuevoEstado
    WHERE id_reserva = @IdReserva
      AND id_estado_reserva = (SELECT id_estado_reserva FROM estado_reserva WHERE nombre = 'Pendiente');

    SELECT @@ROWCOUNT AS FilasAfectadas;
END
GO