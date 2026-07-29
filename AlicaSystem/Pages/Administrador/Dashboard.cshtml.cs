using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AlicaSystem.Datos;

namespace AlicaSystem.Pages.Administrador
{
    // Dashboard del Administrador: version con 4 KPIs (libros en catalogo,
    // usuarios activos, prestamos activos, multas pendientes) + grafico de
    // barras "Libros mas prestados" + panel de accesos rapidos, segun el
    // mockup (screen-dash-admin).
    //
    // sp_ContarPrestamosActivos ya existia (mismo que usa el dashboard del
    // bibliotecario). Los otros 3 conteos + el top de libros se agregaron
    // en el cambio #10-#13 (2026-07-26_admin-dashboard-kpis.sql).
    public class DashboardModel : PageModel
    {
        private readonly ConexionBD conexionBD;

        public DashboardModel(ConexionBD conexionBD)
        {
            this.conexionBD = conexionBD;
        }

        public string NombreUsuario { get; set; } = string.Empty;

        public int LibrosEnCatalogo { get; set; }
        public int UsuariosActivos { get; set; }
        public int PrestamosActivos { get; set; }
        public ResumenMultasPendientes ResumenMultas { get; set; } = new();
        public List<LibroMasPrestado> TopLibros { get; set; } = new();

        public IActionResult OnGet()
        {
            // Solo Administrador puede entrar aqui (exclusivo,
            // a diferencia del dashboard del bibliotecario que
            // tambien deja pasar al Administrador)
            string? rol = HttpContext.Session.GetString("Rol");

            if (rol != "Administrador")
            {
                return RedirectToPage("/Login");
            }

            NombreUsuario = HttpContext.Session.GetString("NombreUsuario") ?? "";

            var libroDatos = new LibroDatos(conexionBD);
            var usuarioDatos = new UsuarioDatos(conexionBD);
            var multaDatos = new MultaDatos(conexionBD);
            var prestamoDatos = new PrestamoDatos(conexionBD);

            LibrosEnCatalogo = libroDatos.ContarLibrosCatalogo();
            UsuariosActivos = usuarioDatos.ContarUsuariosActivos();
            PrestamosActivos = prestamoDatos.ContarPrestamosActivos();
            ResumenMultas = multaDatos.ObtenerResumenMultasPendientes();
            TopLibros = prestamoDatos.ListarTopLibrosMasPrestados(4);

            return Page();
        }
    }
}