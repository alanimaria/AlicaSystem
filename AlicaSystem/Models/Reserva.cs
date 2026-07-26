namespace AlicaSystem.Models
{
    // Representa una fila de la pantalla "Reservas pendientes".
    // Ya trae los datos de usuario y libro resueltos (no ids sueltos),
    // porque esta pensado solo para mostrar en pantalla.
    public class Reserva
    {
        public int IdReserva { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Libro { get; set; } = string.Empty;
        public DateTime FechaReserva { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}