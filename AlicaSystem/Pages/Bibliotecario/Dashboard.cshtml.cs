using AlicaSystem.Datos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlicaSystem.Pages.Bibliotecario
{
    // Este es el "cerebro" de la pantalla principal del bibliotecario.
    // Se encarga de: validar que quien entra tenga permiso (sesion + rol),
    // y de traer los datos que se muestran en pantalla (conteos + actividad reciente)
    public class DashboardModel : PageModel
    {
        private readonly PrestamoDatos prestamoDatos;
        private readonly ReservaDatos reservaDatos;
        private readonly MultaDatos multaDatos;

        public DashboardModel(PrestamoDatos prestamoDatos, ReservaDatos reservaDatos, MultaDatos multaDatos)
        {
            this.prestamoDatos = prestamoDatos;
            this.reservaDatos = reservaDatos;
            this.multaDatos = multaDatos;
        }

        public string NombreUsuario { get; set; } = string.Empty;
        public int PrestamosActivos { get; set; }
        public int ReservasPendientes { get; set; }
        public int MultasPendientes { get; set; }
        public List<ActividadReciente> ActividadReciente { get; set; } = new();

        public IActionResult OnGet()
        {
            string? rol = HttpContext.Session.GetString("Rol");

            if (rol != "Bibliotecario" && rol != "Administrador")
            {
                return RedirectToPage("/Login");
            }

            NombreUsuario = HttpContext.Session.GetString("NombreUsuario") ?? "";

            PrestamosActivos = prestamoDatos.ContarPrestamosActivos();
            ReservasPendientes = reservaDatos.ContarReservasPendientes();
            MultasPendientes = multaDatos.ContarMultasPendientes();
            ActividadReciente = prestamoDatos.ListarActividadReciente(10);

            return Page();
        }
    }
}