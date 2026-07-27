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

        // Marca una multa como Pagada o Perdonada.
        // RN-MUL-04: solo Bibliotecario o Administrador pueden perdonar.
        // Se revalida el rol aqui tambien, no solo en OnGet, porque este
        // handler se llama por separado via fetch (POST directo).
        public IActionResult OnPostActualizarEstado(int idMulta, string estadoDestino)
        {
            string? rol = HttpContext.Session.GetString("Rol");

            if (rol != "Bibliotecario" && rol != "Administrador")
            {
                return new JsonResult(new { exito = false, mensaje = "No autorizado." });
            }

            if (estadoDestino != "Pagada" && estadoDestino != "Perdonada")
            {
                return new JsonResult(new { exito = false, mensaje = "Estado destino invalido." });
            }

            bool exito = multaDatos.ActualizarEstadoMulta(idMulta, estadoDestino);
            return new JsonResult(new { exito });
        }
    }
}