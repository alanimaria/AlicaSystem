namespace AlicaSystem.Models
{
    // Representa una fila de multa. Se usa en dos pantallas:
    // - "Multas" del Bibliotecario: usa Usuario, Matricula, IdPrestamo, Libro,
    //   FechaEsperada, DiasAtraso (para el panel de detalle)
    // - "Mis multas" del Lector: usa Titulo, Motivo
    public class Multa
    {
        public int IdMulta { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Matricula { get; set; } = string.Empty;
        public int IdPrestamo { get; set; }
        public string Libro { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public DateTime FechaEsperada { get; set; }
        public int DiasAtraso { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public DateTime? FechaPago { get; set; }
        public string Estado { get; set; } = string.Empty; // "Pendiente" | "Pagada" | "Perdonada"
        public string? Motivo { get; set; }
    }
}