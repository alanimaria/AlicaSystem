namespace AlicaSystem.Models
{
    // Representa una fila de la pantalla "Multas" del bibliotecario.
    // Incluye ahora info del prestamo asociado para el panel de detalle.
    public class Multa
    {
        public int IdMulta { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Matricula { get; set; } = string.Empty;
        public int IdPrestamo { get; set; }
        public string Libro { get; set; } = string.Empty;
        public DateTime FechaEsperada { get; set; }
        public int DiasAtraso { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public DateTime? FechaPago { get; set; }
        public string Estado { get; set; } = string.Empty; // "Pendiente" | "Pagada" | "Perdonada"
    }
}