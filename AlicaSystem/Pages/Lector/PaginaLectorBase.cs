// Pages/Lector/PaginaLectorBase.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlicaSystem.Pages.Lector
{
    public class PaginaLectorBase : PageModel
    {
        public int IdUsuarioSesion { get; private set; }
        public string NombreUsuarioSesion { get; private set; } = string.Empty;

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            int? id = HttpContext.Session.GetInt32("IdUsuario");

            if (id == null)
            {
                context.Result = RedirectToPage("/Login");
                return;
            }

            IdUsuarioSesion = id.Value;
            NombreUsuarioSesion = HttpContext.Session.GetString("NombreUsuario") ?? "Usuario";

            base.OnPageHandlerExecuting(context);
        }
    }
}