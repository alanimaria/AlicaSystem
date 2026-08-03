// Models/PrestamoActivoResumen.cs
namespace AlicaSystem.Models
{
    public class PrestamoActivoResumen
    {
        public string Usuario { get; set; } = string.Empty;
        public string Matricula { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string CodigoInterno { get; set; } = string.Empty;
        public DateTime FechaDevEsperada { get; set; }
        public int DiasAtraso { get; set; }
    }
}