using AlicaSystem.Datos;
using AlicaSystem.Models;

namespace AlicaSystem.Pages.Bibliotecario
{
    public class CatalogoModel : PaginaBibliotecarioBase
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
        public string? Busqueda { get; set; }
        public int? IdCategoria { get; set; }

        public void OnGet(string? busqueda, int? idCategoria)
        {
            Busqueda = busqueda;
            IdCategoria = idCategoria;
            Categorias = categoriaDatos.ListarCategorias();
            Libros = libroDatos.ListarCatalogo(busqueda, idCategoria, false);
        }
    }
}