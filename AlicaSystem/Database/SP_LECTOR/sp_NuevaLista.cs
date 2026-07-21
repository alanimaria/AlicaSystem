
-- Alani Encarnacion
-- 20/7/2026

USE alica_system;
GO

CREATE OR ALTER PROCEDURE sp_CrearLista
    @IdUsuario INT, @NombreLista VARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;
    IF (SELECT COUNT(DISTINCT nombre_lista) FROM lista WHERE id_usuario=@IdUsuario AND estado=1) >= 10
    BEGIN
        RAISERROR('Ya alcanzaste el límite de 10 listas activas.', 16, 1);
        RETURN;
    END
    IF EXISTS (SELECT 1 FROM lista WHERE id_usuario=@IdUsuario AND nombre_lista=@NombreLista AND estado=1)
    BEGIN
        RAISERROR('Ya existe una lista con ese nombre.', 16, 1);
        RETURN;
    END
    INSERT INTO lista (fecha_lista, id_usuario, id_libro, estado, fecha_libro_agregado, nombre_lista)
    VALUES (GETDATE(), @IdUsuario, NULL, 1, NULL, @NombreLista);
END
GO

-----------------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE sp_ContarListasActivas
    @IdUsuario INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(DISTINCT nombre_lista) AS Total FROM lista WHERE id_usuario=@IdUsuario AND estado=1;
END
GO