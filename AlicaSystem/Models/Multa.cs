namespace AlicaSystem.Models
{
    // Representa una fila de la pantalla "Multas" del bibliotecario.
    public class Multa
    {
        public int IdMulta { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public DateTime? FechaPago { get; set; }
    }
}