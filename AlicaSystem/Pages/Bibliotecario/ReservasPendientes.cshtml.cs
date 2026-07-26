using AlicaSystem.Datos;
using AlicaSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AlicaSystem.Pages.Bibliotecario
{
    // Cerebro de la pantalla "Reservas pendientes".
    public class ReservasPendientesModel : PageModel
    {
        private readonly ReservaDatos reservaDatos;

        public ReservasPendientesModel(ReservaDatos reservaDatos)
        {
            this.reservaDatos = reservaDatos;
        }

        public List<Reserva> Reservas { get; set; } = new();

        public IActionResult OnGet()
        {
            string? rol = HttpContext.Session.GetString("Rol");

            if (rol != "Bibliotecario" && rol != "Administrador")
            {
                return RedirectToPage("/Login");
            }

            Reservas = reservaDatos.ListarReservasPendientes();

            return Page();
        }

        // Se llama desde los botones "Marcar entregado" y "Cancelar".
        // nuevoEstado llega como "COMPLETADA" o "CANCELADA" desde el JS
        // (deben coincidir exacto con los nombres del catalogo ESTADO_RESERVA).
        public IActionResult OnPostActualizarEstado(int idReserva, string nuevoEstado)
        {
            int? idEmpleado = HttpContext.Session.GetInt32("IdEmpleado");
            bool exito = reservaDatos.ActualizarEstadoReserva(idReserva, nuevoEstado, idEmpleado);
            return new JsonResult(new { exito });
        }
    }
}