namespace AlicaSystem.Models
{
    public class Prestamo
    {
        public int IdPrestamo { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string CodigoInterno { get; set; } = string.Empty;
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaDevEsperada { get; set; }
        public string Estado { get; set; } = string.Empty;
        public bool Renovado { get; set; }
    }
}