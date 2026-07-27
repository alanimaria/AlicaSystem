using Microsoft.AspNetCore.Mvc;
using AlicaSystem.Datos;
using AlicaSystem.Models;

namespace AlicaSystem.Pages.Lector
{
    public class ReservaModel : PaginaLectorBase
    {
        private readonly LibroDatos libroDatos;
        private readonly ReservaDatos reservaDatos;

        public ReservaModel(LibroDatos libroDatos, ReservaDatos reservaDatos)
        {
            this.libroDatos = libroDatos;
            this.reservaDatos = reservaDatos;
        }

        public Libro? Libro { get; set; }

        public IActionResult OnGet(int idLibro)
        {
            Libro = libroDatos.ObtenerLibroPorId(idLibro);
            if (Libro == null) return RedirectToPage("/Lector/Catalogo");

            return Page();
        }

        public IActionResult OnPost(int idLibro)
        {
            bool ok = reservaDatos.RegistrarReserva(IdUsuarioSesion, idLibro, out string? error);

            if (!ok)
            {
                TempData["Mensaje"] = error;
                return RedirectToPage(new { idLibro });
            }

            TempData["Mensaje"] = "Reserva confirmada correctamente.";
            return RedirectToPage("/Lector/MisReservas");
        }
    }
}