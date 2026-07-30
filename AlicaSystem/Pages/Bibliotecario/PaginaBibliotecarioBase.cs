using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlicaSystem.Pages.Bibliotecario
{
    public class PaginaBibliotecarioBase : PageModel
    {
        public int IdEmpleadoSesion { get; private set; }
        public string NombreUsuarioSesion { get; private set; } = string.Empty;

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            int? id = HttpContext.Session.GetInt32("IdEmpleado");
            string? rol = HttpContext.Session.GetString("Rol");

            if (id == null || (rol != "Bibliotecario" && rol != "Administrador"))
            {
                context.Result = RedirectToPage("/Login");
                return;
            }

            IdEmpleadoSesion = id.Value;
            NombreUsuarioSesion = HttpContext.Session.GetString("NombreUsuario") ?? "Usuario";

            base.OnPageHandlerExecuting(context);
        }
    }
}