nam-- Alani Encarnacion -- 19/07/2026
USE alica_system;
GO
-- Tabla: lista
-- Se permite id_libro NULL para poder crear una lista vacía (sin libro asignado todavía)
ALTER TABLE lista ALTER COLUMN id_libro INT NULL;
GO
----------------------------------------------------------------

-- Alani Encarnacion -- 19/07/2026
USE alica_system;
GO
-- Tabla: lista
-- Se permite fecha_libro_agregado NULL, ya que una lista vacía no tiene libro ni fecha
ALTER TABLE lista ALTER COLUMN fecha_libro_agregado DATE NULL;
GO
----------------------------------------------------------------

-- Alani Encarnacion -- 20/07/2026
USE alica_system;
GO
-- Tabla: categoria
-- Se agrega columna estado para soft delete (1=habilitada, 0=deshabilitada)
ALTER TABLE categoria ADD estado BIT NOT NULL DEFAULT 1;
GO
----------------------------------------------------------------