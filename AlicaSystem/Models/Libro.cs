namespace AlicaSystem.Models
{
    public class Libro
    {
        public int IdLibro { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string? Isbn { get; set; }
        public string CodigoInterno { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string EstadoLibro { get; set; } = string.Empty;
        public int CantidadDisponible { get; set; }
        public int CantidadTotal { get; set; }
        public string? Autores { get; set; }

        public string? Ubicacion { get; set; }
    }
}