using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlicaSystem.Pages.Administrador
{
    // Dashboard del Administrador: a diferencia del dashboard del
    // bibliotecario, este NO trae ningun conteo ni consulta a la BD
    // (por eso no necesita ningun stored procedure propio). Es solo
    // una pantalla de accesos rapidos a las demas secciones del
    // administrador (Libros, Categorias, Autores, Usuarios,
    // Empleados, Roles). Si mas adelante se quiere agregar KPIs
    // (ej. "1,248 libros en catalogo" como muestra el mockup), ahi
    // si haria falta crear SPs de conteo, similares a los del
    // dashboard del bibliotecario.
    public class DashboardModel : PageModel
    {
        public string NombreUsuario { get; set; } = string.Empty;

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

            return Page();
        }
    }
}