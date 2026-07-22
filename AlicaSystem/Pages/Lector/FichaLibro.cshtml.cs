using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AlicaSystem.Datos;
using AlicaSystem.Models;

namespace AlicaSystem.Pages.Lector
{
    public class FichaLibroModel : PageModel
    {
        private readonly LibroDatos libroDatos;
        private readonly ListaDatos listaDatos;

        public FichaLibroModel(LibroDatos libroDatos, ListaDatos listaDatos)
        {
            this.libroDatos = libroDatos;
            this.listaDatos = listaDatos;
        }

        public Libro? Libro { get; set; }
        public List<string> ListasUsuario { get; set; } = new();

        public IActionResult OnGet(int id)
        {
            int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;
            if (idUsuario == 0) return RedirectToPage("/Login");

            Libro = libroDatos.ObtenerLibroPorId(id);
            if (Libro == null) return RedirectToPage("/Lector/Catalogo");

            ListasUsuario = listaDatos.ObtenerNombresListas(idUsuario);
            return Page();
        }

        public IActionResult OnPostAgregarALista(int idLibro, string nombreLista)
        {
            int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;
            var (exito, mensaje) = listaDatos.AgregarLibroALista(idUsuario, idLibro, nombreLista);
            TempData["Mensaje"] = mensaje;
            return RedirectToPage(new { id = idLibro });
        }
    }
}