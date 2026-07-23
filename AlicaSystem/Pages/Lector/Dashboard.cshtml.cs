using AlicaSystem.Datos;
using AlicaSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlicaSystem.Pages.Lector
{
    public class DashboardModel : PageModel
    {
        private readonly PrestamoDatos prestamoDatos;
        private readonly ReservaDatos reservaDatos;
        private readonly MultaDatos multaDatos;
        private readonly ListaDatos listaDatos;
        private readonly LibroDatos libroDatos;

        public DashboardModel(PrestamoDatos prestamoDatos, ReservaDatos reservaDatos, MultaDatos multaDatos, ListaDatos listaDatos, LibroDatos libroDatos)
        {
            this.prestamoDatos = prestamoDatos;
            this.reservaDatos = reservaDatos;
            this.multaDatos = multaDatos;
            this.listaDatos = listaDatos;
            this.libroDatos = libroDatos;
        }

        public string NombreUsuario { get; set; } = string.Empty;

        public int PrestamosActivos { get; set; }
        public int ReservasPendientes { get; set; }
        public int MultasPendientes { get; set; }
        public int ListasCreadas { get; set; }
        public List<Prestamo> PrestamosRecientes { get; set; } = new();
        public List<Libro> LibrosSugeridos { get; set; } = new();

        public IActionResult OnGet()
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                return RedirectToPage("/Login");
            }

            NombreUsuario = HttpContext.Session.GetString("NombreUsuario") ?? "Usuario";

            PrestamosActivos = prestamoDatos.ContarPrestamosActivosPorUsuario(idUsuario.Value);
            ReservasPendientes = reservaDatos.ContarReservasPendientesPorUsuario(idUsuario.Value);
            MultasPendientes = multaDatos.ContarMultasPendientesPorUsuario(idUsuario.Value);
            ListasCreadas = listaDatos.ContarListasActivas(idUsuario.Value);
            PrestamosRecientes = prestamoDatos.ListarPrestamosActivosPorUsuario(idUsuario.Value).Take(2).ToList();
            LibrosSugeridos = libroDatos.ListarCatalogo(null, null, true).Take(2).ToList();

            return Page();
        }
    }
}