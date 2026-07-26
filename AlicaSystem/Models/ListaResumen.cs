
namespace AlicaSystem.Models
{
    public class ListaResumen
    {
        public string NombreLista { get; set; } = string.Empty;
        public int CantidadLibros { get; set; }
        public DateTime? UltimaActualizacion { get; set; }
    }
}