
-- Alani Encarnacion
-- 20/7/2026
namespace AlicaSystem.Database.SP_ACCESO
USE alica_system;
GO

CREATE OR ALTER PROCEDURE sp_LoginUsuario
    @Email VARCHAR(150), @Password VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_usuario, matricula, nombre, apellido, email, telefono, fecha_registro, estado
    FROM usuario
    WHERE email = @Email AND password = @Password AND estado = 1;
END
GO