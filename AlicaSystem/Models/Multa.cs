namespace AlicaSystem.Models
{
    public class Multa
    {
        public int IdMulta { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public DateTime? FechaPago { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Motivo { get; set; }
    }
}