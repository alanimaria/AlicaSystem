namespace AlicaSystem.Models
{
    public class ReporteCatalogo
    {
        public string Estado { get; set; } = string.Empty;
        public int Total { get; set; }
        public int EjemplaresDisponibles { get; set; }
        public int EjemplaresTotales { get; set; }
    }
}