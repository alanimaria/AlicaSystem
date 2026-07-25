
--Alani Encarnacion
-- 20/7/2026
USE alica_system;
GO

CREATE OR ALTER PROCEDURE sp_ResetearPasswordUsuario
    @Email VARCHAR(150), @NuevaPassword VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE usuario SET password = @NuevaPassword WHERE email = @Email;
    SELECT @@ROWCOUNT AS FilasAfectadas;
END
GO