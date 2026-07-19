using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlicaSystem.Pages.Lector
{
    public class DashboardModel : PageModel
    {
        public string NombreUsuario { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            // Revisamos si hay una sesión activa
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

            if (idUsuario == null)
            {
                // Nadie inició sesión (o expiró) → de vuelta al login
                return RedirectToPage("/Login");
            }

            NombreUsuario = HttpContext.Session.GetString("NombreUsuario") ?? "Usuario";
            return Page();
        }
    }
}