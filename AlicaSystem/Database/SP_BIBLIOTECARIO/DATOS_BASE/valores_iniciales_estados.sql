-- ============================================================
-- VALORES INICIALES DE LAS TABLAS DE ESTADO
-- Autor: Camila
-- Fecha: 20/07/2026
--
-- QUE HACE ESTE ARCHIVO:
-- Las tablas estado_prestamo, estado_libro, estado_reserva y
-- estado_multa son tablas de catalogo: solo guardan una lista fija
-- de "etiquetas" (por ejemplo, un prestamo puede estar "Activo" o
-- "Devuelto"). Estas tablas se crearon vacias en el script original,
-- asi que este archivo les inserta los valores que necesitan para
-- que el resto del sistema (prestamos, reservas, multas) funcione.
--
-- POR QUE ES SEGURO CORRERLO VARIAS VECES:
-- Cada INSERT esta protegido con "IF NOT EXISTS", que revisa primero
-- si el valor ya existe antes de insertarlo. Asi, si por accidente
-- corres este archivo dos veces, no se duplican las filas.
--
-- CUANDO CORRERLO:
-- Una sola vez por base de datos local (la mia y la tuya).
-- Debe correrse ANTES que sp_RegistrarPrestamo.sql, porque ese
-- procedimiento necesita que ya exista el estado "Activo".
-- ============================================================

USE alica_system;
GO

-- ----- Estados de un prestamo -----
-- Activo: el libro esta prestado, todavia no se ha devuelto
-- Devuelto: el prestamo ya se cerro, el libro volvio
-- Vencido: paso la fecha esperada de devolucion y el libro sigue afuera
IF NOT EXISTS (SELECT 1 FROM estado_prestamo WHERE nombre = 'Activo')
    INSERT INTO estado_prestamo (nombre, descripcion) VALUES ('Activo', 'Prestamo en curso, libro no devuelto');
IF NOT EXISTS (SELECT 1 FROM estado_prestamo WHERE nombre = 'Devuelto')
    INSERT INTO estado_prestamo (nombre, descripcion) VALUES ('Devuelto', 'El libro ya fue devuelto');
IF NOT EXISTS (SELECT 1 FROM estado_prestamo WHERE nombre = 'Vencido')
    INSERT INTO estado_prestamo (nombre, descripcion) VALUES ('Vencido', 'Paso la fecha esperada de devolucion sin devolverse');
GO

-- ----- Estados de un libro -----
-- Disponible: el libro sigue activo en el catalogo, se puede prestar
-- Dado de baja: el libro ya no se presta (perdido, danado, etc.)
IF NOT EXISTS (SELECT 1 FROM estado_libro WHERE nombre = 'Disponible')
    INSERT INTO estado_libro (nombre, descripcion) VALUES ('Disponible', 'El libro esta activo en el catalogo');
IF NOT EXISTS (SELECT 1 FROM estado_libro WHERE nombre = 'Dado de baja')
    INSERT INTO estado_libro (nombre, descripcion) VALUES ('Dado de baja', 'El libro ya no esta disponible para prestamo');
GO

-- ----- Estados de una reserva -----
-- Pendiente: el lector reservo el libro, esperando a recogerlo
-- Cumplida: el lector ya recogio el libro reservado
-- Expirada: se vencio el plazo y el lector nunca lo recogio
-- Cancelada: el lector o el sistema cancelo la reserva
IF NOT EXISTS (SELECT 1 FROM estado_reserva WHERE nombre = 'Pendiente')
    INSERT INTO estado_reserva (nombre, descripcion) VALUES ('Pendiente', 'Esperando ser recogida por el lector');
IF NOT EXISTS (SELECT 1 FROM estado_reserva WHERE nombre = 'Cumplida')
    INSERT INTO estado_reserva (nombre, descripcion) VALUES ('Cumplida', 'El lector recogio el libro reservado');
IF NOT EXISTS (SELECT 1 FROM estado_reserva WHERE nombre = 'Expirada')
    INSERT INTO estado_reserva (nombre, descripcion) VALUES ('Expirada', 'Se vencio la fecha limite de la reserva');
IF NOT EXISTS (SELECT 1 FROM estado_reserva WHERE nombre = 'Cancelada')
    INSERT INTO estado_reserva (nombre, descripcion) VALUES ('Cancelada', 'El lector o el sistema cancelo la reserva');
GO

-- ----- Estados de una multa -----
-- Pendiente: la multa se genero y todavia no se ha pagado
-- Pagada: el usuario ya salduo la multa
IF NOT EXISTS (SELECT 1 FROM estado_multa WHERE nombre = 'Pendiente')
    INSERT INTO estado_multa (nombre, descripcion) VALUES ('Pendiente', 'Multa generada, sin pagar');
IF NOT EXISTS (SELECT 1 FROM estado_multa WHERE nombre = 'Pagada')
    INSERT INTO estado_multa (nombre, descripcion) VALUES ('Pagada', 'Multa saldada por el usuario');
GO