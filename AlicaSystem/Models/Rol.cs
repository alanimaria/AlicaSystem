namespace AlicaSystem.Models
{
    // Representa un rol del personal (Bibliotecario, Administrador, etc).
    public class Rol
    {
        public int IdRol { get; set; }
        public string Nombre { get; set; } = "";
        public string? Descripcion { get; set; }

        // Solo se llena en ListarRoles(), para mostrar en la columna
        // "Empleados" de la tabla. No se usa al insertar/actualizar.
        public int CantidadEmpleados { get; set; }
    }
}