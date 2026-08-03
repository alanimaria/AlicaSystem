using Microsoft.AspNetCore.Mvc;
using AlicaSystem.Datos;
using AlicaSystem.Models;

namespace AlicaSystem.Pages.Administrador
{
    public class GestionarLibrosModel : PaginaAdministradorBase
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
        [BindProperty]
        public string? PortadaUrl { get; set; }
        public List<Libro> LibrosDesactivados { get; set; } = new();

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

            LibrosDesactivados = Libros.Where(l => l.EstadoLibro == "Dado de baja").ToList();
            Libros = Libros.Where(l => l.EstadoLibro != "Dado de baja").ToList();
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
                    PortadaUrl = l.PortadaUrl;
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
            if (string.IsNullOrWhiteSpace(Titulo) || string.IsNullOrWhiteSpace(CodigoInterno))
            {
                TempData["Mensaje"] = "Título y código interno son obligatorios.";
                return RedirectToPage();
            }

            if (IdCategoria == 0)
            {
                TempData["Mensaje"] = "Debes seleccionar una categoría.";
                return RedirectToPage();
            }

            if (CantidadTotal < 1)
            {
                TempData["Mensaje"] = "La cantidad total debe ser al menos 1.";
                return RedirectToPage();
            }

            int idEstadoLibro = 1;
            if (IdLibro == 0)
            {
                int idLibroNuevo = libroDatos.InsertarLibro(Titulo, Isbn, CodigoInterno, IdCategoria, idEstadoLibro, CantidadTotal, Ubicacion, PortadaUrl);
                IdLibro = idLibroNuevo;
            }
            else
            {
                var libroActual = libroDatos.ListarLibrosAdmin().First(l => l.IdLibro == IdLibro);
                libroDatos.ActualizarLibro(IdLibro, Titulo, Isbn, CodigoInterno, IdCategoria, libroActual.IdEstadoLibro, CantidadTotal, Ubicacion, PortadaUrl);
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

        public IActionResult OnPostCambiarEstado(int id, bool activar)
        {
            bool ok = libroDatos.CambiarEstadoLibro(id, activar, out string? error);
            TempData["Mensaje"] = ok ? (activar ? "Libro activado." : "Libro desactivado.") : error;
            return RedirectToPage();
        }
    }
}