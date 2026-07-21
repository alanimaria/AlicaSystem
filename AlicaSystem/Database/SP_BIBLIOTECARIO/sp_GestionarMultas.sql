-- ============================================================
-- STORED PROCEDURES - Gestionar multas
-- Autor: Camila
-- Fecha: 21/07/2026
-- ============================================================

USE alica_system;
GO

-- ------------------------------------------------------------
-- 1) Lista las multas en estado "Pendiente" (sin pagar), con el
--    nombre del usuario ya resuelto, ordenadas por fecha de
--    generacion (las mas viejas primero).
-- ------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_ListarMultas]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        m.id_multa,
        u.nombre + ' ' + u.apellido AS Usuario,
        m.monto,
        m.fecha_generacion,
        m.fecha_pago
    FROM multa m
    JOIN usuario u ON u.id_usuario = m.id_usuario
    JOIN estado_multa em ON em.id_estado_multa = m.id_estado_multa
    WHERE em.nombre = 'Pendiente'
    ORDER BY m.fecha_generacion ASC;
END
GO

-- ------------------------------------------------------------
-- 2) Marca una multa como pagada: le pone fecha_pago = hoy y
--    cambia el estado a "Pagada". Solo actualiza si la multa
--    seguia pendiente (para no reescribir una ya pagada).
--    Devuelve: 1 si se actualizo, 0 si no se pudo.
-- ------------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[sp_ActualizarEstadoMulta]
    @IdMulta INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @IdEstadoPagada INT;
    SELECT @IdEstadoPagada = id_estado_multa FROM estado_multa WHERE nombre = 'Pagada';

    UPDATE multa
    SET fecha_pago = GETDATE(), id_estado_multa = @IdEstadoPagada
    WHERE id_multa = @IdMulta
      AND id_estado_multa = (SELECT id_estado_multa FROM estado_multa WHERE nombre = 'Pendiente');

    SELECT @@ROWCOUNT AS FilasAfectadas;
END
GO