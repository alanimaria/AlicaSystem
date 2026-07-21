namespace AlicaSystem.Models
{
    // Representa a un empleado del sistema: puede ser Bibliotecario o Administrador.
    // Es distinto de "Usuario", que es el lector que pide préstamos.
    public class Empleado
    {
        public int IdEmpleado { get; set; }
        public string Nombre { get; set; } = "";
        public string Apellido { get; set; } = "";
        public string Email { get; set; } = "";
        public string Area { get; set; } = "";

        // Viene de la tabla "rol" (join en el stored procedure).
        // Los valores esperados son "Bibliotecario" o "Administrador".
        // Con esto decidimos a dónde redirigir al usuario después del login.
        public string NombreRol { get; set; } = "";
    }
}