
-- Alani Encarnacion
-- 20/7/2026

USE alica_system;
GO

--ALTER TABLE categoria ADD estado BIT NOT NULL DEFAULT 1;
GO

----------------------------------------------------------------

CREATE OR ALTER PROCEDURE sp_ListarCategorias
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_categoria, nombre, descripcion FROM categoria WHERE estado = 1;
END
GO

-----------------------------------------------------------------

CREATE OR ALTER PROCEDURE sp_InsertarCategoria
    @Nombre VARCHAR(100), @Descripcion VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO categoria (nombre, descripcion) VALUES (@Nombre, @Descripcion);
END
GO

-----------------------------------------------------------------

CREATE OR ALTER PROCEDURE sp_ActualizarCategoria
    @IdCategoria INT, @Nombre VARCHAR(100), @Descripcion VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE categoria SET nombre=@Nombre, descripcion=@Descripcion WHERE id_categoria=@IdCategoria;
END
GO

-----------------------------------------------------------------

CREATE OR ALTER PROCEDURE sp_EliminarCategoria
    @IdCategoria INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE categoria SET estado = 0 WHERE id_categoria = @IdCategoria;
END
GO