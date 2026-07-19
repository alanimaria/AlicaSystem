using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AlicaSystem.Datos;
using AlicaSystem.Models;

namespace AlicaSystem.Pages.Administrador
{
    public class GestionarCategoriasModel : PageModel
    {
        private readonly CategoriaDatos categoriaDatos;

        public GestionarCategoriasModel(CategoriaDatos categoriaDatos)
        {
            this.categoriaDatos = categoriaDatos;
        }

        public List<Categoria> Categorias { get; set; } = new();

        [BindProperty]
        public int IdCategoria { get; set; }

        [BindProperty]
        public string Nombre { get; set; } = string.Empty;

        [BindProperty]
        public string? Descripcion { get; set; }

        public void OnGet(int? id)
        {
            ViewData["Activo"] = "GestionarCategorias";
            Categorias = categoriaDatos.ListarCategorias();

            if (id != null)
            {
                var c = Categorias.FirstOrDefault(x => x.IdCategoria == id);
                if (c != null)
                {
                    IdCategoria = c.IdCategoria;
                    Nombre = c.Nombre;
                    Descripcion = c.Descripcion;
                }
            }
        }

        public IActionResult OnPost()
        {
            if (IdCategoria == 0)
                categoriaDatos.InsertarCategoria(Nombre, Descripcion);
            else
                categoriaDatos.ActualizarCategoria(IdCategoria, Nombre, Descripcion);

            TempData["Mensaje"] = "Categoría guardada correctamente.";
            return RedirectToPage();
        }

        public IActionResult OnPostEliminar(int id)
        {
            categoriaDatos.EliminarCategoria(id);
            TempData["Mensaje"] = "Categoría eliminada.";
            return RedirectToPage();
        }
    }
}
