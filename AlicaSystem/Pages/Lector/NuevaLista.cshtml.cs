using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AlicaSystem.Datos;

namespace AlicaSystem.Pages.Lector
{
    public class NuevaListaModel : PageModel
    {
        private readonly ListaDatos listaDatos;
        public NuevaListaModel(ListaDatos listaDatos) { this.listaDatos = listaDatos; }

        [BindProperty]
        public string NombreLista { get; set; } = string.Empty;

        public int ListasActuales { get; set; }
        public string? Mensaje { get; set; }

        private int? IdUsuarioSesion => HttpContext.Session.GetInt32("IdUsuario");

        public IActionResult OnGet()
        {
            if (IdUsuarioSesion == null) return RedirectToPage("/Login");
            ViewData["Activo"] = "MisListas";
            ListasActuales = listaDatos.ContarListasActivas(IdUsuarioSesion.Value);
            return Page();
        }


        public IActionResult OnPost()
        {
            if (IdUsuarioSesion == null) return RedirectToPage("/Login");

            if (string.IsNullOrWhiteSpace(NombreLista) || NombreLista.Length > 25)
            {
                Mensaje = "El nombre debe tener entre 1 y 25 caracteres.";
                ListasActuales = listaDatos.ContarListasActivas(IdUsuarioSesion.Value);
                return Page();
            }

            bool ok = listaDatos.CrearLista(IdUsuarioSesion.Value, NombreLista, out string? error);
 
            if (!ok)
            {
                Mensaje = error;
                ListasActuales = listaDatos.ContarListasActivas(IdUsuarioSesion.Value);
                return Page();
            }

            TempData["Mensaje"] = "Lista creada correctamente.";
            return RedirectToPage("/Lector/MisListas");
        }
    }
}