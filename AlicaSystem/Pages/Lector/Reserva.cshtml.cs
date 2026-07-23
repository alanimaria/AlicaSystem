using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AlicaSystem.Datos;
using AlicaSystem.Models;

namespace AlicaSystem.Pages.Lector
{
    public class ReservaModel : PageModel
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
            int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;
            if (idUsuario == 0) return RedirectToPage("/Login");

            Libro = libroDatos.ObtenerLibroPorId(idLibro);
            if (Libro == null) return RedirectToPage("/Lector/Catalogo");

            return Page();
        }

        public IActionResult OnPost(int idLibro)
        {
            int idUsuario = HttpContext.Session.GetInt32("IdUsuario") ?? 0;
            var (idReserva, mensaje) = reservaDatos.RegistrarReserva(idUsuario, idLibro);

            TempData["Mensaje"] = mensaje;

            if (idReserva > 0)
                return RedirectToPage("/Lector/MisReservas");

            // si falló (ya tiene reserva activa o no hay disponibilidad), vuelve a la misma pantalla
            return RedirectToPage(new { idLibro });
        }
    }
}