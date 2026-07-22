using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AlicaSystem.Datos;
using AlicaSystem.Models;

namespace AlicaSystem.Pages.Lector
{
    public class CatalogoModel : PageModel
    {
        private readonly LibroDatos libroDatos;
        private readonly CategoriaDatos categoriaDatos;

        public CatalogoModel(LibroDatos libroDatos, CategoriaDatos categoriaDatos)
        {
            this.libroDatos = libroDatos;
            this.categoriaDatos = categoriaDatos;
        }

        public List<Libro> Libros { get; set; } = new();
        public List<Categoria> Categorias { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Busqueda { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? IdCategoria { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool SoloDisponibles { get; set; }

        public void OnGet()
        {
            Categorias = categoriaDatos.ListarCategorias();
            Libros = libroDatos.ListarCatalogo(Busqueda, IdCategoria, SoloDisponibles);
        }
    }
}