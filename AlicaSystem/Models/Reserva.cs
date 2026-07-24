namespace AlicaSystem.Models
{
    public class Reserva
    {
        public int IdReserva { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string CodigoInterno { get; set; } = string.Empty;
        public DateTime FechaReserva { get; set; }
        public DateTime FechaExpiracion { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}