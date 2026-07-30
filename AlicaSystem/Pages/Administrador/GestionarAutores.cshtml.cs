using Microsoft.AspNetCore.Mvc;
using AlicaSystem.Datos;
using AlicaSystem.Models;

namespace AlicaSystem.Pages.Administrador
{
    public class GestionarAutoresModel : PaginaAdministradorBase
    {
        private readonly AutorDatos autorDatos;

        public GestionarAutoresModel(AutorDatos autorDatos)
        {
            this.autorDatos = autorDatos;
        }

        public List<Autor> Autores { get; set; } = new();

        [BindProperty]
        public int IdAutor { get; set; }

        [BindProperty]
        public string NombreCompleto { get; set; } = string.Empty;

        [BindProperty]
        public string PaisOrigen { get; set; } = string.Empty;

        public void OnGet(int? id)
        {
            ViewData["Activo"] = "GestionarAutores";
            Autores = autorDatos.ListarAutores();

            if (id != null)
            {
                var a = Autores.FirstOrDefault(x => x.IdAutor == id);
                if (a != null)
                {
                    IdAutor = a.IdAutor;
                    NombreCompleto = a.NombreCompleto;
                    PaisOrigen = a.PaisOrigen;
                }
            }
        }

        public IActionResult OnPost()
        {
            if (IdAutor == 0)
                autorDatos.InsertarAutor(NombreCompleto, PaisOrigen);
            else
                autorDatos.ActualizarAutor(IdAutor, NombreCompleto, PaisOrigen);

            TempData["Mensaje"] = "Autor guardado correctamente.";
            return RedirectToPage();
        }

        public IActionResult OnPostEliminar(int id)
        {
            bool ok = autorDatos.EliminarAutor(id, out string? error);
            TempData["Mensaje"] = ok ? "Autor eliminado." : error;
            return RedirectToPage();
        }
    }
}