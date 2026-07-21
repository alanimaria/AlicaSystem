
-- Alani Encarnacion
-- 20/7/2026

USE alica_system;
GO

-- Cambios de estructura (ya aplicados, se dejan como registro)
--ALTER TABLE lista ALTER COLUMN id_libro INT NULL;
--ALTER TABLE lista ALTER COLUMN fecha_libro_agregado DATE NULL;
GO

CREATE OR ALTER PROCEDURE sp_ListarListasPorUsuario
    @IdUsuario INT, @Busqueda VARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT nombre_lista, COUNT(id_libro) AS cantidad_libros, MAX(fecha_libro_agregado) AS ultima_actualizacion
    FROM lista
    WHERE id_usuario=@IdUsuario AND estado=1 AND (@Busqueda IS NULL OR nombre_lista LIKE '%'+@Busqueda+'%')
    GROUP BY nombre_lista;
END
GO

-----------------------------------------------------------------------

CREATE OR ALTER PROCEDURE sp_ListarLibrosDeLista
    @IdUsuario INT, @NombreLista VARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT l.id_libro, li.titulo, l.fecha_libro_agregado
    FROM lista l JOIN libro li ON li.id_libro = l.id_libro
    WHERE l.id_usuario=@IdUsuario AND l.nombre_lista=@NombreLista AND l.estado=1;
END
GO

------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE sp_RenombrarLista
    @IdUsuario INT, @NombreActual VARCHAR(25), @NombreNuevo VARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE lista SET nombre_lista = @NombreNuevo WHERE id_usuario=@IdUsuario AND nombre_lista=@NombreActual;
END
GO

-----------------------------------------------------------------------

CREATE OR ALTER PROCEDURE sp_EliminarLista
    @IdUsuario INT, @NombreLista VARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE lista SET estado = 0 WHERE id_usuario=@IdUsuario AND nombre_lista=@NombreLista;
END
GO

------------------------------------------------------------------------

CREATE OR ALTER PROCEDURE sp_QuitarLibroDeLista
    @IdUsuario INT, @IdLibro INT, @NombreLista VARCHAR(25)
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM lista WHERE id_usuario=@IdUsuario AND id_libro=@IdLibro AND nombre_lista=@NombreLista;
END
GO