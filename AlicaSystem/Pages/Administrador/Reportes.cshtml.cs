using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AlicaSystem.Datos;
using AlicaSystem.Models;

namespace AlicaSystem.Pages.Administrador
{
    public class ReportesModel : PageModel
    {
        private readonly ReporteDatos reporteDatos;

        public ReportesModel(ReporteDatos reporteDatos)
        {
            this.reporteDatos = reporteDatos;
        }

        public List<ReporteEstado> Prestamos { get; set; } = new();
        public List<ReporteCatalogo> Catalogo { get; set; } = new();
        public List<ReporteMulta> Multas { get; set; } = new();
        public List<ReporteRanking> LibrosMasPrestados { get; set; } = new();
        public List<ReporteRanking> UsuariosMasActivos { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public DateTime Desde { get; set; } = DateTime.Today.AddMonths(-1);

        [BindProperty(SupportsGet = true)]
        public DateTime Hasta { get; set; } = DateTime.Today;

        public void OnGet()
        {
            ViewData["Activo"] = "Reportes";

            Prestamos = reporteDatos.ReportePrestamosPorPeriodo(Desde, Hasta);
            Catalogo = reporteDatos.ReporteDisponibilidadCatalogo();
            Multas = reporteDatos.ReporteMultasPorPeriodo(Desde, Hasta);
            LibrosMasPrestados = reporteDatos.ReporteLibrosMasPrestados();
            UsuariosMasActivos = reporteDatos.ReporteUsuariosMasActivos();
        }

        // Exporta cualquiera de las 5 tablas a CSV, según el parámetro "tabla"
        public IActionResult OnGetExportar(string tabla)
        {
            OnGet(); // recarga los datos con el mismo rango de fechas

            var sb = new System.Text.StringBuilder();

            switch (tabla)
            {
                case "prestamos":
                    sb.AppendLine("Estado,Total");
                    foreach (var r in Prestamos) sb.AppendLine($"{r.Estado},{r.Total}");
                    break;
                case "catalogo":
                    sb.AppendLine("Estado,Total,Disponibles,Totales");
                    foreach (var r in Catalogo) sb.AppendLine($"{r.Estado},{r.Total},{r.EjemplaresDisponibles},{r.EjemplaresTotales}");
                    break;
                case "multas":
                    sb.AppendLine("Estado,Total,Monto");
                    foreach (var r in Multas) sb.AppendLine($"{r.Estado},{r.Total},{r.MontoTotal}");
                    break;
                case "libros":
                    sb.AppendLine("Libro,Prestamos");
                    foreach (var r in LibrosMasPrestados) sb.AppendLine($"\"{r.Nombre}\",{r.Cantidad}");
                    break;
                case "usuarios":
                    sb.AppendLine("Usuario,Prestamos");
                    foreach (var r in UsuariosMasActivos) sb.AppendLine($"\"{r.Nombre}\",{r.Cantidad}");
                    break;
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"reporte_{tabla}.csv");
        }
    }
}