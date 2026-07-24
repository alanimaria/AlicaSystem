using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AlicaSystem.Datos;
using AlicaSystem.Models;

namespace AlicaSystem.Pages.Lector
{
    public class MisReservasModel : PageModel
    {
        private readonly ReservaDatos reservaDatos;
        public MisReservasModel(ReservaDatos reservaDatos) { this.reservaDatos = reservaDatos; }

        public List<Reserva> Reservas { get; set; } = new();
        public string? Tab { get; set; }
        public int CantidadActivas { get; set; }

        private int? IdUsuarioSesion => HttpContext.Session.GetInt32("IdUsuario");

        public IActionResult OnGet(string? tab)
        {
            if (IdUsuarioSesion == null) return RedirectToPage("/Login");

            ViewData["Activo"] = "MisReservas";
            Tab = tab;

            var todas = reservaDatos.ListarReservasPorUsuario(IdUsuarioSesion.Value);
            CantidadActivas = todas.Count(r => r.Estado == "Pendiente");

            Reservas = tab == "historial"
                ? todas.Where(r => r.Estado != "Pendiente").ToList()
                : todas.Where(r => r.Estado == "Pendiente").ToList();

            return Page();
        }

        public IActionResult OnPostCancelar(int idReserva)
        {
            if (IdUsuarioSesion == null) return RedirectToPage("/Login");
            reservaDatos.CancelarReservaUsuario(idReserva, IdUsuarioSesion.Value);
            TempData["Mensaje"] = "Reserva cancelada.";
            return RedirectToPage();
        }
    }
}