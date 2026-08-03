using Microsoft.AspNetCore.Mvc;
using AlicaSystem.Datos;
using AlicaSystem.Models;

namespace AlicaSystem.Pages.Administrador
{
    public class GestionarCategoriasModel : PaginaAdministradorBase
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
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                TempData["Mensaje"] = "El nombre de la categoría es obligatorio.";
                return RedirectToPage();
            }

            if (IdCategoria == 0)
                categoriaDatos.InsertarCategoria(Nombre, Descripcion);
            else
                categoriaDatos.ActualizarCategoria(IdCategoria, Nombre, Descripcion);

            TempData["Mensaje"] = "Categoría guardada correctamente.";
            return RedirectToPage();
        }

        public IActionResult OnPostEliminar(int id)
        {
            bool ok = categoriaDatos.EliminarCategoria(id, out string? error);
            TempData["Mensaje"] = ok ? "Categoría desactivada." : error;
            return RedirectToPage();
        }
    }
}
