namespace AlicaSystem.Models
{
    // Representa una fila de reserva. Se usa en dos pantallas distintas:
    // - "Reservas pendientes" del Bibliotecario: usa Usuario + Libro (resueltos)
    // - "Mis reservas" del Lector: usa Titulo + CodigoInterno (resueltos)
    public class Reserva
    {
        public int IdReserva { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Libro { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string CodigoInterno { get; set; } = string.Empty;
        public DateTime FechaReserva { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}