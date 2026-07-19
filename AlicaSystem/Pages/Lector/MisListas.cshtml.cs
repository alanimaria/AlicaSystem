using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AlicaSystem.Datos;
using AlicaSystem.Models;

namespace AlicaSystem.Pages.Lector
{
    public class MisListasModel : PageModel
    {
        private readonly ListaDatos listaDatos;
        public MisListasModel(ListaDatos listaDatos) { this.listaDatos = listaDatos; }

        public List<ListaResumen> Listas { get; set; } = new();
        public List<LibroEnLista> LibrosDeListaSeleccionada { get; set; } = new();
        public string? Busqueda { get; set; }
        public string? Ver { get; set; }

        private int? IdUsuarioSesion => HttpContext.Session.GetInt32("IdUsuario");

        public IActionResult OnGet(string? busqueda, string? ver)
        {
            if (IdUsuarioSesion == null) return RedirectToPage("/Login");

            ViewData["Activo"] = "MisListas";
            Busqueda = busqueda;
            Ver = ver;

            Listas = listaDatos.ListarListasPorUsuario(IdUsuarioSesion.Value, busqueda);

            if (ver != null)
                LibrosDeListaSeleccionada = listaDatos.ListarLibrosDeLista(IdUsuarioSesion.Value, ver);

            return Page();
        }

        public IActionResult OnPostRenombrar(string nombreActual, string nombreNuevo)
        {
            if (IdUsuarioSesion == null) return RedirectToPage("/Login");
            listaDatos.RenombrarLista(IdUsuarioSesion.Value, nombreActual, nombreNuevo);
            TempData["Mensaje"] = "Lista renombrada.";
            return RedirectToPage();
        }

        public IActionResult OnPostEliminar(string nombreLista)
        {
            if (IdUsuarioSesion == null) return RedirectToPage("/Login");
            listaDatos.EliminarLista(IdUsuarioSesion.Value, nombreLista);
            TempData["Mensaje"] = "Lista eliminada.";
            return RedirectToPage();
        }
    }
}