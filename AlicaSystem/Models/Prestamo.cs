namespace AlicaSystem.Models
{
    // Representa un prestamo de un libro a un lector.
    public class Prestamo
    {
        public int IdPrestamo { get; set; }
        public int IdUsuario { get; set; }
        public int IdLibro { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaDevEsperada { get; set; }
        public DateTime? FechaDevReal { get; set; }   // NULL = todavia no se devolvio
        public int IdEmpleado { get; set; }
        public int IdEstadoPrestamo { get; set; }
    }
}