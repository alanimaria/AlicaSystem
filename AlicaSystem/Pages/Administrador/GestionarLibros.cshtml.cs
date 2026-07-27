using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AlicaSystem.Datos;
using AlicaSystem.Models;

namespace AlicaSystem.Pages.Administrador
{
    public class GestionarLibrosModel : PageModel
    {
        private readonly LibroDatos libroDatos;
        private readonly CategoriaDatos categoriaDatos;
        private readonly AutorDatos autorDatos;

        public GestionarLibrosModel(LibroDatos libroDatos, CategoriaDatos categoriaDatos, AutorDatos autorDatos)
        {
            this.libroDatos = libroDatos;
            this.categoriaDatos = categoriaDatos;
            this.autorDatos = autorDatos;
        }

        public List<Libro> Libros { get; set; } = new();
        public List<Categoria> Categorias { get; set; } = new();
        public string? Buscar { get; set; }

        [BindProperty]
        public int IdLibro { get; set; }
        [BindProperty]
        public string Titulo { get; set; } = string.Empty;
        [BindProperty]
        public string? Isbn { get; set; }
        [BindProperty]
        public string CodigoInterno { get; set; } = string.Empty;
        [BindProperty]
        public int IdCategoria { get; set; }
        [BindProperty]
        public int CantidadTotal { get; set; }
        [BindProperty]
        public string? Ubicacion { get; set; }

        public void OnGet(int? id, string? buscar)
        {
            ViewData["Activo"] = "GestionarLibros";
            Buscar = buscar;

            Libros = libroDatos.ListarLibrosAdmin();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                Libros = Libros.Where(l =>
                    l.Titulo.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                    l.CodigoInterno.Contains(buscar, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            Categorias = categoriaDatos.ListarCategorias();

            if (id != null)
            {
                var l = libroDatos.ListarLibrosAdmin().FirstOrDefault(x => x.IdLibro == id);
                if (l != null)
                {
                    IdLibro = l.IdLibro;
                    Titulo = l.Titulo;
                    Isbn = l.Isbn;
                    CodigoInterno = l.CodigoInterno;
                    IdCategoria = l.IdCategoria;
                    CantidadTotal = l.CantidadTotal;
                    Ubicacion = l.Ubicacion;
                }
            }
        }

        public IActionResult OnGetBuscarAutores(string texto)
        {
            var autores = autorDatos.BuscarAutores(texto);
            return new JsonResult(autores.Select(a => new { id = a.IdAutor, nombre = a.NombreCompleto }));
        }

        public IActionResult OnPost()
        {
            int idEstadoLibro = 1;

            if (IdLibro == 0)
            {
                int idLibroNuevo = libroDatos.InsertarLibro(Titulo, Isbn, CodigoInterno, IdCategoria, idEstadoLibro, CantidadTotal, Ubicacion);
                IdLibro = idLibroNuevo;
            }
            else
            {
                var libroActual = libroDatos.ListarLibrosAdmin().First(l => l.IdLibro == IdLibro);
                libroDatos.ActualizarLibro(IdLibro, Titulo, Isbn, CodigoInterno, IdCategoria, libroActual.IdEstadoLibro, CantidadTotal, Ubicacion);
            }

            var idsAutores = Request.Form["idsAutores"].ToString()
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse);

            foreach (var idAutor in idsAutores)
            {
                libroDatos.AsociarAutorLibro(IdLibro, idAutor);
            }

            TempData["Mensaje"] = "Libro guardado correctamente.";
            return RedirectToPage();
        }

        public IActionResult OnPostEliminar(int id)
        {
            bool ok = libroDatos.EliminarLibro(id, out string? error);
            TempData["Mensaje"] = ok ? "Libro desactivado." : error;
            return RedirectToPage();
        }
    }
}