using AlicaSystem.Datos;
using AlicaSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlicaSystem.Pages.Bibliotecario
{
    public class GestionarMultasModel : PageModel
    {
        private readonly MultaDatos multaDatos;

        public GestionarMultasModel(MultaDatos multaDatos)
        {
            this.multaDatos = multaDatos;
        }

        public List<Multa> Multas { get; set; } = new();

        public IActionResult OnGet()
        {
            string? rol = HttpContext.Session.GetString("Rol");

            if (rol != "Bibliotecario" && rol != "Administrador")
            {
                return RedirectToPage("/Login");
            }

            Multas = multaDatos.ListarMultas();

            return Page();
        }

        public IActionResult OnPostActualizarEstado(int idMulta)
        {
            bool exito = multaDatos.ActualizarEstadoMulta(idMulta);
            return new JsonResult(new { exito });
        }
    }
}