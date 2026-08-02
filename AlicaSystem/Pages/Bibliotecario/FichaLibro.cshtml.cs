using AlicaSystem.Datos;
using AlicaSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace AlicaSystem.Pages.Bibliotecario
{
    public class FichaLibroModel : PaginaBibliotecarioBase
    {
        private readonly LibroDatos libroDatos;

        public FichaLibroModel(LibroDatos libroDatos)
        {
            this.libroDatos = libroDatos;
        }

        public Libro? Libro { get; set; }

        public IActionResult OnGet(int id)
        {
            Libro = libroDatos.ObtenerLibroPorId(id);
            if (Libro == null) return RedirectToPage("/Bibliotecario/Catalogo");

            return Page();
        }
    }
}